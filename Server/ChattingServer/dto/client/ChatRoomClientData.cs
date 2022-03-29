using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClient
{
    [MessagePackObject]
    public class ChatRoomClientData : ChatData
    {
        [Key("Id")]
        public string Id { get; set; }

        [Key("RoomNumber")]
        public int RoomNumber { get; set; }

        [Key("Receiver")]
        public string[] RoomMember { get; set; }
    }
}
