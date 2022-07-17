using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Server
{
    public class ClientData
    {
        public string IP { get; set; }
        public string Port { get; set; }

        public string Key { get; set; }
        public DateTime TimeOfRemoval { get; set; }

    }
}
