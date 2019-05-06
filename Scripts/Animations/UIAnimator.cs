using System.Collections;
using UnityEngine;

namespace TLP.UI
{
    public static class UIAnimator
    {
        public static void Animate(AnimatedPanel target, float delta)
        {
            // Progressing animation by zero seconds doesn't make sense
            if (delta == 0)
                throw new System.ArgumentException("delta cannot be 0!");

            // Already fully shown
            if ((target.AnimationProgress >= 1) && (delta > 0))
            {
                target.AnimationProgress = 1;
                target.CanvasGroup.alpha = 1;
                return;
            }

            // Already fully hidden
            if ((target.AnimationProgress <= 0) && (delta < 0))
            {
                target.AnimationProgress = 0;
                target.CanvasGroup.alpha = 0;
                return;
            }

            // Get transition -- default or custom
            var transition = target.Transition;

            // Modify delta and update progress. Watch out for instant transitions (zero duration or "TransitionType.None")
            float modifiedDelta = delta;
            if ((transition.Duration <= 0) || (transition.Type == UIAnimation.TransitionType.None))
                modifiedDelta = Mathf.Sign(delta);
            else
                modifiedDelta /= transition.Duration;

            target.AnimationProgress = Mathf.Clamp01(target.AnimationProgress + modifiedDelta);

            // Call appropriate frame evaluation function
            EvaluateSingleFrame(target, modifiedDelta);
        }

        public static IEnumerator AnimationRoutine(AnimatedPanel target, float speed, System.Action onFinished = null)
        {
            // No anim whatsoever, so just do a single frame
            if ((speed == 0) || (target.Transition.Duration <= 0) || (target.Transition.Type == UIAnimation.TransitionType.None))
            {
                Animate(target, Mathf.Sign(speed));
            }
            // Start an animation routine otherwise
            else
            {
                if (speed > 0)
                {
                    do
                    {
                        float delta = Time.unscaledDeltaTime / target.Transition.Duration * speed;
                        target.AnimationProgress = Mathf.Clamp01(target.AnimationProgress + delta);

                        EvaluateSingleFrame(target, delta);

                        yield return null;
                    }
                    while (target.AnimationProgress < 1);

                    EvaluateSingleFrame(target, Mathf.Sign(speed));
                }
                else
                {
                    do
                    {
                        float delta = Time.unscaledDeltaTime / target.Transition.Duration * speed;
                        target.AnimationProgress = Mathf.Clamp01(target.AnimationProgress + delta);

                        EvaluateSingleFrame(target, delta);

                        yield return null;
                    }
                    while (target.AnimationProgress > 0);

                    EvaluateSingleFrame(target, Mathf.Sign(speed));
                }
            }

            if (onFinished != null)
                onFinished();
        }

        private static void EvaluateSingleFrame(AnimatedPanel target, float previousDelta)
        {
            switch (target.Transition.Type)
            {
                case UIAnimation.TransitionType.SlideFromLeft:
                    SlideAnimFrame(target, target.Transition, new Vector2(-1, 0));
                    break;
                case UIAnimation.TransitionType.SlideFromRight:
                    SlideAnimFrame(target, target.Transition, new Vector2(1, 0));
                    break;
                case UIAnimation.TransitionType.SlideFromTop:
                    SlideAnimFrame(target, target.Transition, new Vector2(0, 1));
                    break;
                case UIAnimation.TransitionType.SlideFromBottom:
                    SlideAnimFrame(target, target.Transition, new Vector2(0, -1));
                    break;
                case UIAnimation.TransitionType.None:
                case UIAnimation.TransitionType.Fade:
                    FadeAnimFrame(target);
                    break;
                case UIAnimation.TransitionType.Popup:
                    PopupAnimFrame(target, target.Transition, previousDelta > 0);
                    break;
                case UIAnimation.TransitionType.Animator:
                    AnimatorAnimFrame(target);
                    break;
            }
        }

        private static void SlideAnimFrame(AnimatedPanel target, UIAnimation transition, Vector2 direction)
        {
            Vector2 startPos = target.AnchoredPosition + direction * transition.Strength;
            Vector2 endPos = target.AnchoredPosition;

            float f = Mathf.SmoothStep(0, 1, target.AnimationProgress);
            target.CanvasGroup.alpha = f;
            target.RectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, f);
        }
        private static void FadeAnimFrame(AnimatedPanel target)
        {
            target.CanvasGroup.alpha = Mathf.SmoothStep(0, 1, target.AnimationProgress);
        }
        private static void PopupAnimFrame(AnimatedPanel target, UIAnimation transition, bool useInCurve)
        {
            target.CanvasGroup.alpha = Mathf.SmoothStep(0, 1, target.AnimationProgress);
            if (useInCurve)
                target.RectTransform.localScale = Vector3.one * transition.PopupInCurve.Evaluate(target.AnimationProgress);
            else
                target.RectTransform.localScale = Vector3.one * transition.PopupOutCurve.Evaluate(target.AnimationProgress);
        }
        private static void AnimatorAnimFrame(AnimatedPanel target)
        {
            Animator anim = target.CanvasGroup.GetComponent<Animator>();
            if (anim == null)
            {
                FadeAnimFrame(target);
            }
            else
            {
                try
                {
                    anim.SetFloat("Visibility", target.AnimationProgress);
                }
                catch { }
            }
        }
    }
}