using ClouReaderAPI.ClouInterface;
using ClouReaderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using TakeStock.Static;
using TakeStock.ServiceInterf;

namespace TakeStock
{
    public class AsynchronousMessage : IAsynchronousMessage
    {

        public IRabbitService rabbitService  = InitAutofac.GetFromFac<IRabbitService>();

        public void GPIControlMsg(GPI_Model gpi_model)
        {
            Console.WriteLine(gpi_model.ReaderName);
        }

        public void OutPutTags(Tag_Model tag)
        {
            if (StaticEntity.MqttPushWork) {//开启Push
                var addres = StaticEntity.Pool.TryAdd(tag.EPC, DateTime.Now);
                if (addres)
                {
                    rabbitService.PushToMqtt(tag.EPC);
                    Console.WriteLine(tag.ANT1_COUNT + "==>" + tag.ANT2_COUNT + "==>" + tag.RSSI + "==>" + tag.EPC);
                }
            }
           
        }

        public void OutPutTagsOver()
        {
            Console.WriteLine("OutPutTagsOver");
        }

        public void PortClosing(string connID)
        {
            Console.WriteLine(connID);
        }

        public void PortConnecting(string connID)
        {
            Console.WriteLine(connID);
        }

        public void WriteDebugMsg(string msg)
        {
            Console.WriteLine(msg);
        }

        public void WriteLog(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
