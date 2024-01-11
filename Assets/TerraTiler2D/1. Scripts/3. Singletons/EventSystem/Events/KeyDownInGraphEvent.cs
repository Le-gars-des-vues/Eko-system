using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    [Serializable]
    public class KeyDownInGraphEvent : Event
    {
        public KeyDownEvent keyEvent;

        public KeyDownInGraphEvent Init(KeyDownEvent keyEvent)
        {
            this.keyEvent = keyEvent;

            base.Init();

            return this;
        }
    }
}
