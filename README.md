# The Lag Fixer
This mod fixes framerate agnostic stutter present in Getting Over It with Bennett Foddy.\
It does this with vanilla parity by default by:
1. Turning interpolation on for the player and other rigidbodies
2. Attaching a rigidbody with interpolation to the camera, and patching the camera control script to move the rigidbody instead of the transform

You may configure the mod to instead move the camera during LateUpdate instead, which exists to preserve the original proof of concept.\
Do not expect support from me or community members when this is enabled.

## Building
To build, place `Assembly-CSharp.dll` into a `lib/netstandard2.0` folder in the root directory of the project and compile with `dotnet build -c B5` or `dotnet build -c B6` depending on whether you are targetting BepInEx version 5 or 6 respectively. For level loader support (default) you will have to use an assembly containing the level loader, be it a modpack or plain level loader assembly.
