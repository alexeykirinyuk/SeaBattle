using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using SeaBattleLibrary;
using SeaBattleLibrary.src.Player;

namespace SeaBattleClient
{
    public class BattleCient
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
                form.BattleDialog("Error, No connection to the BattleServer");
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
                form.BattleDialog("Cannot listen server: " + e.Message);
            }
        }

        private void SendMethodOnServer(Method method)
        {
            string serializeMethod = method.Serialize();
            byte[] methodByte = Encoding.Unicode.GetBytes(serializeMethod.ToCharArray());
            udpClient.Send(methodByte, methodByte.Length);
        }

        #region server's methods
        public void StartGame(List<Ship> ships, Game.Regime regime)
        {
            try
            {
                if (!isStart) return;
                Method method = new Method(Method.MethodName.StartGame, ParamConvert.Convert(ships), ParamConvert.Convert(regime));
                SendMethodOnServer(method);
            }
            catch (Exception e)
            {
                form.BattleDialog("Error->" + e.Message);
            }
        }

        public void HitTheEnemy(Address address)
        {
            Method method = new Method(Method.MethodName.HitTheEnemy, ParamConvert.Convert(address));
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
                    ProcessTurn(ParamConvert.GetData<Player.Turn>(method[0]));
                    break;
                case Method.MethodName.Message:
                    form.BattleDialog(ParamConvert.GetData<string>(method[0]));
                    break;
                case Method.MethodName.SetResultAfterYourHit:
                    form.BattleDialog(ParamConvert.GetData<string>(method[0]));
                    ProcessTurn(ParamConvert.GetData<Player.Turn>(method[1]));
                    form.SetEnemyMap(ParamConvert.GetData<StatusField[,]>(method[2]));
                    break;
                case Method.MethodName.SetResultAfterEnemyHit:
                    form.BattleDialog(ParamConvert.GetData<string>(method[0]));
                    ProcessTurn(ParamConvert.GetData<Player.Turn>(method[1]));
                    form.SetMyMap(ParamConvert.GetData<StatusField[,]>(method[2]));
                    break;
                case Method.MethodName.GameOver:
                    ProcessStatusOver(ParamConvert.GetData<Player.Turn>(method[0]));
                    break;
                case Method.MethodName.YourEnemyExit:
                    form.LabelTurnText = "Выигрыш";
                    form.BattleDialog(ParamConvert.GetData<String>(method[0]));
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
                    form.BattleDialog("Ты победил врага!");
                    form.LabelTurnText = "Выигрыш";
                    break;
                case Player.Turn.ENEMY:
                    form.EnemyPanelEnamble = false;
                    form.BattleDialog("Ты проиграл!");
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
