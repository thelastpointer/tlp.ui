using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TLP.UI
{
    /// <summary>
    /// Manages UI Windows. Only one window and one popup may active at any time!
    /// </summary>
    public class WindowManager : MonoBehaviour
    {
        // This is a singleton
        private static WindowManager instance;
        public static WindowManager Instance { get { return instance; } }

        #region Editor-assigned fields

        [Header("Window transition animations")]
        [SerializeField] private float transitionTime = 0.1f;
        [SerializeField] private float transitionStrength = 200f;
        [SerializeField] private AnimationCurve popupInCurve = new AnimationCurve(new Keyframe[] {
            new Keyframe { time=0, value=0.75f },
            new Keyframe { time=0.33f, value=1, inTangent=0.72f, outTangent = 0.72f },
            new Keyframe { time=1, value=1, inTangent=0.78f, outTangent=0.78f }
        });
        [SerializeField] private AnimationCurve popupOutCurve = new AnimationCurve(new Keyframe[] {
            new Keyframe { time=0, value=0.75f },
            new Keyframe { time=1, value=1, inTangent=0.81f, outTangent=0.81f }
        });
        
        [Header("Auto-registered windows")]
        [SerializeField] private Window[] autoRegisteredWindows;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip windowShowSound;
        [SerializeField] private AudioClip windowHideSound;
        [SerializeField] private AudioClip submitSound;
        [SerializeField] private AudioClip cancelSound;
        [SerializeField] private AudioClip selectionChangedSound;

        [Header("Callbacks")]
        public UnityEvent OnStart;
        //public UnityEvent OnWindowShown;
        
        #endregion
        
        #region Public stuff       

        public Window CurrentWindow { get { return (openWindows.Count == 0 ? null : openWindows[openWindows.Count - 1]); } }
        public float TransitionTime { get { return transitionTime; } }
        public float TransitionStrength { get { return transitionStrength; } }
        public AnimationCurve PopupInCurve { get { return popupInCurve; } }
        public AnimationCurve PopupOutCurve { get { return popupOutCurve; } }
        
        public enum Sound
        {
            WindowShow,
            WindowHide,
            Submit,
            Cancel,
            SelectionChanged
        }

        public static void PlaySound(Sound sound)
        {
            if (instance.audioSource != null)
            {
                AudioClip clip = null;

                switch (sound)
                {
                    case Sound.WindowShow:
                        clip = instance.windowShowSound;
                        break;
                    case Sound.WindowHide:
                        clip = instance.windowHideSound;
                        break;
                    case Sound.Submit:
                        clip = instance.submitSound;
                        break;
                    case Sound.Cancel:
                        clip = instance.cancelSound;
                        break;
                    case Sound.SelectionChanged:
                        clip = instance.selectionChangedSound;
                        break;
                    default:
                        break;
                }

                if (clip != null)
                    instance.audioSource.PlayOneShot(clip);
            }
        }

        public void ShowWindow(string id)
        {
            if (!windows.ContainsKey(id))
                throw new System.ArgumentException("id");

            if (transitionRoutine == null)
            {
                if (openWindows.Count == 0)
                    transitionRoutine = StartCoroutine(SwitchWindows(null, windows[id], false));
                else
                    transitionRoutine = StartCoroutine(SwitchWindows(openWindows[openWindows.Count - 1], windows[id], false));
            }
                
        }
        public void Back()
        {
            if (transitionRoutine == null)
            {
                if (openWindows.Count > 1)
                    transitionRoutine = StartCoroutine(SwitchWindows(openWindows[openWindows.Count - 1], openWindows[openWindows.Count - 2], true));
                else if (openWindows.Count > 0)
                    transitionRoutine = StartCoroutine(SwitchWindows(openWindows[openWindows.Count - 1], null, true));
            }
        }
        public void ClearAll()
        {
            foreach (var wnd in windows.Values)
                wnd.gameObject.SetActive(false);
            openWindows.Clear();
        }

        public void RegisterWindow(Window wnd, string id)
        {
            // Check for unintended overwrites
            if (windows.ContainsKey(id) && (windows[id] != wnd))
                Debug.LogError("Warning: window \"" + id + "\" overridden! This is probably unintended behaviour.\nPrevious: " + windows[id].name + "\nNew: " + wnd.name, windows[id].gameObject);
            windows[id] = wnd;
        }

        public Window GetWindow(string id)
        {
            if (windows.ContainsKey(id))
                return windows[id];
            return null;
        }

        #endregion

        #region Internals

        private Coroutine transitionRoutine;
        private Dictionary<string, Window> windows = new Dictionary<string, Window>();
        private List<Window> openWindows = new List<Window>();
        private GameObject selectedUIObject;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;

                DontDestroyOnLoad(gameObject);

                foreach (var wnd in autoRegisteredWindows)
                    RegisterWindow(wnd, wnd.ID);
            }
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            if (instance == this)
                OnStart.Invoke();
        }

        private void Update()
        {
            if ((audioSource != null) && (selectionChangedSound != null))
            {
                var newSelected = EventSystem.current.currentSelectedGameObject;
                if (selectedUIObject != newSelected)
                {
                    audioSource.PlayOneShot(selectionChangedSound);
                    selectedUIObject = newSelected;
                }
            }
        }

        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private IEnumerator SwitchWindows(Window from, Window to, bool remove)
        {
            if (from != null)
                yield return HideWindowInternal(from);

            if (remove)
                openWindows.Remove(from);
            
            if (to != null)
                yield return ShowWindowInternal(to);

            transitionRoutine = null;
        }

        private IEnumerator HideWindowInternal(Window wnd)
        {
            wnd.OnWillDeactivate.Invoke();

            if ((audioSource != null) && (windowHideSound != null))
                audioSource.PlayOneShot(windowHideSound);

            yield return WindowAnimator.Animate(wnd, transitionTime, transitionStrength, true, popupInCurve, popupOutCurve);

            wnd.OnDeactivated.Invoke();

            if (wnd.BackgroundDenier != null)
                wnd.BackgroundDenier.gameObject.SetActive(false);

            wnd.gameObject.SetActive(false);
        }
        private IEnumerator ShowWindowInternal(Window wnd)
        {
            if (wnd.BackgroundDenier != null)
            {
                // Get sibling index -- make sure to count the denier itself if they have the same parent
                int siblingIndex = wnd.transform.GetSiblingIndex();
                if (wnd.transform.parent == wnd.BackgroundDenier.parent)
                {
                    int denierIdx = wnd.BackgroundDenier.GetSiblingIndex();
                    if (denierIdx < siblingIndex)
                        siblingIndex--;
                }
                else
                    wnd.BackgroundDenier.SetParent(wnd.transform.parent);

                wnd.BackgroundDenier.SetSiblingIndex(siblingIndex);
                wnd.BackgroundDenier.gameObject.SetActive(true);
            }

            wnd.gameObject.SetActive(true);
            wnd.OnWillActivate.Invoke();
            openWindows.Add(wnd);

            if ((audioSource != null) && (windowShowSound != null))
                audioSource.PlayOneShot(windowShowSound);

            yield return WindowAnimator.Animate(wnd, transitionTime, transitionStrength, false, popupInCurve, popupOutCurve);

            if ((wnd.FirstControl != null) && ShouldActivateFirstControl())
                wnd.FirstControl.Select();
            
            wnd.OnActivated.Invoke();
        }

        private bool ShouldActivateFirstControl()
        {
            // ReWired:
            // return (ReInput.players.SystemPlayer.controllers.joystickCount > 0);
            return false;
        }

        #endregion
    }
}