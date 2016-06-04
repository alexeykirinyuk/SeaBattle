﻿using System;
using System.Text;
using Newtonsoft.Json;

namespace SeaBattleLibrary
{
    [JsonObject("Method")]
    public class Method
    {
        [JsonProperty("Name")]
        private MethodName name;
        [JsonProperty("Paramts")]
        private Param[] paramts;

        [JsonIgnore]
        public MethodName Name
        {
            get
            {
                return name;
            }
        }
        [JsonIgnore]
        public int Length 
        {
            get
            {
                return paramts.Length;
            }
        }

        public Method() { }

        public Method(MethodName name) {
            this.name = name;
        }

        public Method(MethodName name, params Param[] paramts)
        {
            this.name = name;
            this.paramts = paramts;
        }

        public Method(MethodName name, int lengthParams)
        {
            this.name = name;
            this.paramts = new Param[lengthParams];
        }

        public Param this[int i]
        {
            get
            {
                return paramts[i];
            }
            set
            {
                if (value == null)
                        new IncorrectParamsException();
                paramts[i] = value;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Name.ToString());
            if (paramts == null) return sb.ToString() + "();";
            sb.Append("(");
            for (int i = 0; i < paramts.Length; i++)
            {
                if (paramts[i] != null)
                {
                    sb.Append("Params[").Append(i).Append("]: ").Append(paramts[i].ToString());
                    if (i != paramts.Length - 1) sb.Append(",");
                }
            }
            sb.Append(");");
            return sb.ToString();
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        } 
            
        public static Method Deserialize(string serializedString)
        {
            return JsonConvert.DeserializeObject<Method>(serializedString, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        }

        public enum MethodName
        {
            #region server's methods
            StartGame = 0,              // Начать игру (Param[0]: ShipList myListShip, Param[1]: GameRegim gameRegim)
            Exit = 1,                   // Игрок вышел из игры (null)
            HitTheEnemy = 2,            //ударить противника (Param[0]: Address addressForHit)
            #endregion

            #region client's methods
            GetStartGame = 3,           // Запрос начала игры ()
            SetTurn = 4,                //установить чей ход (Param[0]: Turn whoseTurn)
            Message = 5,                //показать диалог с сообщением (Param[0]: ParamString message)
            SetResultAfterYourHit = 6,  //установить результаты после твоего удара  (Param[0]: ParamString message,
                                        //Param[1]: Turn whoseTurn, Param[2]: ParamFieldArray mapEnemy)
            SetResultAfterEnemyHit = 7, //установить результаты после удара врага (Param[0]: ParamString message,
                                        //Param[1]: Turn whoseTurn, Param[2]: ParamFieldArray mapYour)
            GameOver = 8,               //игра окончена (Param[0]: ParamString message, Param[1]: ParamTurn whoWin)
            YourEnemyExit = 9           //ваш враг вышел из игры (null)
            #endregion
        }

        public class IncorrectParamsException : Exception
        {
            public IncorrectParamsException(): base("Не корректные параметры для метода") {}
        }
    }

    
}
