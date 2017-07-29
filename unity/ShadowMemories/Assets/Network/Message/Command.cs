using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Network.Message
{
    // http://www.newtonsoft.com/json/help/html/SerializationAttributes.htm
    // http://stackoverflow.com/questions/20995865/deserializing-json-to-abstract-class
    public class CommandMessage : AbstractMessage
    {
        public string on { get; set; }
        public List<AbstractCommand> commands { get; set; }

        public CommandMessage() {
            this.commands = new List<AbstractCommand>();
        }

        protected override string getId()
        {
            return "command";
        }
    }

    public abstract class AbstractCommand
    {
        protected abstract string getId();
        public string id { get { return getId(); } }
    }

    public abstract class ProducerCommand : AbstractCommand
    {
        public ProducerCommand() {
            name = "auto-generated";
        }
        public string name { get; set; }

    }
}
