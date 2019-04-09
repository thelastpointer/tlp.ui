using UnityEngine;

namespace TLP.UI
{
    [System.Serializable]
    public class WindowTransition
    {
        public TransitionType Type = TransitionType.Fade;
        public float Duration = 0.1f;
        public float Strength = 200f;
        public AnimationCurve PopupInCurve = new AnimationCurve(new Keyframe[] {
            new Keyframe { time=0, value=0.75f },
            new Keyframe { time=0.33f, value=1, inTangent=0.72f, outTangent = 0.72f },
            new Keyframe { time=1, value=1, inTangent=0.78f, outTangent=0.78f }
        });
        public AnimationCurve PopupOutCurve = new AnimationCurve(new Keyframe[] {
            new Keyframe { time=0, value=0.75f },
            new Keyframe { time=1, value=1, inTangent=0.81f, outTangent=0.81f }
        });

        public enum TransitionType
        {
            None,
            SlideFromLeft,
            SlideFromRight,
            SlideFromTop,
            SlideFromBottom,
            Fade,
            Popup,
            Animator
        }
    }
}