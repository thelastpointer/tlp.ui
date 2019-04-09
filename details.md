# TLP.UI.Details

Okay, so here's what you need to know.

## Basics

The WindowManager class handles all windows. It is a singleton -- you can access it with the static `WindowManager.Instance` property. Only the first instance is valid, if you have any more, they will destroy themselves.

All windows have a Window component. Windows are identified by their `windowID`, which is a string.

Windows need to be registered. They register themselves at startup (in `Start()`) automatically, but of course this doesn't happen if the window is deactivated by default (as it usually is), so you can drag it into the WindowManager's "Auto-registered Windows" list to make sure they are registered correctly.

You can show a window by calling `WindowManager.Instance.ShowWindow(id)`. This will hide the current window and show the new one. You can go back by calling `WindowManager.Instance.Back()` -- this will hide the current window and show the previous one, if any.

ShowWindow and Hide are defined in the Window class too, so for your button event handlers you can just refer to the containing Window. This is handy if you have Windows in multiple scenes.

## Tabs
Tabs show and hide tab pages, which are GameObjects with CanvasGroups (again for fading). To show a tab, call `ShowTab(idx)`. It supports Toggles to show tabs; assign a Toggle for each tab page to do that. Tabs will do the rest, including attaching a ToggleGroup and assigning event handlers.

By default, tabs use the WindowManager's transition settings, but you can override them.

## Message boxes
The MessageBoxes (which is, again, a singleton) shows and hides GameObjects, sets Text components and have callbacks when finished.

They should ideally be on a separate Canvas, which has a higher Sort Order than the rest of the UI.

These are intented to be called from code, so they will call a callback function once the user dismisses them. The messagebox functions are:
* `ShowMessage` -- displays a message.
* `ShowError` -- displays a message, _but is red_
* `PromptYesNo` -- the user can choose between two buttons to proceed.
* `PromptText` -- the user can enter a text here.
* `ProgressModal` -- shows a text, sets the FillAmount of an Image (if assigned) and sets the value of a Slider (if assigned). Call this repeatedly to fill the progress bar.
* `HideProgressModal` -- Hides the progress window. Don't forget to call this when finished!
* `ShowProgressOverlay` -- displays a text, a Filled Image, and a Slider as an overlay. The user can use the rest of the UI normally. call this repeatedly.
* `HideProgressOverlay` -- hides the progress overlay. Don't forget to call this when finished!

There's also a `WaitForModals()` function which calls a function as soon as all message boxes are hidden.

## Transitions
Transition define what happens when you change Windows or Tabs. All transition animations are hardcoded. The static WindowAnimator class implements these. The transitions fade a CanvasGroup and scale a RectTransform.

The transitions are:
* None -- change instantly, transition duration is ignored.
* SlideFrom* -- fades the Window and slides it to it's position (slide amount is defined by TransitionStrength).
* Fade -- fades the window.
* Popup -- uses the PopupInCurve and PopupOutCurve to scale the Window. Fades too.
* Animator -- if the Window has an Animator component, sets the "WindowShow" or "WindowHide" triggers.

## Sounds
Call `WindowManager.PlaySound()` to play a sound. This way you can consistently re-use sound effects for your UI.

If you assign an AudioSource and some AudioClips, the WindowManager will automatically play sounds for showing and hiding Windows, and when the selection changes. (The last one might be a bit tricky because of Unity's selection handling, but it might work out for you.)

## Modal windows
If you want a Window to be modal (cover the rest of the UI), assign a full-screen, click-blocking transform to its BackgroundDenier field. When a Window is shown and it has a BackgroundDenier, the denier is placed right above the Window in the hierarchy and activates it too. It is of course hidden when the Window is closed.

## Controller support
The WindowManager has _some_ controller support. If you check `UseController`, it will auto-select the first control in a Window that's displayed. That's it, sorry.

Controller detection is simple: if there is at least one "joystick" connected, I assume you have a controller.

There is also two user-defined buttons: Next UI Button and Previous UI Button. If these are set (and correctly set up in the Input manager), then pressing them will switch between tabs and call a Window's `OnNextWindow` and `OnPreviousWindow` events. Note: if you have both tabs and these events, both will be called. So for example, you could set up the "UINext" and "UIPrev" buttons in the Input manager, assign them a joystick button, enter these in the WindowManager and you're ready to go!