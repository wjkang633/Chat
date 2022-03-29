using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClient
{
    [MessagePackObject]
    public class JoinClientData : ChatData
    {
        [Key("Id")]
        public string Id { get; set; }

        [Key("Pw")]
        public string Pw { get; set; }

        [Key("Name")]
        public string Name { get; set; }
    }
}
