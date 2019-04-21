using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseModel.log
{
    public class JsonHelper
    {
        public static string ObjToJsonString<T>(T obj)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return json;
        }

        public static T JsonToObj<T>(string jsonObject)
        {
            T t = JsonConvert.DeserializeObject<T>(jsonObject);
            return t;
        }



    }
}
