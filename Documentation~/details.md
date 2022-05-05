# TLP.UI

Okay, so here's what you need to know. This thing is not that complex, so you can jump right to [Usage](#usage) and hopefully you'll be able to follow along easily. There are, however, some things that you'd better know about, so read all of this if possible.

# Overview

The WindowManager class handles all windows. It is a singleton -- you can access it with the static `WindowManager.Instance` property. Only the first instance is valid, if you have any more, they will destroy themselves.

The ```WindowManager``` contains ```Layers```, ```Layers``` contain ```Windows```. Essentially, WindowManager manages everything, and Windows are grouped into Layers.

All windows have a Window component. Windows are identified by their `ID`, which is a string. The same is true for ```Layers```.

Windows need to be registered. They register themselves at startup (in `Start()`) automatically, but of course this doesn't happen if the window is deactivated by default (as it usually is), so the WindowManager can gather all Windows in a scene _in the editor_. To do this, select the WindowManager and click the "Register All Windows" button in the inspector.

You can show a window by calling ```WindowManager.Instance.ShowWindow(id)```. 
TODO: Fully qualify IDs like "layer/window"

ShowWindow and Hide are defined in the Window class too, so for your button event handlers you can just refer to the containing Window. This is handy if you have Windows in multiple scenes and you don't have access to the WindowManager.

Showing and hiding windows is a thread-safe operation.

## Layers

creation
in editor
windowmanager.autocreate

## Windows

creation
show/hide

# Usage

## Tiling vs Stacked

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
The WindowManager has _some_ controller support. If you check `UseController`, it will auto-select the first control in a Window that's displayed.

Controller detection is simple: if there is at least one "joystick" connected, I assume you have a controller.

There is also two user-defined buttons: Next UI Button and Previous UI Button. If these are set (and correctly set up in the Input manager), then pressing them will switch between tabs and call a Window's `OnNextWindow` and `OnPreviousWindow` events. Note: if you have both tabs and these events, both will be called.

So for example, you could set up the "UINext" and "UIPrev" buttons in the Input manager, assign them a joystick button, enter these in the WindowManager and you're ready to go!