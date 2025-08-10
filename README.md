# The Lag Fixer (Camera Rigidbody Interpolation edition)
This mod fixes framerate agnostic stutter present in Getting Over It with Bennett Foddy.\
It does this with vanilla parity by default by:
1. Turning interpolation on for the player and other rigidbodies
2. Attaching a rigidbody with interpolation to the camera, and patching the camera control script to move the rigidbody instead of the transform

You may configure the mod to instead move the camera during LateUpdate instead, which exists to preserve the original proof of concept.\
Do not expect support from me or community members when this is enabled.
