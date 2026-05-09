# The Lag Fixer
This mod fixes framerate agnostic stutter present in Getting Over It with Bennett Foddy.\
It does this with vanilla parity by:
1. Turning interpolation on for the player and other rigidbodies
2. Attaching a rigidbody with interpolation to the camera, and patching the camera control script to move the rigidbody instead of the transform

## Building
To build, create a `lib` folder in the root of the project and place the game's `Assembly-CSharp.dll` into it. Then use `dotnet build` with the argument `-r BepInEx5` or `-r BepInEx6` depending on whether you are targeting BepInEx version 5 or 6 respectively.
