# Scene Organization Guide - Camera Rig Approach

Based on your current scene, here's how to organize everything for the **Meta Building Blocks (Camera Rig)** approach.

---

## 🎯 Current Situation

You have `MetaOrigin` with `XRCameraRig` and multiple Meta Building Blocks in your scene. Let's organize everything properly for the Camera Rig approach.

---

## ✅ What to Keep (Meta Building Blocks)

### Core Setup:
- ✅ `MetaOrigin` with `XRCameraRig` - This is your Camera Rig setup (keep this!)
  - Contains `TrackingSpace` with eye anchors and hand anchors
  - Has `[BuildingBlock] Hand Trac` under hand anchors (hand tracking)

### Core Building Blocks:
- ✅ `[BuildingBlock] OVRInteraction` - Grab & poke interactions (keep this!)
  - Contains: OVRHmd, OVRHands, OVRControllers, LeftInteractions, RightInteractions, etc.
- ✅ `Meta_Passthrough` - AR passthrough view (keep this!)
- ✅ `Meta_Real Hands` - Hand tracking (keep this!)
- ✅ `Meta_Environment Raycast Manag` - Ray-based interaction (keep this!)
- ✅ `Poke Interaction` - Poke interaction component (keep this!)
  - Contains `[BuildingBlock] ISDK_Pokelnter` (this is correct)
- ✅ `[BuildingBlock] HandGrabInstallat` - Grab interaction setup (keep this!)

### Optional/Test Objects:
- ⚠️ `Meta_Cube` - Test object (can remove if not needed)
- ⚠️ `[BuildingBlock] Cube` - Test object (can remove if not needed)
  - Contains `[BuildingBlock] ISDK_RayIntera` (ray interaction test)

---

## ❌ What to Remove/Disable

### Test Objects (Optional - Remove if not needed):
- ⚠️ `Meta_Cube` - Test object (remove if not using)
- ⚠️ `[BuildingBlock] Cube` - Test object (remove if not using)

### Note:
Your setup looks good! You have:
- `MetaOrigin` with `XRCameraRig` - This is your Camera Rig (correct!)
- All the necessary Meta Building Blocks
- No conflicting AR Foundation components visible

**You're in good shape!** Just need to organize and optionally remove test objects.

---

## 📋 Recommended Scene Hierarchy

```
IkebanAR Scene
│
├── Directional Light
│
├── MetaOrigin (Your Camera Rig)
│   └── XRCameraRig
│       └── TrackingSpace
│           ├── LeftEyeAnchor
│           ├── CenterEyeAnchor (Camera)
│           ├── RightEyeAnchor
│           ├── TrackerAnchor
│           ├── LeftHandAnchor
│           │   ├── LeftControllerAnchor
│           │   ├── LeftControllerInHandAnch
│           │   └── [BuildingBlock] Hand Trac (hand tracking)
│           ├── RightHandAnchor
│           │   ├── RightControllerAnchor
│           │   ├── RightControllerInHandAnc
│           │   └── [BuildingBlock] Hand Trac (hand tracking)
│           ├── LeftHandAnchorDetached
│           └── RightHandAnchorDetached
│
├── [BuildingBlock] OVRInteraction
│   ├── OVRHmd
│   ├── OVRHands
│   ├── OVRControllers
│   ├── LeftInteractions
│   ├── RightInteractions
│   ├── OVRLeftHandVisual
│   ├── OVRRightHandVisual
│   ├── OVRLeftControllerVisual
│   ├── OVRRightControllerVisual
│   └── Locomotor
│
├── Meta_Passthrough
│
├── Meta_Real Hands
│
├── Meta_Environment Raycast Manag
│
├── [BuildingBlock] HandGrabInstallat
│
├── Poke Interaction
│   └── [BuildingBlock] ISDK_Pokelnter
│
├── Managers (Create this - Empty GameObject)
│   ├── GameStateManager (Component)
│   ├── UIManager (Component)
│   ├── ARManagerMetaSDK (Component)
│   ├── PlateManager (Component)
│   ├── FlowerManager (Component)
│   └── TrimmingManager (Component)
│
├── AR Content (Create this - Empty GameObject)
│   ├── PlateAnchor (Empty GameObject - for placed plate)
│   └── FlowerContainer (Empty GameObject - parent for all flowers)
│
├── Tools (Create this - Empty GameObject)
│   └── Scissors (3D Model - for trimming)
│
├── UI Canvas (Screen Space - Overlay)
│   ├── MainMenuPanel
│   ├── PlateSelectionPanel
│   ├── PlatePlacementPanel
│   ├── FlowerSelectionPanel
│   ├── TrimmingPanel
│   └── ScreenshotPanel
│
└── World Space Canvas (Optional - for 3D UI)
    └── PlateConfirmationUI (for plate placement confirmation)
```

---

## 🎯 Step-by-Step Organization

### Step 1: Verify Your Setup (2 minutes)

Your setup looks good! You have:
- ✅ `MetaOrigin` with `XRCameraRig` - This is your Camera Rig (correct!)
- ✅ `[BuildingBlock] OVRInteraction` - Interactions setup
- ✅ `Meta_Passthrough` - Passthrough view
- ✅ `Meta_Real Hands` - Hand tracking
- ✅ `Meta_Environment Raycast Manag` - Ray interaction
- ✅ `Poke Interaction` - Poke interactions
- ✅ `[BuildingBlock] HandGrabInstallat` - Grab interactions

**Everything you need is there!** ✅

### Step 2: Remove Test Objects (Optional - 1 minute)

If you don't need test objects:
1. **Select `Meta_Cube`** → Right-click → Delete
2. **Select `[BuildingBlock] Cube`** → Right-click → Delete

**Note:** These are just test objects. You can keep them if you want to test interactions.

### Step 3: Create Organization Containers (5 minutes)

1. **Create "Managers" GameObject:**
   - Right-click Hierarchy → Create Empty
   - Name: `Managers`
   - Position: (0, 0, 0)

2. **Create "AR Content" GameObject:**
   - Right-click Hierarchy → Create Empty
   - Name: `AR Content`
   - Position: (0, 0, 0)

3. **Create "Tools" GameObject:**
   - Right-click Hierarchy → Create Empty
   - Name: `Tools`
   - Position: (0, 0, 0)

4. **Create child objects:**
   - Under `AR Content`: Create Empty → Name: `PlateAnchor`
   - Under `AR Content`: Create Empty → Name: `FlowerContainer`
   - Under `Tools`: Add your scissors model (when ready)

### Step 4: Organize Building Blocks (2 minutes)

Your building blocks are already well organized:
- ✅ `MetaOrigin` with `XRCameraRig` - Camera setup (keep as-is)
- ✅ `[BuildingBlock] OVRInteraction` - Interaction system (keep as-is)
- ✅ `Meta_Passthrough`, `Meta_Real Hands`, etc. - Keep at root level
- ✅ `Poke Interaction` with nested `[BuildingBlock] ISDK_Pokelnter` - This is correct!

**Note:** Some building blocks are meant to be nested (like Poke Interaction containing ISDK_PokeInteractor). This is fine!

### Step 5: Add Manager Scripts (5 minutes)

1. **Select `Managers` GameObject**
2. **Add Component** → Add all manager scripts:
   - `GameStateManager`
   - `UIManager`
   - `ARManagerMetaSDK` (use this, not ARManager)
   - `PlateManager`
   - `FlowerManager`
   - `TrimmingManager`

---

## 📝 Quick Checklist

- [x] ✅ `MetaOrigin` with `XRCameraRig` is present (your Camera Rig)
- [x] ✅ `[BuildingBlock] OVRInteraction` is present
- [x] ✅ `Meta_Passthrough` is present
- [x] ✅ `Meta_Real Hands` is present
- [x] ✅ `Meta_Environment Raycast Manag` is present
- [x] ✅ `Poke Interaction` is present
- [ ] Remove test objects (`Meta_Cube`, `[BuildingBlock] Cube`) - Optional
- [ ] Create `Managers` GameObject
- [ ] Create `AR Content` GameObject with `PlateAnchor` and `FlowerContainer`
- [ ] Create `Tools` GameObject
- [ ] Add all manager scripts to `Managers`

---

## ⚠️ Important Notes

1. **Your setup is correct!** - `MetaOrigin` with `XRCameraRig` is your Camera Rig
2. **Some nesting is OK** - Building blocks like `Poke Interaction` containing `ISDK_PokeInteractor` are meant to be nested
3. **Keep building blocks at root** - Main building blocks should be at scene root level
4. **Use ARManagerMetaSDK** - Not ARManager (which is for AR Foundation)
5. **Test objects are optional** - Remove `Meta_Cube` and `[BuildingBlock] Cube` if not needed

---

## 🎨 Visual Organization Tips

### Group by Type:
- **Building Blocks** - Keep at root (all `[BuildingBlock]` items)
- **Managers** - One GameObject with all manager scripts
- **Content** - AR Content container with anchors/containers
- **UI** - Canvas and panels
- **Tools** - Scissors and other tools

### Naming Convention:
- Building Blocks: Keep as-is (`[BuildingBlock] Name`)
- Your GameObjects: Use PascalCase (`Managers`, `ARContent`, `PlateAnchor`)
- Scripts: Use PascalCase (`GameStateManager`, `ARManagerMetaSDK`)

---

## ✅ Final Result

After organization, your Hierarchy should look clean:
- Building blocks at top (easy to find)
- Your custom GameObjects organized below
- No conflicts between AR Foundation and Meta SDK
- Everything ready for your Ikebana app!

---

**Need help?** See `CAMERA_RIG_SETUP_GUIDE.md` for detailed setup instructions.

