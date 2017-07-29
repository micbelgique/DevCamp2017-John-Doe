using UniRx;
using Assets.Scripts.Network.Message;
using Newtonsoft.Json;
using UnityEngine;
using Assets.Scripts.Network.Json;
using System;

namespace Assets.Scripts.Network
{
    // this class expose helper methods for reactive streams and message deserialization
    public class StreamManager
    {
        const string __socketName = "DrawSocket";

        private static JsonSerializerSettings __serializerSettings = null;

        private static void intialize() {
            if (__serializerSettings == null) {
                var messageConverter = new MessageConverter();
                var commandConverter = new CommandConverter();
                __serializerSettings = new JsonSerializerSettings();
                __serializerSettings.Converters.Add(commandConverter);
                __serializerSettings.Converters.Add(messageConverter);
                __serializerSettings.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                __serializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            }
        }

        public static IObservable<StringMessage> onReceiveMessageAsObservable()
        {
            return ObservableWebSocketTrigger.instance.onReceiveMessageAsObservable(__socketName).Select(_ => new StringMessage(_));
        }

        public static IObservable<CommandMessage> onReceiveCommandAsObservable()
        {
            intialize();
            IObservable<string> socket = ObservableWebSocketTrigger.instance.onReceiveMessageAsObservable(__socketName);
            return socket.Select(_ => JsonConvert.DeserializeObject<CommandMessage>(_, __serializerSettings));
        }

        public static IObservable<CommandMessage> onReceiveCommandAsObservable(GameObject gameObject)
        {
            return onReceiveCommandAsObservable().TakeUntilDestroy(gameObject).Where(x => x.on == gameObject.name);
        }

     }
}
