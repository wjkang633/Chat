using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClient
{
    [MessagePackObject]
    public class ChatData
    {
        [Key("Code")]
        public string Code { get; set; }
    }
}
