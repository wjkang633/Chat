using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClient
{
    [MessagePackObject]
    public class PrevMsgsClientData : ChatData
    {
        [Key("SenderId")]
        public string Id { get; set; }

        [Key("RoomNumber")]
        public int RoomNumber { get; set; }
    }
}
