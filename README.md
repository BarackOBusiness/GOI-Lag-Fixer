# The Lag Fixer (Camera Rigidbody Interpolation edition)
This mod is a proof of concept that attempts to fix the framerate irrespective stutter present in Getting Over It with Bennett Foddy.\
This is the Camera Rigidbody Interpolation branch, which attemps to solve this problem by the following methods:
1. Turning interpolation on for the player rigidbodies
2. Attaching a rigidbody with interpolation to the camera, and intercepting the camera movement code to move the rigidbody instead of the transform

Compared to the other branch (LateUpdate), this approach is more faithful to the original game's camera motion, however since target vectors are still computed within FixedUpdate instead of LateUpdate, stutter *is* still possible due to running at the same time as player control code.
