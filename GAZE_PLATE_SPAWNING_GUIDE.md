# Gaze-Based Plate Spawning Guide

This guide explains how to set up and use the gaze-based plate spawning system, which allows you to spawn plate prefabs onto surfaces by looking at them.

## Overview

The `GazePlateSpawner` component enables you to:
- **Look at a surface** to detect where to place a plate
- **Automatically spawn** a plate prefab after looking for a set duration (gaze dwell)
- **Manually spawn** a plate prefab with a button press
- **Visual feedback** showing where the plate will be placed

## Setup Instructions

### 1. Add GazePlateSpawner Component

1. In your scene, find or create a GameObject to hold the GazePlateSpawner (e.g., "AR Managers" or "Interaction Manager")
2. Add the `GazePlateSpawner` component to this GameObject

### 2. Configure Settings

#### Gaze Settings
- **Gaze Dwell Time**: How long to look at a surface before auto-spawning (default: 2 seconds)
- **Auto Spawn On Gaze**: Enable automatic spawning after gaze duration
- **Max Gaze Distance**: Maximum distance to detect surfaces (default: 5 meters)
- **Min Gaze Distance**: Minimum distance to detect surfaces (default: 0.3 meters)

#### Visual Feedback
- **Gaze Indicator Prefab**: Optional prefab to show where you're looking (will create a default cylinder if not set)
- **Valid Gaze Color**: Color when looking at a valid surface (default: Green)
- **Invalid Gaze Color**: Color when not looking at a valid surface (default: Red)
- **Show Gaze Ray**: Enable to show a line from camera to hit point

#### Input Settings
- **Use Manual Input**: If enabled, requires button press instead of auto-spawn
- **Spawn Key**: Keyboard key to spawn plate manually (default: Space)
- **Use XR Input**: Enable XR controller input support

#### Plate Prefab Settings
- **Plate Prefab Override**: (Optional) Assign a specific plate prefab to spawn
- **Use Prefab Override**: Enable to use the override prefab instead of PlateManager's selected plate

### 3. Method 1: Using PlateManager (Recommended)

This method uses the existing PlateManager system:

1. **Ensure PlateManager is set up**:
   - PlateManager component exists in scene
   - Plate prefabs are assigned to PlateManager's plate prefabs list
   - A plate is selected via `PlateManager.Instance.SelectPlate(index)`

2. **Configure GazePlateSpawner**:
   - Leave **Use Prefab Override** = `false`
   - The system will use the plate selected in PlateManager

3. **Usage**:
   ```csharp
   // Select a plate first
   PlateManager.Instance.SelectPlate(0); // Select first plate
   
   // Change to plate placement state
   GameStateManager.Instance.ChangeState(GameState.PlatePlacement);
   
   // Now look at a surface - plate will spawn automatically or on button press
   ```

### 4. Method 2: Using Prefab Override

This method spawns a specific prefab directly:

1. **Assign the plate prefab**:
   - Drag your plate prefab (e.g., `Assets/Prefab/Plate.prefab`) to the **Plate Prefab Override** field
   - Enable **Use Prefab Override** = `true`

2. **Usage**:
   - Simply look at a surface - the assigned prefab will spawn
   - No need to select a plate in PlateManager

## How It Works

### Surface Detection

The system uses two methods to detect surfaces:

1. **AR Foundation Raycast** (if available):
   - Detects AR planes (horizontal/vertical surfaces)
   - Uses `ARRaycastManager` to find trackable planes

2. **Physics Raycast** (fallback):
   - Uses Unity's Physics system
   - Detects colliders on specified layers
   - Configure via **Surface Layer Mask**

### Spawning Modes

#### Automatic Spawning (Default)
- Look at a valid surface
- After **Gaze Dwell Time** seconds, the plate automatically spawns
- Visual indicator shows progress

#### Manual Spawning
- Enable **Use Manual Input** = `true`
- Look at a valid surface
- Press the **Spawn Key** (default: Space) or use XR controller input
- Plate spawns immediately

## Visual Feedback

### Gaze Indicator
- A visual object (default: green cylinder) appears where you're looking
- Shows valid (green) or invalid (red) surface detection
- Positioned at the hit point on the surface

### Gaze Ray
- A line from the camera to the hit point
- Shows the direction you're looking
- Color changes based on validity

### Placement Indicator
- Uses PlateManager's placement indicator if available
- Shows where the plate will be placed

## Code Examples

### Spawn Plate Prefab Directly

```csharp
// Get the GazePlateSpawner component
GazePlateSpawner spawner = FindObjectOfType<GazePlateSpawner>();

// Set a specific prefab to spawn
spawner.platePrefabOverride = myPlatePrefab;
spawner.usePrefabOverride = true;

// Or use PlateManager
PlateManager.Instance.SpawnPlateFromPrefab(myPlatePrefab, position, rotation);
```

### Check Gaze Progress

```csharp
GazePlateSpawner spawner = FindObjectOfType<GazePlateSpawner>();

// Get progress (0-1) for UI display
float progress = spawner.GetGazeProgress();

// Check if currently gazing at surface
bool isGazing = spawner.IsGazingAtSurface();
```

### Manually Trigger Spawn

```csharp
GazePlateSpawner spawner = FindObjectOfType<GazePlateSpawner>();

// Trigger spawn programmatically
spawner.TriggerSpawn();
```

## Troubleshooting

### Plate Not Spawning

1. **Check Game State**:
   - Ensure you're in `GameState.PlatePlacement` state
   - The spawner only works in this state

2. **Check Surface Detection**:
   - Ensure AR planes are being detected (check ARManager)
   - Or ensure surfaces have colliders on the correct layer
   - Check **Surface Layer Mask** setting

3. **Check Distance**:
   - Surface must be within **Min Gaze Distance** and **Max Gaze Distance**
   - Default: 0.3m to 5m

4. **Check Plate Selection**:
   - If using PlateManager, ensure a plate is selected
   - If using prefab override, ensure prefab is assigned

### Visual Feedback Not Showing

1. **Check Gaze Indicator**:
   - Ensure **Gaze Indicator Prefab** is assigned or let it create default
   - Check that surfaces are being detected

2. **Check Gaze Ray**:
   - Ensure **Show Gaze Ray** is enabled
   - Check that LineRenderer component is created

### Plate Spawning in Wrong Position

1. **Check Surface Normal**:
   - The plate rotation uses the surface normal
   - Ensure surfaces have proper normals

2. **Check Anchor Creation**:
   - AR anchors may affect positioning
   - Check ARManager anchor creation

## Integration with Existing System

The GazePlateSpawner integrates with:
- **PlateManager**: Uses selected plate or can override
- **GameStateManager**: Only active in PlatePlacement state
- **ARManager**: Uses AR raycasting for surface detection
- **AR Anchors**: Creates anchors for persistent placement

## Plate Prefab with Children (JapanesePlate/Needle)

If your Plate prefab has children with colliders and rigidbodies (like JapanesePlate and Needle):

### Important Configuration

1. **Exclude Plate Layer from Surface Detection**:
   - Set **Exclude Layers** to include Layer 6 (or whatever layer your Plate prefab is on)
   - This prevents the gaze raycast from detecting already-spawned plates
   - In Unity: Click the layer mask dropdown, select your Plate layer

2. **Surface Offset**:
   - The system automatically adds a small offset (0.01m) above the surface
   - This prevents the plate from clipping into the surface
   - Adjust if needed based on your plate's geometry

3. **Rigidbody Handling**:
   - The system automatically handles child rigidbodies with gravity
   - Gravity is temporarily disabled for 0.1 seconds after spawning
   - This ensures proper initial positioning before physics takes over
   - After the delay, gravity re-enables and the plate settles naturally

### Example Setup for Plate Prefab

```
GazePlateSpawner Settings:
- Plate Prefab Override: [Your Plate.prefab]
- Use Prefab Override: ✓ (checked)
- Exclude Layers: Layer 6 (Plate layer)
- Surface Offset: 0.01
- Gaze Dwell Time: 2.0 seconds
```

## Best Practices

1. **Use Prefab Override** for simple, direct spawning
2. **Use PlateManager** for more complex plate selection systems
3. **Adjust Gaze Dwell Time** based on user experience (2-3 seconds is comfortable)
4. **Provide Visual Feedback** so users know where they're looking
5. **Test Distance Constraints** to ensure comfortable interaction range
6. **Exclude Plate Layer** if your plate prefab has colliders to prevent self-detection
7. **Adjust Surface Offset** if plates are clipping into surfaces or floating too high

## Example Scene Setup

```
Scene Hierarchy:
├── AR Managers
│   ├── ARManager (or ARManagerMetaSDK)
│   ├── PlateManager
│   └── GazePlateSpawner ← Add component here
├── XRCameraRig (or AR Camera)
└── AR Content
    └── PlateAnchor (assigned to PlateManager)
```

## Next Steps

- Customize visual feedback to match your app's style
- Add sound effects for gaze detection and spawning
- Implement haptic feedback for XR controllers
- Add UI progress bar showing gaze dwell time
- Support multiple plate types with different prefabs

