

using MessagePack;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using static ChattingClient.CustomException;

namespace ChattingClient
{
    public partial class FormClient : Form
    {
        private class PageInfo
        {
            public PageType Page { get; private set; }

            public string? Title { get; set; }

            public Size MaximumSize { get; set; }

            public Size DefaultSize { get; set; }

            public Control? FirstControl { get; set; }

            public PageInfo(PageType page, string title)
            {
                Page = page;
                Title = title;
            }

            public PageInfo(PageType page, string title, Size max) : this(page, title)
            {
                MaximumSize = max;
            }
        }

        private enum PageType
        {
            UnKnown,

            //�α���
            Login,

            //ȸ������
            Join,

            //ģ�� �� ��ȭ ��� 
            Main,

            //��ȭ��
            Talk,

            //��ȭ �ɹ� ���
            MemberList,

            //�ʴ� ���� ģ�� ���
            InvitingList
        }

        private const int MIN_WIDTH = 320;
        private const int MAX_WIDTH = MIN_WIDTH;
        private const int MIN_HEIGHT = 550;
        private const int MAX_HEIGHT = MIN_HEIGHT;

        private PageType _currentPage;

        private List<Control> _pages;

        private PageType CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (_currentPage == value)
                {
                    return;
                }

                _currentPage = value;

                SuspendLayout();

                foreach (var page in _pages)
                {
                    if (page.Tag == null)
                        continue;

                    if (page.Tag is PageInfo info)
                    {
                        page.Visible = info.Page == value;

                        if (info.Page == value)
                        {
                            MaximumSize = info.MaximumSize;
                            Size = info.MaximumSize;
                            Text = info.Title;

                            if (info.FirstControl != null)
                                info.FirstControl.Focus();
                            else
                                page.Focus();
                        }

                        switch (info.Title)
                        {
                            case "�α���":
                                txtLoginID.Text = String.Empty;
                                txtLoginPW.Text = String.Empty;
                                break;

                            case "ȸ������":
                                txtJoinID.Text = String.Empty;
                                txtJoinPW.Text = String.Empty;
                                txtJoinName.Text = String.Empty;
                                break;

                            case "��ȭ��":
                                txtMessage.Text = String.Empty;
                                break;
                        }
                    }
                }

                ResumeLayout();
            }
        }

        private static TcpNetwork _tcpNetwork = new TcpNetwork();

        /// <summary>
        /// ������
        /// UI �ʱ� ���� �� ��Ʈ��ũ ���� ���� �̺�Ʈ �ڵ鷯 ���
        /// </summary>
        public FormClient()
        {
            InitializeComponent();

            //�� Panel �ʱ� ����
            pnlLogin.Dock = DockStyle.Fill;
            pnlJoin.Dock = DockStyle.Fill;
            pnlMain.Dock = DockStyle.Fill;
            pnlTalk.Dock = DockStyle.Fill;
            pnlMemberList.Dock = DockStyle.Fill;
            pnlInvitingList.Dock = DockStyle.Fill;

            //Panel ��ȯ ����
            pnlLogin.Tag = new PageInfo(PageType.Login, "�α���", new Size(MAX_WIDTH, MAX_HEIGHT)) { FirstControl = txtLoginID };
            pnlJoin.Tag = new PageInfo(PageType.Join, "ȸ������", new Size(MAX_WIDTH, MAX_HEIGHT)) { FirstControl = txtJoinID };
            pnlMain.Tag = new PageInfo(PageType.Main, "����", new Size(MAX_WIDTH, MAX_HEIGHT)) { FirstControl = lstChat };
            pnlTalk.Tag = new PageInfo(PageType.Talk, "��ȭ��", new Size(MAX_WIDTH, MAX_HEIGHT)) { FirstControl = txtMessage };
            pnlMemberList.Tag = new PageInfo(PageType.MemberList, "��ȭ �ɹ�", new Size(MAX_WIDTH, MAX_HEIGHT)) { FirstControl = btnMemberClose };
            pnlInvitingList.Tag = new PageInfo(PageType.InvitingList, "�ʴ� ���� ģ��", new Size(MAX_WIDTH, MAX_HEIGHT)) { };

            //â �⺻ ����
            Width = MIN_WIDTH;
            Height = MIN_HEIGHT;

            MinimumSize = new Size(MIN_WIDTH, MIN_HEIGHT);
            MaximumSize = new Size(MAX_WIDTH, MAX_HEIGHT);

            StartPosition = FormStartPosition.CenterScreen;

            //UI �̺�Ʈ ����
            //�α���
            btnLogin.Click += Login; //�α���
            btnMoveToJoin.Click += MoveToJoinPage; //ȸ������ ȭ������ ��ȯ

            //ȸ������
            btnJoin.Click += Join; //ȸ������
            btnJoinClose.Click += MoveToLoginPage; //�α��� ȭ������ ��ȯ

            //�α׾ƿ� 
            btnChatListLogout.Click += Logout;

            //ģ�� �� ��ȭ ���
            tabControl.SelectedIndexChanged += new EventHandler(TabControlSelectionChanged);

            //��ȭ�� ���� 
            btnChat.Click += OpenChatRoomFromFriendList; //(��) ��ȭ�� ����
            lstChat.SelectedIndexChanged += OpenChatRoomFromChatList;  //(����) ��ȭ�� ���� 

            //��ȭ��
            btnMember.Click += MoveToMemberPage; //��ȭ �ɹ� ����
            btnInvite.Click += MoveToInvitePage; //�ʴ� ���� ģ�� ��� ���� 
            btnChatRoomExit.Click += ExitChatRoom; //��ȭ ��� ����
            btnChatRoomClose.Click += MoveToChatListPage;  //��ȭ ��� ����
            btnSendMsg.Click += SendMessage; //�޽��� ����
            btnSendFile.Click += SendFile; //���� ����
            btnDownload.Click += downloadFile; //���� �ٿ�ε�

            //�ɹ�����
            btnMemberClose.Click += MoveToTalkPage; //��ȭ�� ����

            //�ʴ��ϱ�
            btnInviting.Click += InviteFriend; //�ʴ��ϱ� 
            btnInviteClose.Click += MoveToTalkPage; //��ȭ�� ����

            _pages = new List<Control>
            {
                pnlLogin,
                pnlJoin,
                pnlMain,
                pnlTalk,
                pnlMemberList,
                pnlInvitingList
            };

            CurrentPage = PageType.Login;

            //��Ʈ��ũ ���� �ݹ� �̺�Ʈ ���
            _tcpNetwork.ResponseHandler = new ResponseHandler();
            _tcpNetwork.ResponseHandler.serverDownEventHandler += ServerDownEventHandlerFun; //���� ���� ������
            _tcpNetwork.ResponseHandler.loginEventHandler += LoginEventHandlerFun; //�α���
            _tcpNetwork.ResponseHandler.joinEventHandler += JoinEventHandlerFun; //ȸ������
            _tcpNetwork.ResponseHandler.logoutEventHandler += LogoutEventHandlerFun; //�α׾ƿ�
            _tcpNetwork.ResponseHandler.getChatListEventHandler += GetChatListEventHandlerFun; //��ȭ ��� ��������
            _tcpNetwork.ResponseHandler.getFriendListEventHandler += GetFriendListEventHandlerFun; //ģ�� ��� ��������
            _tcpNetwork.ResponseHandler.openChatRoomEventHandler += OpenChatRoomEventHandlerFun; //��ȭ�� ����
            _tcpNetwork.ResponseHandler.getPrevMsgsEventHandler += GetPreviousMessagesEventHandlerFun; //���� ��ȭ ��������
            _tcpNetwork.ResponseHandler.sendMsgEventHandler += SendMessageEventHandlerFun; //�޽��� ����
            _tcpNetwork.ResponseHandler.sendFileEventHandler += SendFileEventHandlerFun; //���� ����
            _tcpNetwork.ResponseHandler.receiveFileEventHandler += ReceivePrevMsgsEventHandlerFun; //���� ����
            _tcpNetwork.ResponseHandler.getInvitationListEventHandler += GetInvitationListEventHandlerFun; //�ʴ� ���� ģ�� ��� ��������
            _tcpNetwork.ResponseHandler.inviteEventHandler += InviteEventHandlerFun; //�ʴ� ���� ģ�� ��� ��������
            _tcpNetwork.ResponseHandler.getChatMemberEventHandler += GetChatMemberEventHandlerFun; //�ʴ� ���� ģ�� ��� ��������
            _tcpNetwork.ResponseHandler.exitChatRoomEventHandler += ExitChatRoomEventHandlerFun; //�ʴ� ���� ģ�� ��� ��������
        }

        /// <summary>
        /// ģ�� ��� �� ��ȭ ��� Tab Ŭ�� �̺�Ʈ ó��
        /// </summary>
        private void TabControlSelectionChanged(object sender, EventArgs e)
        {
            if ((sender is TabControl tab) && tab.SelectedTab != null)
            {
                switch (tab.SelectedTab.Text)
                {
                    case "ģ�� ���":
                        MoveToFriendList();
                        break;

                    case "��ȭ ���":
                        MoveToChatList();
                        break;
                }
            }
        }

        /// <summary>
        /// ���� ������ �������� �� �α׾ƿ� ó��
        /// </summary>
        private void ServerDownEventHandlerFun(object sender)
        {
            Invoke(LogoutSucceed);
        }

        /// <summary>
        /// �α��� ��ư Ŭ�� �� ����
        /// </summary>
        private void Login(object? sender, EventArgs e)
        {
            try
            {
                //���̵�� ��й�ȣ ��ȿ�� �˻�
                if (ValidationCheck.hasEmptyString(txtLoginID.Text) || ValidationCheck.hasEmptyString(txtLoginPW.Text))
                {
                    throw new InvalidateInputException("Invalidate input value");
                }

                //������ ���� �α��� ������ ����
                var login = new LoginClientData { Code = "login", Id = txtLoginID.Text, Pw = txtLoginPW.Text };

                //������ ���� ����(���� 1ȸ) 
                if (_tcpNetwork.Connect())
                {
                    //������ ������ ����
                    _tcpNetwork.Send(login);
                    //�����κ��� ������ ���� ����(���� 1ȸ)
                    _tcpNetwork.Receive();
                }
                else
                {
                    MessageBox.Show("������ ������ �����ֽ��ϴ�.");
                }
            }
            catch (InvalidateInputException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �����κ��� �α��� ���������� ���� ���ŵǾ��� �� ����
        /// </summary>
        /// <param name="loginData">���ŵ� �α��� ������</param>
        private void LoginEventHandlerFun(object sender, LoginServerData loginData)
        {
            try
            {
                if (loginData != null)
                {
                    if (loginData.IsTrue)
                    {
                        //Settings�� �� ���̵� ���� 
                        Settings.Default.id = loginData.Id;
                        Settings.Default.Save();

                        //�α��� �Ϸ� ó��
                        Invoke(() => LoginSucceed(loginData.Id));
                    }
                    else
                    {
                        MessageBox.Show(loginData.ResultMessage);
                        return;
                    }
                }
                else
                {
                    throw new ResponseErrorException("Response from server ERROR");
                }
            }
            catch (ResponseErrorException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �α��� ���� �� ��ȭ ������� ���� ����
        /// </summary>
        /// <param name="id">Settings�� ����Ǿ� �ִ� �� ���̵�</param>
        private void LoginSucceed(string id)
        {
            //���� ȭ������ ��ȯ
            CurrentPage = PageType.Main;

            //��ȭ ��� ���� Ŭ�� ó��
            tabControl.SelectedTab = chatListTab;

            //MoveToChatList();
        }

        /// <summary>
        /// �α��� �������� �̵�
        /// </summary>
        private void MoveToLoginPage(object? sender, EventArgs e)
        {
            CurrentPage = PageType.Login;
        }

        /// <summary>
        /// ȸ������ ��ư Ŭ�� �� ����
        /// </summary>
        private void Join(object? sender, EventArgs e)
        {
            try
            {
                //������ ���� ����(���� 1ȸ) 
                if (_tcpNetwork.Connect())
                {
                    //���̵�, ��й�ȣ, �̸� ��ȿ�� �˻�
                    if (!ValidationCheck.isValidId(txtJoinID.Text) ||
                        !ValidationCheck.isValidPw(txtJoinPW.Text) ||
                        !ValidationCheck.isValidName(txtJoinName.Text))
                    {
                        throw new InvalidateInputException("Invalidate input value");
                    }

                    //������ ���� ȸ������ ������ ����
                    var join = new JoinClientData { Code = "join", Id = txtJoinID.Text, Pw = txtJoinPW.Text, Name = txtJoinName.Text };

                    //������ ���� 
                    _tcpNetwork.Send(join);

                    //�����κ��� ������ ���� ����(���� 1ȸ)
                    _tcpNetwork.Receive();
                }
            }
            catch (InvalidateInputException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �����κ��� ȸ������ ���������� ���� ���ŵǾ��� �� ����
        /// </summary>
        /// <param name="joinData">���ŵ� ȸ������ ������</param>
        private void JoinEventHandlerFun(object sender, JoinServerData joinData)
        {
            try
            {
                if (joinData != null)
                {
                    if (joinData.IsTrue)
                    {
                        //ȸ������ ���� ó��
                        Invoke(JoinSucceed);
                    }
                    else
                    {
                        MessageBox.Show(joinData.ResultMessage);
                        return;
                    }
                }
                else
                {
                    throw new ResponseErrorException("Response from server ERROR");
                }
            }
            catch (ResponseErrorException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// ȸ������ ������ ���� ȭ�� ó��
        /// </summary>
        private void JoinSucceed()
        {
            MessageBox.Show("ȸ�������� �Ϸ�Ǿ����ϴ�.");

            //�α��� ȭ������ ���ư���
            CurrentPage = PageType.Login;
        }

        /// <summary>
        /// ȸ������ �������� �̵�
        /// </summary>
        private void MoveToJoinPage(object? sender, EventArgs e)
        {
            CurrentPage = PageType.Join;
        }

        /// <summary>
        /// �α׾ƿ� ��ư Ŭ�� �� ����
        /// </summary>
        private void Logout(object? sender, EventArgs e)
        {
            try
            {
                //Settings���� �� ���̵� ��������
                string id = Settings.Default.id;

                if (id != null)
                {
                    //������ ���� �α׾ƿ� ������ ����
                    var logout = new LogoutClientData { Code = "logout", Id = id };

                    //������ ������ ����
                    _tcpNetwork.Send(logout);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �����κ��� �α׾ƿ� ���������� ���� ���ŵǾ��� �� ����
        /// </summary>
        /// <param name="logoutData">���ŵ� �α׾ƿ� ������</param>
        private void LogoutEventHandlerFun(object sender, ChatData logoutData)
        {
            try
            {
                if (logoutData != null && logoutData.Code == "logout")
                {
                    //�α׾ƿ� ���� ó�� 
                    Invoke(LogoutSucceed);
                }
                else
                {
                    throw new ResponseErrorException("Response from server ERROR");
                }
            }
            catch (ResponseErrorException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �α׾ƿ� ������ ���� ȭ�� ó��
        /// </summary>
        private void LogoutSucceed()
        {
            //���� �������� �α��� �������� �ƴҶ��� �����ϵ���
            //�� �޼ҵ�� �α׾ƿ�, ���� ���� ���� ���� ��Ȳ���� �ߺ����� �Ҹ� �� �ֱ� ���� 
            if (CurrentPage != PageType.Login)
            {
                MessageBox.Show("�α׾ƿ� �Ǿ����ϴ�.");
                //�α��� ȭ������ ���ư���
                CurrentPage = PageType.Login;

                //��Ʈ��ũ ����
                _tcpNetwork.Close();
            }
        }

        /// <summary>
        /// ��ȭ�� ���� �� ��ȭ ����� ������
        /// </summary>
        private void MoveToChatListPage(object? sender, EventArgs e)
        {
            //���� ȭ������ ��ȯ
            CurrentPage = PageType.Main;

            //��ȭ ��� ���� Ŭ�� ó��
            tabControl.SelectedTab = chatListTab;

            //MoveToChatList();
        }

        /// <summary>
        /// ��ȭ ��� ��������
        /// </summary>
        private void MoveToChatList()
        {
            try
            {
                //Settings���� �� ���̵� ��������
                string id = Settings.Default.id;

                if (id != null)
                {
                    //������ ���� ��ȭ ��� �������� ������ ����
                    var chatList = new ChatListClientData { Code = "roomlist", Id = id };

                    //������ ����
                    _tcpNetwork.Send(chatList);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �����κ��� ��ȭ ��� �������� ���������� ���� ���ŵǾ��� �� ����
        /// </summary>
        /// <param name="chatListData">���ŵ� ��ȭ ��� ������ </param>
        private void GetChatListEventHandlerFun(object sender, ChatListServerData chatListData)
        {
            try
            {
                if (chatListData != null && chatListData.Code == "roomlist")
                {
                    //��ųʸ� ���·� ��ȭ�� ���� ����
                    //�� ��ȣ(int)
                    //��ȭ �ɹ�(string[])
                    Dictionary<int, string[]> chatRoomInfoDic = new Dictionary<int, string[]>();

                    //������ ��ȭ���� ���� ��
                    if (chatListData.ChatList == null)
                    {
                        //��ȭ ��� ȭ������ �̵�
                        //�ι�° �Ű������� false�� �־� �����ִ� ��ȭ���� ������ �˸� 
                        Invoke(() => GetChatListSucceed(chatRoomInfoDic, false));
                    }
                    else
                    {
                        //������ ��ȭ���� ���� �� 
                        //��ȭ �ɹ� ��� �������� 
                        //chatListData.ChatList ����: {"���ȣ, �ɹ�1, �ɹ�2, �ɹ�3"}
                        foreach (string str in chatListData.ChatList)
                        {
                            string[] strSpl = str.Split(",");

                            List<string> idList = new List<string>();

                            //0��° �ε������� ���ȣ�� �ֱ� ������ 1��° ������ list�� ����
                            for (int i = 1; i < strSpl.Length; i++)
                            {
                                idList.Add(strSpl[i]);
                            }

                            //�� ��ȣ(int)�� ��ȭ �ɹ�(string[]) ����
                            chatRoomInfoDic.Add(int.Parse(strSpl.First()), idList.ToArray());
                        }

                        //Settings�� ����ȭ�� ��ųʸ��� ���� 
                        Settings.Default.chatRoomsInfo = JsonConvert.SerializeObject(chatRoomInfoDic);
                        Settings.Default.Save();

                        //��ȭ ��� ȭ������ �̵�
                        //true�� �־� ���� ��ȭ���� ������ �˸�
                        Invoke(() => GetChatListSucceed(chatRoomInfoDic, true));
                    }
                }
                //���� �������� ��
                else
                {
                    throw new ResponseErrorException("Response from server ERROR");
                }
            }
            catch (ResponseErrorException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// ��ȭ ��� �������Ⱑ �����ߴٸ� ����
        /// </summary>
        /// <param name="chatRoomInfoDic">��ȭ�� ����(�� ��ȣ�� ��ȭ �ɹ��� ����)</param>
        /// <param name="isAnyChatRoom">��ȭ�� ���� ����</param>
        private void GetChatListSucceed(Dictionary<int, string[]> chatRoomInfoDic, bool isAnyChatRoom)
        {
            //��ȭ ��� �ʱ�ȭ
            lstChat.Items.Clear();

            //��ȭ ��� ǥ��
            foreach (KeyValuePair<int, string[]> pair in chatRoomInfoDic)
            {
                string[] idArr = pair.Value;
                string ids = "";

                //���ڿ� �迭���� ��ȭ �ɹ��� �Ѹ� �����ͼ� ������(',')�� �ձ�
                for (int i = 0; i < idArr.Length; i++)
                {
                    ids += idArr[i];

                    if (i != idArr.Length - 1)
                    {
                        ids += ", ";
                    }
                }

                lstChat.Items.Add(ids);
            }

            //Settings���� �� ���̵� �����ͼ� ȭ�鿡 ǥ��
            userId.Text = Settings.Default.id;

            //��ȭ�� ������ ���� �ȳ� Label ǥ��
            if (isAnyChatRoom)
                noChatRoom.Visible = false;
            else
                noChatRoom.Visible = true;
        }

        /// <summary>
        /// ģ�� ������� ȭ�� ��ȯ
        /// </summary>
        private void MoveToFriendListPage(object? sender, EventArgs e)
        {
            MoveToFriendList();
        }

        /// <summary>
        /// ģ�� ��� ��������
        /// </summary>
        private void MoveToFriendList()
        {
            try
            {
                //������ ���� ChatData ����
                var friendList = new ChatData { Code = "friendlist" };

                //������ ������ ����
                _tcpNetwork.Send(friendList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �����κ��� ģ�� ��� �������� ���������� ���� ���ŵǾ��� �� ����
        /// </summary>
        /// <param name="friendListData">���ŵ� ģ�� ��� ������</param>
        private void GetFriendListEventHandlerFun(object sender, FriendListServerData friendListData)
        {
            try
            {
                if (friendListData != null 
                 && friendListData.Code == "friendlist" 
                 && friendListData.LoginStateList.Length == friendListData.FriendList.Length)
                {
                    //���̵� ���
                    List<string> idList = new List<string>();
                    //�α��� ���� ���
                    List<string> loginStateList = new List<string>();

                    for (int i = 0; i < friendListData.FriendList.Length; i++)
                    {
                        //�� ���̵��� ���� �н�
                        if (friendListData.FriendList[i] == Settings.Default.id)
                        {
                            continue;
                        }

                        idList.Add(friendListData.FriendList[i]);
                        loginStateList.Add(friendListData.LoginStateList[i] ? "(�α���)" : "(�α׾ƿ�)");
                    }

                    //ģ�� ��� ȭ������ �̵�
                    Invoke(() => GetFriendListSucceed(idList, loginStateList));
                }
                //���� �������� ��
                else
                {
                    throw new ResponseErrorException("Response from server ERROR");
                }
            }
            catch (ResponseErrorException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// ģ�� ��� �������Ⱑ �������� �� ȭ�� ó��
        /// </summary>
        /// <param name="idList">���̵� ���</param>
        /// <param name="loginStateList">�α��� ���� ���</param>
        private void GetFriendListSucceed(List<string> idList, List<string> loginStateList)
        {
            //���� ȭ������ ��ȯ
            CurrentPage = PageType.Main;
            //ģ�� ��� �� Ŭ�� ó��
            tabControl.SelectedTab = friendListTab;

            //ģ�� ��� �ʱ�ȭ
            lstFriend.Items.Clear();
            //�α��� ���� ��� �ʱ�ȭ
            lstFriendLoginState.Items.Clear();

            //ģ�� ��� ǥ��
            lstFriend.Items.AddRange(idList.ToArray());
            //�α��� ���� ��� ǥ��
            lstFriendLoginState.Items.AddRange(loginStateList.ToArray());
        }

        /// <summary>
        /// ģ�� ��Ͽ��� ��ȭ�� ����
        /// </summary>
        private void OpenChatRoomFromFriendList(object? sender, EventArgs e)
        {
            //Settings���� �� ���̵� ��������
            string userId = Settings.Default.id;

            //���õ� ģ�� ���
            List<string> selectedIdList = new List<string>();

            //���õ� ģ���� ��������
            foreach (string item in lstFriend.SelectedItems)
            {
                selectedIdList.Add(item);
            }

            if (selectedIdList.Count == 0)
            {
                MessageBox.Show("���õ� ģ���� �����ϴ�.");
                return;
            }

            //Settings�� ����� ��ȭ�� ���� ��������
            Dictionary<int, string[]> chatRoomInfoDic =
                JsonConvert.DeserializeObject<Dictionary<int, string[]>>(Settings.Default.chatRoomsInfo);

            //���õ� ģ������ ��ȭ���� �̹� �����Ǿ� �ִ��� Ȯ���ϱ� ����
            foreach (KeyValuePair<int, string[]> rooNumWithIdArr in chatRoomInfoDic)
            {
                //�� ��ȣ �� ��ȭ �ɹ� ��������
                string[] storedIdArr = rooNumWithIdArr.Value;
                List<string> storedIdList = storedIdArr.ToList();

                //�ɹ� ��Ͽ��� ���� ����
                storedIdList.Remove(userId);

                //���ĺ� �� ������ ����
                Array.Sort(storedIdList.ToArray());
                Array.Sort(selectedIdList.ToArray());

                //Settings�� ����Ǿ� �ִ� ��ȭ �ɹ� ��ϰ� ���õ� ģ�� ��� �� ���� ���� �ִ��� Ȯ��
                if (storedIdList.SequenceEqual(selectedIdList))
                {
                    //������ �� ����
                    OpenExistChatRoom(selectedIdList.ToArray());
                    return;
                }
            }

            //������ �� ���� 
            if (userId != null)
            {
                //������ ���� ��ȭ�� ������ ����
                var chatRoomData = new ChatRoomClientData { Code = "chattingroom", Id = userId, RoomMember = selectedIdList.ToArray() };

                try
                {
                    //������ ������ ����
                    _tcpNetwork.Send(chatRoomData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// ���� ��ȭ�� ����
        /// </summary>
        /// <param name="idArr"></param>
        private void OpenExistChatRoom(string[] idArr)
        {
            //Settings�� ����� �� ���̵� ��������
            string userId = Settings.Default.id;

            //Settings�� ����� ��ȭ�� ���� ��������
            Dictionary<int, string[]> ChatRoomInfoDic =
                JsonConvert.DeserializeObject<Dictionary<int, string[]>>(Settings.Default.chatRoomsInfo);

            if (ChatRoomInfoDic != null && userId != null)
            {
                //��ȭ �ɹ� ������� �� ��ȣ ��������
                int roomNum = ChatRoomInfoDic.FirstOrDefault(x => x.Value.SequenceEqual(idArr)).Key;

                if (roomNum != null)
                {
                    //������ ���� ��ȭ�� ������ ����
                    var chatRoomData = new ChatRoomClientData { Code = "chattingroom", Id = userId, RoomNumber = roomNum };

                    try
                    {
                        //������ ������ ����
                        _tcpNetwork.Send(chatRoomData);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// ��ȭ ��Ͽ��� ��ȭ�� ����
        /// </summary>
        private void OpenChatRoomFromChatList(object? sender, EventArgs e)
        {
            try
            {
                //ListBox���� Ŭ���� �������� ���� ��
                if (lstChat.SelectedItem != null)
                {
                    //Ŭ���� ��ȭ�� �� ��������
                    //��ȭ�� ���� ��ȭ �ɹ��� �����Ǿ� ����
                    string ids = lstChat.SelectedItem.ToString();

                    string[] idArr = ids.Split(", ");

                    //���� ��ȭ�濡�� �ش� ��ȭ �ɹ��� �´� ��ȭ�� ����
                    OpenExistChatRoom(idArr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �����κ��� ��ȭ�� ���� ���������� ���� ���ŵǾ��� �� ����
        /// </summary>
        /// <param name="chatRoomData">���ŵ� ��ȭ�� ������</param>
        private void OpenChatRoomEventHandlerFun(object sender, ChatRoomServerData chatRoomData)
        {
            try
            {
                if (chatRoomData != null && chatRoomData.Code == "chattingroom")
                {
                    //Settings���� ��ȭ�� ���� ��������
                    Dictionary<int, string[]> ChatRoomInfoDic =
                        JsonConvert.DeserializeObject<Dictionary<int, string[]>>(Settings.Default.chatRoomsInfo);

                    if (chatRoomData.RoomNumber != null)
                    {
                        //������ ��ȭ�� �������� �ش� �� ��ȣ�� �̹� �ִ��� Ȯ��
                        if (ChatRoomInfoDic.ContainsKey(chatRoomData.RoomNumber))
                        {
                            //���� �� open
                            Debug.WriteLine("������");
                        }
                        else
                        {
                            //�� �� open
                            Debug.WriteLine("����");
                        }

                        //��ȭ�� ����
                        Invoke(() => OpenChatRoomSucceed(chatRoomData.RoomNumber));
                    }
                    else
                    {
                        throw new ResponseErrorException();
                    }
                }
                else
                {
                    throw new ResponseErrorException();
                }
            }
            catch (ResponseErrorException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// ��ȭ�� ���⸦ �������� �� ȭ�� ó��
        /// </summary>
        /// <param name="roomNumber"></param>
        private void OpenChatRoomSucceed(int roomNumber)
        {
            //��ȭ������ ȭ�� �̵�
            CurrentPage = PageType.Talk;

            //���� ��ȭ�� ��ȣ Settings�� ����
            Settings.Default.currentRoomNumber = roomNumber;
            Settings.Default.Save();

            //���� ��ȭ ��������
            GetPreviousMessages();
        }

        /// <summary>
        /// ���� ��ȭ ��������
        /// </summary>
        private void GetPreviousMessages()
        {
            //Settings���� �� ���̵� ��������
            string userId = Settings.Default.id;
            //Settings���� ���� �� ��ȣ ��������
            int roomNumber = Settings.Default.currentRoomNumber;

            if (roomNumber == 0)
            {
                Debug.WriteLine("��ȭ�� ��ȣ ã�� �� ����");
                return;
            }

            try
            {
                //������ ���� ���� ��ȭ ������ ����
                var prevMsgsData = new PrevMsgsClientData { Code = "past", Id = userId, RoomNumber = roomNumber };

                //������ ������ ����
                _tcpNetwork.Send(prevMsgsData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �����κ��� ���� ��ȭ �������� ���������� ���� ���ŵǾ��� �� ����
        /// </summary>
        /// <param name="prevMsgsData">���ŵ� ���� ��ȭ ������</param>
        private void GetPreviousMessagesEventHandlerFun(object sender, PrevMsgsServerData prevMsgsData)
        {
            try
            {
                if (prevMsgsData != null && prevMsgsData.Code == "past")
                {
                    if (prevMsgsData.IsTrue)
                    {
                        //���� ������ �о����
                        _tcpNetwork.ReceiveFile(prevMsgsData.FileSize);
                    }
                    else
                    {
                        throw new ResponseErrorException("���� ��ȭ�� �����ϴ�.");
                    } 
                }
                else
                {
                    throw new ResponseErrorException("Response from server ERROR");
                }
            }
            catch (ResponseErrorException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �����κ��� ���� ��ȭ ������ ���������� ���ŵǾ��� �� ����
        /// </summary>
        /// <param name="fileBytes">���� ������</param>
        private void ReceivePrevMsgsEventHandlerFun(object sender, byte[] fileBytes)
        {
            try
            {
                if (fileBytes != null)
                {
                    try
                    {
                        //���ŵ� �����͸� �� ���� ����
                        string filePath = Directory.GetCurrentDirectory() + "\\past.txt";

                        //�̹� �ش� ��ο� �����ϴ� �����̸� 
                        if (File.Exists(filePath))
                        {
                            Debug.WriteLine(filePath + " ������ �̹� �����մϴ�.");
                            return;
                        }

                        if (!string.IsNullOrEmpty(filePath))
                        {
                            //�ش� ����� ���Ͽ� ���ŵ� ������ ����
                            FileStream fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                            fs.Write(fileBytes, 0, fileBytes.Length);
                            fs.Close();

                            //���Ͽ� ������ �����͸� �ؽ�Ʈ�� �о���̱� 
                            string txt = File.ReadAllText(filePath);

                            //���� ��ȭ ��� �ҷ����� ���� ó��
                            Invoke(() => GetPreviousMessagesSucceed(txt));

                            //������ ������ ����
                            File.Delete(filePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    throw new ResponseErrorException("Response from server ERROR");
                }
            }
            catch (ResponseErrorException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// ���� ��ȭ �ҷ����� ������ ���� ȭ�� ó��
        /// </summary>
        /// <param name="rawMsg">���� ��ȭ ����</param>
        private void GetPreviousMessagesSucceed(string rawMsg)
        {
            //���� ��ȭ �ʱ�ȭ 
            lstTalk.Items.Clear();

            string[] msgArr = rawMsg.Split("\n");

            List<string> messages = new List<string>();

            foreach (string msg in msgArr)
            {
                messages.Add(msg.Replace(",", ": "));
            }

            lstTalk.Items.AddRange(messages.ToArray());
        }

        /// <summary>
        /// ��ȭ������ �̵�
        /// </summary>
        private void MoveToTalkPage(object? sender, EventArgs e)
        {
            CurrentPage = PageType.Talk;
        }

        /// <summary>
        /// �޽��� ����
        /// </summary>
        private void SendMessage(object? sender, EventArgs e)
        {
            string sendTxt = txtMessage.Text;

            if (sendTxt.Length == 0)
            {
                MessageBox.Show("�޽����� �Է����ּ���.");
                return;
            }

            //�� ���̵� ��������
            string id = Settings.Default.id;

            if (id == null)
            {
                Debug.WriteLine("���̵� ã�� �� ����");
                return;
            }

            //���� �� ��ȣ ��������
            int roomNumber = Settings.Default.currentRoomNumber;

            if (roomNumber == 0)
            {
                Debug.WriteLine("��ȭ�� ��ȣ ã�� �� ����");
                return;
            }

            //������ ���� �޽��� ������ ����
            var chatMsgData = new MsgClientData { Code = "msg", Id = id, RoomNumber = roomNumber, Message = sendTxt };

            try
            {
                //������ ������ ����
                _tcpNetwork.Send(chatMsgData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �����κ��� �޽��� ���� ���������� ���� ���ŵǾ��� �� ����
        /// </summary>
        /// <param name="msgData">���ŵ� �޽��� ������</param>
        private void SendMessageEventHandlerFun(object sender, MsgServerData msgData)
        {
            try
            {
                if (msgData != null)
                {
                    //�޽��� ���� ���� ó��
                    Invoke(() => SendMessageSucceed(msgData.Id, msgData.Message));
                }
                else
                {
                    throw new ResponseErrorException("Response from server ERROR");
                }
            }
            catch (ResponseErrorException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �޽��� ���� ������ ���� ȭ�� ó��
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        private void SendMessageSucceed(string id, string msg)
        {
            lstTalk.Items.Add(id + ": " + msg);
        }

        /// <summary>
        /// ���� ������
        /// </summary>
        private void SendFile(object? sender, EventArgs e)
        {
            //��ũ�� �ִ� ���� �ҷ�����
            openFileDialog.InitialDirectory = "C:\\";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                FileInfo fileInfo = new FileInfo(filePath);

                //�� ���̵� ��������
                string id = Settings.Default.id;

                if (id == null)
                {
                    Debug.WriteLine("���̵� ã�� �� ����");
                    return;
                }

                //���� �� ��ȣ ��������
                int roomNumber = Settings.Default.currentRoomNumber;

                if (roomNumber == 0)
                {
                    Debug.WriteLine("��ȭ�� ��ȣ ã�� �� ����");
                    return;
                }

                //������ ���� �޽��� ������ ����
                var fileData  = new FileClientData { 
                    Code = "file", 
                    Id = id,
                    RoomNumber = roomNumber,
                    FileName = filePath, 
                    FileSize = fileInfo.Length,
                    FileCreatedAt = fileInfo.CreationTime,
                    FileEditedAt = fileInfo.LastWriteTime 
                };

                try
                {
                    //���� ���� ���� ����
                    _tcpNetwork.Send(fileData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// �����κ��� ���� ���� ���������� ���� ���ŵǾ��� �� ����
        /// </summary>
        /// <param name="fileData">���ŵ� ���� ������</param>
        private void SendFileEventHandlerFun(object sender, FileServerData fileData)
        {
            byte[] fileBytes;

            if (File.Exists(fileData.FileName))
            {
                using (FileStream fileStream = new FileStream(fileData.FileName, FileMode.Open, FileAccess.Read))
                {
                    int fileLength = (int)fileStream.Length;

                    using (BinaryReader reader = new BinaryReader(fileStream))
                    {
                        fileBytes = reader.ReadBytes(fileLength);
                    }
                }

                _tcpNetwork.SendFile(fileBytes);
            }
            else
            {
                
            }
        }

        /// <summary>
        /// ���� ���� ���� �� ȭ�� ó��
        /// </summary>
        private void SendFileSucceed()
        {

        }

        /// <summary>
        /// ���� �ٿ�ε� 
        /// </summary>
        private void downloadFile(object? sender, EventArgs e)
        {

        }
        
        /// <summary>
        /// �ʴ��ϱ� ȭ������ �̵�
        /// </summary>
        private void MoveToInvitePage(object? sender, EventArgs e)
        {
            CurrentPage = PageType.InvitingList;

            //�ʴ� ���� ģ�� ��� �������� 
            GetInvitationList();
        }

        /// <summary>
        /// �ʴ� ������ ģ�� ��� ��������
        /// </summary>
        private void GetInvitationList()
        {
            try
            {
                //���� �� ��ȣ ��������
                int roomNumber = Settings.Default.currentRoomNumber;

                //������ ���� �ʴ� ���� ģ�� ��� �������� ������ ����
                var invitationListData = new InvitationListClientData { Code = "invitelist", RoomNumber = roomNumber };

                //������ ������ ����
                _tcpNetwork.Send(invitationListData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �����κ��� �ʴ� ���� ģ�� ��� �������� ���������� ���� ���ŵǾ��� �� ����
        /// </summary>
        /// <param name="invitationListData">���ŵ� �ʴ� ���� ģ�� ���</param>
        private void GetInvitationListEventHandlerFun(object sender, InvitationListServerData invitationListData)
        {
            try
            {
                if (invitationListData != null && invitationListData.Code == "invitelist")
                {
                    //�ʴ� ���� ģ�� ��� �������� ���� ó��
                    Invoke(() => GetInvitationListSucceed(invitationListData.LoginStateList, invitationListData.FriendList));
                }
                else
                {
                    throw new ResponseErrorException("Response from server ERROR");
                }
            }
            catch (ResponseErrorException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �ʴ� ���� ģ�� ��� �������� ������ ���� ȭ�� ó��
        /// </summary>
        /// <param name="isLoginList"></param>
        /// <param name="friendList"></param>
        private void GetInvitationListSucceed(bool[] isLoginList, string[] friendList)
        {
            if (isLoginList.Length != friendList.Length)
            {
                MessageBox.Show("�ʴ� ���� ģ�� ��� ������ ����");
                return;
            }

            List<string> loginStateList = new List<string>();

            foreach (bool loginState in isLoginList)
            {
                loginStateList.Add(loginState ? "(�α���)" : "(�α׾ƿ�)");
            }

            //�ʴ� ���� ģ�� ��� ǥ��
            lstInvitation.Items.AddRange(friendList);
            //�α��� ���� ��� ǥ��
            lstInvitationLoginState.Items.AddRange(loginStateList.ToArray());
        }

        /// <summary>
        /// �ʴ��ϱ�
        /// </summary>
        private void InviteFriend(object? sender, EventArgs e)
        {
            //TODO: ���� ��ȭ �ɹ��� Ŭ�� �ȵǵ��� ���� 

            if (lstInvitation.SelectedItem == null)
            {
                MessageBox.Show("���õ� ģ���� �����ϴ�.");
                return;
            }

            //���� �� ��ȣ ��������
            int roomNumber = Settings.Default.currentRoomNumber;

            //�� ���̵� ��������
            string userId = Settings.Default.id;

            //���õ� ģ�� ��� ��������
            List<string> selectedIdList = new List<string>();

            foreach (string id in lstInvitation.SelectedItems)
            {
                selectedIdList.Add(id);
            }

            //������ ���� �ʴ� ������ ����
            var inviteData = new InviteClientData { Code = "invite", Id = userId, RoomNumber = roomNumber, RoomMember = selectedIdList.ToArray() };

            try
            {
                //������ ������ ����
                _tcpNetwork.Send(inviteData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �����κ��� �ʴ��ϱ� ���������� ���� ���ŵǾ��� �� ����
        /// </summary>
        /// <param name="inviteData">���ŵ� �ʴ� ������</param>
        private void InviteEventHandlerFun(object sender, ChatRoomServerData inviteData)
        {
            try
            {
                if (inviteData != null && inviteData.Code == "invite")
                {
                    if (inviteData.RoomNumber == Settings.Default.currentRoomNumber)
                    {
                        //�ʴ� ���� ó��
                        Invoke(InviteFriendSucceed);
                    }
                    else
                    {
                        throw new ResponseErrorException("�ʴ밡 �������� �ʾҽ��ϴ�.");
                    }
                }
                //���� �������� ��
                else
                {
                    throw new ResponseErrorException("Response from server ERROR");
                }
            }
            catch (ResponseErrorException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �ʴ� ������ ���� ȭ�� ó��
        /// </summary>
        private void InviteFriendSucceed()
        {
            MessageBox.Show("�ʴ�Ǿ����ϴ�.");
        }

        /// <summary>
        /// ��ȭ �ɹ� ����
        /// </summary>
        private void MoveToMemberPage(object? sender, EventArgs e)
        {
            //��ȭ �ɹ� ȭ������ ��ȯ
            CurrentPage = PageType.MemberList;

            //��ȭ �ɹ� �ҷ����� 
            GetChatMemberList();
        }

        /// <summary>
        /// ��ȭ �ɹ� ��������
        /// </summary>
        private void GetChatMemberList()
        {
            //���� �� ��ȣ ��������
            int roomNumber = Settings.Default.currentRoomNumber;

            //������ ���� ��ȭ �ɹ� �������� ������ ����
            var chatMemberData = new ChatMemberClientData { Code = "chattingmember", RoomNumber = roomNumber };

            try
            {
                //������ ������ ����
                _tcpNetwork.Send(chatMemberData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �����κ��� ��ȭ �ɹ� �������� ���������� ���� ���ŵǾ��� �� ����
        /// </summary>
        /// <param name="chatMemberData">���ŵ� ��ȭ �ɹ� ������</param>
        private void GetChatMemberEventHandlerFun(object sender, ChatMemberServerData chatMemberData)
        {
            try
            {
                if (chatMemberData != null && chatMemberData.Code == "chattingmember")
                {
                    //��ȭ �ɹ� �������� ���� ó��
                    Invoke(() => GetChatMemberSucceed(chatMemberData.LoginStateList, chatMemberData.FriendList));
                }
                else
                {
                    throw new ResponseErrorException("Response from server ERROR");
                }
            }
            catch (ResponseErrorException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// ��ȭ �ɹ� �������� ������ ���� ȭ�� ó��
        /// </summary>
        /// <param name="isLoginList">�α��� ���� ���</param>
        /// <param name="friendList">�ɹ� ���</param>
        private void GetChatMemberSucceed(bool[] isLoginList, string[] friendList)
        {
            List<string> loginStateList = new List<string>();

            if (isLoginList != null)
            {
                foreach (bool loginState in isLoginList)
                {
                    loginStateList.Add(loginState ? "(�α���)" : "(�α׾ƿ�)");
                }
            }

            //��ȭ �ɹ� ��� ǥ��
            lstMember.Items.AddRange(friendList);
            //�α��� ���� ��� ǥ��
            lstMemberLoginState.Items.AddRange(loginStateList.ToArray());
        }

        /// <summary>
        /// ��ȭ�� ������
        /// </summary>
        private void ExitChatRoom(object? sender, EventArgs e)
        {
            //���� �� ��ȣ ��������
            int roomNumber = Settings.Default.currentRoomNumber;
            //�� ���̵� ��������
            string userId = Settings.Default.id;

            //������ ���� �� ������ ������ ����
            var exitChatRoomData = new ExitChatRoomClientData { Code = "exit", Id = userId, RoomNumber = roomNumber };

            try
            {
                //������ ������ ����
                _tcpNetwork.Send(exitChatRoomData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �����κ��� ��ȭ�� ������ ���������� ���� ���ŵǾ��� �� ����
        /// </summary>
        /// <param name="exitChatRoomData">���ŵ� ��ȭ�� ������ ������</param>
        private void ExitChatRoomEventHandlerFun(object sender, ExitChatRoomServerData exitChatRoomData)
        {
            try
            {
                if (exitChatRoomData != null && exitChatRoomData.Code == "exit")
                {
                    Invoke(ExitRoomSucceed);
                }
                else
                {
                    throw new ResponseErrorException("Response from server ERROR");
                }
            }
            catch (ResponseErrorException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// �� ������ �� ȭ�� ó��
        /// </summary>
        private void ExitRoomSucceed()
        {
            //���� ȭ������ ��ȯ
            CurrentPage = PageType.Main;

            //��ȭ ��� ���� Ŭ�� ó��
            tabControl.SelectedTab = chatListTab;

            //MoveToChatList();
        }
    }
}