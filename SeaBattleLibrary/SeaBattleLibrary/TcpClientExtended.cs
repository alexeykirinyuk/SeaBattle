using System;
using System.Net.Sockets;
using System.Text;

namespace SeaBattleLibrary
{
    public static class SendReceive
    {
        public static void SendMethod(this TcpClient tcpClient, Method method)
        {
            try
            {
                var stream = tcpClient.GetStream();
                var strMethod = method.Serialize();
                var byteMethod = Encoding.UTF8.GetBytes(strMethod);

                Console.WriteLine(method);
                stream.Write(byteMethod, 0, byteMethod.Length);
            }
            catch(Exception)
            {
                Console.WriteLine("Ошибка при отправке сообщения");
            }
        }

        public static Method ListenMethod(this TcpClient tcpClient)
        {
                var byteMethod = new byte[tcpClient.ReceiveBufferSize];
                tcpClient.GetStream().Read(byteMethod, 0, byteMethod.Length);
                var strMethod = Encoding.UTF8.GetString(byteMethod);
                return Method.Deserialize(strMethod);
        }
    }
}
