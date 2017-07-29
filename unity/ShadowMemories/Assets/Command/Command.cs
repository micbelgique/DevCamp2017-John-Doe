using Assets.Scripts.Network.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Command
{
    public class TestCommand : ProducerCommand
    {
        public TestCommand()
        {
        }

        protected override string getId()
        {
            return "test";
        }
    }

    public class FrequencyCommand : AbstractCommand
    {
        public int value { get; set; }

        protected override string getId()
        {
            return "frequency";
        }
    }

    public class FocusCommand : AbstractCommand
    {
        public float value { get; set; }

        protected override string getId()
        {
            return "focus";
        }
    }
}
