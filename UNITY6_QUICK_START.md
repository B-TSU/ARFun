# Unity 6.0 Quick Start Guide for AR Ikebana

## Package Installation (First Step)

Before creating scene building blocks, install these packages via **Window → Package Manager**:

### Required Packages:
1. **Meta XR SDK** 
   - Already installed via Building Blocks
   - Provides Camera Rig, Hand Tracking, Interactions

### Already Installed:
- ✅ Meta XR SDK (com.meta.xr.sdk.all)
- ✅ Universal Render Pipeline (URP)
- ✅ Input System
- ✅ XR Management

---

## Quick Setup Steps

### 1. Verify Building Blocks (2 minutes)
- Check that `[BuildingBlock] Camera Rig` is in scene
- Check that `[BuildingBlock] Real Hands` is in scene
- Check that `[BuildingBlock] OVRInteraction` is in scene
- Check that `[BuildingBlock] Passthrough` is in scene
- Remove/Disable `XROrigin` if present (conflicts with Camera Rig)

### 2. Create Scene Building Blocks (15 minutes)
Follow the detailed guide in `SCENE_HIERARCHY_GUIDE.md`:
- Create Managers GameObject
- Create AR Content containers
- Create UI Canvas with all panels
- Create World Space Canvas

### 3. Add Manager Scripts (5 minutes)
- Select `Managers` GameObject
- Add all manager scripts as components
- Configure references in inspector

### 4. Verify Setup (1 minute)
- Add `SceneSetupVerifier` script to any GameObject
- Click "Verify Setup" in inspector
- Fix any missing components

---

## Unity 6 Specific Notes

### XR Origin Creation
In Unity 6, when you create **XR → XR Origin (AR Foundation)**, it automatically:
- Creates the XR Origin GameObject
- Adds XROrigin component
- Creates Main Camera as child
- Creates Left/Right Controller GameObjects

**You still need to manually add:**
- ARPlaneManager component
- ARRaycastManager component  
- ARAnchorManager component

### UI System
Unity 6 uses **TextMeshPro** by default for UI text. When creating UI elements:
- Use **Text - TextMeshPro** (not legacy Text)
- First-time setup may import TMP Essentials (click Import if prompted)

### Input System
Your project uses the **New Input System**. For AR interactions:
- Use `Input.GetMouseButtonDown(0)` for editor testing
- Use `Input.touchCount` for mobile
- For Quest controllers, use Meta XR SDK input methods

### AR Foundation in Unity 6
- AR Foundation 6.x is compatible with Unity 6
- Make sure to configure **XR Plug-in Management**:
  - Edit → Project Settings → XR Plug-in Management
  - Enable **ARCore** (Android) or **ARKit** (iOS)

---

## Common Issues

### Black Screen in Play Mode

**Problem:** Screen turns black when entering Play mode

**Solution:**
1. **Enable XR Plug-in Management:**
   - Edit → Project Settings → XR Plug-in Management
   - ✅ Check **Meta XR SDK**
   - ✅ Check **Initialize XR on Startup**

2. **Disable Environment Depth (for editor):**
   - Select `Meta_Environment Raycast Manag` in Hierarchy
   - Disable **EnvironmentDepthManager** component
   - (This doesn't work in editor, only on device)

3. **Fix HandGrabInstallationRoutine:**
   - Select `[BuildingBlock] HandGrabInstallationRoutine`
   - Add Component → **Rigidbody**
   - Set **Is Kinematic** = true
   - Set **Use Gravity** = false

**See `TROUBLESHOOTING_BLACK_SCREEN.md` for detailed troubleshooting.**

---

## Common Issues (Other) & Solutions

### Issue: "ARPlaneManager not found"
**Solution**: 
1. Select XR Origin
2. Add Component → Search "AR Plane Manager"
3. Configure Plane Detection Mode

### Issue: "Canvas not showing UI"
**Solution**:
- Check Canvas Render Mode is "Screen Space - Overlay"
- Check Canvas Scaler settings
- Ensure EventSystem exists in scene

### Issue: "AR not working in Play mode"
**Solution**:
- AR requires a physical device (Quest, phone, etc.)
- Editor simulation is limited
- Test on actual device for full functionality

### Issue: "Scripts not compiling"
**Solution**:
- Check that all manager scripts are in `Assets/Scripts/Managers/`
- Ensure no syntax errors (check Console)
- Reimport scripts if needed

---

## Testing Checklist

Before moving to next phase, verify:

- [ ] All packages installed
- [ ] XR Origin created with AR components
- [ ] Managers GameObject with all scripts
- [ ] UI Canvas with all panels
- [ ] SceneSetupVerifier reports no errors
- [ ] Can enter Play mode without errors
- [ ] Main Menu panel is visible
- [ ] Other panels are hidden

---

## Next Phase: Prefabs & Interactions

After building blocks are ready:
1. Create plate prefabs (3D models)
2. Create flower prefabs (3D models)
3. Wire up button OnClick events
4. Test AR plane detection
5. Implement plate placement
6. Implement flower spawning

---

## File Reference

- **Detailed Building Blocks**: `UNITY6_SCENE_BUILDING_BLOCKS.md`
- **Scene Hierarchy Guide**: `SCENE_HIERARCHY_GUIDE.md`
- **Setup Checklist**: `SETUP_CHECKLIST.md`
- **Scripts Documentation**: `Assets/Scripts/README.md`

---

## Need Help?

1. Run `SceneSetupVerifier` to check what's missing
2. Check Console for error messages
3. Verify all manager references are assigned
4. Ensure AR Foundation packages are installed
5. Test on actual device (AR doesn't work well in editor)

