using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClient
{
    [MessagePackObject]
    public class ChatMemberServerData : ChatData
    {
        [Key("RoomNumber")]
        public int RoomNumber { get; set; }

        [Key("Login")]
        public bool[] LoginStateList { get; set; }

        [Key("ChattingMember")]
        public string[] FriendList { get; set; }
    }
}
