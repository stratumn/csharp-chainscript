using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratumn.Chainscript.utils
{
    /// <summary>
    /// Lets Newtonsoft.Json and Protobuf's json converters play nicely
    /// together.  The default Netwtonsoft.Json Deserialize method will
    /// not correctly deserialize proto messages.
    /// </summary>
    public class ProtoMessageConverter : JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            return typeof(Google.Protobuf.IMessage)
                .IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
            System.Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            // Read an entire object from the reader.
            var converter = new ExpandoObjectConverter();
            object o = converter.ReadJson(reader, objectType, existingValue,
                serializer);
            // Convert it back to json text.
            string text = JsonConvert.SerializeObject(o);
            // And let protobuf's parser parse the text.
            IMessage message = (IMessage)Activator
                .CreateInstance(objectType);
            return Google.Protobuf.JsonParser.Default.Parse(text,
                message.Descriptor);
        }

        public override void WriteJson(JsonWriter writer, object value,
            JsonSerializer serializer)
        {
            writer.WriteRawValue(Google.Protobuf.JsonFormatter.Default
                .Format((IMessage)value));
        }
    }
}
