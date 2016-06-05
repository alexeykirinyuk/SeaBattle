using Newtonsoft.Json;

namespace SeaBattleLibrary
{
    public class ParamConvert
    {
        public static Param Convert<T>(T data)
        {
            return new ParamData<T>(data);
        }

        public static T GetData<T>(Param param)
        {
            return ((ParamData<T>)param).Data;
        }
        
        [JsonObject("ParamData")]
        private class ParamData<T>: Param
        {
            [JsonProperty("Data")]
            internal T Data { get; private set; }

            public ParamData() { }

            public ParamData(T data)
            {
                this.Data = data;
            }

            public override string ToString()
            {
                return Data.GetType().ToString();
            }
        }
    }

    public interface Param { }
}
