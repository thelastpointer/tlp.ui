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
        [Header("Tabs & Toggles")]
        [SerializeField]
        private CanvasGroup[] tabPages;

        [SerializeField]
        private Toggle[] toggles;

        [Header("Transition")]
        [SerializeField]
        private WindowAnimationType transition = WindowAnimationType.FadeIn;

        [SerializeField]
        private bool useWindowManagerTransitionSetup = true;

        [SerializeField]
        private float transitionTime = 0.15f;

        [SerializeField]
        private float transitionStrength = 100;

        [SerializeField]
        private AnimationCurve popupInCurve = new AnimationCurve(new Keyframe[] {
            new Keyframe { time=0, value=0.75f },
            new Keyframe { time=0.33f, value=1, inTangent=0.72f, outTangent = 0.72f },
            new Keyframe { time=1, value=1, inTangent=0.78f, outTangent=0.78f }
        });

        [SerializeField]
        private AnimationCurve popupOutCurve = new AnimationCurve(new Keyframe[] {
            new Keyframe { time=0, value=0.75f },
            new Keyframe { time=1, value=1, inTangent=0.81f, outTangent=0.81f }
        });

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
        }

        public void ShowTab(int idx)
        {
            if (transitionRoutine != null)
                return;

            List<CanvasGroup> hideTabs = new List<CanvasGroup>();
            for (int i = 0; i < idx; i++)
            {
                if (tabPages[i].gameObject.activeSelf)
                    hideTabs.Add(tabPages[i]);
            }
            for (int i = idx+1; i < tabPages.Length; i++)
            {
                if (tabPages[i].gameObject.activeSelf)
                    hideTabs.Add(tabPages[i]);
            }

            if ((hideTabs.Count == 0) && tabPages[idx].gameObject.activeSelf)
                return;

            transitionRoutine = StartCoroutine(Transition(hideTabs, tabPages[idx]));
        }
        
        private IEnumerator Transition(List<CanvasGroup> hideTabs, CanvasGroup showTab)
        {
            Coroutine lastHideRoutine = null;
            if (hideTabs.Count > 0)
            {
                foreach (var group in hideTabs)
                {
                    lastHideRoutine = StartCoroutine(WindowAnimator.Animate(
                        transition,
                        group,
                        group.GetComponent<RectTransform>(),
                        (useWindowManagerTransitionSetup ? WindowManager.Instance.TransitionTime : transitionTime),
                        (useWindowManagerTransitionSetup ? WindowManager.Instance.TransitionStrength : transitionStrength),
                        true,
                        (useWindowManagerTransitionSetup ? WindowManager.Instance.PopupInCurve : popupInCurve),
                        (useWindowManagerTransitionSetup ? WindowManager.Instance.PopupOutCurve : popupOutCurve))
                    );
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
                    transition,
                    showTab,
                    showTab.GetComponent<RectTransform>(),
                    (useWindowManagerTransitionSetup ? WindowManager.Instance.TransitionTime : transitionTime),
                    (useWindowManagerTransitionSetup ? WindowManager.Instance.TransitionStrength : transitionStrength),
                    false,
                    (useWindowManagerTransitionSetup ? WindowManager.Instance.PopupInCurve : popupInCurve),
                    (useWindowManagerTransitionSetup ? WindowManager.Instance.PopupOutCurve : popupOutCurve))
                );
            }

            transitionRoutine = null;
        }

        private void ValidateToggleGroup()
        {
            ToggleGroup tgroup = null;
            foreach (var toggle in toggles)
            {
                if (toggle.group != null)
                {
                    tgroup = toggle.group;
                    break;
                }
            }

            if (tgroup == null)
                tgroup = gameObject.AddComponent<ToggleGroup>();
            
            foreach (var toggle in toggles)
                toggle.group = tgroup;

            tgroup.allowSwitchOff = false;
            tgroup.SetAllTogglesOff();
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
            var tabPageProperty = serializedObject.FindProperty("tabPages");
            var togglesProperty = serializedObject.FindProperty("toggles");

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(tabPageProperty, true);
            EditorGUILayout.PropertyField(togglesProperty, true);

            if (EditorGUI.EndChangeCheck())
                this.Repaint();
            
            //if ((togglesProperty.arraySize > 0) && (togglesProperty.arraySize != tabPageProperty.arraySize))
            //    EditorGUILayout.HelpBox("Warning: tab page count and toggle count mismatch!", MessageType.Warning);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("transition"));

            EditorGUI.BeginChangeCheck();

            var wmsetupProperty = serializedObject.FindProperty("useWindowManagerTransitionSetup");
            EditorGUILayout.PropertyField(wmsetupProperty);

            if (EditorGUI.EndChangeCheck())
                this.Repaint();

            if (!wmsetupProperty.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("transitionTime"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("transitionStrength"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("popupInCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("popupOutCurve"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif