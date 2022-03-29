

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

            //로그인
            Login,

            //회원가입
            Join,

            //친구 및 대화 목록 
            Main,

            //대화방
            Talk,

            //대화 맴버 목록
            MemberList,

            //초대 가능 친구 목록
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
                            case "로그인":
                                txtLoginID.Text = String.Empty;
                                txtLoginPW.Text = String.Empty;
                                break;

                            case "회원가입":
                                txtJoinID.Text = String.Empty;
                                txtJoinPW.Text = String.Empty;
                                txtJoinName.Text = String.Empty;
                                break;

                            case "대화방":
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
        /// 생성자
        /// UI 초기 설정 및 네트워크 수신 관련 이벤트 핸들러 등록
        /// </summary>
        public FormClient()
        {
            InitializeComponent();

            //각 Panel 초기 설정
            pnlLogin.Dock = DockStyle.Fill;
            pnlJoin.Dock = DockStyle.Fill;
            pnlMain.Dock = DockStyle.Fill;
            pnlTalk.Dock = DockStyle.Fill;
            pnlMemberList.Dock = DockStyle.Fill;
            pnlInvitingList.Dock = DockStyle.Fill;

            //Panel 전환 설정
            pnlLogin.Tag = new PageInfo(PageType.Login, "로그인", new Size(MAX_WIDTH, MAX_HEIGHT)) { FirstControl = txtLoginID };
            pnlJoin.Tag = new PageInfo(PageType.Join, "회원가입", new Size(MAX_WIDTH, MAX_HEIGHT)) { FirstControl = txtJoinID };
            pnlMain.Tag = new PageInfo(PageType.Main, "메인", new Size(MAX_WIDTH, MAX_HEIGHT)) { FirstControl = lstChat };
            pnlTalk.Tag = new PageInfo(PageType.Talk, "대화방", new Size(MAX_WIDTH, MAX_HEIGHT)) { FirstControl = txtMessage };
            pnlMemberList.Tag = new PageInfo(PageType.MemberList, "대화 맴버", new Size(MAX_WIDTH, MAX_HEIGHT)) { FirstControl = btnMemberClose };
            pnlInvitingList.Tag = new PageInfo(PageType.InvitingList, "초대 가능 친구", new Size(MAX_WIDTH, MAX_HEIGHT)) { };

            //창 기본 세팅
            Width = MIN_WIDTH;
            Height = MIN_HEIGHT;

            MinimumSize = new Size(MIN_WIDTH, MIN_HEIGHT);
            MaximumSize = new Size(MAX_WIDTH, MAX_HEIGHT);

            StartPosition = FormStartPosition.CenterScreen;

            //UI 이벤트 연결
            //로그인
            btnLogin.Click += Login; //로그인
            btnMoveToJoin.Click += MoveToJoinPage; //회원가입 화면으로 전환

            //회원가입
            btnJoin.Click += Join; //회원가입
            btnJoinClose.Click += MoveToLoginPage; //로그인 화면으로 전환

            //로그아웃 
            btnChatListLogout.Click += Logout;

            //친구 및 대화 목록
            tabControl.SelectedIndexChanged += new EventHandler(TabControlSelectionChanged);

            //대화방 열기 
            btnChat.Click += OpenChatRoomFromFriendList; //(새) 대화방 열기
            lstChat.SelectedIndexChanged += OpenChatRoomFromChatList;  //(기존) 대화방 열기 

            //대화방
            btnMember.Click += MoveToMemberPage; //대화 맴버 보기
            btnInvite.Click += MoveToInvitePage; //초대 가능 친구 목록 보기 
            btnChatRoomExit.Click += ExitChatRoom; //대화 목록 보기
            btnChatRoomClose.Click += MoveToChatListPage;  //대화 목록 보기
            btnSendMsg.Click += SendMessage; //메시지 전송
            btnSendFile.Click += SendFile; //파일 전송
            btnDownload.Click += downloadFile; //파일 다운로드

            //맴버보기
            btnMemberClose.Click += MoveToTalkPage; //대화방 보기

            //초대하기
            btnInviting.Click += InviteFriend; //초대하기 
            btnInviteClose.Click += MoveToTalkPage; //대화방 보기

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

            //네트워크 수신 콜백 이벤트 등록
            _tcpNetwork.ResponseHandler = new ResponseHandler();
            _tcpNetwork.ResponseHandler.serverDownEventHandler += ServerDownEventHandlerFun; //서버 연결 끊어짐
            _tcpNetwork.ResponseHandler.loginEventHandler += LoginEventHandlerFun; //로그인
            _tcpNetwork.ResponseHandler.joinEventHandler += JoinEventHandlerFun; //회원가입
            _tcpNetwork.ResponseHandler.logoutEventHandler += LogoutEventHandlerFun; //로그아웃
            _tcpNetwork.ResponseHandler.getChatListEventHandler += GetChatListEventHandlerFun; //대화 목록 가져오기
            _tcpNetwork.ResponseHandler.getFriendListEventHandler += GetFriendListEventHandlerFun; //친구 목록 가져오기
            _tcpNetwork.ResponseHandler.openChatRoomEventHandler += OpenChatRoomEventHandlerFun; //대화방 열기
            _tcpNetwork.ResponseHandler.getPrevMsgsEventHandler += GetPreviousMessagesEventHandlerFun; //이전 대화 가져오기
            _tcpNetwork.ResponseHandler.sendMsgEventHandler += SendMessageEventHandlerFun; //메시지 전송
            _tcpNetwork.ResponseHandler.sendFileEventHandler += SendFileEventHandlerFun; //파일 전송
            _tcpNetwork.ResponseHandler.receiveFileEventHandler += ReceivePrevMsgsEventHandlerFun; //파일 수신
            _tcpNetwork.ResponseHandler.getInvitationListEventHandler += GetInvitationListEventHandlerFun; //초대 가능 친구 목록 가져오기
            _tcpNetwork.ResponseHandler.inviteEventHandler += InviteEventHandlerFun; //초대 가능 친구 목록 가져오기
            _tcpNetwork.ResponseHandler.getChatMemberEventHandler += GetChatMemberEventHandlerFun; //초대 가능 친구 목록 가져오기
            _tcpNetwork.ResponseHandler.exitChatRoomEventHandler += ExitChatRoomEventHandlerFun; //초대 가능 친구 목록 가져오기
        }

        /// <summary>
        /// 친구 목록 및 대화 목록 Tab 클릭 이벤트 처리
        /// </summary>
        private void TabControlSelectionChanged(object sender, EventArgs e)
        {
            if ((sender is TabControl tab) && tab.SelectedTab != null)
            {
                switch (tab.SelectedTab.Text)
                {
                    case "친구 목록":
                        MoveToFriendList();
                        break;

                    case "대화 목록":
                        MoveToChatList();
                        break;
                }
            }
        }

        /// <summary>
        /// 서버 연결이 끊어졌을 때 로그아웃 처리
        /// </summary>
        private void ServerDownEventHandlerFun(object sender)
        {
            Invoke(LogoutSucceed);
        }

        /// <summary>
        /// 로그인 버튼 클릭 시 실행
        /// </summary>
        private void Login(object? sender, EventArgs e)
        {
            try
            {
                //아이디와 비밀번호 유효성 검사
                if (ValidationCheck.hasEmptyString(txtLoginID.Text) || ValidationCheck.hasEmptyString(txtLoginPW.Text))
                {
                    throw new InvalidateInputException("Invalidate input value");
                }

                //서버로 보낼 로그인 데이터 생성
                var login = new LoginClientData { Code = "login", Id = txtLoginID.Text, Pw = txtLoginPW.Text };

                //서버와 연결 설정(최초 1회) 
                if (_tcpNetwork.Connect())
                {
                    //서버로 데이터 전송
                    _tcpNetwork.Send(login);
                    //서버로부터 데이터 수신 설정(최초 1회)
                    _tcpNetwork.Receive();
                }
                else
                {
                    MessageBox.Show("서버와 연결이 끊겨있습니다.");
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
        /// 서버로부터 로그인 리스폰스가 정상 수신되었을 때 동작
        /// </summary>
        /// <param name="loginData">수신된 로그인 데이터</param>
        private void LoginEventHandlerFun(object sender, LoginServerData loginData)
        {
            try
            {
                if (loginData != null)
                {
                    if (loginData.IsTrue)
                    {
                        //Settings에 내 아이디 저장 
                        Settings.Default.id = loginData.Id;
                        Settings.Default.Save();

                        //로그인 완료 처리
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
        /// 로그인 성공 시 대화 목록으로 가기 위함
        /// </summary>
        /// <param name="id">Settings에 저장되어 있는 내 아이디</param>
        private void LoginSucceed(string id)
        {
            //메인 화면으로 전환
            CurrentPage = PageType.Main;

            //대화 목록 탭을 클릭 처리
            tabControl.SelectedTab = chatListTab;

            //MoveToChatList();
        }

        /// <summary>
        /// 로그인 페이지로 이동
        /// </summary>
        private void MoveToLoginPage(object? sender, EventArgs e)
        {
            CurrentPage = PageType.Login;
        }

        /// <summary>
        /// 회원가입 버튼 클릭 시 실행
        /// </summary>
        private void Join(object? sender, EventArgs e)
        {
            try
            {
                //서버와 연결 설정(최초 1회) 
                if (_tcpNetwork.Connect())
                {
                    //아이디, 비밀번호, 이름 유효성 검사
                    if (!ValidationCheck.isValidId(txtJoinID.Text) ||
                        !ValidationCheck.isValidPw(txtJoinPW.Text) ||
                        !ValidationCheck.isValidName(txtJoinName.Text))
                    {
                        throw new InvalidateInputException("Invalidate input value");
                    }

                    //서버로 보낼 회원가입 데이터 생성
                    var join = new JoinClientData { Code = "join", Id = txtJoinID.Text, Pw = txtJoinPW.Text, Name = txtJoinName.Text };

                    //서버로 전송 
                    _tcpNetwork.Send(join);

                    //서버로부터 데이터 수신 설정(최초 1회)
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
        /// 서버로부터 회원가입 리스폰스가 정상 수신되었을 때 동작
        /// </summary>
        /// <param name="joinData">수신된 회원가입 데이터</param>
        private void JoinEventHandlerFun(object sender, JoinServerData joinData)
        {
            try
            {
                if (joinData != null)
                {
                    if (joinData.IsTrue)
                    {
                        //회원가입 성공 처리
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
        /// 회원가입 성공에 따른 화면 처리
        /// </summary>
        private void JoinSucceed()
        {
            MessageBox.Show("회원가입이 완료되었습니다.");

            //로그인 화면으로 돌아가기
            CurrentPage = PageType.Login;
        }

        /// <summary>
        /// 회원가입 페이지로 이동
        /// </summary>
        private void MoveToJoinPage(object? sender, EventArgs e)
        {
            CurrentPage = PageType.Join;
        }

        /// <summary>
        /// 로그아웃 버튼 클릭 시 실행
        /// </summary>
        private void Logout(object? sender, EventArgs e)
        {
            try
            {
                //Settings에서 내 아이디 가져오기
                string id = Settings.Default.id;

                if (id != null)
                {
                    //서버로 보낼 로그아웃 데이터 생성
                    var logout = new LogoutClientData { Code = "logout", Id = id };

                    //서버로 데이터 전송
                    _tcpNetwork.Send(logout);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 서버로부터 로그아웃 리스폰스가 정상 수신되었을 때 동작
        /// </summary>
        /// <param name="logoutData">수신된 로그아웃 데이터</param>
        private void LogoutEventHandlerFun(object sender, ChatData logoutData)
        {
            try
            {
                if (logoutData != null && logoutData.Code == "logout")
                {
                    //로그아웃 성공 처리 
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
        /// 로그아웃 성공에 따른 화면 처리
        /// </summary>
        private void LogoutSucceed()
        {
            //현재 페이지가 로그인 페이지가 아닐때만 동작하도록
            //본 메소드는 로그아웃, 서버 연결 끊김 등의 상황에서 중복으로 불릴 수 있기 때문 
            if (CurrentPage != PageType.Login)
            {
                MessageBox.Show("로그아웃 되었습니다.");
                //로그인 화면으로 돌아가기
                CurrentPage = PageType.Login;

                //네트워크 종료
                _tcpNetwork.Close();
            }
        }

        /// <summary>
        /// 대화방 끄기 시 대화 목록을 보여줌
        /// </summary>
        private void MoveToChatListPage(object? sender, EventArgs e)
        {
            //메인 화면으로 전환
            CurrentPage = PageType.Main;

            //대화 목록 탭을 클릭 처리
            tabControl.SelectedTab = chatListTab;

            //MoveToChatList();
        }

        /// <summary>
        /// 대화 목록 가져오기
        /// </summary>
        private void MoveToChatList()
        {
            try
            {
                //Settings에서 내 아이디 가져오기
                string id = Settings.Default.id;

                if (id != null)
                {
                    //서버로 보낼 대화 목록 가져오기 데이터 생성
                    var chatList = new ChatListClientData { Code = "roomlist", Id = id };

                    //서버로 전송
                    _tcpNetwork.Send(chatList);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 서버로부터 대화 목록 가져오기 리스폰스가 정상 수신되었을 때 동작
        /// </summary>
        /// <param name="chatListData">수신된 대화 목록 데이터 </param>
        private void GetChatListEventHandlerFun(object sender, ChatListServerData chatListData)
        {
            try
            {
                if (chatListData != null && chatListData.Code == "roomlist")
                {
                    //딕셔너리 형태로 대화방 정보 저장
                    //방 번호(int)
                    //대화 맴버(string[])
                    Dictionary<int, string[]> chatRoomInfoDic = new Dictionary<int, string[]>();

                    //개설된 대화방이 없을 때
                    if (chatListData.ChatList == null)
                    {
                        //대화 목록 화면으로 이동
                        //두번째 매개변수에 false를 주어 열려있는 대화방이 없음을 알림 
                        Invoke(() => GetChatListSucceed(chatRoomInfoDic, false));
                    }
                    else
                    {
                        //개설된 대화방이 있을 때 
                        //대화 맴버 목록 가져오기 
                        //chatListData.ChatList 예시: {"방번호, 맴버1, 맴버2, 맴버3"}
                        foreach (string str in chatListData.ChatList)
                        {
                            string[] strSpl = str.Split(",");

                            List<string> idList = new List<string>();

                            //0번째 인덱스에는 방번호가 있기 때문에 1번째 값부터 list에 저장
                            for (int i = 1; i < strSpl.Length; i++)
                            {
                                idList.Add(strSpl[i]);
                            }

                            //방 번호(int)와 대화 맴버(string[]) 저장
                            chatRoomInfoDic.Add(int.Parse(strSpl.First()), idList.ToArray());
                        }

                        //Settings에 직렬화한 딕셔너리를 저장 
                        Settings.Default.chatRoomsInfo = JsonConvert.SerializeObject(chatRoomInfoDic);
                        Settings.Default.Save();

                        //대화 목록 화면으로 이동
                        //true를 주어 열린 대화방이 있음을 알림
                        Invoke(() => GetChatListSucceed(chatRoomInfoDic, true));
                    }
                }
                //응답 비정상일 때
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
        /// 대화 목록 가져오기가 성공했다면 동작
        /// </summary>
        /// <param name="chatRoomInfoDic">대화방 정보(방 번호와 대화 맴버로 구성)</param>
        /// <param name="isAnyChatRoom">대화방 존재 유무</param>
        private void GetChatListSucceed(Dictionary<int, string[]> chatRoomInfoDic, bool isAnyChatRoom)
        {
            //대화 목록 초기화
            lstChat.Items.Clear();

            //대화 목록 표시
            foreach (KeyValuePair<int, string[]> pair in chatRoomInfoDic)
            {
                string[] idArr = pair.Value;
                string ids = "";

                //문자열 배열에서 대화 맴버를 한명씩 가져와서 구분자(',')로 잇기
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

            //Settings에서 내 아이디 가져와서 화면에 표시
            userId.Text = Settings.Default.id;

            //대화방 유무에 따른 안내 Label 표시
            if (isAnyChatRoom)
                noChatRoom.Visible = false;
            else
                noChatRoom.Visible = true;
        }

        /// <summary>
        /// 친구 목록으로 화면 전환
        /// </summary>
        private void MoveToFriendListPage(object? sender, EventArgs e)
        {
            MoveToFriendList();
        }

        /// <summary>
        /// 친구 목록 가져오기
        /// </summary>
        private void MoveToFriendList()
        {
            try
            {
                //서버로 보낼 ChatData 생성
                var friendList = new ChatData { Code = "friendlist" };

                //서버로 데이터 전송
                _tcpNetwork.Send(friendList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 서버로부터 친구 목록 가져오기 리스폰스가 정상 수신되었을 때 동작
        /// </summary>
        /// <param name="friendListData">수신된 친구 목록 데이터</param>
        private void GetFriendListEventHandlerFun(object sender, FriendListServerData friendListData)
        {
            try
            {
                if (friendListData != null 
                 && friendListData.Code == "friendlist" 
                 && friendListData.LoginStateList.Length == friendListData.FriendList.Length)
                {
                    //아이디 목록
                    List<string> idList = new List<string>();
                    //로그인 상태 목록
                    List<string> loginStateList = new List<string>();

                    for (int i = 0; i < friendListData.FriendList.Length; i++)
                    {
                        //내 아이디일 때는 패스
                        if (friendListData.FriendList[i] == Settings.Default.id)
                        {
                            continue;
                        }

                        idList.Add(friendListData.FriendList[i]);
                        loginStateList.Add(friendListData.LoginStateList[i] ? "(로그인)" : "(로그아웃)");
                    }

                    //친구 목록 화면으로 이동
                    Invoke(() => GetFriendListSucceed(idList, loginStateList));
                }
                //응답 비정상일 때
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
        /// 친구 목록 가져오기가 성공했을 때 화면 처리
        /// </summary>
        /// <param name="idList">아이디 목록</param>
        /// <param name="loginStateList">로그인 상태 목록</param>
        private void GetFriendListSucceed(List<string> idList, List<string> loginStateList)
        {
            //메인 화면으로 전환
            CurrentPage = PageType.Main;
            //친구 목록 탭 클릭 처리
            tabControl.SelectedTab = friendListTab;

            //친구 목록 초기화
            lstFriend.Items.Clear();
            //로그인 상태 목록 초기화
            lstFriendLoginState.Items.Clear();

            //친구 목록 표시
            lstFriend.Items.AddRange(idList.ToArray());
            //로그인 상태 목록 표시
            lstFriendLoginState.Items.AddRange(loginStateList.ToArray());
        }

        /// <summary>
        /// 친구 목록에서 대화방 열기
        /// </summary>
        private void OpenChatRoomFromFriendList(object? sender, EventArgs e)
        {
            //Settings에서 내 아이디 가져오기
            string userId = Settings.Default.id;

            //선택된 친구 목록
            List<string> selectedIdList = new List<string>();

            //선택된 친구들 가져오기
            foreach (string item in lstFriend.SelectedItems)
            {
                selectedIdList.Add(item);
            }

            if (selectedIdList.Count == 0)
            {
                MessageBox.Show("선택된 친구가 없습니다.");
                return;
            }

            //Settings에 저장된 대화방 정보 가져오기
            Dictionary<int, string[]> chatRoomInfoDic =
                JsonConvert.DeserializeObject<Dictionary<int, string[]>>(Settings.Default.chatRoomsInfo);

            //선택된 친구들의 대화방이 이미 개설되어 있는지 확인하기 위함
            foreach (KeyValuePair<int, string[]> rooNumWithIdArr in chatRoomInfoDic)
            {
                //방 번호 별 대화 맴버 가져오기
                string[] storedIdArr = rooNumWithIdArr.Value;
                List<string> storedIdList = storedIdArr.ToList();

                //맴버 목록에서 나는 삭제
                storedIdList.Remove(userId);

                //알파벳 순 소팅을 위함
                Array.Sort(storedIdList.ToArray());
                Array.Sort(selectedIdList.ToArray());

                //Settings에 저장되어 있던 대화 맴버 목록과 선택된 친구 목록 중 같은 것이 있는지 확인
                if (storedIdList.SequenceEqual(selectedIdList))
                {
                    //개설된 방 있음
                    OpenExistChatRoom(selectedIdList.ToArray());
                    return;
                }
            }

            //개설된 방 없음 
            if (userId != null)
            {
                //서버로 보낼 대화방 데이터 생성
                var chatRoomData = new ChatRoomClientData { Code = "chattingroom", Id = userId, RoomMember = selectedIdList.ToArray() };

                try
                {
                    //서버로 데이터 전송
                    _tcpNetwork.Send(chatRoomData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 기존 대화방 열기
        /// </summary>
        /// <param name="idArr"></param>
        private void OpenExistChatRoom(string[] idArr)
        {
            //Settings에 저장된 내 아이디 가져오기
            string userId = Settings.Default.id;

            //Settings에 저장된 대화방 정보 가져오기
            Dictionary<int, string[]> ChatRoomInfoDic =
                JsonConvert.DeserializeObject<Dictionary<int, string[]>>(Settings.Default.chatRoomsInfo);

            if (ChatRoomInfoDic != null && userId != null)
            {
                //대화 맴버 목록으로 방 번호 가져오기
                int roomNum = ChatRoomInfoDic.FirstOrDefault(x => x.Value.SequenceEqual(idArr)).Key;

                if (roomNum != null)
                {
                    //서버로 보낼 대화방 데이터 생성
                    var chatRoomData = new ChatRoomClientData { Code = "chattingroom", Id = userId, RoomNumber = roomNum };

                    try
                    {
                        //서버로 데이터 전송
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
        /// 대화 목록에서 대화방 열기
        /// </summary>
        private void OpenChatRoomFromChatList(object? sender, EventArgs e)
        {
            try
            {
                //ListBox에서 클릭된 아이템이 있을 때
                if (lstChat.SelectedItem != null)
                {
                    //클릭된 대화방 명 가져오기
                    //대화방 명은 대화 맴버로 구성되어 있음
                    string ids = lstChat.SelectedItem.ToString();

                    string[] idArr = ids.Split(", ");

                    //기존 대화방에서 해당 대화 맴버에 맞는 대화방 열기
                    OpenExistChatRoom(idArr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 서버로부터 대화방 열기 리스폰스가 정상 수신되었을 때 동작
        /// </summary>
        /// <param name="chatRoomData">수신된 대화방 데이터</param>
        private void OpenChatRoomEventHandlerFun(object sender, ChatRoomServerData chatRoomData)
        {
            try
            {
                if (chatRoomData != null && chatRoomData.Code == "chattingroom")
                {
                    //Settings에서 대화방 정보 가져오기
                    Dictionary<int, string[]> ChatRoomInfoDic =
                        JsonConvert.DeserializeObject<Dictionary<int, string[]>>(Settings.Default.chatRoomsInfo);

                    if (chatRoomData.RoomNumber != null)
                    {
                        //가져온 대화방 정보에서 해당 방 번호가 이미 있는지 확인
                        if (ChatRoomInfoDic.ContainsKey(chatRoomData.RoomNumber))
                        {
                            //기존 방 open
                            Debug.WriteLine("기존방");
                        }
                        else
                        {
                            //새 방 open
                            Debug.WriteLine("새방");
                        }

                        //대화방 열기
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
        /// 대화방 열기를 성공했을 때 화면 처리
        /// </summary>
        /// <param name="roomNumber"></param>
        private void OpenChatRoomSucceed(int roomNumber)
        {
            //대화방으로 화면 이동
            CurrentPage = PageType.Talk;

            //현재 대화방 번호 Settings에 저장
            Settings.Default.currentRoomNumber = roomNumber;
            Settings.Default.Save();

            //이전 대화 가져오기
            GetPreviousMessages();
        }

        /// <summary>
        /// 이전 대화 가져오기
        /// </summary>
        private void GetPreviousMessages()
        {
            //Settings에서 내 아이디 가져오기
            string userId = Settings.Default.id;
            //Settings에서 현재 방 번호 가져오기
            int roomNumber = Settings.Default.currentRoomNumber;

            if (roomNumber == 0)
            {
                Debug.WriteLine("대화방 번호 찾을 수 없음");
                return;
            }

            try
            {
                //서버로 보낼 이전 대화 데이터 생성
                var prevMsgsData = new PrevMsgsClientData { Code = "past", Id = userId, RoomNumber = roomNumber };

                //서버로 데이터 전송
                _tcpNetwork.Send(prevMsgsData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 서버로부터 이전 대화 가져오기 리스폰스가 정상 수신되었을 때 동작
        /// </summary>
        /// <param name="prevMsgsData">수신된 이전 대화 데이터</param>
        private void GetPreviousMessagesEventHandlerFun(object sender, PrevMsgsServerData prevMsgsData)
        {
            try
            {
                if (prevMsgsData != null && prevMsgsData.Code == "past")
                {
                    if (prevMsgsData.IsTrue)
                    {
                        //파일 데이터 읽어오기
                        _tcpNetwork.ReceiveFile(prevMsgsData.FileSize);
                    }
                    else
                    {
                        throw new ResponseErrorException("이전 대화가 없습니다.");
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
        /// 서버로부터 이전 대화 파일이 정상적으로 수신되었을 때 동작
        /// </summary>
        /// <param name="fileBytes">파일 데이터</param>
        private void ReceivePrevMsgsEventHandlerFun(object sender, byte[] fileBytes)
        {
            try
            {
                if (fileBytes != null)
                {
                    try
                    {
                        //수신된 데이터를 쓸 파일 생성
                        string filePath = Directory.GetCurrentDirectory() + "\\past.txt";

                        //이미 해당 경로에 존재하는 파일이면 
                        if (File.Exists(filePath))
                        {
                            Debug.WriteLine(filePath + " 파일이 이미 존재합니다.");
                            return;
                        }

                        if (!string.IsNullOrEmpty(filePath))
                        {
                            //해당 경로의 파일에 수신된 데이터 쓰기
                            FileStream fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                            fs.Write(fileBytes, 0, fileBytes.Length);
                            fs.Close();

                            //파일에 쓰여진 데이터를 텍스트로 읽어들이기 
                            string txt = File.ReadAllText(filePath);

                            //이전 대화 목록 불러오기 성공 처리
                            Invoke(() => GetPreviousMessagesSucceed(txt));

                            //생성된 파일은 삭제
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
        /// 이전 대화 불러오기 성공에 따른 화면 처리
        /// </summary>
        /// <param name="rawMsg">이전 대화 내용</param>
        private void GetPreviousMessagesSucceed(string rawMsg)
        {
            //이전 대화 초기화 
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
        /// 대화방으로 이동
        /// </summary>
        private void MoveToTalkPage(object? sender, EventArgs e)
        {
            CurrentPage = PageType.Talk;
        }

        /// <summary>
        /// 메시지 전송
        /// </summary>
        private void SendMessage(object? sender, EventArgs e)
        {
            string sendTxt = txtMessage.Text;

            if (sendTxt.Length == 0)
            {
                MessageBox.Show("메시지를 입력해주세요.");
                return;
            }

            //내 아이디 가져오기
            string id = Settings.Default.id;

            if (id == null)
            {
                Debug.WriteLine("아이디 찾을 수 없음");
                return;
            }

            //현재 방 번호 가져오기
            int roomNumber = Settings.Default.currentRoomNumber;

            if (roomNumber == 0)
            {
                Debug.WriteLine("대화방 번호 찾을 수 없음");
                return;
            }

            //서버로 보낼 메시지 데이터 생성
            var chatMsgData = new MsgClientData { Code = "msg", Id = id, RoomNumber = roomNumber, Message = sendTxt };

            try
            {
                //서버로 데이터 전송
                _tcpNetwork.Send(chatMsgData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 서버로부터 메시지 전송 리스폰스가 정상 수신되었을 때 동작
        /// </summary>
        /// <param name="msgData">수신된 메시지 데이터</param>
        private void SendMessageEventHandlerFun(object sender, MsgServerData msgData)
        {
            try
            {
                if (msgData != null)
                {
                    //메시지 전송 성공 처리
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
        /// 메시지 전송 성공에 따른 화면 처리
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        private void SendMessageSucceed(string id, string msg)
        {
            lstTalk.Items.Add(id + ": " + msg);
        }

        /// <summary>
        /// 파일 보내기
        /// </summary>
        private void SendFile(object? sender, EventArgs e)
        {
            //디스크에 있는 파일 불러오기
            openFileDialog.InitialDirectory = "C:\\";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                FileInfo fileInfo = new FileInfo(filePath);

                //내 아이디 가져오기
                string id = Settings.Default.id;

                if (id == null)
                {
                    Debug.WriteLine("아이디 찾을 수 없음");
                    return;
                }

                //현재 방 번호 가져오기
                int roomNumber = Settings.Default.currentRoomNumber;

                if (roomNumber == 0)
                {
                    Debug.WriteLine("대화방 번호 찾을 수 없음");
                    return;
                }

                //서버로 보낼 메시지 데이터 생성
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
                    //파일 정보 먼저 전송
                    _tcpNetwork.Send(fileData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 서버로부터 파일 전송 리스폰스가 정상 수신되었을 때 동작
        /// </summary>
        /// <param name="fileData">수신된 파일 데이터</param>
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
        /// 파일 전송 성공 시 화면 처리
        /// </summary>
        private void SendFileSucceed()
        {

        }

        /// <summary>
        /// 파일 다운로드 
        /// </summary>
        private void downloadFile(object? sender, EventArgs e)
        {

        }
        
        /// <summary>
        /// 초대하기 화면으로 이동
        /// </summary>
        private void MoveToInvitePage(object? sender, EventArgs e)
        {
            CurrentPage = PageType.InvitingList;

            //초대 가능 친구 목록 가져오기 
            GetInvitationList();
        }

        /// <summary>
        /// 초대 가능한 친구 목록 가져오기
        /// </summary>
        private void GetInvitationList()
        {
            try
            {
                //현재 방 번호 가져오기
                int roomNumber = Settings.Default.currentRoomNumber;

                //서버로 보낼 초대 가능 친구 목록 가져오기 데이터 생성
                var invitationListData = new InvitationListClientData { Code = "invitelist", RoomNumber = roomNumber };

                //서버로 데이터 전송
                _tcpNetwork.Send(invitationListData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 서버로부터 초대 가능 친구 목록 가져오기 리스폰스가 정상 수신되었을 때 동작
        /// </summary>
        /// <param name="invitationListData">수신된 초대 가능 친구 목록</param>
        private void GetInvitationListEventHandlerFun(object sender, InvitationListServerData invitationListData)
        {
            try
            {
                if (invitationListData != null && invitationListData.Code == "invitelist")
                {
                    //초대 가능 친구 목록 가져오기 성공 처리
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
        /// 초대 가능 친구 목록 가져오기 성공에 따른 화면 처리
        /// </summary>
        /// <param name="isLoginList"></param>
        /// <param name="friendList"></param>
        private void GetInvitationListSucceed(bool[] isLoginList, string[] friendList)
        {
            if (isLoginList.Length != friendList.Length)
            {
                MessageBox.Show("초대 가능 친구 목록 데이터 에러");
                return;
            }

            List<string> loginStateList = new List<string>();

            foreach (bool loginState in isLoginList)
            {
                loginStateList.Add(loginState ? "(로그인)" : "(로그아웃)");
            }

            //초대 가능 친구 목록 표시
            lstInvitation.Items.AddRange(friendList);
            //로그인 상태 목록 표시
            lstInvitationLoginState.Items.AddRange(loginStateList.ToArray());
        }

        /// <summary>
        /// 초대하기
        /// </summary>
        private void InviteFriend(object? sender, EventArgs e)
        {
            //TODO: 현재 대화 맴버는 클릭 안되도록 막기 

            if (lstInvitation.SelectedItem == null)
            {
                MessageBox.Show("선택된 친구가 없습니다.");
                return;
            }

            //현재 방 번호 가져오기
            int roomNumber = Settings.Default.currentRoomNumber;

            //내 아이디 가져오기
            string userId = Settings.Default.id;

            //선택된 친구 목록 가져오기
            List<string> selectedIdList = new List<string>();

            foreach (string id in lstInvitation.SelectedItems)
            {
                selectedIdList.Add(id);
            }

            //서버로 보낼 초대 데이터 생성
            var inviteData = new InviteClientData { Code = "invite", Id = userId, RoomNumber = roomNumber, RoomMember = selectedIdList.ToArray() };

            try
            {
                //서버로 데이터 전송
                _tcpNetwork.Send(inviteData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 서버로부터 초대하기 리스폰스가 정상 수신되었을 때 동작
        /// </summary>
        /// <param name="inviteData">수신된 초대 데이터</param>
        private void InviteEventHandlerFun(object sender, ChatRoomServerData inviteData)
        {
            try
            {
                if (inviteData != null && inviteData.Code == "invite")
                {
                    if (inviteData.RoomNumber == Settings.Default.currentRoomNumber)
                    {
                        //초대 성공 처리
                        Invoke(InviteFriendSucceed);
                    }
                    else
                    {
                        throw new ResponseErrorException("초대가 수락되지 않았습니다.");
                    }
                }
                //응답 비정상일 때
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
        /// 초대 성공에 따른 화면 처리
        /// </summary>
        private void InviteFriendSucceed()
        {
            MessageBox.Show("초대되었습니다.");
        }

        /// <summary>
        /// 대화 맴버 보기
        /// </summary>
        private void MoveToMemberPage(object? sender, EventArgs e)
        {
            //대화 맴버 화면으로 전환
            CurrentPage = PageType.MemberList;

            //대화 맴버 불러오기 
            GetChatMemberList();
        }

        /// <summary>
        /// 대화 맴버 가져오기
        /// </summary>
        private void GetChatMemberList()
        {
            //현재 방 번호 가져오기
            int roomNumber = Settings.Default.currentRoomNumber;

            //서버로 보낼 대화 맴버 가져오기 데이터 생성
            var chatMemberData = new ChatMemberClientData { Code = "chattingmember", RoomNumber = roomNumber };

            try
            {
                //서버로 데이터 전송
                _tcpNetwork.Send(chatMemberData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 서버로부터 대화 맴버 가져오기 리스폰스가 정상 수신되었을 때 동작
        /// </summary>
        /// <param name="chatMemberData">수신된 대화 맴버 데이터</param>
        private void GetChatMemberEventHandlerFun(object sender, ChatMemberServerData chatMemberData)
        {
            try
            {
                if (chatMemberData != null && chatMemberData.Code == "chattingmember")
                {
                    //대화 맴버 가져오기 성공 처리
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
        /// 대화 맴버 가져오기 성공에 따른 화면 처리
        /// </summary>
        /// <param name="isLoginList">로그인 상태 목록</param>
        /// <param name="friendList">맴버 목록</param>
        private void GetChatMemberSucceed(bool[] isLoginList, string[] friendList)
        {
            List<string> loginStateList = new List<string>();

            if (isLoginList != null)
            {
                foreach (bool loginState in isLoginList)
                {
                    loginStateList.Add(loginState ? "(로그인)" : "(로그아웃)");
                }
            }

            //대화 맴버 목록 표시
            lstMember.Items.AddRange(friendList);
            //로그인 상태 목록 표시
            lstMemberLoginState.Items.AddRange(loginStateList.ToArray());
        }

        /// <summary>
        /// 대화방 나가기
        /// </summary>
        private void ExitChatRoom(object? sender, EventArgs e)
        {
            //현재 방 번호 가져오기
            int roomNumber = Settings.Default.currentRoomNumber;
            //내 아이디 가져오기
            string userId = Settings.Default.id;

            //서버로 보낼 방 나가기 데이터 생성
            var exitChatRoomData = new ExitChatRoomClientData { Code = "exit", Id = userId, RoomNumber = roomNumber };

            try
            {
                //서버로 데이터 전송
                _tcpNetwork.Send(exitChatRoomData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 서버로부터 대화방 나가기 리스폰스가 정상 수신되었을 때 동작
        /// </summary>
        /// <param name="exitChatRoomData">수신된 대화방 나가기 데이터</param>
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
        /// 방 나가기 후 화면 처리
        /// </summary>
        private void ExitRoomSucceed()
        {
            //메인 화면으로 전환
            CurrentPage = PageType.Main;

            //대화 목록 탭을 클릭 처리
            tabControl.SelectedTab = chatListTab;

            //MoveToChatList();
        }
    }
}