
using MessagePack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClient
{
    /* ==================================================================================
   * @kwjjjj 2021.12.24
   * ----------------------------------------------------------------------------------
   * 주요 기능: 서버와의 연결, 데이터 송수신 처리를 담당함
   * 
   * 
   * <TODO>
   * 
   * ----------------------------------------------------------------------------------
   * History
   * @kwjjjj 2021.12.24
   * - 최초 작성
   * ================================================================================== */
    public class TcpNetwork
    {
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;

        private const int BUFSIZE = 8192;

        private bool _isConnected = false;

        /// <summary>
        /// 서버로부터 데이터 수신 시 이벤트 핸들러 동작시키기 위함
        /// </summary>
        public ResponseHandler ResponseHandler { get; set; }

        /// <summary>
        /// 서버와의 연결 설정
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            if (_tcpClient == null)
            {
                try
                {
                    //_tcpClient = new TcpClient("10.0.0.116", 1000); //지은씨 서버
                    _tcpClient = new TcpClient("10.0.0.58", 2000); //테스트 서버
                    _networkStream = _tcpClient.GetStream();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 서버로 데이터 전송
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="chatData">전송할 데이터</param>
        public void Send<T>(T chatData)
        {
            try
            {
                byte[] inputBuf = MessagePackSerializer.Serialize<T>(chatData);
                _networkStream.Write(inputBuf, 0, inputBuf.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 서버로 파일 데이터 전송
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="chatData">파일 데이터</param>
        public void SendFile(byte[] fileBytes)
        {
            try
            {
                //파일 내용 전송
                _networkStream.Write(fileBytes, 0, fileBytes.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 서버로부터 데이터 수신
        /// </summary>
        public void Receive()
        {
            if (!_isConnected)
            {
                _isConnected = true;

                //수신을 위한 개별 스레드 생성
                Thread thread = new Thread(() => {
                    int nBytes;
                    byte[] outbuf = new byte[BUFSIZE];

                    //계속해서 데이터 수신받도록 while 사용
                    while (true)
                    {
                        try
                        {
                            //스트림에서 데이터 읽어들임
                            nBytes = _networkStream.Read(outbuf, 0, outbuf.Length);

                            if (ResponseHandler != null)
                            {
                                //데이터 수신 콜백 이벤트를 핸들러에 전달
                                ResponseHandler.HandleData(outbuf);
                            }
                        }
                        //서버와의 연결이 끊어졌을 경우 
                        catch (IOException ex)
                        {
                            //네트워크 설정 모두 해제 
                            Close();

                            //로그아웃 처리를 위한 핸들러 알림
                            if (ResponseHandler != null)
                            {
                                ResponseHandler.HandleServerDown();
                            }

                            return;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                });

                thread.Start();
            }
        }

        /// <summary>
        /// 파일 데이터 받아오기
        /// </summary>
        public void ReceiveFile(long fileSize)
        {
            byte[] outbuf = new byte[fileSize];

            _networkStream.Read(outbuf, 0, outbuf.Length);

            if (ResponseHandler != null)
            {
                ResponseHandler.HandleFileData(outbuf);
            }
        }

        /// <summary>
        /// 모든 네트워크 설정 해제
        /// </summary>
        public void Close()
        {
            _networkStream.Close();
            _tcpClient.Close();
        }
    }
}
