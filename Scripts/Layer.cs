using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TLP.UI
{
    [System.Serializable]
    public class Layer
    {
        public string ID;
        public int Order;

        // "Modal" or "Stack"? "SingleWindow"? how the fuck would I call it when there's always only one window open?
        public bool Stacked = false;

        // Visible windows in this layer
        private List<Window> windows;

        public void ShowLayer(bool show) { }
        public void EnableLayer(bool enabled) { }

        public void SetOrder() { }
        //MoveUp? MoveDown? Move these to WindowManager?
    }
}