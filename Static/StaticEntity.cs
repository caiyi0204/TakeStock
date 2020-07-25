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
    public static class StaticMsg{
        public static string NoCom = "无此命令";
        public static string ComponentFailed = "读写器启动失败";
        public static string ComponentCloseFailed = "读写器关闭失败";
        public static string Success = "操作成功";
    }
}
