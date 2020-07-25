using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using TakeStock.Dtos;
using TakeStock.Static;

namespace TakeStock.ServiceInterf.Impl
{
    public class RabbitService : IRabbitService
    {
        public RabbitConnectDto rabbitConnectDto { set; get; }
        public IConnection Connection { set; get; }
        public IModel PoolChannel { set; get; }

        /// <summary>
        /// 接收命令
        /// </summary>
        /// <param name="Process"></param>
        /// <returns></returns>
        public bool ListenReceive(Func<string, bool> Process) {
            var consumer = new EventingBasicConsumer(PoolChannel);
            PoolChannel.BasicQos(0, 30, false);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Process(message);
                PoolChannel.BasicAck(ea.DeliveryTag, false);
            };
            PoolChannel.BasicConsume(rabbitConnectDto.Receive, false, consumer);
            return true;
        }

        /// <summary>
        /// 逐条推送到服务端处理队列
        /// </summary>
        /// <param name="Rfid"></param>
        /// <param name="frameCode"></param>
        /// <param name="StoType"></param>
        /// <param name="picName"></param>
        /// <param name="triggerRayTime"></param>
        /// <returns></returns>
        public bool PushToMqtt(string Rfid)
        {
            if (!StaticEntity.MqttPushWork) {
                return false;
            }
            TakeUpServiceOutPut takeUpServiceOutPut = new TakeUpServiceOutPut() {
                CreateTime = DateTime.Now,
                Rfid = Rfid
                };
            string msgJson = JsonConvert.SerializeObject(takeUpServiceOutPut);
            var body = Encoding.UTF8.GetBytes(msgJson);
            this.PoolChannel.BasicPublish("", rabbitConnectDto.Send,null, body);
            return true;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnection()
        {
            if (!this.PoolChannel.IsClosed) {
                PoolChannel.Close();
            }
            if (this.Connection != null)
            {
                this.Connection.Close();
            }
        }

    }
}
