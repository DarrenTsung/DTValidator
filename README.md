# DTValidator
Tool for validating (ensuring no broken / missing outlets) objects like GameObjects, ScriptableObjects, etc, in the Unity Editor.

More importantly, it's easy to set it up as a unit test for integration with your build pipeline.

### Why use DTValidator?
It's so easy to break outlets by renaming functions, removing assets, losing .meta files, etc. DTValidator finds missing outlets (any UnityEngine.Object exposed in the inspector), broken UnityEvents, and missing MonoBehaviour scripts.

[Read more about DTValidator here.](https://medium.com/@darrentsung/goodbye-missingreferenceexception-automated-validation-on-unity-projects-38bbb2fc7a1a)

### To Install:
1. Download the DTValidator project from this repository by pressing [this link](https://github.com/DarrenTsung/DTValidator/archive/master.zip). It should automatically download the latest state of the master branch.
2. Place the downloaded folder in your project. I recommend placing it in the Assets/Plugins directory so [it doesn’t add to your compile time](https://medium.com/@darrentsung/the-clocks-ticking-how-to-optimize-compile-time-in-unity-45d1f200572b). 
3. Make a copy of [these](https://gist.github.com/DarrenTsung/b21d2645cf6e9519ac6f341d2f553eb1) validation unit tests in your project named `ValidationTests.cs` in your project under any Editor folder (default: `Assets/Editor`).

Now you have unit tests running to validate your project! Try running them by opening the `Test Runner` window under `Window->Test Runner`. It's likely that you'll have errors in your project - that's okay! Most likely the errors will be outlets that you need to mark `[Optional]`. See the  [`FAQ`](DealingWithErrors.md) for how to deal with these errors.
