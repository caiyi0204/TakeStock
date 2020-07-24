using System;

namespace TakeStock.ServiceInterf
{
    public interface IRabbitService
    {
        bool ListenReceive(Func<string, bool> Process);
        bool PushToMqtt(string Rfid);
        void CloseConnection();
    }
}
