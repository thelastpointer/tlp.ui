# TLP.UI

## What
A window and UI manager library, designed to be very simple while solving common UI-related tasks in game development.

It's main features:
* Very simple -- a few scripts, basic features only; comes with readily usable prefabs.
* No dependencies.
* A simple window stack -- only one window can be active at a time.
* Tabs.
* Thread-safe -- no "can only be called from the main thread" headaches.

Additionally:
* Window animations -- nothing too fancy, just enough to make it look good.
* Sounds -- optionally _whoosh_ and _boink_ and _clack_ and _bzzzt_
* Options for controller navigation.
* Button that makes a click sound (wow) and is deselected after clicked (_wow_).

Goodies:
* Text beautifier
* Keyboard shortcut helper
* Drag & drop
* Scaleable UI
* Tooltips

## Why

## How

## Who
Website: [https://thelastpointer.github.io/](https://thelastpointer.github.io/)

Tell me if you like it or hate it, if you have bugfixes or ideas. Please keep in mind that I'd like to keep this as simple as possible.

## License
MIT -- if you extend it and release the source, credit this project so we can help each other. I won't come after you if you don't.

## TODO
* Some kind of promt implementation -- call a function, it displays a message and returns with the user choice.
* Controller support:
  * Auto-select first control
  * Window next/prev controls somehow (custom Input button?)
  * Tabs next/prev controls
  * Hide/show image for controller enabled/disabled