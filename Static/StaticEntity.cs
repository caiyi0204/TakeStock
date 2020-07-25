using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeStock.Static
{
    public static class StaticEntity
    {
        public static ConcurrentDictionary<string, DateTime> Pool { set; get; }=new ConcurrentDictionary<string, DateTime>();
        public static bool MachineWork { set; get; } = false;
        public static bool MqttPushWork { set; get; } = false;
    }

}
