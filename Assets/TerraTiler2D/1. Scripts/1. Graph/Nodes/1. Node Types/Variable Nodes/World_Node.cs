using UnityEngine;

namespace TerraTiler2D
{
    public class World_Node : Node
    {
        private Port<World> outputPort;

        private World outputWorld;

        public World_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.World;
            SetTooltip("Create a new empty world.");
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeInputPorts()
        {

        }

        protected override void InitializeOutputPorts()
        {
            //Create an output port.
            outputPort = GeneratePort<World>("New world", PortDirection.Output, "World", PortCapacity.Single, false, "New empty world. Returns a reference to the same World object on subsequent calls.");
            outputPort.SetOutputPortMethod(GetWorldOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetWorldOutput()
        {
            if (outputWorld == null)
            {
                //TODO
                outputWorld = new World();
            }

            return outputWorld;
        }

        public override void ResetNodeVariables()
        {
            outputWorld = null;

            base.ResetNodeVariables();
        }
    }
}
