using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeStock.Static
{
    public enum CommandEnum
    {
        /// <summary>
        /// 读写器关闭
        /// </summary>
        Stop = 0,
        /// <summary>
        /// 读写器开启
        /// </summary>
        Start = 1,
        /// <summary>
        /// 推送开启
        /// </summary>
        PushStart = 2,
        /// <summary>
        /// 推送关闭
        /// </summary>
        PushStop = 3,
        /// <summary>
        /// 全部开启
        /// </summary>
        AllStart = 4,
        /// <summary>
        /// 全部关闭
        /// </summary>
        AllStop = 5
    }
}
