# AR Ikebana Scripts

This folder contains all the scripts for the AR Ikebana application.

## Folder Structure

```
Scripts/
├── Managers/
│   ├── GameStateManager.cs       - Manages overall game state flow
│   ├── UIManager.cs              - Controls UI panel visibility
│   ├── ARManager.cs              - Handles AR plane detection and raycasting
│   ├── PlateManager.cs           - Manages plate selection and placement
│   ├── FlowerManager.cs          - Manages flower selection and spawning
│   └── TrimmingManager.cs        - Handles flower trimming functionality
│
└── Utilities/
    └── MeshTrimmer.cs            - Utility for mesh cutting (basic implementation)
```

## Setup Instructions

### 1. Create Manager GameObjects in Scene

1. Create an empty GameObject named "Managers" in your scene
2. Add each manager script as a component to this GameObject (or create separate GameObjects for each)

### 2. Configure Managers

#### GameStateManager
- No special configuration needed
- Automatically initializes to MainMenu state

#### UIManager
- Assign all UI panel GameObjects in the inspector:
  - MainMenuPanel
  - PlateSelectionPanel
  - PlatePlacementPanel
  - FlowerSelectionPanel
  - TrimmingPanel

#### ARManager
- Assign AR Foundation components:
  - ARRaycastManager
  - ARPlaneManager
  - ARAnchorManager
- Assign the AR Camera

#### PlateManager
- Create a "PlateAnchor" empty GameObject in your scene
- Assign it to the PlateAnchor field
- Add plate prefabs to the PlatePrefabs list
- Optionally assign a placement indicator prefab

#### FlowerManager
- A FlowerContainer will be created automatically
- Add flower prefabs to the FlowerPrefabs list

#### TrimmingManager
- Assign the flower layer mask
- Assign scissor tool prefab
- Create a parent GameObject for the scissor tool

## Usage Examples

### Changing Game State

```csharp
// In a button click handler or other script
GameStateManager.Instance.ChangeState(GameState.PlateSelection);
```

### Selecting and Placing a Plate

```csharp
// Select a plate
PlateManager.Instance.SelectPlate(0); // Select first plate

// Place plate at AR hit position
if (ARManager.Instance.RaycastFromScreenCenter(out Vector3 hitPos, out Quaternion hitRot))
{
    PlateManager.Instance.SpawnPlate(hitPos, hitRot);
    PlateManager.Instance.ConfirmPlatePlacement();
}
```

### Spawning a Flower

```csharp
// Select a flower
FlowerManager.Instance.SelectFlower(0); // Select first flower

// Spawn at position
Vector3 spawnPosition = new Vector3(0, 0, 1);
FlowerManager.Instance.SpawnFlower(spawnPosition, Quaternion.identity);
```


## Next Steps

1. **Create UI Panels**: Set up all UI panels in your Canvas
2. **Create Prefabs**: Create plate and flower prefabs
3. **Implement AR Placement**: Create a script that handles plate placement interaction
4. **Implement Mesh Cutting**: Complete the mesh trimming functionality (consider using a library like EzySlice)
5. **Add Hand Tracking**: Implement hand/controller tracking for scissor tool
6. **Connect UI Buttons**: Wire up UI buttons to manager methods

## Notes

- All managers use the Singleton pattern for easy access
- The mesh trimming functionality is a basic placeholder - you'll need to implement or use a library for full mesh cutting
- AR Foundation components need to be set up in your scene for AR functionality to work
- Make sure to configure XR settings in Unity's Project Settings

