#if (UNITY_EDITOR)
using UnityEditor.Experimental.GraphView;

namespace TerraTiler2D
{
    public class Edge : UnityEditor.Experimental.GraphView.Edge
    {
        private Port currentInputPort;
        private Port currentOutputPort;

        public override void OnPortChanged(bool isInput)
        {
            if (isInput)
            {
                //If there is an old input port.
                if (currentInputPort != null)
                {
                    //TODO: Let the old input port handle the change.
                    //(currentInputPort.node as TT_Node)
                }

                currentInputPort = input;
            }
            else
            {
                //If there is an old output port.
                if (currentOutputPort != null)
                {
                    //TODO: Let the old output port handle the change.
                    //(currentOutputPort.node as TT_Node)
                }

                currentOutputPort = output;
            }

            base.OnPortChanged(isInput);
        }
    }
}
#endif
