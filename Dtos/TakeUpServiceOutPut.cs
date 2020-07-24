using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeStock.Dtos
{
    /// <summary>
    /// 提交服务器的类
    /// </summary>
    public class TakeUpServiceOutPut
    {
        public DateTime CreateTime { set; get; }
        public string Rfid { set; get; }
    }
}
