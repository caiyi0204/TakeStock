using Autofac;
using RabbitMQ.Client;
using System;
using System.Configuration;
using TakeStock.Dtos;
using TakeStock.ServiceInterf;
using TakeStock.ServiceInterf.Impl;

namespace TakeStock
{
    public static class InitAutofac
    {
        static ContainerBuilder _Builder;
        static IContainer _container;

        public static void InitAutofacs()
        {
            RabbitConnectDto rabbitConnectDto = new RabbitConnectDto();
            rabbitConnectDto.Send = ConfigurationManager.AppSettings["Send"].ToString();
            rabbitConnectDto.Receive = ConfigurationManager.AppSettings["Receive"].ToString();
            rabbitConnectDto.HostName = ConfigurationManager.AppSettings["HostName"].ToString();
            rabbitConnectDto.UserName = ConfigurationManager.AppSettings["UserName"].ToString();
            rabbitConnectDto.Password = ConfigurationManager.AppSettings["Password"].ToString();


            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = rabbitConnectDto.HostName;
            factory.UserName = rabbitConnectDto.UserName;
            factory.Password = rabbitConnectDto.Password;

            IConnection connection;
            IModel PoolChannel;

            try
            {
                connection = factory.CreateConnection();
                PoolChannel = connection.CreateModel();
                PoolChannel.QueueDeclare(queue: rabbitConnectDto.Send, durable: true, exclusive: false, autoDelete: false, arguments: null);
                PoolChannel.QueueDeclare(queue: rabbitConnectDto.Receive, durable: true, exclusive: false, autoDelete: false, arguments: null);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            _Builder = new ContainerBuilder();
            _Builder.Register<RabbitConnectDto>(c=> rabbitConnectDto).SingleInstance();
            _Builder.Register<IConnection>(c => connection).SingleInstance();
            _Builder.Register<IModel>(c => PoolChannel).SingleInstance();
            _Builder.RegisterType<RabbitService>().As<IRabbitService>().SingleInstance().PropertiesAutowired();
            _container = _Builder.Build();
        }
        static IContainer Container
        {
            get
            {
                if (_container == null)
                {
                    _container = _Builder.Build();
                }
                return _container;
            }
        }
        public static T GetFromFac<T>()
        {
            T t = Container.Resolve<T>();
            return t;
        }
    }
}
