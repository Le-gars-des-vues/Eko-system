using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class SetSeed_Node : Flow_Node
    {
        private Port<int> seedPort;

        //========== Initialization ==========

        public SetSeed_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.SetSeed;
            SetTooltip("Set the seed. A seed defines how random values are generated.");
            searchMenuEntry = new string[] { "Flow" };
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            seedPort = GeneratePort<int>("Seed", PortDirection.Input, "Seed", PortCapacity.Single, false, "The integer to set the seed to. Default value is 'System.DateTime.Now.Ticks'.");
        }

        protected override void InitializeOutputPorts()
        {
            base.InitializeOutputPorts();
        }

        protected override void InitializeAdditionalElements()
        {

        }

        //========== Node methods ==========

        public override void ApplyBehaviour(Flow flow, bool trickleDown = true, bool waitingOnResult = false)
        {
            int seed;

            object seedObject = seedPort.GetPortVariable();
            if (seedObject != null)
            {
                seed = (int)seedObject;
            }
            else
            {                
                //seed = Random.Range(int.MinValue, int.MaxValue);
                seed = (int)System.DateTime.Now.Ticks;
            }

            Random.InitState(seed);

            base.ApplyBehaviour(flow, trickleDown);
        }

        //========== Port data passing ==========

    }
}
