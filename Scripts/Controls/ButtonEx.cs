using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TLP.UI.Controls
{
    public class ButtonEx : Button
    {
        public UISound SfxClick = new UISound(UISoundType.Click);
        public UISound SfxHover = new UISound(UISoundType.Hover);

        public bool DeselectAfterClick = true;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            // TODO: Hightlighted or Selected??
            if (state == SelectionState.Highlighted)
                UISounds.Play(SfxHover);

            //state == SelectionState.
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            UISounds.Play(SfxClick);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (DeselectAfterClick && (EventSystem.current.currentSelectedGameObject == gameObject))
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}