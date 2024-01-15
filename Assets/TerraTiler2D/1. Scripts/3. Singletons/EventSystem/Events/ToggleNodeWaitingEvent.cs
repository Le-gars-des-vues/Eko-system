﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public class ToggleNodeWaitingEvent : Event
    {
        public Node node;
        public bool toggle;

        public ToggleNodeWaitingEvent Init(Node node, bool toggle)
        {
            this.node = node;
            this.toggle = toggle;

            base.Init();

            return this;
        }
    }
}