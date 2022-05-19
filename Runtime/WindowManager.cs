using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLP.UI
{
    /// <summary>
    /// Manages UI Windows.
    /// </summary>
    public class WindowManager : MonoBehaviour
    {
        // This is a singleton
        public static WindowManager Instance { get; private set; }

        #region Editor-assigned fields

#pragma warning disable 0649

        [Header("Auto-registered windows")]
        [SerializeField] private Window[] autoRegisteredWindows;

        [Header("Layers")]
        [SerializeField] private bool createMissingLayers = false;
        [SerializeField] private string defaultLayer = "";
        [SerializeField] private string[] autoCreatedLayers;
        
        [Header("Controller Settings")]
        public bool UseController = false;

        [Header("Callbacks")]
        public UnityEvent OnReady;
        public UnityEvent OnTopWindowChanged;

#pragma warning restore 0649

        #endregion

        #region Windows

        // Get the Window that was activated last
        public Window LastShownWindow { get; private set; }

        // Top layer, top window
        public Window TopWindow { get { throw new System.NotImplementedException(); } }

        public Window[] GetActiveWindows() { throw new System.NotImplementedException(); }

        /// <summary>
        /// Schedule the window for display.
        /// </summary>
        /// <param name="id">The window must be registered with this ID. Case-sensitive.</param>
        public void ShowWindow(string id)
        {
            string explicitLayerName;
            string windowName;
            SplitWindowPath(id, out explicitLayerName, out windowName);

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
                LastShownWindow = window;
            }
        }

        public void CloseWindow(string id)
        {
            id = SanitizeID(id);

            // TODO: If layer specified, close ONLY if on the correct layer
            // OR: Just fucking ignore the layer bruh
            Window window = GetWindow(id);
            lock (windowChangeLock)
            {
                window.CurrentLayer.CloseWindow(window);
            }
        }

        public void ShowWindow(Window window)
        {
            throw new System.NotImplementedException();
        }
        public void CloseWindow(Window window)
        {
            throw new System.NotImplementedException();
        }

        public Window GetWindow(string id)
        {
            id = SanitizeID(id);

            if (registeredWindows.TryGetValue(id, out Window result))
            {
                return result;
            }

            return null;
        }

        // TODO: Is this even needed?
        public void UnregisterAllWindows()
        {
            foreach (var wnd in registeredWindows.Values)
                wnd.gameObject.SetActive(false);
        }

        public void RegisterWindow(Window wnd, string id)
        {
            id = SanitizeID(id);

            // Check for unintended overwrites
            if (registeredWindows.ContainsKey(id) && (registeredWindows[id] != wnd))
                Debug.LogError("Warning: window \"" + id + "\" overridden! This is probably unintended behaviour.\nPrevious: " + registeredWindows[id].name + "\nNew: " + wnd.name, registeredWindows[id].gameObject);
            registeredWindows[id] = wnd;
        }

        #endregion

        #region Layers

        public const char LayerSeparator = '/';
        public Layer DefaultLayer { get; private set; }

        public Layer GetLayer(string id)
        {
            if (layers.TryGetValue(SanitizeID(id), out Layer result))
                return result;

            return null;

            // TODO: Why would this throw
            //throw new System.ArgumentException("No layer registered as " + id);
        }

        public bool LayerExists(string id)
        {
            return layers.ContainsKey(SanitizeID(id));
        }

        // TODO: Not sure about this one though...
        public IEnumerable<Layer> GetLayers()
        {
            return layers.Values;
        }

        #endregion

        private readonly Dictionary<string, Window> registeredWindows = new Dictionary<string, Window>();
        private readonly Dictionary<string, Layer> layers = new Dictionary<string, Layer>();

        private readonly object windowChangeLock = new object();

        private GameObject selectedUIObject;

        private void Setup()
        {
            lock (windowChangeLock)
            {
                // Find child layers and register them
                foreach (var layer in gameObject.GetComponentsInChildren<Layer>())
                {
                    AddExistingLayer(layer);
                }

                // Create layers from preCreatedLayers
                foreach (var layer in autoCreatedLayers)
                {
                    CreateLayer(layer);
                }

                // Create or assign default layer
                if (!string.IsNullOrEmpty(defaultLayer))
                {
                    DefaultLayer = GetLayer(defaultLayer);
                    if (DefaultLayer == null)
                        DefaultLayer = CreateLayer(defaultLayer);
                }

                foreach (var wnd in autoRegisteredWindows)
                {
                    RegisterWindow(wnd, wnd.ID);

                    // Add to correct layer
                    AssignWindowDefaultLayer(wnd);

                    // Hide by default
                    wnd.Close();
                }
            }
        }

        private Layer AssignWindowLayer(Window window, string explicitLayerName)
        {
            Layer newLayer = null;

            if (string.IsNullOrEmpty(explicitLayerName))
            {
                // Fast quit if no assignment needed
                if (window.CurrentLayer != null)
                    return window.CurrentLayer;

                // No previous layer, no specified layer -- move to default layer
                newLayer = DefaultLayer;

                // Default layer is null, throw
                if (newLayer == null)
                    throw new System.InvalidOperationException("Unable to resolve default layer for window \"" + window.ID + "\"");
            }
            else
            {
                // Try to find new layer
                newLayer = GetLayer(explicitLayerName);

                if (newLayer == null)
                {
                    // Create missing layer
                    if (createMissingLayers)
                    {
                        newLayer = CreateLayer(explicitLayerName);
                    }
                    // No such layer and no permission to create it, throw
                    else
                        throw new System.InvalidOperationException("Trying to assign window \"" + window.ID + "\" to missing layer \"" + explicitLayerName + "\"");
                }
            }

            MoveWindowToLayer(window, newLayer);

            return newLayer;
        }

        private Layer AssignWindowDefaultLayer(Window window)
        {
            // Resolve layer
            //      return current if already resolved
            //      if none, find preferred
            //      if no preferred, use default
            
            if (window.CurrentLayer != null)
            {
                return window.CurrentLayer;
            }

            Layer layer = null;

            if (!string.IsNullOrEmpty(window.PreferredLayer))
            {
                layer = GetLayer(window.PreferredLayer);
                if (layer == null)
                {
                    if (createMissingLayers)
                    {
                        layer = CreateLayer(window.PreferredLayer);
                    }
                    else
                    {
                        layer = DefaultLayer;
                    }
                }
            }

            if (layer == null)
            {
                layer = DefaultLayer;
            }

            if (layer == null)
            {
                throw new System.InvalidOperationException("Unable to resolve default layer for window \"" + window.ID + "\"");
            }

            MoveWindowToLayer(window, layer);

            return layer;
        }

        private void MoveWindowToLayer(Window window, Layer newLayer)
        {
            if (window == null)
                throw new System.ArgumentNullException("window");

            if (newLayer == null)
                throw new System.ArgumentNullException("newLayer");

            // Add to layer for the first time
            if (window.CurrentLayer == null)
            {
                newLayer.AddWindow(window);
                window.CurrentLayer = newLayer;
            }
            // Move between layers
            else if (newLayer != window.CurrentLayer)
            {
                window.CurrentLayer.RemoveWindow(window);
                newLayer.AddWindow(window);
                window.CurrentLayer = newLayer;
            }
        }

        #region Unity messages

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                DontDestroyOnLoad(gameObject);

                Setup();

                OnReady?.Invoke();
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
                // NOTE: Controller support means that a window gets its first control selected by default
                // Enable controller support if there's a joystick connected
                UseController = (Input.GetJoystickNames().Length > 0);
            }
        }

        private void Reset()
        {
            //audioSource = GetComponent<AudioSource>();
        }

        #endregion

        private void AddExistingLayer(Layer layer)
        {
            string id = SanitizeID(layer.ID);

            if (layers.TryGetValue(id, out Layer existingLayer))
            {
                if (layer != existingLayer)
                {
                    throw new System.InvalidOperationException("Trying to register two different layers with the same id: \"" + id + "\"");
                }
            }
            else
            {
                // TODO: Move to correct order?
                layers[id] = layer;
            }
        }

        private Layer CreateLayer(string name)
        {
            name = SanitizeID(name);

            // If layer already exists, return it
            if (layers.TryGetValue(name, out Layer result))
                return result;

            GameObject go = new GameObject("Layer-" + name);
            go.transform.SetParent(transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;

            var canvas = go.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 0;

            var raycaster = go.AddComponent<GraphicRaycaster>();

            var canvasGroup = go.AddComponent<CanvasGroup>();

            var layer = go.AddComponent<Layer>();
            layer.ID = name;

            AddExistingLayer(layer);

            return layer;
        }

        

        // TODO: Maybe fuck this? Just inserting possible extra GC for almost every use case?
        public static string SanitizeID(string value)
        {
            //return value.ToLowerInvariant();
            return value;
        }

        /// <summary>
        /// REM: names are sanitized
        /// </summary>
        /// <param name="compositeName"></param>
        /// <param name="layerName"></param>
        /// <param name="windowName"></param>
        private static void SplitWindowPath(string compositeName, out string layerName, out string windowName)
        {
            compositeName = SanitizeID(compositeName);

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

            if (GUILayout.Button("Register All Windows"))
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