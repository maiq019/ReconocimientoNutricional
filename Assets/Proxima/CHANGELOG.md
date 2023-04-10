# Proxima Changelog

## Version 1.1.0
- Added support for non-english characters in gameObject names, logs, etc.
  - Right-to-left languages are forced to display left-to-right in the browser pages to match Unity's behavior.
- Add "Run Script" button to run a sequence of commands in the console. See https://www.unityproxima.com/docs/console
- Added a button to collapse the navigation panel to just icons for smaller screens.
- Added touch-drag support for modifying numbers and arrays in the proxima inspector.
- Added an option "Set Run In Background" to Proxima Inspector to have Unity continue running when not in focus while
  Proxima is running. This is useful if you are connecting to Proxima from a browser on the same device, since the
  browser will cause Unity to lose focus.
- Prevent messages from sending when the connection is closed to avoid logged exceptions.

## Version 1.0.0
 - Initial Release