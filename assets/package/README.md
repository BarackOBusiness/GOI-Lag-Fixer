# The Lag Fixer
Fixes framerate agnostic stutter present in Getting Over It.\
This occurs due to physics updates in FixedUpdate running at a fixed 120Hz interval without any rigidbodies having interpolation.\
This mod enables interpolation on all rigidbodies in the game and intercepts the camera to move a rigidbody instead of its own transform to smooth it out while maintaining parity with vanilla follow behavior.
