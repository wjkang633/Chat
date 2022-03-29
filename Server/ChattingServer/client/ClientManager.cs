using ChattingServer.data;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using static ChattingServer.TcpNetwork;

namespace ChattingServer
{
    public class ClientManager
    {
        //Key는 클라이언트의 객체의 Uid
        //Value는 클라이언트 객체
        public static ConcurrentDictionary<int, ClientData> _clientDic = new ConcurrentDictionary<int, ClientData>();

        //클라이언트 접속 시 발생하는 이벤트
        public event Action<string, LogType> _accessEventHandler = null;
        //클라이언트의 메시지 수신 시 발생하는 이벤트
        public event Action<int, byte[]> _receivedMsgEventHandler = null;

        /// <summary>
        /// 클라이언트 객체 생성 및 개별 네트워크 수신 설정
        /// 클라이언트 관리를 위한 전역 변수에 생성한 클라이언트 추가
        /// </summary>
        /// <param name="newClient"></param>
        public void AddClient(TcpClient newClient)
        {
            //클라이언트 객체 생성
            ClientData currentClient = new ClientData(newClient);

            try
            {
                //클라이언트별 메시지 수신
                currentClient
                    .TcpClient
                    .GetStream()
                    .BeginRead(currentClient.ReadBuffer, 0, currentClient.ReadBuffer.Length, new AsyncCallback(DataReceived), currentClient);

                //클라이언트 객체 추가
                _clientDic.TryAdd(currentClient.ClientUid, currentClient);
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// 메시지 수신 콜백
        /// 수신 후 다시 수신 요청을 반복하여 무한 Read 하도록 세팅
        /// </summary>
        /// <param name="ar"></param>
        private void DataReceived(IAsyncResult ar)
        {
            ClientData client = ar.AsyncState as ClientData;

            try
            {
                int byteLength = client
                    .TcpClient
                    .GetStream()
                    .EndRead(ar);

                try
                {
                    //다시 수신 요청
                    client
                  .TcpClient
                  .GetStream()
                  .BeginRead(client.ReadBuffer, 0, client.ReadBuffer.Length, new AsyncCallback(DataReceived), client);
                }
                catch (StackOverflowException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                //클라이언트 접속 시 로그 파일에 로그를 남김 
                if (_accessEventHandler != null)
                {
                    string accessLog = string.Format("[{0}] {1}번 클라이언트 접속", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), client.ClientUid);
                    _accessEventHandler.Invoke(accessLog, LogType.ADD_ACCESS_LOG);
                }

                //클랑이언트 메시지 수신 시 이벤트 발생
                if (_receivedMsgEventHandler != null)
                {
                    _receivedMsgEventHandler.Invoke(client.ClientUid, client.ReadBuffer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}