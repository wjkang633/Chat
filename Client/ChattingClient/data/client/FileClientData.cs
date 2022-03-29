using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClient
{
    [MessagePackObject]
    public class FileClientData : ChatData
    {
        [Key("SenderId")]
        public string Id { get; set; }

        [Key("RoomNumber")]
        public int RoomNumber { get; set; }

        [Key("FileSize")]
        public long FileSize { get; set; }

        [Key("FileName")]
        public string FileName { get; set; }

        [Key("FileEdit")]
        public DateTime FileEditedAt { get; set; }

        [Key("FileCreate")]
        public DateTime FileCreatedAt { get; set; }
    }
}
