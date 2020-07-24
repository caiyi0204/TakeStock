using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeStock.Dtos
{
    public class RabbitConnectDto
    {
        public string Send { set; get; }
        public string Receive { set; get; }
        public string HostName { set; get; }
        public string UserName { set; get; }
        public string Password { set; get; }
    }
}
