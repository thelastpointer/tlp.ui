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
        [SerializeField] private CanvasGroup[] tabPages;
        [SerializeField] private Toggle[] toggles;

        [Header("Transition")]
        [SerializeField] private bool useDefaultTransition = true;
        [SerializeField] private WindowTransition customTransition;

        [Header("Callbacks")]
        public UnityEngine.Events.UnityEvent<int> OnTabSelected;

#pragma warning disable 0649

        private int currentIdx = 0;
        private Coroutine transitionRoutine = null;

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
            tabPages[0].gameObject.SetActive(true);
            for (int i=1; i<tabPages.Length; i++)
                tabPages[i].gameObject.SetActive(false);

            if (OnTabSelected != null)
                OnTabSelected.Invoke(0);
        }

        private void Update()
        {
            if (WindowManager.Instance.HasPrevNextButtons())
            {
                if (Input.GetButtonDown(WindowManager.Instance.PreviousUIButton))
                    ShowPrevious();
                if (Input.GetButtonDown(WindowManager.Instance.NextUIButton))
                    ShowNext();
            }
        }

        public void ShowTab(int idx)
        {
            if ((idx < 0) || (idx >= tabPages.Length))
                throw new System.ArgumentException("idx");

            if (transitionRoutine != null)
                return;

            if (currentIdx == idx)
                return;

            currentIdx = idx;

            List<CanvasGroup> hideTabs = new List<CanvasGroup>();
            for (int i = 0; i < currentIdx; i++)
            {
                if (tabPages[i].gameObject.activeSelf)
                    hideTabs.Add(tabPages[i]);
            }
            for (int i = currentIdx + 1; i < tabPages.Length; i++)
            {
                if (tabPages[i].gameObject.activeSelf)
                    hideTabs.Add(tabPages[i]);
            }

            if ((toggles != null) && (toggles.Length > currentIdx))
                toggles[currentIdx].isOn = true;

            if ((hideTabs.Count == 0) && tabPages[currentIdx].gameObject.activeSelf)
                return;

            transitionRoutine = StartCoroutine(Transition(hideTabs, tabPages[currentIdx]));

            if (OnTabSelected != null)
                OnTabSelected.Invoke(currentIdx);
        }

        public void ShowNext()
        {
            int newIdx = currentIdx + 1;
            if (newIdx >= tabPages.Length)
                newIdx = 0;

            ShowTab(newIdx);
        }

        public void ShowPrevious()
        {
            int newIdx = currentIdx - 1;
            if (newIdx < 0)
                newIdx = tabPages.Length - 1;

            ShowTab(newIdx);
        }
        
        private IEnumerator Transition(List<CanvasGroup> hideTabs, CanvasGroup showTab)
        {
            Coroutine lastHideRoutine = null;
            if (hideTabs.Count > 0)
            {
                foreach (var group in hideTabs)
                {
                    lastHideRoutine = StartCoroutine(WindowAnimator.Animate(
                        group,
                        group.GetComponent<RectTransform>(),
                        (useDefaultTransition ? WindowManager.Instance.DefaultTransition : customTransition),
                        true
                    ));
                }

                yield return lastHideRoutine;

                foreach (var group in hideTabs)
                {
                    group.gameObject.SetActive(false);
                }
            }

            if (!showTab.gameObject.activeSelf)
            {
                showTab.gameObject.SetActive(true);

                yield return StartCoroutine(WindowAnimator.Animate(
                    showTab,
                    showTab.GetComponent<RectTransform>(),
                    (useDefaultTransition ? WindowManager.Instance.DefaultTransition : customTransition),
                    false
                ));
            }

            transitionRoutine = null;
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
