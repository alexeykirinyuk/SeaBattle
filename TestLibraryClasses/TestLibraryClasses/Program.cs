using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace TestLibraryClasses
{
    [JsonObject("A")]
    public class Address : Param
    {
        [JsonProperty("X")]
        private int x;
        [JsonProperty("Y")]
        private int y;

        [JsonIgnore]
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
        [JsonIgnore]
        public int Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public Address() { }
        public Address(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public bool Equals(Address address)
        {
            return x == address.x && y == address.y;
        }

        public static void Main()
        {
            Param par = new Address(1, 2);
            var ps = (Address)JsonConvert.DeserializeObject<Param>(str);
            Console.WriteLine(ps.X + "  " + ps.Y);
        }

    }

    public interface Param 
    {
        [JsonProperty("TypeObj")]
        private Type type
        {
        }
    }

    
}
