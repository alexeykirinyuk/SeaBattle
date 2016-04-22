using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SeaBattleLibrary
{
    public class ParamConvert
    {
        #region convert method
        /*
        public static Param Convert(string convertString)
        {
            return new ParamData<string>(convertString);
        }

        public static Param Convert(StatusField[,] convertStatusFieldArray)
            

        

        public static Param Convert(List<Ship> convertShipList)
        {
            return new ParamShipList(convertShipList);
        }

        public static Param Convert(Game.Regime convertGameRegime)
        {
            return new ParamGameRegime(convertGameRegime);
        }
        */

        public static Param Convert<T>(T data)
        {
            return new ParamData<T>(data);
        }
        #endregion

        #region deconvert method
        /*
        public static String GetString(Param paramString)
        {
            ParamString str = (ParamString)paramString;
            return str.Data;
        }

        public static StatusField[,] GetFieldArray(Param paramFieldArray)
        {
            ParamFieldArray fieldArray = (ParamFieldArray)paramFieldArray;
            return fieldArray.Data;
        }

        public static Player.Turn GetTurn(Param paramTurn)
        {
            ParamTurn turn = (ParamTurn)paramTurn;
            return turn.Data;
        }

        public static List<Ship> GetShipList(Param paramShipList)
        {
            ParamShipList shipList = (ParamShipList)paramShipList;
            return shipList.Data;
        }

        public static Game.Regime GetGameRegime(Param paramGameRegime)
        {
            return ((ParamGameRegime)paramGameRegime).Data;
        }
        */

        public static T GetData<T>(Param param)
        {
            return ((ParamData<T>)param).Data;
        }
        #endregion

        #region wrapper classes
        /*
        [JsonObject("ParamString")]
        public class ParamString : Param
        {
            [JsonProperty("DataString")]
            private string data;

            [JsonIgnore]
            public string Data
            {
                get
                {
                    return data;
                }
            }

            public ParamString() { }
            public ParamString(string data)
            {
                this.data = data;
            }
            public override string ToString()
            {
                return "ParamString";
            }
        }

        [JsonObject("ParamFields")]
        public class ParamFieldArray : Param
        {
            [JsonProperty("DataFields")]
            private StatusField[,] data;

            [JsonIgnore]
            public StatusField[,] Data
            {
                get
                {
                    return data;
                }
            }

            public ParamFieldArray() { }
            public ParamFieldArray(StatusField[,] data)
            {
                this.data = data;
            }
            public override string ToString()
            {
                return "ParamFieldArray";
            }
        }
        
        [JsonObject("ParamTurn")]
        public class ParamTurn : Param
        {
            [JsonProperty("DataTurn")]
            private Player.Turn data;

            [JsonIgnore]
            public Player.Turn Data
            {
                get
                {
                    return data;
                }
            }

            public ParamTurn() { }
            public ParamTurn(Player.Turn data)
            {
                this.data = data;
            }
            public override string ToString()
            {
                return "ParamTurn";
            }
        }
        
        [JsonObject("ParamShips")]
        public class ParamShipList : Param
        {
            [JsonProperty("DataShips")]
            private List<Ship> data;
            
            [JsonIgnore]
            public List<Ship> Data
            {
                get
                {
                    return data;
                }
            }

            public ParamShipList() { }

            public ParamShipList(List<Ship> data)
            {
                this.data = data;
            }

            public override string ToString()
            {
                return "ParamShipList";
            }
        }

        [JsonObject("ParamGameRegime")]
        public class ParamGameRegime: Param
        {
            [JsonProperty("DataGameRegime")]
            private Game.Regime data;

            public Game.Regime Data
            {
                get
                {
                    return data;
                }
            }
            
            public ParamGameRegime() { }

            public ParamGameRegime(Game.Regime data)
            {
                this.data = data;
            }

            public override string ToString()
            {
                return "ParamGameRegime";
            }
        }
        */

        [JsonObject("ParamData")]
        public class ParamData<T>: Param
        {
            [JsonProperty("Data")]
            private T data;

            [JsonIgnore]
            public T Data
            {
                get
                {
                    return data;
                }
            }

            public ParamData() { }

            public ParamData(T data)
            {
                this.data = data;
            }

            public override string ToString()
            {
                return data.GetType().ToString();
            }
        }

        
        #endregion
    }

    public interface Param { }
}
