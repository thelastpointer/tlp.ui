using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLP.UI
{
    [AddComponentMenu("Layout/Canvas Scaler2", 101)]
    [ExecuteAlways]
    [RequireComponent(typeof(Canvas))]
    public class CanvasScaler2 : CanvasScaler
    {
        //[Range(0.1f, 5f)]
        //public float AdditionalScale = 1f;
        public ReferenceResolution Reference = ReferenceResolution.Resolution1080p;

        private static float additionalScale = 1f;
        public static float UIScaling
        {
            get { return additionalScale; }
            set { additionalScale = value; }
        }

        protected override void Handle()
        {
            float referenceWidth = 1920;

            switch (Reference)
            {
                case ReferenceResolution.Resolution4K:
                    referenceWidth = 3840;
                    break;
                case ReferenceResolution.Resolution1080p:
                    referenceWidth = 1920;
                    break;
                case ReferenceResolution.Resolution720p:
                    referenceWidth = 1280;
                    break;
            }

            this.scaleFactor = ((float)Screen.width / referenceWidth) * additionalScale;

            base.Handle();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            uiScaleMode = ScaleMode.ConstantPixelSize;
        }

        public enum ReferenceResolution
        {
            Resolution4K,       // 3840x2160
            Resolution1080p,    // 1920x1080
            Resolution720p      // 1280x720
        }

#if UNITY_EDITOR
        [MenuItem("CONTEXT/CanvasScaler/Replace With CanvasScaler2")]
        public static void ReplaceBuiltin(MenuCommand command)
        {
            var scaler = (command.context as CanvasScaler);
            if (scaler != null)
            {
                ReferenceResolution resolution = ReferenceResolution.Resolution1080p;

                if (scaler.uiScaleMode == ScaleMode.ScaleWithScreenSize)
                {
                    if (scaler.referenceResolution.x < 1600)
                        resolution = ReferenceResolution.Resolution720p;
                    else if (scaler.referenceResolution.x < 2880)
                        resolution = ReferenceResolution.Resolution1080p;
                    else
                        resolution = ReferenceResolution.Resolution4K;
                }

                var go = scaler.gameObject;

                DestroyImmediate(scaler);

                var scaler2 = go.AddComponent<CanvasScaler2>();
                scaler2.Reference = resolution;
            }
        }
#endif
    }
}

#if UNITY_EDITOR

namespace TLP.UI.Editors
{
    [CustomEditor(typeof(CanvasScaler2))]
    public class CanvasScaler2Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Reference"));
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ReferencePixelsPerUnit"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif