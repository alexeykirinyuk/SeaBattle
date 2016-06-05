using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using SeaBattleLibrary;
using System.Threading.Tasks;

namespace SeaBattleClient
{
    public class BattleCient
    {
        private TcpClient _tcpClient;
        private BattleForm _form;
        private List<Ship> _ships;
        private GameRegime _regime;

        public bool IsStart { get; private set; }

        public BattleCient(BattleForm form, List<Ship> ships, GameRegime regime)
        {
            this._form = form;
            this._ships = ships;
            this._regime = regime;
            StartClient();
        }

        private void StartClient()
        {
            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.Connect("127.0.0.1", 14000);
                IsStart = true;

                Thread serverListenerThread = new Thread(FromServer);
                serverListenerThread.IsBackground = true;
                serverListenerThread.Start();
            }
            catch (Exception)
            {
                _form.BattleDialog("Error, No connection to the BattleServer");
            }
        }

        private void FromServer()
        {
            try
            {
                while (IsStart)
                {
                    var method = _tcpClient.ListenMethod();
                    Task task = new Task(() => CallMethod(method));
                    task.Start();
                }
            }
            catch (Exception e)
            {
                _form.BattleDialog("Cannot listen server: " + e.Message);
                IsStart = false;
                _tcpClient.Close();
            }
        }

        ~BattleCient()
        {
            _tcpClient.Close();
        }

        #region Send
        public void HitTheEnemy(Address address)
        {
            if (!IsStart) return;

            Method method = new Method(
                Method.MethodName.HitTheEnemy, 
                ParamConvert.Convert(address)
                );

            _tcpClient.SendMethod(method);
        }

        public void ClientExit()
        {
            try
            {
                if (!IsStart) return;

                Method method = new Method(
                    Method.MethodName.Exit
                    );

                _tcpClient.SendMethod(method);
                _tcpClient.Close();
                IsStart = false;
            }
            catch (Exception) { }
        }
        #endregion
        
        #region Process server's message
        private void CallMethod(Method method)
        {
            if (method == null) return;

            switch (method.Name)
            {
                case Method.MethodName.GetStartGame:
                    GetStartGame();
                    break;
                case Method.MethodName.SetTurn:
                    SetTurn(
                        ParamConvert.GetData<Turn>(method[0])
                        );
                    break;
                case Method.MethodName.Message:
                    Message(
                        ParamConvert.GetData<string>(method[0])
                        );
                    break;
                case Method.MethodName.SetResultAfterYourHit:
                    SetResultAfterYourHit(
                        ParamConvert.GetData<string>(method[0]),
                        ParamConvert.GetData<Turn>(method[1]),
                        ParamConvert.GetData<StatusField[,]>(method[2])
                        );
                    break;
                case Method.MethodName.SetResultAfterEnemyHit:
                    SetResultAfterEnemyHit(
                        ParamConvert.GetData<string>(method[0]), 
                        ParamConvert.GetData<Turn>(method[1]),
                        ParamConvert.GetData<StatusField[,]>(method[2])
                        );
                    break;
                case Method.MethodName.GameOver:
                    GameOver(
                        ParamConvert.GetData<Turn>(method[0])
                        );
                    break;
                case Method.MethodName.YourEnemyExit:
                    YourEnemyExit(
                        ParamConvert.GetData<String>(method[0])
                        );
                    break;
            }
        }

        
        public void GetStartGame()
        {
            try
            {
                if (!IsStart) return;

                Method method = new Method(Method.MethodName.StartGame,
                    ParamConvert.Convert(_ships),
                    ParamConvert.Convert(_regime));

                _tcpClient.SendMethod(method);
            }
            catch (Exception e)
            {
                _form.BattleDialog("Error->" + e.Message);
            }
        }

        private void SetTurn(Turn turn)
        {
            switch (turn)
            {
                case Turn.YOUR:
                    _form.EnemyPanelEnamble = true;
                    _form.LabelTurnText = "ты";
                    break;
                case Turn.ENEMY:
                    _form.EnemyPanelEnamble = false;
                    _form.LabelTurnText = "противник";
                    break;
            }
        }

        private void Message(string message)
        {
            _form.BattleDialog(message);
        }

        private void SetResultAfterYourHit(string message, Turn whoTurn, StatusField[,] enemyMap)
        {
            _form.BattleDialog(message);
            SetTurn(whoTurn);
            _form.SetEnemyMap(enemyMap);
        }

        private void SetResultAfterEnemyHit(string message, Turn whoTurn, StatusField[,] myMap)
        {
            _form.BattleDialog(message);
            SetTurn(whoTurn);
            _form.SetMyMap(myMap);
        }

        private void GameOver(Turn turn)
        {
            switch (turn)
            {
                case Turn.YOUR:
                    _form.EnemyPanelEnamble = false;
                    _form.BattleDialog("Ты победил врага!");
                    _form.LabelTurnText = "Выигрыш";
                    break;
                case Turn.ENEMY:
                    _form.EnemyPanelEnamble = false;
                    _form.BattleDialog("Ты проиграл!");
                    _form.LabelTurnText = "Проигрыш";
                    break;
            }
        }

        private void YourEnemyExit(string message)
        {
            _form.LabelTurnText = "Выигрыш";
            _form.BattleDialog(message);
        }

        private void SetEnemyMap(StatusField[,] enemyMap)
        {
            _form.SetEnemyMap(enemyMap);
        }
        #endregion
    }
}
