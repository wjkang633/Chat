using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChattingServer.data
{
    public class ClientData
    {
        public TcpClient TcpClient { get; set; }
        public Byte[] ReadBuffer { get; set; }
        public StringBuilder CurrentMsg { get; set; }

        /// <summary>
        /// 클라이언트 고유 아이디
        /// 클라이언트의 ip 4번째 옥텟으로 함
        /// </summary>
        public int ClientUid { get; set; }

        /// <summary>
        /// 클라이언트 유저 아이디
        /// </summary>
        public string ClientId { get; set; }

        public ClientData(TcpClient tcpClient)
        {
            this.TcpClient = tcpClient;

            ReadBuffer = new byte[8192];

            CurrentMsg = new StringBuilder();

            char[] splitDivision = new char[2];
            splitDivision[0] = '.';
            splitDivision[1] = ':';

            //4번째 옥텟 가져오는 과정
            this.ClientUid = int.Parse(tcpClient.Client.LocalEndPoint.ToString().Split(splitDivision)[3]);
        }
    }
}
