using UnityEngine;

namespace TerraTiler2D
{
    public class WhileLoop_Node : Flow_Node
    {
        private Port<bool> togglePort;

        private Port<Flow> loopPort;

        private Port<Flow> completedPort;

        public WhileLoop_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.WhileLoop;
            SetTooltip("Repeatedly executes the graph along the Loop flow until the input boolean returns false. Then continues graph execution along the Completed flow.");
            searchMenuEntry = new string[] { "Control" };
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            togglePort = GeneratePort<bool>("Condition", PortDirection.Input, "Toggle", PortCapacity.Single, true);
        }

        protected override void InitializeOutputPorts()
        {
            loopPort = GeneratePort<Flow>("Loop", PortDirection.Output, "Loop", PortCapacity.Single, true);
            loopPort.SetOutputPortMethod(GetOutput);

            base.InitializeOutputPorts();

#if (UNITY_EDITOR)
            outputPort.port.portName = "Completed";
            outputPort.tooltip = "Continues graph execution after finishing looping.";
#endif

            completedPort = outputPort;
        }

        protected override void InitializeAdditionalElements()
        {

        }

        private Flow flow;
        private bool trickleDown;

        public override void ApplyBehaviour(Flow flow, bool trickleDown = true, bool waitingOnResult = false)
        {
            if (Glob.GetInstance().PauseBetweenNodes)
            {
                //If this node should loop at least once
                if ((bool)togglePort.GetPortVariable())
                {
                    this.flow = flow;
                    this.trickleDown = trickleDown;

                    startNextLoop();
                    return;
                }
            }
            else
            {
#if (UNITY_EDITOR)
                if (loopPort.port.connected)
                {
#else
                if (loopPort.connections.Count > 0)
                {
#endif
                    outputPort = loopPort;

                    while ((bool)togglePort.GetPortVariable())
                    {
                        base.ApplyBehaviour(flow, trickleDown);
                    }

                    outputPort = completedPort;
                }
            }


            base.ApplyBehaviour(flow, trickleDown);
        }

        private void startNextLoop()
        {
            outputPort = loopPort;

            EventManager.GetInstance().AddListener<ToggleNodeWaitingEvent>(handleEndOfLoop);

            base.ApplyBehaviour(flow, trickleDown, true);
        }

        private void handleEndOfLoop(ToggleNodeWaitingEvent evt)
        {
            //If a node can stop waiting
            if (!evt.toggle)
            {
                //If it is this node
                if (evt.node == this)
                {
                    EventManager.GetInstance().RemoveListener<ToggleNodeWaitingEvent>(handleEndOfLoop);

                    if (GraphRunner.GetInstance().HasGraphBeenStopped)
                    {
                        return;
                    }

                    //If this node is not finished looping yet
                    if ((bool)togglePort.GetPortVariable())
                    {
                        //Start the next loop
                        startNextLoop();
                        return;
                    }
                    //If this node should stop looping
                    else
                    {
                        outputPort = completedPort;

                        base.ApplyBehaviour(flow, trickleDown);
                    }
                }
            }
        }
    }
}
