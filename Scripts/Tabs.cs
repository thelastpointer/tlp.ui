using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLP.UI
{
    public class Tabs : MonoBehaviour
    {
#pragma warning disable 0649

        [Header("Tabs & Toggles")]
        [SerializeField] private AnimatedPanel[] tabPages;
        [SerializeField] private Toggle[] toggles;

        [Header("Transition")]
        [SerializeField] private bool useDefaultTransition = true;
        [SerializeField] private UIAnimation customTransition;

        [Header("Callbacks")]
        public TabSelectedEvent OnTabSelected;

        [System.Serializable]
        public class TabSelectedEvent : UnityEngine.Events.UnityEvent<int> { }

#pragma warning restore

        private int currentIdx = 0;
        private object idxLock = new object();

        public void ShowTab(int idx)
        {
            int oldIdx = currentIdx;

            lock (idxLock)
            {
                if ((idx < 0) || (idx >= tabPages.Length))
                    throw new System.ArgumentException("idx");

                if (currentIdx == idx)
                    return;

                currentIdx = idx;
            }

            if (oldIdx != currentIdx)
            {
                if (OnTabSelected != null)
                    OnTabSelected.Invoke(currentIdx);
            }
        }

        public void ShowNext()
        {
            lock (idxLock)
            {
                int newIdx = currentIdx + 1;
                if (newIdx >= tabPages.Length)
                    newIdx = 0;

                ShowTab(newIdx);
            }
        }

        public void ShowPrevious()
        {
            lock (idxLock)
            {
                int newIdx = currentIdx - 1;
                if (newIdx < 0)
                    newIdx = tabPages.Length - 1;

                ShowTab(newIdx);
            }
        }

        private void Awake()
        {
            // Check if toggles have a group
            ValidateToggleGroup();

            // Show an error if pages & toggles are mismatched
            int count = Mathf.Min(toggles.Length, tabPages.Length);
            if ((toggles.Length > 0) && (toggles.Length != tabPages.Length))
                Debug.LogWarning("Warning: tab page count and toggle count mismatch");

            // Toggle events
            for (int i = 0; i < count; i++)
            {
                int closure = i;
                toggles[i].onValueChanged.AddListener((value) => { if (value) ShowTab(closure); });
            }

            // Show the first tab, no anim
            for (int i = 1; i < tabPages.Length; i++)
                UIAnimator.Animate(tabPages[i], -1);
            UIAnimator.Animate(tabPages[0], 1);
            
            if (OnTabSelected != null)
                OnTabSelected.Invoke(0);
        }

        private void Update()
        {
            // Update tabs
            lock (idxLock)
            {
                for (int i=0; i<tabPages.Length; i++)
                {
                    if (i == currentIdx)
                        UIAnimator.Animate(tabPages[i], Time.unscaledDeltaTime);
                    else
                        UIAnimator.Animate(tabPages[i], -Time.unscaledDeltaTime);
                }
            }

            // Prev/next
            if (WindowManager.Instance.HasPrevNextButtons())
            {
                if (Input.GetButtonDown(WindowManager.Instance.PreviousUIButton))
                    ShowPrevious();
                if (Input.GetButtonDown(WindowManager.Instance.NextUIButton))
                    ShowNext();
            }
        }

        private void ValidateToggleGroup()
        {
            // Check if we already have a toggle group?
            ToggleGroup tgroup = null;
            foreach (var toggle in toggles)
            {
                if (toggle.group != null)
                {
                    tgroup = toggle.group;
                    break;
                }
            }

            // If none, create one
            if (tgroup == null)
                tgroup = gameObject.AddComponent<ToggleGroup>();

            // Assign toggle groups again (note: some toggles might not had the toggle group assigned)
            foreach (var toggle in toggles)
            {
                toggle.group = tgroup;
                toggle.isOn = false;
            }

            tgroup.SetAllTogglesOff();
            toggles[0].isOn = true;

            tgroup.allowSwitchOff = false;
        }
    }
}

#if UNITY_EDITOR

namespace TLP.UI.Editors
{
    [CustomEditor(typeof(Tabs))]
    public class TabsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var tabPageProperty = serializedObject.FindProperty("tabPages");
            var togglesProperty = serializedObject.FindProperty("toggles");

            if ((togglesProperty.arraySize > 0) && (togglesProperty.arraySize != tabPageProperty.arraySize))
                EditorGUILayout.HelpBox("Warning: tab page count and toggle count mismatch!", MessageType.Warning);
        }
    }
}

#endif
