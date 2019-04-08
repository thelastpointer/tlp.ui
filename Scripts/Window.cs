using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TLP.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Window : MonoBehaviour
    {
        [SerializeField] private string windowID;
        [SerializeField] private WindowAnimationType transition = WindowAnimationType.FadeIn;
        [SerializeField] private Selectable firstControl;
        [SerializeField] private Transform backgroundDenier;
        
        public string ID { get { return windowID; } }
        public WindowAnimationType Transition { get { return transition; } }
        public Selectable FirstControl { get { return firstControl; } }
        public Transform BackgroundDenier { get { return backgroundDenier; } }
        
        public UnityEvent OnActivated;
        public UnityEvent OnDeactivated;

        public UnityEvent OnWillActivate;
        public UnityEvent OnWillDeactivate;

        public UnityEvent OnSubmit;
        public UnityEvent OnCancel;

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
            if (WindowManager.Instance != null)
                WindowManager.Instance.RegisterWindow(this, windowID);
        }
    }
}