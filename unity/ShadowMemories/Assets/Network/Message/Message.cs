using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using UniRx;
using UniRx.Triggers;

namespace Assets.Scripts.Network.Message
{
    public abstract class AbstractMessage
    {
        public string id { get { return getId(); } }

        public AbstractMessage() {
        }

        protected abstract string getId();
    }

    public class StringMessage : AbstractMessage
    {
        public string Payload { get; set; }
        public StringMessage() : base() {
        }

        public StringMessage(string Payload) 
        {
            this.Payload = Payload;
        }

        protected override string getId()
        {
            return "string";
        }
    }

 }
