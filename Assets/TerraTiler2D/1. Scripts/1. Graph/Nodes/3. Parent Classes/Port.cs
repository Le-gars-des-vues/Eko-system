namespace TerraTiler2D
{
    public class Port<T> : Port_Abstract
    {
        public Port(Node node, string portName, PortDirection portDirection, System.Type portType, PortCapacity capacity = PortCapacity.Single, bool isMandatory = true, string tooltip = "") : base(node, portName, portDirection, portType, capacity, isMandatory, tooltip)
        {

        }

#if (UNITY_EDITOR)
        public override void HandleConnected(Port_Abstract otherPort)
        {
            base.HandleConnected(otherPort);
        }

        public override void HandleDisconnected(UnityEditor.Experimental.GraphView.Edge edge)
        {
            base.HandleDisconnected(edge);

        }
#endif

        public override void CopyPort(Port_Abstract copyFrom)
        {
            //Nothing to copy for a normal port
        }
    }
}
