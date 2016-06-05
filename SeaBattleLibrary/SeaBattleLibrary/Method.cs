using System;
using System.Text;
using Newtonsoft.Json;

namespace SeaBattleLibrary
{
    [JsonObject("Method")]
    public class Method
    {
        [JsonProperty("Name")]
        public MethodName Name { get; private set; }

        [JsonProperty("Paramts")]
        private Param[] _paramts;

        [JsonIgnore]
        public int Length 
        {
            get
            {
                return _paramts.Length;
            }
        }

        #region Constructors
        public Method() { }

        public Method(MethodName name) {
            this.Name = name;
        }

        public Method(MethodName name, params Param[] paramts)
        {
            this.Name = name;
            this._paramts = paramts;
        }

        public Method(MethodName name, int lengthParams)
        {
            this.Name = name;
            this._paramts = new Param[lengthParams];
        }
        #endregion

        public Param this[int i]
        {
            get
            {
                return _paramts[i];
            }
            set
            {
                if (value == null)
                        new IncorrectParamsException();
                _paramts[i] = value;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Name.ToString());

            if (_paramts == null) return sb.ToString() + "();";

            sb.Append("(");
            for (int i = 0; i < _paramts.Length; i++)
            {
                if (_paramts[i] != null)
                {
                    sb.Append("Params[").Append(i).Append("]: ").Append(_paramts[i].ToString());
                    if (i != _paramts.Length - 1) sb.Append(",");
                }
            }
            sb.Append(");");
            return sb.ToString();
        }

        #region JSON
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, 
                new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        } 
            
        public static Method Deserialize(string serializedString)
        {
            return JsonConvert.DeserializeObject<Method>(serializedString, 
                new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        }
        #endregion

        public enum MethodName
        {
            #region server's methods
            /// <remarks></remarks>
            StartGame = 0,              // Начать игру (Param[0]: ShipList myListShip, Param[1]: GameRegim gameRegim)
            Exit = 1,                   // Игрок вышел из игры (null)
            HitTheEnemy = 2,            // Ударить противника (Param[0]: Address addressForHit)
            #endregion

            #region client's methods
            GetStartGame = 3,           // Запрос начала игры ()
            SetTurn = 4,                // Установить чей ход (Param[0]: Turn whoseTurn)
            Message = 5,                // Показать диалог с сообщением (Param[0]: ParamString message)
            SetResultAfterYourHit = 6,  // Установить результаты после твоего удара  (Param[0]: ParamString message,
                                        // Param[1]: Turn whoseTurn, Param[2]: ParamFieldArray mapEnemy)
            SetResultAfterEnemyHit = 7, // Установить результаты после удара врага (Param[0]: ParamString message,
                                        // Param[1]: Turn whoseTurn, Param[2]: ParamFieldArray mapYour)
            GameOver = 8,               // Игра окончена (Param[0]: ParamString message, Param[1]: ParamTurn whoWin)
            YourEnemyExit = 9           // Ваш враг вышел из игры (null)
            #endregion
        }

        public class IncorrectParamsException : Exception
        {
            public IncorrectParamsException(): base("Не корректные параметры для метода") {}
        }
    }
}
