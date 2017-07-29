using Assets.Scripts.Network.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Assets.Command;

namespace Assets.Scripts.Network.Json
{
    public class CommandConverter : CustomJsonConverter<AbstractCommand>
    {
        private static Dictionary<string, Type> __Factory = new Dictionary<string, Type>() {
            {"focus", typeof(FocusCommand) },
            {"frequency", typeof(FrequencyCommand) } 
        };

        protected override AbstractCommand Create(Type objectType, JObject jsonObject)
        {
            // examine the id value
            string id = (jsonObject["id"]).ToString();

            Type type;
            if(__Factory.TryGetValue(id, out type)) {
                return (AbstractCommand)Activator.CreateInstance(type);
            }
            else
                return null;
        }
    }
}
