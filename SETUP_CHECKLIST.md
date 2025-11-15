# AR Ikebana Setup Checklist

Use this checklist to set up your AR Ikebana scene step by step.

## Pre-Setup: Scene Hierarchy

- [ ] Create the main scene hierarchy as outlined in `SCENE_HIERARCHY_GUIDE.md`
- [ ] Verify `[BuildingBlock] Camera Rig` is in scene
- [ ] Remove/Disable `XROrigin` if present (conflicts with Camera Rig)
- [ ] Create empty GameObject named "Managers"
- [ ] Create empty GameObject named "AR Content"
- [ ] Create empty GameObject named "Tools"
- [ ] Create empty GameObject named "Effects"

## Step 1: Meta Building Blocks Setup

- [ ] Verify `[BuildingBlock] Camera Rig` is active
- [ ] Verify `[BuildingBlock] Real Hands` is active
- [ ] Verify `[BuildingBlock] OVRInteraction` is active
- [ ] Verify `[BuildingBlock] Passthrough` is active
- [ ] Add `[BuildingBlock] Ray Interaction` (optional but recommended)
- [ ] Check dependencies in Inspector (should show green checkmarks)

## Step 2: Manager Scripts Setup

- [ ] Add `GameStateManager` script to Managers GameObject
- [ ] Add `UIManager` script to Managers GameObject
- [ ] Add `ARManagerMetaSDK` script to Managers GameObject (for Camera Rig approach)
- [ ] Add `PlateManager` script to Managers GameObject
- [ ] Add `FlowerManager` script to Managers GameObject
- [ ] Add `TrimmingManager` script to Managers GameObject

## Step 3: UI Setup

- [ ] Create Canvas (Screen Space - Overlay)
- [ ] Create MainMenuPanel with:
  - [ ] Title text
  - [ ] Start button
  - [ ] Tutorial button
  - [ ] Settings button
- [ ] Create PlateSelectionPanel with:
  - [ ] Title text
  - [ ] ScrollView with grid layout for plate buttons
  - [ ] Back button
- [ ] Create PlatePlacementPanel with:
  - [ ] Instruction text
  - [ ] Confirm button
  - [ ] Cancel button
- [ ] Create FlowerSelectionPanel with:
  - [ ] Title text
  - [ ] ScrollView with grid layout for flower buttons
  - [ ] Back button
- [ ] Create TrimmingPanel with:
  - [ ] Instruction text
  - [ ] Scissor icon
  - [ ] Done button
  - [ ] Cancel button
- [ ] Assign all panels to UIManager in inspector
- [ ] Initially hide all panels except MainMenuPanel

## Step 4: AR Content Setup

- [ ] Create "PlateAnchor" empty GameObject under AR Content
- [ ] Create "FlowerContainer" empty GameObject under AR Content
- [ ] Assign PlateAnchor to PlateManager
- [ ] Assign FlowerContainer to FlowerManager (or let it auto-create)

## Step 5: Prefabs Creation

### Plate Prefabs
- [ ] Create Plate_1 prefab with:
  - [ ] 3D model mesh
  - [ ] MeshRenderer
  - [ ] MeshFilter
  - [ ] Collider
- [ ] Create Plate_2 prefab
- [ ] Create Plate_3 prefab
- [ ] Add all plate prefabs to PlateManager's platePrefabs list

### Flower Prefabs
- [ ] Create Flower_1 prefab with:
  - [ ] 3D model mesh
  - [ ] MeshRenderer
  - [ ] MeshFilter
  - [ ] Collider
  - [ ] TrimmableFlower component (to be created)
- [ ] Create additional flower prefabs
- [ ] Add all flower prefabs to FlowerManager's flowerPrefabs list

### Scissor Tool Prefab
- [ ] Create Scissor prefab with:
  - [ ] 3D scissor model
  - [ ] Collider
  - [ ] LineRenderer for cut visualization
- [ ] Assign to TrimmingManager

### Placement Indicator Prefab
- [ ] Create PlacementIndicator prefab (e.g., semi-transparent plate outline)
- [ ] Assign to PlateManager

## Step 6: Manager Configuration

### ARManager
- [ ] Assign ARRaycastManager reference
- [ ] Assign ARPlaneManager reference
- [ ] Assign ARAnchorManager reference
- [ ] Assign AR Camera reference

### PlateManager
- [ ] Assign PlateAnchor transform
- [ ] Add plate prefabs to list
- [ ] Assign placement indicator prefab

### FlowerManager
- [ ] Add flower prefabs to list
- [ ] Verify FlowerContainer is created/assigned

### TrimmingManager
- [ ] Set flower layer mask
- [ ] Assign scissor tool prefab
- [ ] Create scissor tool parent GameObject
- [ ] Assign cut material (optional)


## Step 7: Interaction Scripts

- [ ] Add `PlatePlacementController` to scene
- [ ] Connect UI button click events to manager methods
- [ ] Set up input handlers for:
  - [ ] Plate selection
  - [ ] Plate placement confirmation
  - [ ] Flower selection
  - [ ] Flower trimming

## Step 8: State Flow Implementation

- [ ] Connect MainMenu Start button → ChangeState(PlateSelection)
- [ ] Connect PlateSelection → ChangeState(PlatePlacement)
- [ ] Connect PlatePlacement confirm → ChangeState(PlateConfirmation)
- [ ] Connect PlateConfirmation → ChangeState(FlowerSelection)
- [ ] Connect FlowerSelection → ChangeState(FlowerArrangement)
- [ ] Connect Trimming button → ChangeState(Trimming)

## Step 9: Mesh Trimming Implementation

- [ ] Research and choose mesh cutting library (EzySlice, MeshCut, etc.)
- [ ] OR implement custom mesh cutting algorithm
- [ ] Complete `MeshTrimmer.cs` implementation
- [ ] Test mesh cutting on flower prefabs
- [ ] Implement cut visualization (particle effects, etc.)

## Step 10: Hand/Controller Tracking (Optional)

- [ ] Set up OVR hand tracking (if using Quest)
- [ ] OR set up controller tracking
- [ ] Implement scissor tool following hand/controller
- [ ] Add haptic feedback for cutting

## Step 11: Testing

- [ ] Test main menu navigation
- [ ] Test plate selection
- [ ] Test AR plane detection
- [ ] Test plate placement
- [ ] Test flower spawning
- [ ] Test flower arrangement
- [ ] Test trimming functionality
- [ ] Test on target device (Quest, etc.)

## Step 13: Polish

- [ ] Add sound effects
- [ ] Add particle effects
- [ ] Add animations
- [ ] Optimize performance
- [ ] Add error handling
- [ ] Add loading screens
- [ ] Add settings menu
- [ ] Add save/load functionality (optional)

## Additional Notes

- Make sure AR Foundation packages are installed
- Configure XR settings in Project Settings
- Set up build settings for target platform
- Test on actual device (AR doesn't work well in editor)

