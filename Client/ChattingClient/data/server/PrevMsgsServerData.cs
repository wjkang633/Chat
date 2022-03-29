using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClient
{
    [MessagePackObject]
    public class PrevMsgsServerData : ChatData
    {
        [Key("Result")]
        public bool IsTrue { get; set; }
        
        [Key("RoomNumber")]
        public int RoomNumber { get; set; }

        [Key("FileSize")]
        public long FileSize { get; set; }
    }
}
