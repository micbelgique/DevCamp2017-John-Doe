using Assets.Scripts.Network.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Assets.Scripts.Network.Json
{
    public class MessageConverter : CustomJsonConverter<AbstractMessage>
    {
        protected override AbstractMessage Create(Type objectType, JObject jsonObject)
        {
            // examine the id value
            string id = (jsonObject["id"]).ToString();

            // based on the $type, instantiate and return a new object
            switch (id)
            {
                case "string":
                    return new StringMessage();
                case "command":
                    return new CommandMessage();
                default:
                    return null;
            }
        }
    }
}
