using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SeaBattleLibrary
{
    public class ParamConvert
    {
        #region convert method
        public static Param Convert(String convertString)
        {
            return new ParamString(convertString);
        }

        public static Param Convert(StatusField[,] convertStatusFieldArray)
        {
            return new ParamFieldArray(convertStatusFieldArray);
        }

        public static Param Convert(Player.Turn convertTurn)
        {
            return new ParamTurn(convertTurn);
        }

        public static Param Convert(List<Ship> convertShipList)
        {
            return new ParamShipList(convertShipList);
        }
        #endregion

        #region deconvert method
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
        #endregion

        #region wrapper classes
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
        #endregion
    }
}
