using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TakeStock.ServiceInterf;
using TakeStock.Static;
using Topshelf;

namespace TakeStock
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                string description = "汉脑盘点机器人服务";
                string displayName = "HNTakeStock";
                string serviceName = "HNTakeStock";

                HostFactory.Run(x =>
                {
                    x.Service<Service>(s =>
                    {
                        s.ConstructUsing(name => new Service());
                        s.WhenStarted(tc => tc.Start());
                        s.WhenStopped(tc => tc.Stop());
                    });

                    x.RunAsLocalSystem();
                    x.SetDescription(description); 
                    x.SetDisplayName(displayName);
                    x.SetServiceName(serviceName);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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

    public class Service
    {
        public IHttpService _httpService;
        public Service() {
            InitAutofac.InitAutofacs();
            _httpService = InitAutofac.GetFromFac<IHttpService>();
        }
        public void Start()
        {
            _httpService.Start();
        }
        public void Stop()
        {
            _httpService.Dispose();
        }
    }

}
