# TLP.UI

## What
This is a very simple window manager for your Unity3d project. Use it if you need a low-complexity GUI.

It's main features:
* Very simple -- a few scripts, basic features only; comes with readily usable prefabs.
* No dependencies.
* A simple window stack -- only one window can be active at a time.
* Tabs.
* The most neccessary modal dialogs:
	* Simple message
	* Error message
	* Yes/no dialog
	* Text prompt
	* Progress dialogs

Additionally:
* Window animations -- nothing too fancy, just enough to make it look good.
* Sounds -- optionally _whoosh_ and _boink_ and _clack_ and _bzzzt_
* Options to make controller navigation feasible.
	
## Why
For a lot of projects -- especially if you have a lot of prototypes -- you need to create some GUI features over and over again. I got tired of this, so I made this library to help with common tasks. For example, you can set this up and readily display error messages even when prototyping.

## How
This is intended to be a [git submodule](https://git-scm.com/docs/git-submodule), but frankly, git sucks and submodules _are even worse_, so just clone/download the repo and copy the files into your project. I included a .unitypackage too.

I included examples for everything and it fits into a single scene easily. Check that out. A detailed explanation is in [details.md](details.md).

## Who
I'm on twitter as [@thelastpointer](https://twitter.com/thelastpointer) if you'd like to chat!

Tell me if you like it or hate it, if you have bugfixes or ideas. Please keep in mind that I'd like to keep this as simple as possible.

## License
MIT -- if you extend it and release the source, credit this project so we can help each other. I won't come after you if you don't.

## TODO
There are still a couple of things I want to do:
* Text prompt might need a character validation callback for passwords
* ~~Message box transitions~~
* ~~Move transitions into their own struct~~
* A submodule how-to document
* In-code documentation :P
* Controller support:
  * ~~Auto-select first control~~
  * ~~Window next/prev controls somehow (custom Input button?)~~
  * ~~Tabs next/prev controls~~
  * ~~Document all this~~