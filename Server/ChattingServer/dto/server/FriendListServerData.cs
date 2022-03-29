using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClient
{
    [MessagePackObject]
    public class FriendListServerData : ChatData
    {
        [Key("Login")]
        public bool[] LoginStateList { get; set; }

        [Key("FriendList")]
        public string[] FriendList { get; set; }
    }
}
