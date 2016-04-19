using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using SeaBattleLibrary;

namespace SeaBattleClient
{
    class BattleCient
    {  
        private int port;
        private string hostName;
        private UdpClient udpClient;
        private IPEndPoint remoteIpEndPoint;
        private BattleForm form;
        public bool isStart { get; private set; }

        public BattleCient(string hostName, int port, BattleForm form)
        {
            this.hostName = hostName;
            this.port = port;
            this.form = form;
            StartClient();
        }

        private void StartClient()
        {
            try
            {
                udpClient = new UdpClient(hostName, port);
                IPHostEntry remoteHostEntry = Dns.GetHostEntry(hostName);
                remoteIpEndPoint = new IPEndPoint(remoteHostEntry.AddressList[0], port);
                isStart = true;
                Thread serverListenerThread = new Thread(new ThreadStart(FromServer));
                serverListenerThread.IsBackground = true;
                serverListenerThread.Start();
            }
            catch (Exception)
            {
                form.ShowMessageBox("Error, No connection to the BattleServer");
            }
        }

        private void FromServer()
        {
            try
            {
                while (isStart)
                {
                    byte[] receive = udpClient.Receive(ref remoteIpEndPoint);
                    string message = Encoding.Unicode.GetString(receive);
                    CallMethod(Method.Deserialize(message));
                }
            }
            catch (Exception e)
            {
                form.ShowMessageBox("Cannot listen server: " + e.Message);
            }
        }

        private void SendMethodOnServer(Method method)
        {
            string serializeMethod = method.Serialize();
            byte[] methodByte = Encoding.Unicode.GetBytes(serializeMethod.ToCharArray());
            form.ShowMessageBox(method.ToString());
            udpClient.Send(methodByte, methodByte.Length);
        }

        #region server's methods
        public void SendShips(List<Ship> ships)
        {
            try
            {
                if (!isStart) return;
                Method method = new Method(Method.MethodName.SetShips, ParamConvert.Convert(ships));
                SendMethodOnServer(method);
            }
            catch (Exception e)
            {
                form.ShowMessageBox("Error->" + e.Message);
            }
        }

        public void HitTheEnemy(Address address)
        {
            Method method = new Method(Method.MethodName.HitTheEnemy, address);
            SendMethodOnServer(method);
        }

        public void ClientExit()
        {
            Method method = new Method(Method.MethodName.Exit, null);
            SendMethodOnServer(method);
            udpClient.Close();
            isStart = false;
        }
        #endregion

        private void CallMethod(Method method)
        {
            switch (method.Name)
            {
                case Method.MethodName.SetTurn:
                    ProcessTurn(ParamConvert.GetTurn(method[0]));
                    break;
                case Method.MethodName.Message:
                    form.ShowMessageBox(ParamConvert.GetString(method[0]));
                    break;
                case Method.MethodName.SetResultAfterYourHit:
                    form.ShowMessageBox(ParamConvert.GetString(method[0]));
                    ProcessTurn(ParamConvert.GetTurn(method[1]));
                    form.SetEnemyMap(ParamConvert.GetFieldArray(method[2]));
                    break;
                case Method.MethodName.SetResultAfterEnemyHit:
                    form.ShowMessageBox(ParamConvert.GetString(method[0]));
                    ProcessTurn(ParamConvert.GetTurn(method[1]));
                    form.SetMyMap(ParamConvert.GetFieldArray(method[2]));
                    break;
                case Method.MethodName.GameOver:
                    ProcessStatusOver(ParamConvert.GetTurn(method[0]));
                    break;
                case Method.MethodName.YourEnemyExit:
                    form.LabelTurnText = "Выигрыш";
                    form.ShowMessageBox(ParamConvert.GetString(method[0]));
                    break;
            }
        }

        private void ProcessTurn(Player.Turn turn)
        {
            switch (turn)
            {
                case Player.Turn.YOUR:
                    form.EnemyPanelEnamble = true;
                    form.LabelTurnText = "ты";
                    break;
                case Player.Turn.ENEMY:
                    form.EnemyPanelEnamble = false;
                    form.LabelTurnText = "противник";
                    break;
            }
        }

        private void ProcessStatusOver(Player.Turn turn)
        {
            switch (turn)
            {
                case Player.Turn.YOUR:
                    form.EnemyPanelEnamble = false;
                    form.ShowMessageBox("Ты победил врага!");
                    form.LabelTurnText = "Выигрыш";
                    break;
                case Player.Turn.ENEMY:
                    form.EnemyPanelEnamble = false;
                    form.ShowMessageBox("Ты проиграл!");
                    form.LabelTurnText = "Проигрыш";
                    break;
            }
        }

        private void SetEnemyMap(StatusField[,] enemyMap)
        {
            form.SetEnemyMap(enemyMap);
        }

        

        
    }
}
