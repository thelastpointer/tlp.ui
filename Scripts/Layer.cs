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

        private List<Window> windowStack;

        public void ShowWindow(Window window)
        {
            Window previousWindow = null;

            if (windowStack.Count > 0)
            {
                // Check if it's already at the top (== being shown)
                // Note: this also covers the case when it is the only window in the stack so we can safely remove stuff later
                if (windowStack[windowStack.Count - 1] == window)
                    return;

                previousWindow = windowStack[windowStack.Count - 1];

                // Check if it's in the list
                for (int i = 0; i < windowStack.Count - 1; i++)
                {
                    // If it's in, remove and push it to the top
                    if (windowStack[i] == window)
                    {
                        windowStack.Add(windowStack[i]);
                        windowStack.RemoveAt(i);
                        return;
                    }
                }
            }

            // No changes were made, so just add it to the top
            windowStack.Add(window);

            // Call events
            if (previousWindow != windowStack[windowStack.Count - 1])
            {
                if ((previousWindow != null) && (previousWindow.OnDeactivated != null))
                    previousWindow.OnDeactivated.Invoke();
            }
        }

        public void CloseWindow(Window window)
        {
            // If this is the top window, remove it from the stack
            if ((windowStack.Count > 0) && (windowStack[windowStack.Count - 1] == window))
                windowStack.RemoveAt(windowStack.Count - 1);

            // ...it is already being hidden otherwise
        }

        public void Back()
        {
            // Remove top window from the stack
            if (windowStack.Count > 0)
                windowStack.RemoveAt(windowStack.Count - 1);
        }

        public void AddWindow(Window window)
        {
            //if (!windowStack.Contains(window))
            //    windowStack.Add(window);
        }
        public void RemoveWindow(Window window)
        {
            //windowStack.Remove(window);
        }
    }
}