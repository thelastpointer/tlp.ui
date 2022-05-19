using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TLP.UI
{
    /// <summary>
    /// Central handler for tooltips.
    /// </summary>
    public class TooltipHandler : MonoBehaviour
    {
        /// <summary>
        /// Singleton access.
        /// </summary>
        public static TooltipHandler Instance { get; private set; }

        private GraphicRaycaster raycaster;
        private EventSystem eventSystem;

        // Built-in handler
        private bool useBuiltInTooltip = true;
        private RectTransform builtInTooltipContainer;
        private TMPro.TextMeshProUGUI builtInTooltipText;

        public delegate void TooltipEvent(Tooltip tooltip, Vector2 controlPosition);

        private void Awake()
        {
            if (Instance == null)
			{
                Instance = this;

                //...
			}
			else
            {
                // Remove component from gameobject
                Destroy(this);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}