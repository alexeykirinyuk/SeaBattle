using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using SeaBattleLibrary;
using System.Threading.Tasks;

namespace SeaBattleClient
{
    public class BattleCient
    {
        private TcpClient tcpClient;
        private BattleForm form;
        public bool isStart { get; private set; }

        private List<Ship> ships;
        private GameRegime regime;

        public BattleCient(BattleForm form, List<Ship> ships, GameRegime regime)
        {
            this.form = form;
            this.ships = ships;
            this.regime = regime;
            StartClient();
        }

        private void StartClient()
        {
            try
            {
                tcpClient = new TcpClient();
                
                tcpClient.Connect("127.0.0.1", 14000);
                isStart = true;
                Thread serverListenerThread = new Thread(FromServer);
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
                    var method = tcpClient.ListenMethod();
                    Task task = new Task(() => CallMethod(method));
                    task.Start();
                }
            }
            catch (Exception e)
            {
                form.BattleDialog("Cannot listen server: " + e.Message);
                isStart = false;
                tcpClient.Close();
            }
        }

        ~BattleCient()
        {
            tcpClient.Close();
        }

        #region server's methods
        public void StartGame()
        {
            try
            {
                if (!isStart) return;
                Method method = new Method(Method.MethodName.StartGame, ParamConvert.Convert(ships), ParamConvert.Convert(regime));
                tcpClient.SendMethod(method);
            }
            catch (Exception e)
            {
                form.BattleDialog("Error->" + e.Message);
            }
        }

        public void HitTheEnemy(Address address)
        {
            Method method = new Method(Method.MethodName.HitTheEnemy, ParamConvert.Convert(address));
            tcpClient.SendMethod(method);
        }

        public void ClientExit()
        {
            Method method = new Method(Method.MethodName.Exit);
            tcpClient.SendMethod(method);
            tcpClient.Close();
            isStart = false;
        }
        #endregion

        private void CallMethod(Method method)
        {
            if (method == null) return;

            switch (method.Name)
            {
                case Method.MethodName.GetStartGame:
                    StartGame();
                    break;
                case Method.MethodName.SetTurn:
                    ProcessTurn(ParamConvert.GetData<Turn>(method[0]));
                    break;
                case Method.MethodName.Message:
                    form.BattleDialog(ParamConvert.GetData<string>(method[0]));
                    break;
                case Method.MethodName.SetResultAfterYourHit:
                    form.BattleDialog(ParamConvert.GetData<string>(method[0]));
                    ProcessTurn(ParamConvert.GetData<Turn>(method[1]));
                    form.SetEnemyMap(ParamConvert.GetData<StatusField[,]>(method[2]));
                    break;
                case Method.MethodName.SetResultAfterEnemyHit:
                    form.BattleDialog(ParamConvert.GetData<string>(method[0]));
                    ProcessTurn(ParamConvert.GetData<Turn>(method[1]));
                    form.SetMyMap(ParamConvert.GetData<StatusField[,]>(method[2]));
                    break;
                case Method.MethodName.GameOver:
                    ProcessStatusOver(ParamConvert.GetData<Turn>(method[0]));
                    break;
                case Method.MethodName.YourEnemyExit:
                    form.LabelTurnText = "Выигрыш";
                    form.BattleDialog(ParamConvert.GetData<String>(method[0]));
                    break;
            }
        }

        private void ProcessTurn(Turn turn)
        {
            switch (turn)
            {
                case Turn.YOUR:
                    form.EnemyPanelEnamble = true;
                    form.LabelTurnText = "ты";
                    break;
                case Turn.ENEMY:
                    form.EnemyPanelEnamble = false;
                    form.LabelTurnText = "противник";
                    break;
            }
        }

        private void ProcessStatusOver(Turn turn)
        {
            switch (turn)
            {
                case Turn.YOUR:
                    form.EnemyPanelEnamble = false;
                    form.BattleDialog("Ты победил врага!");
                    form.LabelTurnText = "Выигрыш";
                    break;
                case Turn.ENEMY:
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
