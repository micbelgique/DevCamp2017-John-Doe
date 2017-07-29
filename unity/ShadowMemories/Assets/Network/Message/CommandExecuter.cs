using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Network.Message
{
    public class CommandAction {
        public Action<AbstractCommand> launch { get; set; }

        public CommandAction(Action<AbstractCommand> launch) {
            this.launch = launch;
        }
    }

    public class CommandExecuter
    {
        private Dictionary<string, CommandAction> actions;

        public CommandExecuter(Dictionary<string, CommandAction> actions) {
            this.actions = actions;
        }

        public void execute(List<AbstractCommand> commands) {
            CommandAction action;
            CommandAction defaultAction;

            actions.TryGetValue("default", out defaultAction);

            foreach (AbstractCommand command in commands) {
                if(actions.TryGetValue(command.id, out action)) {
                    action.launch(command);
                }
                else if (defaultAction!=null)
                    defaultAction.launch(command);
            }
        }
    }
}
