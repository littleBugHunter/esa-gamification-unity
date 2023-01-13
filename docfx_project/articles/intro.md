# Image Annotation

A package to annotate Images for the ESA Moon Lander Project. This Package provides functions to interface with the Server and multiple example Marking Techniques, like Maps Like Marking and Two Finger Marking

## Links

[Github Repo](https://github.com/littleBugHunter/esa-gamification-unity)

[Documentation](https://littlebughunter.github.io/esa-gamification-unity/)

[API Reference](https://littlebughunter.github.io/esa-gamification-unity/api)

## Installation

### 1. Prerequisites

#### [Git](https://git-scm.com/)

You must have git installed in order to use the Unity Package Manager.

The command line version from the [Downloads Section](https://git-scm.com/downloads) should be enough.

Make sure to add git to your Environment Variables (PATH). This can be done via the installer.

If you're still facing problems check the [Unity Documentation](https://docs.unity3d.com/Manual/upm-git.html) for the Pacakage Manager

#### [Naughty Attributes](https://github.com/dbrizov/NaughtyAttributes)

This package depends on the Naughty Attributes package. Please make sure to install it beforehand:

Go to the Unity Package Manager and click the `➕` Sign in the top left corner to "Add from git URL" then paste the following Link:

`https://github.com/dbrizov/NaughtyAttributes.git#upm`

### 2. Package Installation

Go to the Unity Package Manager and click the `➕` Sign in the top left corner to "Add from git URL" then paste the following Link:

`https://github.com/littleBugHunter/esa-gamification-unity.git#upm`

## Samples

The Package comes with Samples that can be selected from the Unity Package Manager. In the Samples you will find example Setups for the different Marking Setups along with Prefabs.

The Example Scenes can be found in: `Assets/Samples/Annotation/Scenes`

## Component Setup

To customize the look and behaviour of the Annotation tool I reccommend copying the GameObjects from one of the Samples Scenes.

### General Structure

The Annotation tool is split into two main categories. Managers taking care of the Server Connection and the UI Taking care of the actual Annotation Process.

#### Managers

You will always need a `ServerConnection` Component, so the App knows how to communicate with the server.

On top of that you will need a Client for each of the Modes. For now we only have the `PuzzleModeClient`.

The Client takes care of fetching, presenting and submitting the images.

#### UI

The UI is split into 3 Main Component Groups

##### Panels

Panels are shown and hidden by the Client when needed. For example the `PuzzleClient` shows a `MarkingPanel` to prompt the User to mark an image.

Panels usually hold the other UI Components

##### Markers

Markers offer the User various ways of marking an image. Depending on which Marker you place, the Marking User Experience will Change.

Right now we have two different Markers. The `TwoFingerMarker` and the `CenterMarker` (for Maps Style Marking)

##### Visuals

Visuals are UI Component Prefabs that will be placed by the different UI Components.

For example the `CraterLogger` places CraterVisuals to show here the user has marked already. And the `TwoFingerMarking` has optional Visuals to show around the fingers, in the center and as lines between the fingers.

Visuals often have specific requirements to their Scale and setup.

If you want to skin your own visuals I reccomend copying the Visual Prefabs from the Samples folder

#### Crater Logger

The `CraterLogger` is a temporary storage of the currently placed craters. It takes care of rendering Visuals and communicates the craters to the active Client.

### Connecting your custom code

Your custom code should call the `StartPuzzle()` function of the `PuzzleModeClient`. The Client will then fetch the puzzle data from the server and open up the `MarkingPanel`.

You should give the user the possibility to call the `FinishMarking()` function on your `MarkingPanel` to go on to the next puzzle. This can be done with a Button for example.

Once all Images have been annotated, the `PuzzleModeClient` will submit them to the server and fetch a Score Object which contains the annotation accuracy. It will then invoke the `OnPuzzleDone` event which you can hook into in order to give out rewards and continue on with the gameplay.