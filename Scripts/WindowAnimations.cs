using System.Collections;
using UnityEngine;

namespace TLP.UI
{
    public static class WindowAnimator
    {
        public static IEnumerator Animate(CanvasGroup group, RectTransform rt, WindowTransition transition, bool invert)
        {
            switch (transition.Type)
            {
                case WindowTransition.TransitionType.None:
                    yield return InstantRoutine(group, invert);
                    break;
                case WindowTransition.TransitionType.SlideFromLeft:
                    yield return SlideFromLeftRoutine(group, rt, transition.Duration, transition.Strength, invert);
                    break;
                case WindowTransition.TransitionType.SlideFromRight:
                    yield return SlideFromRightRoutine(group, rt, transition.Duration, transition.Strength, invert);
                    break;
                case WindowTransition.TransitionType.SlideFromTop:
                    yield return SlideFromTopRoutine(group, rt, transition.Duration, transition.Strength, invert);
                    break;
                case WindowTransition.TransitionType.SlideFromBottom:
                    yield return SlideFromBottomRoutine(group, rt, transition.Duration, transition.Strength, invert);
                    break;
                case WindowTransition.TransitionType.Fade:
                    yield return FadeInRoutine(group, transition.Duration, invert);
                    break;
                case WindowTransition.TransitionType.Popup:
                    yield return PopupRoutine(group, rt, transition.Duration, (invert ? transition.PopupOutCurve : transition.PopupInCurve), invert);
                    break;
                case WindowTransition.TransitionType.Animator:
                    yield return AnimatorRoutine(group, transition.Duration, invert);
                    break;
                default:
                    break;
            }
        }

        public static IEnumerator Animate(Window wnd, bool invert)
        {
            yield return Animate(wnd.GetComponent<CanvasGroup>(), wnd.GetComponent<RectTransform>(), wnd.Transition, invert);
        }

        public static IEnumerator Animate(GameObject go, WindowTransition transition, bool invert)
        {
            CanvasGroup group = go.GetComponent<CanvasGroup>();
            RectTransform rt = go.GetComponent<RectTransform>();

            if ((group == null) || (rt == null))
            {
                Debug.LogError("WindowAnimations.Animate requires a CanvasGroup and a RectTransform", go);
                throw new System.InvalidOperationException("Required components missing");
            }

            yield return Animate(group, rt, transition, invert);
        }

        public static IEnumerator Animate(GameObject go, WindowTransition transition, bool invert, System.Action onFinished)
        {
            CanvasGroup group = go.GetComponent<CanvasGroup>();
            RectTransform rt = go.GetComponent<RectTransform>();

            if ((group == null) || (rt == null))
            {
                Debug.LogError("WindowAnimations.Animate requires a CanvasGroup and a RectTransform", go);
                throw new System.InvalidOperationException("Required components missing");
            }

            yield return Animate(group, rt, transition, invert);

            if (onFinished != null)
                onFinished();
        }

        public static IEnumerator SlideFromLeftRoutine(CanvasGroup group, RectTransform rt, float duration, float strength, bool invert)
        {
            yield return SlideRoutineGeneric(group, rt, duration, strength, new Vector2(-1, 0), invert);
        }
        public static IEnumerator SlideFromRightRoutine(CanvasGroup group, RectTransform rt, float duration, float strength, bool invert)
        {
            yield return SlideRoutineGeneric(group, rt, duration, strength, new Vector2(1, 0), invert);
        }
        public static IEnumerator SlideFromTopRoutine(CanvasGroup group, RectTransform rt, float duration, float strength, bool invert)
        {
            yield return SlideRoutineGeneric(group, rt, duration, strength, new Vector2(0, 1), invert);
        }
        public static IEnumerator SlideFromBottomRoutine(CanvasGroup group, RectTransform rt, float duration, float strength, bool invert)
        {
            yield return SlideRoutineGeneric(group, rt, duration, strength, new Vector2(0, -1), invert);
        }

        private static IEnumerator SlideRoutineGeneric(CanvasGroup group, RectTransform rt, float duration, float strength, Vector2 startDirection, bool invert)
        {
            float startAlpha = 0;
            Vector2 startPos = rt.anchoredPosition + startDirection * strength;

            float endAlpha = 1;
            Vector2 endPos = rt.anchoredPosition;

            float start = Time.unscaledTime;
            while ((Time.unscaledTime - start) < duration)
            {
                float f = Mathf.SmoothStep(invert ? 1 : 0, invert ? 0 : 1, (Time.unscaledTime - start) / duration);

                group.alpha = Mathf.Lerp(startAlpha, endAlpha, f);
                rt.anchoredPosition = Vector2.Lerp(startPos, endPos, f);

                yield return null;
            }

            float endF = (invert ? 0 : 1);
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, endF);
            rt.anchoredPosition = endPos;
        }

        public static IEnumerator FadeInRoutine(CanvasGroup group, float duration, bool invert)
        {
            float startAlpha = 0;
            float endAlpha = 1;

            float start = Time.unscaledTime;
            while ((Time.unscaledTime - start) < duration)
            {
                float f = Mathf.SmoothStep(invert ? 1 : 0, invert ? 0 : 1, (Time.unscaledTime - start) / duration);
                group.alpha = Mathf.Lerp(startAlpha, endAlpha, f);

                yield return null;
            }

            float endF = (invert ? 0 : 1);
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, endF);
        }

        public static IEnumerator PopupRoutine(CanvasGroup group, RectTransform rt, float duration, AnimationCurve curve, bool invert)
        {
            Vector2 baseScale = rt.localScale;
            float startAlpha = 0;
            float endAlpha = 1;

            float start = Time.unscaledTime;
            while ((Time.unscaledTime - start) < duration)
            {
                float f = Mathf.SmoothStep(invert ? 1 : 0, invert ? 0 : 1, (Time.unscaledTime - start) / duration);
                group.alpha = Mathf.Lerp(startAlpha, endAlpha, f);
                rt.localScale = baseScale * curve.Evaluate(f);

                yield return null;
            }

            float endF = (invert ? 0 : 1);
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, endF);
            rt.localScale = baseScale;
        }

        public static IEnumerator InstantRoutine(CanvasGroup group, bool invert)
        {
            group.alpha = (invert ? 0 : 1);
            yield return null;
        }

        private static IEnumerator AnimatorRoutine(CanvasGroup group, float duration, bool invert)
        {
            Animator anim = group.GetComponent<Animator>();
            if (anim == null)
            {
                yield return InstantRoutine(group, invert);
            }
            else
            {
                anim.SetTrigger(invert ? "WindowHide" : "WindowShow");
                yield return new WaitForSeconds(duration);
            }
        }
    }
}