using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClient
{
    [MessagePackObject]
    public class InviteServerData : ChatData
    {
        [Key("RoomNumber")]
        public int RoomNumber { get; set; }
    }
}
