using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class TilePlacerNoisemap_Node : TilePlacer_Node
    {
        private Port<Texture2D> noisemapPort;
        private PortWithField<float> brightnessThresholdPort;

        //========== Initialization ==========

        public TilePlacerNoisemap_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.TilePlacerNoisemap;
            SetTooltip("Places tiles in a world, based on the brightness of pixels in a Texture2D.");
            searchMenuEntry = new string[] { "Flow" };
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            noisemapPort = GeneratePort<Texture2D>("Noisemap", PortDirection.Input, "Noisemap", PortCapacity.Single, true, "The noisemap that specifies where to place tiles.");

            brightnessThresholdPort = GeneratePortWithField<float>("Brightness threshold", PortDirection.Input, 0.5f, "Brightness", PortCapacity.Single, false, "The minimum brightness value a pixel from the noisemap should have to place a tile.");
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

        public override void ApplyBehaviour(Flow flow, bool trickleDown = true, bool waitingOnResult = false)
        {
            base.ApplyBehaviour(flow, trickleDown);
        }

        public override TileLayer ApplyBehaviourOnTileLayer(TileLayer tileLayer)
        {
            base.ApplyBehaviourOnTileLayer(tileLayer);

            int tileIndex = tileLayer.GetIndexByTile(GetTile());

            //Get the brightness threshold from the brightnessThresholdPort
            float brightnessThreshold = (float)brightnessThresholdPort.GetPortVariable();

            //Get the noisemap
            Texture2D noiseMapTexture = (Texture2D)noisemapPort.GetPortVariable();
            if (noiseMapTexture == null)
            {
                return tileLayer;
            }

            //Check if the noisemap is big enough, and warn the user if it is not
            if (noiseMapTexture.width < tileLayer.generatedTiles.GetLength(0) || noiseMapTexture.height < tileLayer.generatedTiles.GetLength(1))
            {
                Glob.GetInstance().DebugString("The noisemap passed into node '" + GetTitle() + "' is smaller than the TileLayer.", Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
            }

            //Get the pixels from the noisemap
            Color[] noiseMapPixels = noiseMapTexture.GetPixels(0, 0, tileLayer.generatedTiles.GetLength(0), tileLayer.generatedTiles.GetLength(1));

            //For every tile along the Y axis
            for (int y = 0; y < generatedTiles.GetLength(1); y++)
            {
                //For every tile along the X axis
                for (int x = 0; x < generatedTiles.GetLength(0); x++)
                {
                    //Get the color of the pixel at position (X,Y)
                    Color currentPixel = noiseMapPixels[(y * generatedTiles.GetLength(0)) + x];

                    //If the brightness of the color is higher than the brightness threshold
                    if ((currentPixel.r + currentPixel.g + currentPixel.b) / 3.0f >= brightnessThreshold)
                    {
                        generatedTiles[x, y] = tileIndex;
                    }
                }
            }

            //Apply the generatedTiles to the tileLayer
            ApplyChanges(tileLayer);

            return tileLayer;
        }

        //========== Port data passing ==========

    }
}
