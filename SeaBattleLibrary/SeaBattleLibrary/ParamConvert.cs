using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        [JsonObject("PS")]
        public class ParamString : Param
        {
            [JsonProperty("DS")]
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
                this.type = "DS";
            }
        }

        [JsonObject("PFA")]
        public class ParamFieldArray : Param
        {
            [JsonProperty("DSF")]
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
                this.type = "DSF";
            }
        }
        
        [JsonObject("PT")]
        public class ParamTurn : Param
        {
            [JsonProperty("DT")]
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
                this.type = "DT";
            }
        }
        
        [JsonObject("PSL")]
        public class ParamShipList : Param
        {
            [JsonProperty("DSL")]
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
                this.type = "DSL";
            }
        }
        #endregion
    }
}
