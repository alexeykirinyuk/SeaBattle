using System;
using System.Collections.Generic;
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
    }

    public interface Param { }
}
