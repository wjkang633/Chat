using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClient
{
    [MessagePackObject]
    public class LoginServerData : ChatData
    {
        [Key("Result")]
        public bool IsTrue { get; set; }

        [Key("Value")]
        public string ResultMessage { get; set; }

        [Key("Id")]
        public string Id { get; set; }
    }
}
