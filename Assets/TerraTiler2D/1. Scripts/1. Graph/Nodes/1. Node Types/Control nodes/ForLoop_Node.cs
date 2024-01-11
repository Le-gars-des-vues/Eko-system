using System;
using UnityEngine;

namespace TerraTiler2D
{
    public class ForLoop_Node : Flow_Node
    {
        private PortWithField<int> firstIndexPort;
        private PortWithField<int> lastIndexPort;

        private Port<Flow> loopPort;
        private Port<int> indexPort;

        private Port<Flow> completedPort;

        public ForLoop_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.ForLoop;
            SetTooltip("Executes the graph along the Loop flow a number of times, and then continues graph execution along the Completed flow.");
            searchMenuEntry = new string[] { "Control" };
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            firstIndexPort = GeneratePortWithField<int>("First index", PortDirection.Input, 0, "FirstIndex", PortCapacity.Single, false);
            lastIndexPort = GeneratePortWithField<int>("Last index", PortDirection.Input, 10, "LastIndex", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            loopPort = GeneratePort<Flow>("Loop", PortDirection.Output, "Loop", PortCapacity.Single, false);
            loopPort.SetOutputPortMethod(GetOutput);

            indexPort = GeneratePort<int>("Index", PortDirection.Output, "LoopIndex", PortCapacity.Multi, false);
            indexPort.SetOutputPortMethod(GetIndex);

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

        private int loopIndex = int.MinValue;
        private Flow flow;
        private bool trickleDown;

        public override void ApplyBehaviour(Flow flow, bool trickleDown = true, bool waitingOnResult = false)
        {
            if (Glob.GetInstance().PauseBetweenNodes)
            {
                //Set the starting index
                loopIndex = (int)firstIndexPort.GetPortVariable();

                //If this node should loop at least once
                if (loopIndex < (int)lastIndexPort.GetPortVariable())
                {
                    this.flow = flow;
                    this.trickleDown = trickleDown;

                    startNextLoop();
                    return;
                }
            }
            else
            {
                outputPort = loopPort;

                for (loopIndex = (int)firstIndexPort.GetPortVariable(); loopIndex < (int)lastIndexPort.GetPortVariable(); loopIndex++)
                {
                    base.ApplyBehaviour(flow, trickleDown);
                }

                outputPort = completedPort;
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

                    loopIndex++;

                    //If this node is not finished looping yet
                    if (loopIndex < (int)lastIndexPort.GetPortVariable())
                    {
                        //Start the next loop
                        startNextLoop();
                        return;
                    }
                    //If this node has looped enough times
                    else
                    {
                        outputPort = completedPort;

                        base.ApplyBehaviour(flow, trickleDown);
                    }
                }
            }
        }

        private object GetIndex()
        {
            if (loopIndex == int.MinValue)
            {
                return (int)firstIndexPort.GetPortVariable();
            }

            return (int)loopIndex;
        }
    }
}
