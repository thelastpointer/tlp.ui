using UnityEngine;

namespace TLP.UI
{
    [System.Serializable]
    public class UIAnimation
    {
        // Fade + Move + Scale
        //      use DOTween?
        //      custom animation?

        /*
        plan:
            this should be a single coroutine that runs on the WindowManager
            a window can only have a single animation running at the same time
            callbacks?
                SHOW() -> Callback() -> Anim()
                HIDE() -> Anim() -> Callback()
            eh this class doesn't concern itself with this. have an onStart() and onEnd()
        */

        /*
            animations to be saved as scriptableobjects for easier sharing?
        */

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