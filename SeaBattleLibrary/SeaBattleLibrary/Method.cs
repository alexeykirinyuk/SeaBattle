using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SeaBattleLibrary
{
    [JsonObject("M")]
    public class Method: Param
    {
        [JsonProperty("N")]
        private MethodName name;
        [JsonProperty("P")]
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
        public Method(MethodName Name) {
            this.name = Name;
        }
        public Method(MethodName Name, params Param[] paramts)
        {
            this.name = Name;
            this.paramts = paramts;
        }
        public Method(MethodName Name, int lengthParams)
        {
            paramts = new Param[lengthParams];
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

        public string Serialize()
        {
            JsonConverter[] converts = { new FooConverter() };
            return  JsonConvert.SerializeObject(this, new JsonSerializerSettings() { Converters = converts });
        } 
            
        public static Method Deserialize(string serializedString)
        {
            JsonConverter[] converts = { new FooConverter() };
            return JsonConvert.DeserializeObject<Method>(serializedString, new JsonSerializerSettings() { Converters = converts });
        }

        public enum MethodName
        {
            //методы сервера
            SetShips,               //установить корабли (Param[0]: List<Ship>)
            Exit,                   //игрок вышел из игры ()
            HitTheEnemy,            //ударить противника (Param[0]: Address)
            //методы клиента
            SetTurn,                //установить чей ход (Param[0]: Turn)
            Message,                //показать диалог с сообщением (Param[0]: ParamString)
            SetResultAfterYourHit,  //установить результаты после твоего удара  (Param[0]: ParamString, Param[1]: Turn, Param[2]: ParamFieldArray)
            SetResultAfterEnemyHit, //установить результаты после удара врага (Param[0]: ParamString, Param[1]: Turn, Param[2]: ParamFieldArray)
            GameOver,               //игра окончена (Param[0]: ParamTurn)
            YourEnemyExit           //ваш враг вышел из игры
        }
        public class IncorrectParamsException : Exception
        {
            public IncorrectParamsException(): base("Не корректные параметры для метода") {}
        }
    }

    public class FooConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Param));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            if (jo["type"].Value<string>() == "A")
                return jo.ToObject<Address>(serializer);

            if (jo["type"].Value<string>() == "M")
                return jo.ToObject<Method>(serializer);
            if (jo["type"].Value<string>() == "PS")
                return jo.ToObject<ParamConvert.ParamString>(serializer);
            if (jo["type"].Value<string>() == "PFA")
                return jo.ToObject<ParamConvert.ParamShipList>(serializer);
            if (jo["type"].Value<string>() == "PT")
                return jo.ToObject<ParamConvert.ParamTurn>(serializer);
            if (jo["type"].Value<string>() == "PSL")
                return jo.ToObject<ParamConvert.ParamShipList>(serializer);

            return null;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
