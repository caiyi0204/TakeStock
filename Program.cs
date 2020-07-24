using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TakeStock.ServiceInterf;
using TakeStock.Static;

namespace TakeStock
{
    class Program
    {
        static void Main(string[] args)
        {
            InitAutofac.InitAutofacs();
            var rabbitser = InitAutofac.GetFromFac<IRabbitService>();
            rabbitser.ListenReceive(
                delegate (string command)
                    {
                        StaticEntity.Pool.Clear();
                        if (command == CommandEnum.Start.ToString()) StaticEntity.IsWork = true;
                        else if(command == CommandEnum.Stop.ToString()) StaticEntity.IsWork = false;
                        return true;
                    }
            );
            //while (true) {
            //    Thread.Sleep(3000);
            //    Console.WriteLine("Send.....");
            //    rabbitser.PushToMqtt("dasdasda1321");
            //}

            //var readerComponent = new ReaderComponent();
            //readerComponent.Usbconnect();
            //readerComponent.GeteAntennaNo(4);
            //readerComponent.StartReadEpc(false);
            Console.ReadLine();
        }
    }
}
