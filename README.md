# VRMarchingCubeEditorForUnity
Goal: Use VR to design scenes.

## Current state
- Currently in early development
- Currently designed for play mode use

## Requrements
### Unity 3D
Tested with 2022.3.22f1 with BIRP

### XR Interaction Toolkit
Follow the first part of this until 4:28  
https://www.youtube.com/watch?v=DhqCLamOIQk

### Marching Cube Editor on Playmode Editor branch
https://github.com/iffn/MarchingCubeEditorForUnity/tree/PlaymodeEditor

## ToDo
### Soon
- [x] Implement different MarchingCube tools and shapes
- [ ] Test - Fix object spawning, moving, duplicating and removing
- [ ] Test - Implement separate player and editor controllers
- [ ] Test - Implement start walking around from raycast position
- [ ] Test - Bind UI to hand if Device Simulator not active
- [ ] Test - Fix player scaling
- [ ] Test - Scale rest of components when scaling
- [ ] Disable tools when entering walking mode
- [ ] Implement undo on exit playmode
- [ ] Improve runtime marching cube controller

### Later
- [ ] Implement light settings
- [ ] Implement ability to swap hands
- [ ] Implement control indicators
- [ ] Automatically detect MarchingCubeEditor and select correct one if there are multiple
- [ ] Show borders of MarchingUbeEditor