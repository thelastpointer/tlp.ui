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
        [SerializeField] private Selectable firstControl;
        [SerializeField] private Transform backgroundDenier;
#pragma warning restore 0649

        public string ID { get { return windowID; } }

        public Selectable FirstControl { get { return firstControl; } }
        public Transform BackgroundDenier { get { return backgroundDenier; } }
        
        public UnityEvent OnActivated;
        public UnityEvent OnDeactivated;

        //OnShow, OnHide
        public UnityEvent OnNextWindow;
        public UnityEvent OnPreviousWindow;

        
        //MoveUp, MoveDown

        public void ShowWindow(string id)
        {
            WindowManager.Instance.ShowWindow(id);
        }
        public void Back()
        {
            WindowManager.Instance.Back();
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