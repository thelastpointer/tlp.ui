using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLP.UI
{
    /// <summary>
    /// Manages UI Windows. Only one window and one popup may active at any time!
    /// </summary>
    public class WindowManager : MonoBehaviour
    {
        // This is a singleton
        public static WindowManager Instance { get; private set; }

        #region Editor-assigned fields

#pragma warning disable 0649

        [Header("Default Transition")]
        [SerializeField] private UIAnimation defaultTransition;

        [Header("Auto-registered windows")]
        [SerializeField] private Window[] autoRegisteredWindows;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip windowShowSound;
        [SerializeField] private AudioClip windowHideSound;
        [SerializeField] private AudioClip submitSound;
        [SerializeField] private AudioClip cancelSound;
        [SerializeField] private AudioClip selectionChangedSound;

        [Header("Controller Settings")]
        public bool UseController = false;
        public string NextUIButton = "";
        public string PreviousUIButton = "";

        [Header("Callbacks")]
        public UnityEvent OnStart;
        //public UnityEvent OnWindowShown;

#pragma warning restore 0649

        #endregion

        #region Public stuff

        // Getters
        public Window CurrentWindow { get { return (windowStack.Count == 0 ? null : windowStack[windowStack.Count - 1]); } }
        public UIAnimation DefaultTransition { get { return defaultTransition; } }

        /// <summary>
        /// Schedule the window for display.
        /// </summary>
        /// <param name="id">The window must be registered with this ID. Case-sensitive.</param>
        public void ShowWindow(string id)
        {
            // Unknown window
            if (!registeredWindows.ContainsKey(id))
                throw new System.ArgumentException("id");

            Window previousWindow = null;

            lock (windowStackLock)
            {
                if (windowStack.Count > 0)
                {
                    // Check if it's already at the top (== being shown)
                    // Note: this also covers the case when it is the only window in the stack so we can safely remove stuff later
                    if (windowStack[windowStack.Count - 1].ID == id)
                        return;

                    previousWindow = windowStack[windowStack.Count - 1];

                    // Check if it's in the list
                    for (int i = 0; i < windowStack.Count - 1; i++)
                    {
                        // If it's in, remove and push it to the top
                        if (windowStack[i].ID == id)
                        {
                            windowStack.Add(windowStack[i]);
                            windowStack.RemoveAt(i);
                            return;
                        }
                    }
                }

                // No changes were made, so just add it to the top
                windowStack.Add(registeredWindows[id]);
            }

            // Call events
            if (previousWindow != windowStack[windowStack.Count - 1])
            {
                if ((previousWindow != null) && (previousWindow.OnDeactivated != null))
                    previousWindow.OnDeactivated.Invoke();

                if (windowStack[windowStack.Count - 1].OnWillActivate != null)
                    windowStack[windowStack.Count - 1].OnWillActivate.Invoke();
            }
        }
        public void Close(string id)
        {
            lock (windowStackLock)
            {
                // If this is the top window, remove it from the stack
                if ((windowStack.Count > 0) && (windowStack[windowStack.Count - 1].ID == id))
                    windowStack.RemoveAt(windowStack.Count - 1);

                // ...it is already being hidden otherwise
            }
        }
        public void Back()
        {
            lock (windowStackLock)
            {
                // Remove top window from the stack
                if (windowStack.Count > 0)
                    windowStack.RemoveAt(windowStack.Count - 1);
            }
        }
        public void ClearAll()
        {
            foreach (var wnd in registeredWindows.Values)
                wnd.gameObject.SetActive(false);
            windowStack.Clear();
        }

        public void RegisterWindow(Window wnd, string id)
        {
            // Check for unintended overwrites
            if (registeredWindows.ContainsKey(id) && (registeredWindows[id] != wnd))
                Debug.LogError("Warning: window \"" + id + "\" overridden! This is probably unintended behaviour.\nPrevious: " + registeredWindows[id].name + "\nNew: " + wnd.name, registeredWindows[id].gameObject);
            registeredWindows[id] = wnd;
        }

        public Window GetWindow(string id)
        {
            if (registeredWindows.ContainsKey(id))
                return registeredWindows[id];
            return null;
        }

        // Sound
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
            if (Instance.audioSource != null)
            {
                AudioClip clip = null;

                switch (sound)
                {
                    case Sound.WindowShow:
                        clip = Instance.windowShowSound;
                        break;
                    case Sound.WindowHide:
                        clip = Instance.windowHideSound;
                        break;
                    case Sound.Submit:
                        clip = Instance.submitSound;
                        break;
                    case Sound.Cancel:
                        clip = Instance.cancelSound;
                        break;
                    case Sound.SelectionChanged:
                        clip = Instance.selectionChangedSound;
                        break;
                    default:
                        break;
                }

                if (clip != null)
                    Instance.audioSource.PlayOneShot(clip);
            }
        }

        public bool HasPrevNextButtons()
        {
            return (!string.IsNullOrEmpty(PreviousUIButton) && !string.IsNullOrEmpty(NextUIButton));
        }

        #endregion

        #region Internals

        private Dictionary<string, Window> registeredWindows = new Dictionary<string, Window>();
        private object windowStackLock = new object();
        private List<Window> windowStack = new List<Window>();
        private Window currentTopWindow;

        private GameObject selectedUIObject;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                DontDestroyOnLoad(gameObject);

                foreach (var wnd in autoRegisteredWindows)
                    RegisterWindow(wnd, wnd.ID);
            }
            else
            {
                // Remove this component from the GameObject
                Destroy(this);
            }
        }

        private void Start()
        {
            if (Instance == this)
            {
                // Enable controller support if there's a joystick connected
                UseController = (Input.GetJoystickNames().Length > 0);

                OnStart.Invoke();
            }
        }

        private void Update()
        {
            Window top = null;

            // Update window showing/hiding
            lock (windowStackLock)
            {
                // Always try to show the top window
                if (windowStack.Count > 0)
                {
                    top = windowStack[windowStack.Count - 1];
                    top.gameObject.SetActive(true);
                    UIAnimator.Animate(top, Time.unscaledDeltaTime);
                }

                // All other windows should be hidden
                foreach (var wnd in registeredWindows.Values)
                {
                    if (wnd != top)
                    {
                        UIAnimator.Animate(wnd, -Time.unscaledDeltaTime);
                        if (wnd.AnimationProgress == 0)
                            wnd.gameObject.SetActive(false);
                    }
                }
            }

            // Update background deniers
            if (top != currentTopWindow)
            {
                // Disable old denier
                if ((currentTopWindow != null) && (currentTopWindow.BackgroundDenier != null))
                    currentTopWindow.BackgroundDenier.gameObject.SetActive(false);

                // Enable background denier and move it in the hierarchy
                if ((top != null) && (top.BackgroundDenier != null))
                {
                    // Get sibling index -- make sure to count the denier itself if they have the same parent
                    int siblingIndex = top.transform.GetSiblingIndex();
                    if (top.transform.parent == top.BackgroundDenier.parent)
                    {
                        int denierIdx = top.BackgroundDenier.GetSiblingIndex();
                        if (denierIdx < siblingIndex)
                            siblingIndex--;
                    }
                    else
                        top.BackgroundDenier.SetParent(top.transform.parent);

                    top.BackgroundDenier.SetSiblingIndex(siblingIndex);
                    top.BackgroundDenier.gameObject.SetActive(true);
                }

                currentTopWindow = top;
            }

            // Play changed sound when the current selection changed
            if ((audioSource != null) && (selectionChangedSound != null))
            {
                var newSelected = EventSystem.current.currentSelectedGameObject;
                if (selectedUIObject != newSelected)
                {
                    audioSource.PlayOneShot(selectionChangedSound);
                    selectedUIObject = newSelected;
                }
            }

            // Prev/next buttons
            if ((CurrentWindow != null) && HasPrevNextButtons())
            {
                if (Input.GetButtonDown(PreviousUIButton) && (CurrentWindow.OnPreviousWindow != null))
                    CurrentWindow.OnPreviousWindow.Invoke();
                if (Input.GetButtonDown(NextUIButton) && (CurrentWindow.OnNextWindow != null))
                    CurrentWindow.OnNextWindow.Invoke();
            }

        }

        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();
        }

#if UNITY_EDITOR
        public void Editor_SetPreregistered(Window[] windows)
        {
            autoRegisteredWindows = windows;
        }
#endif

#endregion
    }
}

#if UNITY_EDITOR

namespace TLP.UI.Editors
{
    [CustomEditor(typeof(WindowManager))]
    public class WindowManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            if (GUILayout.Button("Assign all Windows"))
            {
                // Get all Window components in this scene
                List<Window> windows = new List<Window>();
                var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                foreach (var root in roots)
                    windows.AddRange(root.GetComponentsInChildren<Window>(true));

                // Sort by ID
                windows.Sort((w1, w2) => { return string.Compare(w1.ID, w2.ID); });

                (target as WindowManager).Editor_SetPreregistered(windows.ToArray());
                Debug.Log("Assigned " + windows.Count + " windows.");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif