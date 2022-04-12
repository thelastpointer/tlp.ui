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
    /// Manages UI Windows
    /// </summary>
    public class WindowManager : MonoBehaviour
    {
        // This is a singleton
        public static WindowManager Instance { get; private set; }

        #region Editor-assigned fields

#pragma warning disable 0649

        [Header("Auto-registered windows")]
        [SerializeField] private Window[] autoRegisteredWindows;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        /*
        [SerializeField] private AudioClip windowShowSound;
        [SerializeField] private AudioClip windowHideSound;
        [SerializeField] private AudioClip submitSound;
        [SerializeField] private AudioClip cancelSound;
        [SerializeField] private AudioClip selectionChangedSound;
        */

        [Header("Controller Settings")]
        public bool UseController = false;
        public string NextUIButton = "";
        public string PreviousUIButton = "";

        [Header("Callbacks")]
        public UnityEvent OnStart;
        public UnityEvent OnTopWindowChanged;

#pragma warning restore 0649

        #endregion

        public Layer CreateLayer(string id, int order) { return null; }
        public Layer GetLayer(string layerName)
        {
            throw new System.ArgumentException("No layer registered as " + layerName);
            return null;
        }

        private Window lastActivated;

        // Get the Window that was activated last
        public Window LastActivatedWindow => lastActivated;

        // Top layer, top window
        public Window TopWindow { get { return null; } }

        public Window[] GetActiveWindows() { return null; }

        public Layer DefaultLayer => null;

        //public UIAnimation DefaultTransition { get { return defaultTransition; } }

        /// <summary>
        /// Schedule the window for display.
        /// </summary>
        /// <param name="id">The window must be registered with this ID. Case-sensitive.</param>
        public void ShowWindow(string name)
        {
            string explicitLayerName;
            string windowName;
            SplitWindowPath(name, out explicitLayerName, out windowName);

            Window window = GetWindow(windowName);

            // Unknown window
            if (window == null)
                throw new System.ArgumentException("No window registered as " + windowName);

            lock (windowChangeLock)
            {
                // Resolve layer
                //      if explicitly specified, force that
                //      if not specified, use current layer
                //      if no current layer:
                //          check if window has a *layer preference*
                //          if not, use default
                Layer layer;
                if (!string.IsNullOrEmpty(explicitLayerName))
                {
                    layer = GetLayer(explicitLayerName);
                }
                else
                {
                    layer = window.CurrentLayer;

                    if (layer == null)
                    {
                        if (!string.IsNullOrEmpty(window.PreferredLayer))
                        {
                            layer = GetLayer(window.PreferredLayer);
                        }
                        else
                        {
                            layer = DefaultLayer;
                        }
                    }
                }

                // Add to layer for the first time
                if (window.CurrentLayer == null)
                {
                    layer.AddWindow(window);
                    window.CurrentLayer = layer;
                }
                // Move between layers
                else if (layer != window.CurrentLayer)
                {
                    window.CurrentLayer.RemoveWindow(window);
                    layer.AddWindow(window);
                    window.CurrentLayer = layer;

                    // TODO: Any events for this? Not sure if needed
                }

                // Pass window to layer (it will decide what happens, modal, bring to front, etc)
                layer.ShowWindow(window);
            }

            /*
            lock (windowChangeLock)
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
            }
            */
        }

        public void CloseWindow(string id)
        {
            // TODO: If layer specified, close ONLY if on the correct layer
            // OR: Just fucking ignore the layer bruh
            Window window = GetWindow(id);
            lock (windowChangeLock)
            {
                window.CurrentLayer.CloseWindow(window);
            }
        }

        public void Back()
        {
            lock (windowChangeLock)
            {
                LastActivatedWindow.CurrentLayer.Back();
            }
        }

        public void UnregisterAll()
        {
            foreach (var wnd in registeredWindows.Values)
                wnd.gameObject.SetActive(false);
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
            if (registeredWindows.TryGetValue(id, out Window result))
            {
                return result;
            }

            return null;
        }

        /*
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
        */

        public bool HasPrevNextButtons()
        {
            return (!string.IsNullOrEmpty(PreviousUIButton) && !string.IsNullOrEmpty(NextUIButton));
        }

        private Dictionary<string, Window> registeredWindows = new Dictionary<string, Window>();

        private object windowChangeLock = new object();
        private readonly List<Layer> layerStack = new List<Layer>();

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
            //Window top = null;
            /*
            // Update window showing/hiding
            lock (windowChangeLock)
            {
                // Always try to show the top window
                if (windowStack.Count > 0)
                {
                    top = windowStack[windowStack.Count - 1];
                    top.gameObject.SetActive(true);
                    //UIAnimator.Animate(top, Time.unscaledDeltaTime);
                }

                // All other windows should be hidden
                foreach (var wnd in registeredWindows.Values)
                {
                    if (wnd != top)
                    {
                        //UIAnimator.Animate(wnd, -Time.unscaledDeltaTime);
                        //if (wnd.AnimationProgress == 0)
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

                if ((currentTopWindow != null) && (currentTopWindow.OnActivated != null))
                    currentTopWindow.OnActivated.Invoke();
            }
            */
            /*
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
            */
        }

        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public const char LayerSeparator = '/';

        public static string SanitizeName(string value)
        {
            return value.ToLowerInvariant();
        }

        /// <summary>
        /// REM: names are sanitized
        /// </summary>
        /// <param name="compositeName"></param>
        /// <param name="layerName"></param>
        /// <param name="windowName"></param>
        private static void SplitWindowPath(string compositeName, out string layerName, out string windowName)
        {
            compositeName = SanitizeName(compositeName);

            int idx = compositeName.IndexOf(LayerSeparator);
            if (idx != -1)
            {
                layerName = compositeName.Substring(0, idx);
                windowName = compositeName.Substring(idx + 1);
                return;
            }

            layerName = "";
            windowName = compositeName;
        }

#if UNITY_EDITOR
        public void Editor_SetPreregistered(Window[] windows)
        {
            autoRegisteredWindows = windows;
        }
#endif

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