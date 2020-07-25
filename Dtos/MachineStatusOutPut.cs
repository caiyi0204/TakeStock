using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeStock.Dtos
{
    public class MachineStatusOutPut
    {
        /// <summary>
        /// 读写器运作情况
        /// </summary>
        public bool MachineWork { set; get; }
        /// <summary>
        /// 数据是否开启推送
        /// </summary>
        public bool MqttPushWork { set; get; }
    }
}
