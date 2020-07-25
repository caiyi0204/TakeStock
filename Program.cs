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
            var httpService = InitAutofac.GetFromFac<IHttpService>();

            httpService.Start();

            //rabbitser.ListenReceive(
            //    delegate (string command)
            //        {
            //            StaticEntity.Pool.Clear();
            //            if (command == CommandEnum.Start.ToString()) StaticEntity.MqttPushWork = true;
            //            else if(command == CommandEnum.Stop.ToString()) StaticEntity.MqttPushWork = false;
            //            return true;
            //        }
            //);

            //while (true) {
            //    Thread.Sleep(3000);
            //    Console.WriteLine("Send.....");
            //    rabbitser.PushToMqtt("dasdasda1321");
            //}

            //HttpServer httpServer = new HttpServer("http://*:8820/");
            //httpServer.Start();

            //var readerComponent = new ReaderComponent();
            //readerComponent.Usbconnect();
            //readerComponent.GeteAntennaNo(4);
            //readerComponent.StartReadEpc(false);

            Console.ReadLine();
        }
    }
}
