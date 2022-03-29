using MessagePack;
using MessagePack.Resolvers;
using System.Diagnostics;
using System.Text;

namespace ChattingClient
{
    public class ResponseHandler
    {
        public delegate void ServerDownEventHandler(object sender); //서버 연결 끊어짐
        public delegate void LoginEventHandler(object sender, LoginServerData data); //로그인
        public delegate void JoinEventHandler(object sender, JoinServerData data); //회원가입
        public delegate void LogoutEventHandler(object sender, ChatData data); //로그아웃
        public delegate void GetChatListEventHandler(object sender, ChatListServerData data); //대화 목록 가져오기
        public delegate void GetFriendListEventHandler(object sender, FriendListServerData data); //친구 목록 가져오기
        public delegate void OpenChatRoomEventHandler(object sender, ChatRoomServerData data); //대화방 열기
        public delegate void GetPrevMsgsEventHandler(object sender, PrevMsgsServerData data); //이전 대화 가져오기
        public delegate void SendMsgEventHandler(object sender, MsgServerData data); //메시지 전송
        public delegate void SendFileEventHandler(object sender, FileServerData data); //파일 전송
        public delegate void ReceiveFileEventHandler(object sender, byte[] fileBytes); //파일 수신
        public delegate void GetInvitationListEventHandler(object sender, InvitationListServerData data); //초대 가능 친구 목록 가져오기
        public delegate void InviteEventHandler(object sender, ChatRoomServerData data); //초대하기
        public delegate void GetChatMemberEventHandler(object sender, ChatMemberServerData data); //대화 맴버 보기
        public delegate void ExitChatRoomEventHandler(object sender, ExitChatRoomServerData data); //대화방 나가기

        public event ServerDownEventHandler serverDownEventHandler = null;
        public event LoginEventHandler loginEventHandler = null;
        public event JoinEventHandler joinEventHandler = null;
        public event LogoutEventHandler logoutEventHandler = null;
        public event GetChatListEventHandler getChatListEventHandler = null;
        public event GetFriendListEventHandler getFriendListEventHandler = null;
        public event OpenChatRoomEventHandler openChatRoomEventHandler = null;
        public event GetPrevMsgsEventHandler getPrevMsgsEventHandler = null;
        public event SendMsgEventHandler sendMsgEventHandler = null;
        public event SendFileEventHandler sendFileEventHandler = null;
        public event ReceiveFileEventHandler receiveFileEventHandler = null;
        public event GetInvitationListEventHandler getInvitationListEventHandler = null;
        public event InviteEventHandler inviteEventHandler = null;
        public event GetChatMemberEventHandler getChatMemberEventHandler = null;
        public event ExitChatRoomEventHandler exitChatRoomEventHandler = null;

        /// <summary>
        /// 서버로부터 수신받은 데이터가 일반 데이터일 때의 이벤트 핸들러
        /// </summary>
        /// <param name="bytes">수신 데이터</param>
        public void HandleData(byte[] bytes)
        {
            try
            {
                //code값 추출을 위해 1차 Deserialize
                ChatData data = MessagePackSerializer.Deserialize<ChatData>(bytes);

                switch (data.Code)
                {
                    //수신된 데이터가 로그인 데이터일 때
                    case "login":
                        LoginServerData loginData = MessagePackSerializer.Deserialize<LoginServerData>(bytes);

                        if (loginEventHandler != null)
                        {
                            loginEventHandler(this, loginData);
                        }
                        break;

                    //수신된 데이터가 회원가입 데이터일 때
                    case "join":
                        JoinServerData joinData = MessagePackSerializer.Deserialize<JoinServerData>(bytes);

                        if (joinEventHandler != null)
                        {
                            joinEventHandler(this, joinData);
                        }

                        break;

                    //수신된 데이터가 로그아웃 데이터일 때
                    case "logout":
                        ChatData logoutData = MessagePackSerializer.Deserialize<ChatData>(bytes);

                        if (logoutEventHandler != null)
                        {
                            logoutEventHandler(this, logoutData);
                        }

                        break;

                    //수신된 데이터가 대화 목록 데이터일 때
                    case "roomlist":
                        ChatListServerData chatListData = MessagePackSerializer.Deserialize<ChatListServerData>(bytes);

                        if (getChatListEventHandler != null)
                        {
                            getChatListEventHandler(this, chatListData);
                        }

                        break;

                    //수신된 데이터가 친구 목록 데이터일 때
                    case "friendlist":
                        FriendListServerData friendListData = MessagePackSerializer.Deserialize<FriendListServerData>(bytes);

                        if (getFriendListEventHandler != null)
                        {
                            getFriendListEventHandler(this, friendListData);
                        }

                        break;

                    //수신된 데이터가 대화방 데이터일 때
                    case "chattingroom":
                        ChatRoomServerData chatRoomData = MessagePackSerializer.Deserialize<ChatRoomServerData>(bytes);

                        if (openChatRoomEventHandler != null)
                        {
                            openChatRoomEventHandler(this, chatRoomData);
                        }

                        break;

                    //수신된 데이터가 이전 대화 데이터일 때
                    case "past":
                        PrevMsgsServerData prevMsgsData = MessagePackSerializer.Deserialize<PrevMsgsServerData>(bytes);

                        if (getPrevMsgsEventHandler != null)
                        {
                            getPrevMsgsEventHandler(this, prevMsgsData);
                        }

                        break;

                    //수신된 데이터가 메시지 데이터일 때
                    case "msg":
                        MsgServerData msgData = MessagePackSerializer.Deserialize<MsgServerData>(bytes);

                        if (sendMsgEventHandler != null)
                        {
                            sendMsgEventHandler(this, msgData);
                        }

                        break;

                    //수신된 데이터가 파일 데이터일 때
                    case "file":
                        FileServerData fileData = MessagePackSerializer.Deserialize<FileServerData>(bytes);

                        if (sendFileEventHandler != null)
                        {
                            sendFileEventHandler(this, fileData);
                        }

                        break;

                    //수신된 데이터가 초대 가능 친구 목록 데이터일 때
                    case "invitelist":
                        InvitationListServerData invitationListData = MessagePackSerializer.Deserialize<InvitationListServerData>(bytes);

                        if (getInvitationListEventHandler != null)
                        {
                            getInvitationListEventHandler(this, invitationListData);
                        }

                        break;

                    //수신된 데이터가 초대 데이터일 때
                    case "invite":
                        ChatRoomServerData inviteData = MessagePackSerializer.Deserialize<ChatRoomServerData>(bytes);

                        if (inviteEventHandler != null)
                        {
                            inviteEventHandler(this, inviteData);
                        }

                        break;

                    //수신된 데이터가 대화 맴버 데이터일 때
                    case "chattingmember":
                        ChatMemberServerData chatMemberData = MessagePackSerializer.Deserialize<ChatMemberServerData>(bytes);

                        if (getChatMemberEventHandler != null)
                        {
                            getChatMemberEventHandler(this, chatMemberData);
                        }

                        break;

                    //수신된 데이터가 대화방 나가기 데이터일 때
                    case "exit":
                        ExitChatRoomServerData exitChatRoomData = MessagePackSerializer.Deserialize<ExitChatRoomServerData>(bytes);

                        if (exitChatRoomEventHandler != null)
                        {
                            exitChatRoomEventHandler(this, exitChatRoomData);
                        }

                        break;

                    default:
                        break;
                }
            }
            catch(Exception ex)
            {
                //수신된 데이터가 서버와의 Connection 확인을 위한 데이터일 때
                if (Encoding.Default.GetString(bytes).Contains("연결 확인"))
                {
                    return;
                }
                ////파일 데이터일 때
                //else 
                //{
                //    if (receiveFileEventHandler != null)
                //    {
                //        receiveFileEventHandler(this, bytes);
                //    }
                //}
            }
        }

        /// <summary>
        /// 서버로부터 수신받은 데이터가 파일일 때의 이벤트 핸들러
        /// </summary>
        /// <param name="fileBytes">파일의 byte 데이터</param>
        public void HandleFileData(byte[] fileBytes)
        {
            if (receiveFileEventHandler != null)
            {
                receiveFileEventHandler(this, fileBytes);
            }
        }

        /// <summary>
        /// 서버와의 연결이 끊어졌을 때의 이벤트 핸들러
        /// </summary>
        public void HandleServerDown()
        {
            if (serverDownEventHandler != null)
            {
                serverDownEventHandler(this);
            }
        }
    }
}
