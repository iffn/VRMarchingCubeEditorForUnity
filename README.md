# VRMarchingCubeEditorForUnity
Goal: Use VR to design scenes.

## Main features
- Marching Cube editing in VR.
- Object adding, moving and removing. Changes applied to the scene after exiting play mode.
- Swapping between scaling ghost mode for editing and walking around mode for testing.

## Current state
- Currently in early development
- Currently designed for play mode use

## Use
### Project setup
- Use a Unity project.
  - Tested with 2022.3.22f1 with BIRP
  - Recommendation: Use a fresh project due to the XR Interaction Toolkit.
- Install and setup the XR Interaction toolkit.
  - Follow the first part of this until 4:28. https://www.youtube.com/watch?v=DhqCLamOIQk
  - For the XR Interaction Toolkit in Samples, add the Starter Assets and XR Device Simulator.
- Additional recommended settings:
  - For shaders to render on both eyes: Project Settings - XR Plug-in Management > OpenXR - Render Mode = Multi Pass
- Add the Marching Cube Editor on the PlaymodeEditor branch somewhere in the Asset folder: https://github.com/iffn/MarchingCubeEditorForUnity/tree/PlaymodeEditor
- Add this Repository somewhere in the Asset folder

### Scene setup
- The main folder of this project contains an example scene.
  - Disable the XR Device Simulator before using it in VR
- For your own scene
  - Add the VREditor in UseMe to your scene
    - Assign the Marching Cube asset in your scene to it
  - Assign the MoveableObject script to any objects you want to be able to move.
    - The button lets you set up the collider automatically
- Moveable object prefabs
  - Add the MoveableObject script to any prefabs you want to be able to spawn.
  - Assign the prefabs you want to be able to spawn to the VREditor.

### Use
- Hit play. You can either test it in desktop using the XR Device Simulator or in VR.
- When exiting play, the Marching cube data is saved and all MoveableObject additions, movements and removals will be applied to the scene.

## ToDo
### Soon
- [x] Implement different MarchingCube tools and shapes
- [x] Fix object spawning, moving, duplicating and removing
- [x] Implement separate player and editor controllers
- [x] Implement start walking around from raycast position
- [x] Bind UI to hand if Device Simulator not active
- [x] Fix player scaling
- [x] Scale rest of components when scaling
- [ ] Test - Implement Save, Load and Save on exit playmode UI options for Marching Cube Editor
- [ ] Disable tools when entering walking mode
- [ ] Implement undo on exit playmode
- [ ] Implement load Marching Cube Editor on exit play mode
- [ ] Implement paint options as scriptable objects
- [ ] Backup, turn off and reset Static flag on Enter/Exit play mode
- [ ] Capitalize enums for display
- [ ] Improve runtime marching cube controller architecture
- [ ] Apply marching cube data on exit play mode
- [ ] Add namespaces

### Later
- [ ] Implement light settings
- [ ] Implement ability to swap hands
- [ ] Implement control indicators
- [ ] Automatically detect MarchingCubeEditor and select correct one if there are multiple
- [ ] Show borders of MarchingUbeEditor