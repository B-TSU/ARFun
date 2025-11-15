# AR Ikebana Scene Hierarchy Guide

## Recommended Scene Structure

This document outlines the recommended scene hierarchy for your AR Ikebana application. This structure will help organize your project and make it easier to manage different states and components.

---

## Main Scene Hierarchy

```
IkebanAR Scene
├── Directional Light
│
├── [BuildingBlock] Camera Rig (Meta SDK)
│   └── TrackingSpace
│       ├── CenterEyeAnchor (Camera)
│       ├── LeftHandAnchor
│       │   └── [BuildingBlock] Real Hands (left)
│       └── RightHandAnchor
│           └── [BuildingBlock] Real Hands (right)
│
├── [BuildingBlock] OVRInteraction
│   ├── Left Interactors (grab, poke, etc.)
│   └── Right Interactors (grab, poke, etc.)
│
├── [BuildingBlock] Passthrough
│
├── [BuildingBlock] Ray Interaction
│
├── Poke Interaction
│
├── Managers (Empty GameObject)
│   ├── GameStateManager (Component)
│   ├── ARManagerMetaSDK (Component)
│   ├── UIManager (Component)
│   ├── PlateManager (Component)
│   ├── FlowerManager (Component)
│   └── TrimmingManager (Component)
│
├── UI Canvas (Screen Space - Overlay)
│   ├── MainMenuPanel
│   │   ├── Title
│   │   ├── StartButton
│   │   ├── TutorialButton
│   │   └── SettingsButton
│   │
│   ├── PlateSelectionPanel
│   │   ├── Title
│   │   ├── PlateGrid (ScrollView)
│   │   │   └── Content
│   │   │       ├── PlateButton_1
│   │   │       ├── PlateButton_2
│   │   │       └── PlateButton_3
│   │   └── BackButton
│   │
│   ├── PlatePlacementPanel
│   │   ├── InstructionText
│   │   ├── ConfirmButton
│   │   └── CancelButton
│   │
│   ├── FlowerSelectionPanel
│   │   ├── Title
│   │   ├── FlowerGrid (ScrollView)
│   │   │   └── Content
│   │   │       ├── FlowerButton_1
│   │   │       ├── FlowerButton_2
│   │   │       └── ...
│   │   └── BackButton
│   │
│   └── TrimmingPanel
│       ├── InstructionText
│       ├── ScissorIcon
│       ├── DoneButton
│       └── CancelButton
│
├── AR Content (Empty GameObject - World Space)
│   ├── ARPlaneVisualizer (Optional - for debugging)
│   │
│   ├── PlateAnchor (Spawned at runtime)
│   │   └── CurrentPlate (3D Model)
│   │       ├── PlateMesh
│   │       ├── PlacementIndicator
│   │       └── ConfirmationUI (World Space Canvas)
│   │           ├── ConfirmButton
│   │           └── CancelButton
│   │
│   └── FlowerContainer (Empty GameObject - Parent for all flowers)
│       ├── Flower_1 (Spawned at runtime)
│       │   ├── FlowerMesh
│       │   ├── TrimmingPoints (Empty GameObject)
│       │   │   ├── CutPoint_1
│       │   │   ├── CutPoint_2
│       │   │   └── ...
│       │   └── FlowerCollider
│       │
│       ├── Flower_2
│       └── ...
│
├── Tools (Empty GameObject)
│   ├── ScissorTool (3D Model)
│   │   ├── ScissorMesh
│   │   ├── ScissorCollider
│   │   └── CutLineRenderer (LineRenderer component)
│   │
│   └── RaycastPointer (For interaction)
│
├── Effects (Empty GameObject)
│   ├── ParticleEffects
│   │   ├── CutEffect
│   │   └── PlacementEffect
│   │
│   └── AudioSources
│       ├── UISoundSource
│       ├── AmbientSoundSource
│       └── EffectSoundSource
│
└── Lighting
    ├── Directional Light
    └── AR Light Estimation (if supported)
```

---

## Detailed Component Breakdown

### 1. **XR Origin**
- **Purpose**: Main AR tracking and camera setup
- **Components Needed**:
  - `XROrigin` (AR Foundation)
  - `ARPlaneManager` (for surface detection)
  - `ARRaycastManager` (for raycasting to place objects)
  - `ARAnchorManager` (for anchoring the plate)

### 2. **Managers**
All managers should be singleton scripts that control different aspects of the app:

- **GameManager**: Controls overall game state and flow
- **ARManager**: Handles AR-specific functionality (plane detection, placement)
- **UIManager**: Manages UI panel visibility and transitions
- **PlateManager**: Handles plate selection, spawning, and placement
- **FlowerManager**: Manages flower selection, spawning, and arrangement
- **TrimmingManager**: Handles flower trimming logic and mesh cutting

### 3. **UI Canvas Structure**
- Use **Screen Space - Overlay** for main UI panels
- Use **World Space Canvas** for AR-embedded UI (like confirmation menu near plate)
- Each panel should be a separate GameObject that can be enabled/disabled

### 4. **AR Content**
- **PlateAnchor**: The anchor point where the plate is placed in AR space
- **FlowerContainer**: Parent object for all flowers to keep hierarchy clean
- Each flower should be a prefab that can be instantiated

### 5. **Tools**
- **ScissorTool**: 3D model that follows hand/controller for trimming
- **RaycastPointer**: Visual indicator for where user is pointing

---

## Prefab Structure Recommendations

### Plate Prefab
```
Plate_Prefab
├── PlateModel (MeshRenderer + MeshFilter)
├── PlateCollider (for placement detection)
├── PlacementIndicator (visual feedback)
└── PlateData (ScriptableObject reference)
```

### Flower Prefab
```
Flower_Prefab
├── FlowerModel (MeshRenderer + MeshFilter)
├── FlowerCollider
├── TrimmingPoints (Empty GameObject)
│   └── [Multiple cut point markers]
├── FlowerData (ScriptableObject reference)
└── TrimmableFlower (Script component)
```

### Scissor Tool Prefab
```
Scissor_Prefab
├── ScissorModel (MeshRenderer + MeshFilter)
├── ScissorCollider
├── CutLineRenderer (LineRenderer)
└── ScissorController (Script component)
```

---

## ScriptableObjects for Data

Create ScriptableObjects to store:
- **PlateData**: Plate models, names, descriptions
- **FlowerData**: Flower models, names, trimming points, properties
- **AITipData**: Tips and guidance for beginners

---

## State Machine Flow

```
MainMenu
  ↓
PlateSelection
  ↓
PlatePlacement (AR)
  ↓
PlateConfirmation
  ↓
FlowerSelection
  ↓
FlowerArrangement (AR)
  ↓
[Optional: Trimming]
  ↓
Screenshot
```

---

## Key Scripts to Create

1. **GameStateManager.cs** - Manages state transitions
2. **PlatePlacementController.cs** - Handles AR plate placement
3. **FlowerSpawner.cs** - Spawns and manages flowers
4. **MeshTrimmer.cs** - Handles 3D mesh cutting/trimming
5. **ARInteractionHandler.cs** - Handles AR interactions

---

## AR-Specific Setup Notes

1. **AR Foundation Setup**:
   - Ensure ARPlaneManager is configured
   - Set up ARRaycastManager for object placement
   - Configure ARAnchorManager for persistent placement

2. **World Space UI**:
   - Create a separate Canvas with "World Space" render mode
   - Position it relative to the plate anchor
   - Use Billboard script to face camera

3. **Hand Tracking** (if using Quest):
   - Set up OVRHand components
   - Configure hand tracking in Oculus settings
   - Use hand poses for scissor tool interaction

---

## Performance Considerations

1. **Object Pooling**: Pool flower prefabs instead of instantiating/destroying
2. **LOD Groups**: Use LOD for complex flower models
3. **Occlusion Culling**: Enable for better performance
4. **Texture Compression**: Optimize textures for mobile/VR
5. **Mesh Optimization**: Keep polygon counts reasonable for real-time trimming

---

## Next Steps

1. Set up the basic scene hierarchy in Unity
2. Create empty GameObjects for each manager
3. Set up UI Canvas and panels
4. Create placeholder prefabs for plates and flowers
5. Implement basic state machine
6. Add AR Foundation components
7. Test AR plane detection and placement

---

## Additional Resources

- Unity AR Foundation Documentation
- Meta XR SDK Documentation
- Unity Input System for hand/controller input
- Mesh cutting algorithms (consider using libraries like MeshCut or implementing custom solution)

