using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClient
{
    [MessagePackObject]
    public class ChatListClientData : ChatData
    {
        [Key("Id")]
        public string Id { get; set; }
    }
}
