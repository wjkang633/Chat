using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClient
{
    [MessagePackObject]
    public class MsgClientData : ChatData
    {
        [Key("SenderId")]
        public string Id { get; set; }

        [Key("RoomNumber")]
        public int RoomNumber { get; set; }

        [Key("Msg")]
        public string Message { get; set; }
    }
}
