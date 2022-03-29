using ChattingServer.database;
using MessagePack;
using System.Diagnostics;

namespace ChattingServer
{
    public partial class FormServer : Form
    {
        private TcpNetwork _tcpNetwork = new TcpNetwork();
   
        public FormServer()
        {
            InitializeComponent();

            btnStart.Click += StartServer;
            btnStop.Click += StopServer;
            btnCurrentClient.Click += ShowCurrentClient;
            btnAccessLog.Click += ShowAccessLog;
            btnChattingLog.Click += ShowChattingLog;

            _tcpNetwork._receivedMsgEventHandler += ReceivedMsgEventHandler;
            _tcpNetwork._sentMsgEventHandler += SendingMsgEventHandler;
        }

        private void ReceivedMsgEventHandler(object? sender, byte[] bytes)
        {
            string msg = MessagePackSerializer.ConvertToJson(bytes) + "\n";

            Invoke(new Action(() => receivedMonitor.Items.Add(msg)));
        }

        private void SendingMsgEventHandler(object? sender, byte[]  bytes)
        {
            string msg = "";

            try
            {
                msg = MessagePackSerializer.ConvertToJson(bytes) + "\n";
            }
            catch (Exception ex)
            {

            }

            Invoke(new Action(() => sendingMonitor.Items.Add(msg))); 
        }

        private void StartServer(object? sender, EventArgs e)
        {
            btnStart.Enabled = false;

            _tcpNetwork.StartSever();
        }

        private void StopServer(object? sender, EventArgs e)
        {
            btnStart.Enabled = true;

            _tcpNetwork.StopServer();
        }

        private void ShowCurrentClient(object? sender, EventArgs e)
        {
            _tcpNetwork.ShowCurrentClient();
        }

        private void ShowChattingLog(object? sender, EventArgs e)
        {
            _tcpNetwork.ShowCattingLog();
        }

        private void ShowAccessLog(object? sender, EventArgs e)
        {
            _tcpNetwork.ShowAccessLog();
        }
    }
}