using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using SeaBattleLibrary;

namespace SeaBattleClient
{
    class BattleCient
    {  
        private int Port;
        private string HostName;

        private UdpClient Client;
        private IPEndPoint RemoteIpEndPoint;
        private BattleForm Form;
        private Thread ServerListener;

        public bool isStart { get; set; }

        public BattleCient(string HostName, int Port)
        {
            this.HostName = HostName;
            this.Port = Port;
        }

        public void StartClient()
        {
            try
            {
                Client = new UdpClient(HostName, Port);
                IPHostEntry remoteHostEntry = Dns.GetHostEntry(HostName);
                RemoteIpEndPoint = new IPEndPoint(remoteHostEntry.AddressList[0], Port);
                isStart = true;
                ServerListener = new Thread(new ThreadStart(FromServer));
                ServerListener.Start();
            }
            catch (Exception)
            {
                Form.ShowMessageBox("Error, No connection to the BattleServer");
            }
        }

        public void SetBattleForm(BattleForm Form)
        {
            this.Form = Form;
        }

        public void SendShips(List<Ship> ships)
        {
            try
            {
                if (!isStart) return;
                string serializeList = JsonConvert.SerializeObject(ships);
                Method method = new Method(Method.NamesServer.SetShips, new string[] { serializeList });
                SendMethodOnServer(method);
            }
            catch (Exception e)
            {
                Form.ShowMessageBox("Error->" + e.Message);
            }
        }

        private void SendMethodOnServer(Method method)
        {
            string serializeMethod = JsonConvert.SerializeObject(method);
            byte[] methodByte = Encoding.ASCII.GetBytes(serializeMethod.ToCharArray());
            Form.ShowMessageBox(serializeMethod);
            Client.Send(methodByte, methodByte.Length);
        }

        private void FromServer()
        {   
            try {
                while (isStart)
                {
                    var receive = Client.Receive(ref RemoteIpEndPoint);
                    var message = Encoding.ASCII.GetString(receive);
                    CallMethod(ParseMethod(message));
                }
            }
            catch (Exception e)
            {
                Form.ShowMessageBox("Cannot listen server: " + e.Message);
            }
        }

        private Method ParseMethod(string message)
        {
            Method method = JsonConvert.DeserializeObject<Method>(message);
            return method;
        }

        private void CallMethod(Method method)
        {
            switch (method.MethodName)
            {
                case Method.NamesClient.SetStatus:
                    PrintStatus(JsonConvert.DeserializeObject<Game.Status>(method.Parameters[0]));
                    ProcessStatus(JsonConvert.DeserializeObject<Game.Status>(method.Parameters[0]));
                    break;
                case Method.NamesClient.Message:
                    Form.ShowMessageBox(method.Parameters[0]);
                    break;
                case Method.NamesClient.SetEnemyMap:
                    SetEnemyMap(JsonConvert.DeserializeObject<StatusField[,]>(method.Parameters[0]));
                    break;
            }
        }

        private void PrintStatus(Game.Status status)
        {
            Form.SetLabelStatus(status + "");
        }

        private void ProcessStatus(Game.Status status)
        {
            switch (status)
            {
                case Game.Status.YOUR:
                    Form.MyTurn();
                    break;
                case Game.Status.ENEMY:
                    Form.EnemyTurn();
                    break;
            }
        }

        public void HitTheEnemy(Address address)
        {
            Method method = new Method(Method.NamesServer.HitTheEnemy, new string[] {JsonConvert.SerializeObject(address)});
            SendMethodOnServer(method);
        }

        private void SetEnemyMap(StatusField[,] enemyMap)
        {
            Form.SetEnemyMap(enemyMap);
        }

        public void Exit()
        {
            Method method = new Method(Method.NamesServer.Exit, null);
            SendMethodOnServer(method);
            Client.Close();
            isStart = false;
            if(ServerListener.IsAlive) 
                ServerListener.Interrupt();
        }

        
        
    }
}
