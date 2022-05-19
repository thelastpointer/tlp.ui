using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TLP.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Window : MonoBehaviour
    {
#pragma warning disable 0649

        [SerializeField] private string windowID;
        [SerializeField] private string preferredLayer;

        [SerializeField] private Selectable firstControl;

#pragma warning restore 0649

        public string ID => windowID;

        // TODO: Replace with readonly prop and SetCurrentLayer
        public Layer CurrentLayer;

        public string PreferredLayer => preferredLayer;

        public Selectable FirstControl => firstControl;

        public int OrderInLayer => -1;
        
        /// <summary>
        /// Window order changed, becomes the topmost window on its layer.
        /// </summary>
        public UnityEvent OnActivated;
        /// <summary>
        /// Window order changed, window is not on top anymore.
        /// </summary>
        public UnityEvent OnDeactivated;

        /// <summary>
        /// Window is shown, eg. its GameObject becomes activated.
        /// </summary>
        public UnityEvent OnShow;
        /// <summary>
        /// Window is hidden, eg. its GameObject becomes deactivated.
        /// </summary>
        public UnityEvent OnHide;

        //public UnityEvent OnLayerChanged;

        public void Show()
        {
            // Show THIS window
            WindowManager.Instance.ShowWindow(windowID);
        }
        
        public void Close()
        {
            WindowManager.Instance.CloseWindow(windowID);
        }

        protected void Start()
        {
            if (string.IsNullOrEmpty(windowID))
                Debug.LogWarning("Warning: window has no ID, this is probably unintended!", gameObject);
            else
            {
                if (WindowManager.Instance != null)
                    WindowManager.Instance.RegisterWindow(this, windowID);
            }
        }
    }
}