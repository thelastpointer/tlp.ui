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
        public Layer CurrentLayer;

        public string PreferredLayer => preferredLayer;

        public Selectable FirstControl => firstControl;


        public int OrderInLayer => -1;
        
        /// <summary>
        /// Window becomes the topmost window on its layer.
        /// </summary>
        public UnityEvent OnActivated;
        // Window is not topmost anymore.
        public UnityEvent OnDeactivated;

        public UnityEvent OnShow;
        public UnityEvent OnHide;

        //OnShow, OnHide
        //public UnityEvent OnNextWindow;
        //public UnityEvent OnPreviousWindow;

        //MoveUp, MoveDown

        public void Show()
        {
            // Show THIS window
            WindowManager.Instance.ShowWindow(windowID);
        }
        /*
        // Back, or close current window
        public void Back()
        {
            WindowManager.Instance.Back();
        }
        */

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