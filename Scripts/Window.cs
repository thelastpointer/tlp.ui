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
        [SerializeField] private bool useDefaultTransition = true;
        [SerializeField] private WindowTransition customTransition;
#pragma warning restore 0649

        public string ID { get { return windowID; } }
        
        public Selectable FirstControl { get { return firstControl; } }
        public Transform BackgroundDenier { get { return backgroundDenier; } }
        public WindowTransition Transition
        {
            get
            {
                if (useDefaultTransition && (WindowManager.Instance != null))
                    return WindowManager.Instance.DefaultTransition;
                return customTransition;
            }
        }

        public UnityEvent OnActivated;
        public UnityEvent OnDeactivated;

        public UnityEvent OnWillActivate;
        public UnityEvent OnWillDeactivate;

        public UnityEvent OnSubmit;
        public UnityEvent OnCancel;

        public UnityEvent OnNextWindow;
        public UnityEvent OnPreviousWindow;

        public void ShowWindow(string id)
        {
            WindowManager.Instance.ShowWindow(id);
        }
        public void Back()
        {
            WindowManager.Instance.Back();
        }

        protected virtual void Start()
        {
            if (string.IsNullOrEmpty(windowID))
                Debug.LogWarning("Warning: window has no ID, this is probably unintended!", gameObject);
            else
            {
                if (WindowManager.Instance != null)
                    WindowManager.Instance.RegisterWindow(this, windowID);
            }
        }

        protected void Update()
        {
            if (WindowManager.Instance.HasPrevNextButtons())
            {
                if (Input.GetButtonDown(WindowManager.Instance.PreviousUIButton) && (OnPreviousWindow != null))
                    OnPreviousWindow.Invoke();
                if (Input.GetButtonDown(WindowManager.Instance.NextUIButton) && (OnNextWindow != null))
                    OnNextWindow.Invoke();
            }
        }
    }
}