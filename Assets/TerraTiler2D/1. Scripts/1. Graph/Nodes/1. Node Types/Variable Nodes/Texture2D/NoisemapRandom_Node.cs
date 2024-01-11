using UnityEngine;

namespace TerraTiler2D
{
    public class NoisemapRandom_Node : Noisemap_Node
    {
        public NoisemapRandom_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.NoisemapRandom;
            SetTooltip("Generates white noise.");
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();
        }

        protected override void InitializeOutputPorts()
        {
            base.InitializeOutputPorts();
        }

        protected override void InitializeAdditionalElements()
        {
            base.InitializeAdditionalElements();
        }

        //========== Node methods ==========

        protected override Texture2D GetNoisemap()
        {
            NoisemapRandom noisemapGenerator = new NoisemapRandom();
            return GetNoisemap(noisemapGenerator);
        }
    }
}
