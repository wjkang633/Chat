using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClient
{
    [MessagePackObject]
    public class ChatListServerData : ChatData
    {
        [Key("RoomList")]
        public string[] ChatList { get; set; }
    }
}
