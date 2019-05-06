using System.Collections;
using UnityEngine;

namespace TLP.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class AnimatedPanel : MonoBehaviour
    {
        [HideInInspector]
        /// <summary>Show/hide animation progress, from 0 (hidden) to 1 (fully active).</summary>
        public float AnimationProgress;
        /// <summary>CanvasGroup reference for fading.</summary>
        public CanvasGroup CanvasGroup;
        /// <summary>RectTransform reference.</summary>
        public RectTransform RectTransform;
        /// <summary>Target anchoredPosition of the RectTransform. The actual anchoredPosition will be animated relative to this.</summary>
        public Vector2 AnchoredPosition;

#pragma warning disable 0649 // "Never assigned to"
        /// <summary>Use WindowManager.Instance.DefaultTransition.</summary>
        [SerializeField] private bool useDefaultTransition = true;
        /// <summary>The customized transition to use if UseDefaultTransition is false.</summary>
        [SerializeField] private UIAnimation customTransition;
#pragma warning restore

        public UIAnimation Transition { get { return (useDefaultTransition ? WindowManager.Instance.DefaultTransition : customTransition); } }

        protected virtual void Start()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            RectTransform = GetComponent<RectTransform>();
        }

        protected virtual void Reset()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            RectTransform = GetComponent<RectTransform>();
            AnchoredPosition = RectTransform.anchoredPosition;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            StartCoroutine(UIAnimator.AnimationRoutine(this, 1f));
        }
        public void Hide()
        {
            StartCoroutine(UIAnimator.AnimationRoutine(this, -1f, () => { gameObject.SetActive(false); }));
        }
    }
}