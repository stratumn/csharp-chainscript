using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratumn.Chainscript.utils
{
    public static class JsonHelper
    {
        public static T FromJson<T>(String json)
        {
            return JsonConvert.DeserializeObject<T>(json, new ProtoMessageConverter());
        }

        public static string ToJson(Object json)
        {
            return JsonConvert.SerializeObject(json, new ProtoMessageConverter());
        }

        public static T ObjectToObject<T>(object srcObj)
        {
            string json = null;
            if (srcObj is String)
            {
                json = srcObj.ToString();
            }
            else
            {
                json = ToJson(srcObj);
            }
            var value = FromJson<T>(json);

            return value;
        }

        public static Dictionary<String, Object> ObjectToMap(Object srcObject)
        {
            return (Dictionary<String, Object>)ObjectToObject<Dictionary<String,Object>>(srcObject);
        }



    }
}
