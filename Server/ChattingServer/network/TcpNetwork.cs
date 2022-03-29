using ChattingClient;
using ChattingServer.data;
using MessagePack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChattingServer
{
    public class TcpNetwork
    {
        ClientManager _clientManager = null;
        ClientMsgHandler _clinetMsgHandler = null;

        ConcurrentBag<string> _chattingLog = null;
        ConcurrentBag<string> _accessLog = null;

        Thread _serverThread = null;
        Thread _connectionCheckThread = null;

        private TcpListener listener;

        public event EventHandler<byte[]> _receivedMsgEventHandler;
        public event EventHandler<byte[]> _sentMsgEventHandler;

        /// <summary>
        /// 로그는 접근 로그, 채팅 로그로 분류
        /// </summary>
        public enum LogType
        {
            ADD_ACCESS_LOG = 1,
            ADD_CHATTING_LOG = 2
        }

        /// <summary>
        /// 생성자에서는 접근 로그, 채팅 로그, 클라이언트 매니저, 클라이언트 메시지 분류기 객체 생성
        /// </summary>
        public TcpNetwork()
        {
            _chattingLog = new ConcurrentBag<string>();
            _accessLog = new ConcurrentBag<string>();

            _clientManager = new ClientManager();
            _clientManager._accessEventHandler += ClientAccessEvent;
            _clientManager._receivedMsgEventHandler += ClientMsgReceptionEvent;

            _clinetMsgHandler = new ClientMsgHandler();
        }

        /// <summary>
        /// 서버 스레드 및 Connection 확인 스레드 시작
        /// </summary>
        public void StartSever()
        {
            //서버 스레드
            _serverThread = new Thread(RunServer);
            _serverThread.Start();

            //클라이언트 Connection 확인 스레드
            _connectionCheckThread = new Thread(CheckClientConnection);
            _connectionCheckThread.Start();
        }

        /// <summary>
        /// 서버 중지
        /// </summary>
        public void StopServer()
        {
            _serverThread.Join();
            _connectionCheckThread.Join();

            listener.Stop();

            //클라이언트 매니저가 관리하는 모든 클라이언트들의 TcpClient 해제
            foreach (var item in ClientManager._clientDic)
            {
                item.Value.TcpClient.Close();
            }
        }

        /// <summary>
        /// 서버 시작
        /// </summary>
        private void RunServer()
        {
            try
            {
                listener = new TcpListener(new IPEndPoint(IPAddress.Any, 2000));
                listener.Start();

                //클라이언트의 연결을 계속 받아들임
                while (true)
                {
                    Task<TcpClient> acceptTask = listener.AcceptTcpClientAsync();

                    acceptTask.Wait();

                    TcpClient newClient = acceptTask.Result;

                    //클라이언트 매니저에 접속한 클라이언트 추가
                    _clientManager.AddClient(newClient);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 클라이언트와의 Connection을 10초에 한번씩 체크함
        /// </summary>
        private void CheckClientConnection()
        {
            while (true)
            {
                foreach (var item in ClientManager._clientDic)
                {
                    try
                    {
                        string sendString = "연결 확인";
                        byte[] sendBytes = new byte[sendString.Length];

                        sendBytes = Encoding.Default.GetBytes(sendString);

                        item.Value
                            .TcpClient
                            .GetStream()
                            .Write(sendBytes, 0, sendBytes.Length);
                    }
                    catch (Exception e)
                    {
                        //연결이 끊겨있으면 클라이언트 매니저에서 해당 클라이언트를 삭제
                        RemoveClient(item.Value);

                        //접근 로그 남김
                        string accessLog = string.Format("[{0}] {1}번 클라이언트 끊김", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), item.Value.ClientUid);
                        ClientAccessEvent(accessLog, LogType.ADD_ACCESS_LOG);
                    }
                }

                Thread.Sleep(10000);
            }
        }

        /// <summary>
        /// 클라이언트가 연결이 끊겼을 때 클라이언트 매니저에서 해당 클라이언트 객체를 삭제
        /// </summary>
        /// <param name="client">제거할 클라이언트</param>
        private void RemoveClient(ClientData client)
        {
            ClientData removedClient = null;

            ClientManager._clientDic.TryRemove(client.ClientUid, out removedClient);
        }

        /// <summary>
        /// 클라이언트의 메시지 수신 시 동작
        /// </summary>
        /// <param name="clientUid">클라이언트 Uid</param>
        /// <param name="receivedBytes">수신된 byte 데이터</param>
        private void ClientMsgReceptionEvent(int clientUid, byte[] receivedBytes)
        {
            //수신된 byte 데이터를 특정 데이터(클래스)로 변환
            //서버로 보낼 데이터 생성
            _clinetMsgHandler.categorizeMsg(clientUid, receivedBytes);

            //클라이언트의 Uid로 유저 아이디 가져오기
            string clientId = "";

            ClientData foundClient = null;

            if (ClientManager._clientDic.TryGetValue(clientUid, out foundClient) != null)
            {
                clientId = ClientManager._clientDic[clientUid].ClientId;
            }

            //클라이언트 요청이 파일 요청인지 전송인지 구분 
            bool isFileReceived = _clinetMsgHandler.IsFileReceived;
            //멀티 클라이언트에게 전송해야 할 작업인지 구분
            bool IsSentToMultiClient = _clinetMsgHandler.IsSentToMultiClient;

            //클라이언트에게 전송할 데이터 
            byte[] bytesToSend = _clinetMsgHandler.BytesToSend;
            //클라이언트에게 전송할 파일 데이터 
            byte[] fileBytesToSend = _clinetMsgHandler.FileBytesToSend;

            ////일반 데이터 요청일 때
            //if (bytesToSend != null && !isFileReceived && fileBytesToSend == null)
            //{
            //    //메시지 전송
            //    SendToSingleClient(clientUid, clientId, bytesToSend);
            //}
            ////이전 대화 파일 요청일 때
            //else if (bytesToSend != null && !isFileReceived && fileBytesToSend != null)
            //{
            //    //1차로 메시지 전송
            //    SendToSingleClient(clientUid, clientId, bytesToSend);
            //    //2차로 파일 전송
            //    SendToSingleClient(clientUid, clientId, fileBytesToSend);

            //    _clinetMsgHandler.FileBytesToSend = null;
            //}
            ////파일이 전송되었을 때 
            //else if (bytesToSend != null && isFileReceived)
            //{
            //    if (fileBytesToSend == null)
            //    {
            //        //1차로 메시지 전송
            //        SendToSingleClient(clientUid, clientId, bytesToSend);
            //    }
            //    else
            //    {
            //        //2차로 파일 전송
            //        SendToMultiClient(_clinetMsgHandler.roomMemberUidArr, clientId, _clinetMsgHandler.FileBytesToSend);
            //    }
            //}
            ////메시지
            //else if (IsSentToMultiClient && bytesToSend != null)
            //{
            //    int[] roomMember = _clinetMsgHandler.roomMemberUidArr;

            //    SendToMultiClient(roomMember, clientId, bytesToSend);

            //    _clinetMsgHandler.IsSentToMultiClient = false;
            //}
            //else
            //{
            //    MessageBox.Show("송신할 데이터 오류");
            //}

            //멀티 클라이언트에게 전송
            //클라이언트 요청이 파일 전송은 아닐 때
            if (IsSentToMultiClient && !isFileReceived)
            {
                //초대하기 
                //메시지 보내기
                int[] roomMember = _clinetMsgHandler.roomMemberUidArr;

                if (roomMember != null && bytesToSend != null)
                {
                    SendToMultiClient(roomMember, clientId, bytesToSend);
                }
            }
            //멀티 클라이언트에게 전송
            //클라이언트가 파일을 보냈을 때 
            else if (IsSentToMultiClient && isFileReceived)
            {
                if (bytesToSend != null && fileBytesToSend == null)
                {
                    SendToSingleClient(clientUid, clientId, bytesToSend);
                }
                else if (bytesToSend == null && fileBytesToSend != null)
                {
                    int[] roomMember = _clinetMsgHandler.roomMemberUidArr;

                    if (roomMember != null)
                    {
                        SendToMultiClient(roomMember, clientId, fileBytesToSend);
                    }
                }
            }
            //싱글 클라이언트에게 전송
            //클라이언트 요청이 파일 전송은 아닐 때
            else if (!IsSentToMultiClient && !isFileReceived)
            {
                //로그인, 회원가입, 로그아웃
                //대화방 나가기, 대화방 맴버보기, 초대 가능 친구 목록 보기
                //대화방 오픈
                //친구 목록, 대화 목록 

                if (bytesToSend != null && fileBytesToSend == null)
                {
                    SendToSingleClient(clientUid, clientId, bytesToSend);
                }

                //이전 대화 보기
                if (bytesToSend != null && fileBytesToSend != null)
                {
                    SendToSingleClient(clientUid, clientId, bytesToSend);
                    SendToSingleClient(clientUid, clientId, fileBytesToSend);

                    _clinetMsgHandler.FileBytesToSend = null;
                }
            }
            else 
            {
                Debug.WriteLine("전송 타입 오류");
            }

            //클라이언트가 보낸 메시지를 UI에 나타내기 위함
            _receivedMsgEventHandler?.Invoke(this, receivedBytes);
        }

        /// <summary>
        /// 단일 클라이언트에게 메시지 전송
        /// </summary>
        /// <param name="clientUid">전송할 클라이언트의 Uid</param>
        /// <param name="clientId">전송하는 클라이언트의 아이디</param>
        /// <param name="bytesToSend">전송할 byte 데이터</param>
        private void SendToSingleClient(int clientUid, string clientId, byte[] bytesToSend)
        { 
            ClientData foundClient = null;

            //클라이언트 번호로 전송할 클라이언트 객체 특정
            if (ClientManager._clientDic.TryGetValue(clientUid, out foundClient) != null)
            {
                ClientManager
                    ._clientDic[clientUid]
                    .TcpClient
                    .GetStream()
                    .Write(bytesToSend, 0, bytesToSend.Length);

                string msg = MessagePackSerializer.ConvertToJson(bytesToSend) + "\n";
                string chattingLog = string.Format("[{0}] [{1}] -> [{2}] , {3}",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), clientId, ClientManager._clientDic[clientUid].ClientId, msg);

                //채팅 로그 남김
                ClientAccessEvent(chattingLog, LogType.ADD_CHATTING_LOG);

                //서버에서 보낸 메시지를 UI에 나타내기 위함
                _sentMsgEventHandler?.Invoke(this, bytesToSend);
            }
        }

        /// <summary>
        /// 멀티 클라이언트에게 메시지 전송
        /// </summary>
        /// <param name="clientUidArr">전송할 클라이언트들의 Uid 목록</param>
        /// <param name="clientId">전송하는 클라이언트의 아이디</param>
        /// <param name="bytesToSend">전송할 byte 데이터</param>
        private void SendToMultiClient(int[] clientUidArr,string clientId, byte[] bytesToSend)
        {
            ClientData foundClient = null;

            foreach (int uid in clientUidArr)
            {
                if (ClientManager._clientDic.TryGetValue(uid, out foundClient) != null)
                {
                    ClientManager
                        ._clientDic[uid]
                        .TcpClient
                        .GetStream()
                        .Write(bytesToSend, 0, bytesToSend.Length);

                    string msg = MessagePackSerializer.ConvertToJson(bytesToSend) + "\n";
                    string chattingLog  = string.Format("[{0}] [{1}] -> [{2}] , {3}", 
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), clientId, ClientManager._clientDic[uid].ClientId, msg);

                    //채팅 로그 남김
                    ClientAccessEvent(chattingLog, LogType.ADD_CHATTING_LOG);

                    //서버에서 보낸 메시지를 UI에 나타내기 위함
                    _sentMsgEventHandler?.Invoke(this, bytesToSend);
                }
            }
        }

        /// <summary>
        /// 채팅 로그와 접근 로그를 저장함
        /// </summary>
        /// <param name="message">남길 로그 메시지</param>
        /// <param name="logType">로그 종류</param>
        private void ClientAccessEvent(string message, LogType logType)
        {
            switch (logType)
            {
                case LogType.ADD_ACCESS_LOG:
                    {
                        //_accessLog.Add(message);
                        WriteAccessLogAsync(message);
                        break;
                    }
                case LogType.ADD_CHATTING_LOG:
                    {
                        //_chattingLog.Add(message);
                        WriteChattingLog(message);
                        break;
                    }
            }
        }

        /// <summary>
        /// 접속 로그를 디스크 파일에 씀
        /// </summary>
        /// <param name="message">남길 메시지</param>
        private async Task WriteAccessLogAsync(string message)
        {
            string filePath = "access_log.txt";

            if (File.Exists(filePath))
            {
                using StreamWriter file = new(filePath, append: true);
                await file.WriteLineAsync(message);
            }
        }

        /// <summary>
        /// 채팅 로그를 디스크 파일에 씀
        /// </summary>
        /// <param name="message">남길 메시지</param>
        private async Task WriteChattingLog(string message)
        {
            string filePath = "chatting_log.txt";

            if (File.Exists(filePath))
            {
                using StreamWriter file = new(filePath, append: true);
                await file.WriteLineAsync(message);
            }
        }

        /// <summary>
        /// 현재 접속 클라이언트 확인
        /// </summary>
        public void ShowCurrentClient()
        {
            if (ClientManager._clientDic.Count == 0)
            {
                MessageBox.Show("접속자가 없습니다.");
                return;
            }

            string clients = "";

            foreach (var item in ClientManager._clientDic)
            {
                clients += item.Value.ClientUid + " ";
            }

            MessageBox.Show("현재 접속중인 클라이언트\n" + clients);
        }

        /// <summary>
        /// 채팅 로그 확인
        /// </summary>
        public void ShowCattingLog()
        {
            if (_chattingLog.Count == 0)
            {
                MessageBox.Show("채팅 기록이 없습니다.");
                return;
            }

            string logs = "";

            foreach (var item in _chattingLog)
            {
                logs += item + " ";
            }

            MessageBox.Show("채팅 로그\n" + logs);
        }

        /// <summary>
        /// 접근 로그 확인
        /// </summary>
        public void ShowAccessLog()
        {
            if (_accessLog.Count == 0)
            {
                MessageBox.Show("접근 기록이 없습니다.");
                return;
            }

            string logs = "";

            foreach (var item in _accessLog)
            {
                logs += item + " ";
            }

            MessageBox.Show("접근 로그\n" + logs);
        }
    }
}

