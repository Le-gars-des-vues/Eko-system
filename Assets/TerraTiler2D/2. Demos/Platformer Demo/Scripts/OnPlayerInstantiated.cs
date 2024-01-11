using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    [Serializable]
    public class Demo_OnPlayerInstantiated : Event
    {
        public Demo_CharacterController player;

        public Demo_OnPlayerInstantiated Init(Demo_CharacterController player)
        {
            this.player = player;

            base.Init();

            return this;
        }
    }
}
