using ChattingClient;
using ChattingServer.data;
using ChattingServer.database;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingServer
{
    public class ClientMsgHandler
    {
        //DB 객체 
        private ChattingDB db = new ChattingDB();

        public int[] roomMemberUidArr = new int[100];

        public PrevMsgsClientData _prevMsgsClientData = null;
        public FileClientData _fileClientData = null;

        /// <summary>
        /// 클라이언트에게 보낼 일반 데이터
        /// </summary>
        public byte[] BytesToSend { get; set; }

        /// <summary>
        /// 클라이언트에게 보낼 파일 데이터
        /// </summary>
        public byte[] FileBytesToSend { get; set; }

        /// <summary>
        /// 클라이언트의 요청이 파일 수신인지, 전송인지 구분하기 위함
        /// 파일 수신(ex. 이전 대화) 
        /// 파일 전송(ex. 파일 보내기)
        /// </summary>
        public bool IsFileReceived { get; set; }

        /// <summary>
        /// 멀티 클라이언트에게 전송해야 할 작업인지 확인
        /// </summary>
        public bool IsSentToMultiClient { get; set; }

        /// <summary>
        /// 수신된 byte 데이터를 특정 데이터(클래스)로 변환
        /// 변환된 데이터의 유효성 검사 및 DB 작업 후 서버로 보낼 byte 데이터 생성
        /// </summary>
        /// <param name="clientUid">클라이언트의 Uid</param>
        /// <param name="receivedBytes">수신된 byte 데이터</param>
        public void categorizeMsg(int clientUid, byte[] receivedBytes)
        {
            try
            {
                ChatData chatData = MessagePackSerializer.Deserialize<ChatData>(receivedBytes);

                switch (chatData.Code)
                {
                    case "login":
                        LoginClientData loginClientData = MessagePackSerializer.Deserialize<LoginClientData>(receivedBytes);

                        LoginServerData loginServerData;

                        if (CheckLoginDataValidation(loginClientData))
                        {
                            loginServerData = new LoginServerData { Code = "login", Id = loginClientData.Id, IsTrue = true, ResultMessage = "로그인 성공" };
                        }
                        else
                        {
                            loginServerData = new LoginServerData { Code = "login", Id = loginClientData.Id, IsTrue = false, ResultMessage = "유효하지 않은 로그인 정보입니다." };
                        }

                        BytesToSend = MessagePackSerializer.Serialize(loginServerData);

                        FileBytesToSend = null;
                        IsSentToMultiClient = false;
                        IsFileReceived = false;

                        break;

                    case "join":
                        JoinClientData joinClientData = MessagePackSerializer.Deserialize<JoinClientData>(receivedBytes);

                        JoinServerData joinServerData;

                        if (CheckJoinDataValidation(clientUid, joinClientData))
                        {
                            joinServerData = new JoinServerData { Code = "join", IsTrue = true, ResultMessage = "회원가입이 완료되었습니다." };
                        }
                        else
                        {
                            joinServerData = new JoinServerData { Code = "join", IsTrue = false, ResultMessage = "유효하지 않은 회원가입 정보입니다." };
                        }

                        BytesToSend = MessagePackSerializer.Serialize(joinServerData);

                        FileBytesToSend = null;
                        IsSentToMultiClient = false;
                        IsFileReceived = false;

                        break;

                    case "logout":
                        LogoutClientData logoutClientData = MessagePackSerializer.Deserialize<LogoutClientData>(receivedBytes);

                        ChatData logoutServerData;

                        if (CheckLogoutDataValidation(logoutClientData.Id))
                        {
                            logoutServerData = new ChatData { Code = "logout" };
                        }
                        else
                        {
                            logoutServerData = new ChatData();
                        }

                        BytesToSend = MessagePackSerializer.Serialize(logoutServerData);

                        FileBytesToSend = null;
                        IsSentToMultiClient = false;
                        IsFileReceived = false;

                        break;

                    case "roomlist":
                        ChatListClientData chatListClientData = MessagePackSerializer.Deserialize<ChatListClientData>(receivedBytes);
                       
                        ChatListServerData chatListServerData;

                        //대화방 목록 가져오기
                        string[] chatRoomList = GetChatRoomList(chatListClientData.Id);

                        if (chatRoomList != null)
                        {
                            chatListServerData = new ChatListServerData { Code = "roomlist", ChatList = chatRoomList };
                        }
                        else
                        {
                            chatListServerData = new ChatListServerData { Code = "roomlist" };
                        }

                        BytesToSend = MessagePackSerializer.Serialize(chatListServerData);

                        FileBytesToSend = null;
                        IsSentToMultiClient = false;
                        IsFileReceived = false;

                        break;

                    case "friendlist":
                        ChatData friendListClientData = MessagePackSerializer.Deserialize<ChatData>(receivedBytes);
                       
                        FriendListServerData friendListServerData;

                        //친구 목록 가져오기
                        string[] list = GetFriendList();

                        //list 0번째 인덱스에는 아이디 목록이 존재 
                        string[] friendList = list[0].Split(",");

                        //list 1번째 인덱스에는 로그인 상태 목록이 존재 
                        bool[] loginStateList = Array.ConvertAll(list[1].Split(","), x => x == "True" ? true : false);

                        if (friendList != null && loginStateList != null)
                        {
                            friendListServerData = new FriendListServerData { Code = "friendlist", FriendList = friendList, LoginStateList = loginStateList};
                        }
                        else
                        {
                            friendListServerData = new FriendListServerData { Code = "friendlist"};
                        }

                        BytesToSend = MessagePackSerializer.Serialize(friendListServerData);

                        FileBytesToSend = null;
                        IsSentToMultiClient = false;
                        IsFileReceived = false;

                        break;

                    case "chattingroom":
                        ChatRoomClientData chatRoomClientData = MessagePackSerializer.Deserialize<ChatRoomClientData>(receivedBytes);
                        
                        ChatRoomServerData chatRoomServerData;

                        //오픈할 대화방 번호 가져오기
                        int roomNum = GetRoomNumber(chatRoomClientData.Id, chatRoomClientData.RoomNumber, chatRoomClientData.RoomMember);

                        if (roomNum != 0)
                        {
                            chatRoomServerData = new ChatRoomServerData { Code = "chattingroom", RoomNumber = roomNum };
                        }
                        else
                        {
                            chatRoomServerData = new ChatRoomServerData { Code = "chattingroom"};
                        }

                        BytesToSend = MessagePackSerializer.Serialize(chatRoomServerData);

                        FileBytesToSend = null;
                        IsSentToMultiClient = false;
                        IsFileReceived = false;

                        break;

                    case "past":
                        //다른 클래스에서 사용하기 위해 전역 변수에 저장
                        _prevMsgsClientData = MessagePackSerializer.Deserialize<PrevMsgsClientData>(receivedBytes);

                        PrevMsgsServerData prevMsgsServerData;

                        FileBytesToSend = GetPrevMsgsFile();

                        if (FileBytesToSend != null)
                        {
                            prevMsgsServerData = new PrevMsgsServerData { Code = "past", IsTrue = true, RoomNumber = _prevMsgsClientData.RoomNumber, FileSize = FileBytesToSend.Length };
                        }
                        else
                        {
                            prevMsgsServerData = new PrevMsgsServerData { Code = "past", IsTrue = false };
                        }
                      
                        BytesToSend = MessagePackSerializer.Serialize(prevMsgsServerData);

                        IsSentToMultiClient = false;

                        //이전 대화 파일을 '요청'하는 경우이므로 false 처리 
                        IsFileReceived = false;

                        break;

                    case "file":
                        //다른 클래스에서 사용하기 위해 전역 변수에 저장
                        _fileClientData = MessagePackSerializer.Deserialize<FileClientData>(receivedBytes);

                        //수신된 파일 byte를 클라이언트로 by pass 하기 위함
                        BytesToSend = receivedBytes;
                        FileBytesToSend = null;

                        //멀티 클라이언트에게 메시지를 보내야하기 때문에 true
                        IsSentToMultiClient = true;

                        //클라이언트에서 파일을 전송하여 서버는 '수신'받은 경우이므로 true 처리 
                        IsFileReceived = true;

                        break;

                    case "msg":
                        //다른 클래스에서 사용하기 위해 전역 변수에 저장
                        MsgClientData msgClientData = MessagePackSerializer.Deserialize<MsgClientData>(receivedBytes);

                        //메시지를 보낼 대화방 맴버 가져옴
                        roomMemberUidArr = GetChatRoomMemberUid(msgClientData.RoomNumber);

                        //수신된 메시지의 byte를 클라이언트로 by pass 하기 위함
                        BytesToSend = receivedBytes;
                        FileBytesToSend = null;

                        //멀티 클라이언트에게 메시지를 보내야하기 때문에 true
                        IsSentToMultiClient = true;

                        //파일이 수신된 경우가 아니므로 false 
                        IsFileReceived = false;

                        //메시지 파일에 쓰기
                        SaveMsgToFileAsync(msgClientData.Id, msgClientData.RoomNumber, msgClientData.Message);

                        break;

                    case "invitelist":
                        InvitationListClientData invitationListClientData = MessagePackSerializer.Deserialize<InvitationListClientData>(receivedBytes);

                        InvitationListServerData invitationListServerData;

                        //초대 가능 친구 목록 가져오기
                        List<string> invitationList = GetInvitationList(invitationListClientData.RoomNumber);

                        //친구 목록 
                        friendList = invitationList.GetRange(0,invitationList.Count/2).ToArray();
                        //로그인 상태 목록
                        loginStateList = Array.ConvertAll(invitationList.GetRange(invitationList.Count / 2, invitationList.Count/2).ToArray(), x => x == "True" ? true : false);
              
                        if (friendList != null && loginStateList != null)
                        {
                            invitationListServerData = new InvitationListServerData { Code = "invitelist", RoomNumber = invitationListClientData.RoomNumber, FriendList = friendList, LoginStateList = loginStateList };
                        }
                        else
                        {
                            invitationListServerData = new InvitationListServerData { Code = "invitelist"  };
                        }

                        BytesToSend = MessagePackSerializer.Serialize(invitationListServerData);

                        FileBytesToSend = null;
                        IsSentToMultiClient = false;
                        IsFileReceived = false;

                        break;

                    case "invite":
                        InviteClientData inviteClientData = MessagePackSerializer.Deserialize<InviteClientData>(receivedBytes);

                        InviteServerData inviteServerData;

                        if (InviteFriends(inviteClientData.RoomNumber, inviteClientData.RoomMember))
                        {
                            inviteServerData = new InviteServerData { Code = "invite", RoomNumber = inviteClientData.RoomNumber };
                        }
                        else
                        {
                            inviteServerData = new InviteServerData { Code = "invite" };
                        }

                        BytesToSend = MessagePackSerializer.Serialize(inviteServerData);

                        FileBytesToSend = null;
                        IsSentToMultiClient = false;
                        IsFileReceived = false;

                        break;

                    case "chattingmember":
                        ChatMemberClientData chatMemberClientData = MessagePackSerializer.Deserialize<ChatMemberClientData>(receivedBytes);

                        ChatMemberServerData chatMemberServerData;

                        string[] chatMembers = GetChatMember(chatMemberClientData.RoomNumber);

                        if (chatMembers != null)
                        {
                            chatMemberServerData = new ChatMemberServerData { Code = "chattingmember", RoomNumber = chatMemberClientData.RoomNumber, FriendList = chatMembers, LoginStateList = null};
                        }
                        else
                        {
                            chatMemberServerData = new ChatMemberServerData { Code = "chattingmember" };
                        }

                        BytesToSend = MessagePackSerializer.Serialize(chatMemberServerData);

                        FileBytesToSend = null;
                        IsSentToMultiClient = false;
                        IsFileReceived = false;

                        break;

                    case "exit":
                        ExitChatRoomClientData exitChatRoomClientData = MessagePackSerializer.Deserialize<ExitChatRoomClientData>(receivedBytes);

                        ExitChatRoomServerData exitChatRoomServerData;

                        if (ExitChatRoom(exitChatRoomClientData.RoomNumber, exitChatRoomClientData.Id))
                        {
                            exitChatRoomServerData = new ExitChatRoomServerData { Code = "exit" };
                        }
                        else
                        {
                            exitChatRoomServerData = new ExitChatRoomServerData();
                        }

                        BytesToSend = MessagePackSerializer.Serialize(exitChatRoomServerData);

                        FileBytesToSend = null;
                        IsSentToMultiClient = false;
                        IsFileReceived = false;

                        break;
                }
            }
            catch (Exception ex)
            {
                //클라이언트로부터 파일 byte가 수신되었을 때
                if(IsFileReceived)
                {
                    //수신된 byte를 클라이언트로 by pass 하기 위함
                    FileBytesToSend = receivedBytes;
                    BytesToSend = null;

                    //DB에서 방 번호를 통해 대화방 맴버의 Uid를 가져옴
                    roomMemberUidArr = GetChatRoomMemberUid(_fileClientData.RoomNumber);
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 수신된 로그인 데이터의 유효성을 검사
        /// </summary>
        /// <param name="loginData">로그인 데이터</param>
        private bool CheckLoginDataValidation(LoginClientData loginData)
        {
            //아이디, 비밀번호가 DB와 대조하여 유효한 값이면
            if (db.IsLoginPossible(loginData.Id, loginData.Pw))
            {
                //해당 아이디의 로그인 상태 정보 DB에 저장
                db.UpdateUserLoginState(loginData.Id, true);

                //로그인 성공 반환
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 수신된 회원가입 데이터의 유효성 검사
        /// </summary>
        /// <param name="joinData">회원가입 데이터</param>
        private bool CheckJoinDataValidation(int clientUid, JoinClientData joinData)
        {
            //아이디가 DB에 등록되지 않은 값이라면 회원가입 가능
            if (db.IsJoinPossible(joinData.Id))
            {
                //DB에 회원 정보 저장
                db.InsertUser(clientUid, joinData.Id, joinData.Pw, joinData.Name);

                //로그인 시 클라이언트 객체에 클라이언트의 유저 아이디 할당
                ClientData foundClient = null;

                if (ClientManager._clientDic.TryGetValue(clientUid, out foundClient) != null)
                {
                    ClientManager._clientDic[clientUid].ClientId = joinData.Id;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 로그아웃 가능한 상태인지 확인
        /// </summary>
        /// <param name="id">로그아웃 할 아이디</param>
        private bool CheckLogoutDataValidation(string id)
        {
            //해당 아이디가 현재 로그인 상태일 때 로그아웃 가능
            if (db.IsLogoutPossible(id))
            {
                //로그인 상태 정보 DB에 저장
                db.UpdateUserLoginState(id, false);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 대화방 목록을 가져옴
        /// </summary>
        /// <param name="chatListData">대화방 목록 요청 데이터</param>
        private string[] GetChatRoomList(string id)
        {
            return db.GetChatRoomList(id);
        }

        /// <summary>
        /// 친구 목록을 가져옴
        /// </summary>
        /// <param name="friendListData">친구 목록 요청 데이터</param>
        private string[] GetFriendList()
        {
            return db.GetFriendList();
        }

        /// <summary>
        /// 대화방 오픈을 위한 DB 작업 후 오픈 가능한 대화방 번호를 반환
        /// </summary>
        /// <param name="id">아아디</param>
        /// <param name="roomNumber">대화방 번호</param>
        /// <param name="member">대화방 맴버</param>
        private int GetRoomNumber(string id, int roomNumber, string[] member)
        {
            //대화방 번호는 없고 맴버만 있을 때 -> 새로운 방 요청으로 간주
            if (roomNumber == 0 && member != null)
            {
                List<string> chatMember = new List<string>();
                chatMember.AddRange(member);
                chatMember.Add(id);

                //새로운 대화방 번호 반환
                return db.CreateChatRoom(chatMember.ToArray());
            }
            //대화방 번호는 있고 맴버는 없을 때 -> 기존 방 요청으로 간주
            else if (roomNumber != 0 && member == null)
            {
                //기존 대화방 번호 반환
                return db.GetExistChatRoom(roomNumber);
            }
            //그 외는 잘못된 요청으로 간주
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 이전 대화 파일 DB에서 가져옴
        /// </summary>
        private byte[] GetPrevMsgsFile()
        {
            //클라이언트의 아이디 및 요청 방 번호와 일치하는
            //이전 대화 파일이 DB에 존재하는지 확인
            string fileName = db.GetPrevMsgsFileName(_prevMsgsClientData.Id, _prevMsgsClientData.RoomNumber);
            string filePath = @"C:\txt\" + fileName + ".txt";

            //디스크에서 filePath와 일치하는 파일이 있는지 확인
            if (File.Exists(filePath))
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(fileStream))
                    {
                        //txt 파일에서 읽어들인 byte 반환
                         return reader.ReadBytes((int) fileStream.Length);
                    }
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 대화방 맴버의 Uid 목록을 가져옴
        /// </summary>
        private int[] GetChatRoomMemberUid(int roomNumber)
        {
            return db.GetChatRoomMemberUid(roomNumber);
        }

        /// <summary>
        /// 메시지를 DB에 저장된 파일 path에 쓰기
        /// </summary>
        private async Task SaveMsgToFileAsync(string id, int roomNumber, string msg)
        {
            string fileName = db.GetPrevMsgsFileName(id, roomNumber);
            string filePath;

            //DB에 저장된 파일명이 없으면
            if (fileName == null)
            {
                fileName = roomNumber + "_" + id;
                filePath = @"C:\txt\" + fileName + ".txt";

                //DB에 파일 정보 저장
                db.InsertFileInfo(id, roomNumber, fileName, filePath);
            }
            else
            {
                filePath = @"C:\txt\" + fileName + ".txt";
            }

            //파일에 메시지 내용 쓰기
            StreamWriter writer = File.AppendText(filePath);                                                     
            writer.WriteLine(msg);    
            writer.Close();

            //if (File.Exists(filePath))
            //{
            //    File.WriteAllText(fileName, msg);
            //}
            //else
            //{
            //    fileName = @"C:\txt\" + roomNumber + "_" + id + ".txt";

            //    Debug.WriteLine(fileName);
            //    File.WriteAllText(fileName, msg);

            //    //DB에 파일 정보 저장
            //    db.InsertFileInfo(id, roomNumber, id+"_"+roomNumber, fileName);
            //}
        }

        /// <summary>
        /// 초대 가능 친구 목록을 가져옴
        /// </summary>
        /// <param name="roomNumber">대화방 번호</param>
        private List<string> GetInvitationList(int roomNumber)
        {
            return db.GetInvitationList(roomNumber);
        }

        /// <summary>
        /// 초대하기가 가능한지 확인
        /// </summary>
        /// <param name="roomMember">초대할 대화방 번호</param>
        /// <param name="roomMember">초대할 맴버</param>
        private bool InviteFriends(int roomNumber, string[] roomMember)
        {
            return db.AddFriendsToChatRoom(roomNumber, roomMember);
        }

        /// <summary>
        /// 대화방 맴버를 가져옴
        /// </summary>
        /// <param name="roomNumber">대화방 번호</param>
        private string[] GetChatMember(int roomNumber)
        {
            return db.GetChatRoomMember(roomNumber);
        }

        /// <summary>
        /// 대화방 나가기가 가능한지 확인
        /// </summary>
        /// <param name="roomNumber">대화방 번호</param>
        /// <param name="id">아이디</param>
        private bool ExitChatRoom(int roomNumber, string id)
        {
            return db.RemoveUserFromChatRoom(roomNumber, id);
        }

    }
}
