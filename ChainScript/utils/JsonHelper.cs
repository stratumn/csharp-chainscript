using Newtonsoft.Json;
using Stratumn.CanonicalJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratumn.Chainscript.utils
{
    public static class JsonHelper
    {

        private static List<JsonConverter> ConverterList;
        static JsonHelper(){

            ConverterList = new List<JsonConverter> {new ProtoMessageConverter(),
                 new MemoryStreamJsonConverter() };

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = ConverterList
            };
        }

        /// <summary>
        /// Adds a new Converter to the internal list of converters used by Json
        /// </summary>
        /// <param name="converter"></param>
        public static void RegisterConverter(JsonConverter converter)
        {
            ConverterList.Add(converter);
        }

        public static T FromJson<T>(String json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string ToJson(Object json)
        {
            return JsonConvert.SerializeObject(json); 
        }

        public static T ObjectToObject<T>(object srcObj)
        {
            T value;
            if (srcObj == null)
                value = default(T);
            else
            if (typeof(T).IsAssignableFrom(srcObj.GetType()))
            {
                value = (T)srcObj;
            }
            else
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
                value = FromJson<T>(json);
            }
            return value;
        }

        public static Dictionary<String, Object> ObjectToMap(Object srcObject)
        {
            return (Dictionary<String, Object>)ObjectToObject<Dictionary<String, Object>>(srcObject);
        }



        public static string ToCanonicalJson(Object src)
        {
            string json = ToJson(src);

            return Canonicalizer.Canonicalize(json);

        }
    }

}
