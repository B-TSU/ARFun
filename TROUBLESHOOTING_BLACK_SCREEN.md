# Troubleshooting: Black Screen & Errors

## ⚠️ IMPORTANT: Editor vs Device

**If you're testing in the Unity Editor:**
- ❌ **Black screen is EXPECTED** - Passthrough doesn't work in editor!
- ❌ **AR features don't work in editor**
- ✅ **You MUST test on Quest 3 device to see AR**

**The black screen in editor is normal for AR apps!** You need a real device to see passthrough.

---

## Issues Found

1. **Black Screen** - Normal in editor, or XR not configured
2. **EnvironmentDepth errors** - Not supported in editor (disable it)
3. **EnvironmentRaycastManager errors** - Not supported in editor (disable it)
4. **HandGrabInstallationRoutine errors** - Missing Rigidbody (fix this!)

---

## 🔧 Fix 0: Enable OpenXR Features (IMPORTANT!)

Before enabling XR, make sure required OpenXR features are enabled:

1. **Edit → Project Settings → XR Plug-in Management → OpenXR**
2. **Check these features:**
   - ✅ **Meta Quest: Camera (Passthrough)** - For AR view
   - ✅ **Hand Tracking Subsystem** - For hand tracking
   - ✅ **Meta Quest: Raycasts** - For object placement
3. **Add Interaction Profile:**
   - Click **"+"** in "Enabled Interaction Profiles"
   - Add **"Hand Tracking Profile"**

**See `OPENXR_FEATURES_SETUP.md` for complete guide.**

---

## 🔧 Fix 1: Enable XR Plug-in Management

### Step 1: Open Project Settings
1. Go to **Edit → Project Settings**
2. Navigate to **XR Plug-in Management**

### Step 2: Enable Meta XR SDK
1. In **XR Plug-in Management**, find **Meta XR SDK**
2. ✅ **Check the box** to enable it
3. If you see **OpenXR**, you can also enable that (Meta XR SDK should work with it)

### Step 3: Verify Settings
- **Initialize XR on Startup**: Should be enabled
- **Meta XR SDK** should be checked

---

## 🔧 Fix 2: Disable Environment Depth (Not Needed)

Environment Depth is not needed for basic AR functionality and doesn't work in the editor.

### Option A: Disable in Scene
1. Find `Meta_Environment Raycast Manag` in Hierarchy
2. Look for **EnvironmentDepthManager** component
3. **Uncheck** the component (disable it)

### Option B: Remove from Building Block
1. Select `Meta_Environment Raycast Manag` GameObject
2. In Inspector, find **EnvironmentDepthManager**
3. Click the **three dots** (⋮) → **Remove Component**

**Note:** You can keep `EnvironmentRaycastManager` disabled for now - it's only needed for advanced surface detection.

---

## 🔧 Fix 3: Fix HandGrabInstallationRoutine (Missing Rigidbody)

The `[BuildingBlock] HandGrabInstallationRoutine` needs a Rigidbody component.

### Solution:
1. Select `[BuildingBlock] HandGrabInstallationRoutine` in Hierarchy
2. In Inspector, click **Add Component**
3. Search for **Rigidbody**
4. Add **Rigidbody** component
5. Configure Rigidbody:
   - **Is Kinematic**: ✅ Check this (for UI/interaction objects)
   - **Use Gravity**: ❌ Uncheck (not needed for UI)
   - **Is Trigger**: Leave as default

**Why?** GrabInteractable components require a Rigidbody to work properly.

---

## 🔧 Fix 4: Disable Environment Raycast Manager (For Editor)

If you're testing in the editor, Environment Raycast Manager won't work. You can disable it:

1. Select `Meta_Environment Raycast Manag` GameObject
2. In Inspector, find **EnvironmentRaycastManager** component
3. **Uncheck** the component (disable it)

**Note:** This will work on device, but for editor testing, you can disable it.

---

## 🎯 Quick Fix Checklist

- [ ] **Enable XR Plug-in Management** → Meta XR SDK checked
- [ ] **Disable EnvironmentDepthManager** (on Meta_Environment Raycast Manag)
- [ ] **Disable EnvironmentRaycastManager** (optional, for editor testing)
- [ ] **Add Rigidbody** to `[BuildingBlock] HandGrabInstallationRoutine`
  - Set **Is Kinematic** = true
  - Set **Use Gravity** = false

---

## 📝 Step-by-Step: Enable XR Provider

### Detailed Steps:

1. **Open Project Settings:**
   - Edit → Project Settings
   - Or: Ctrl+Shift+, (Windows) / Cmd+, (Mac)

2. **Navigate to XR Plug-in Management:**
   - Left sidebar → XR Plug-in Management

3. **Enable Provider:**
   - Find **"Meta XR SDK"** in the list
   - ✅ **Check the box** next to it
   - If you see **"OpenXR"**, you can enable that too

4. **Verify:**
   - **"Initialize XR on Startup"** should be checked
   - **"Meta XR SDK"** should be in the list and checked

5. **Restart Unity:**
   - Close and reopen Unity
   - This ensures XR is properly initialized

---

## 🎮 Testing in Editor vs Device

### Editor Testing:
- ❌ Environment Depth won't work
- ❌ Environment Raycast won't work (unless Link is set up)
- ✅ Basic AR features should work
- ✅ Hand tracking won't work (needs device)

### Device Testing (Quest):
- ✅ Everything should work
- ✅ Environment Depth works
- ✅ Environment Raycast works
- ✅ Hand tracking works

**For now, disable Environment Depth and Environment Raycast for editor testing.**

---

## ⚠️ CRITICAL: Are You Testing in Editor?

**If testing in Editor:**
- ❌ **Passthrough WILL NOT WORK** - Black screen is expected!
- ❌ **AR features don't work in editor**
- ✅ **Solution:** Test on actual Quest 3 device

**The black screen in editor is NORMAL for AR apps!** Passthrough requires a real device.

**See `FIX_BLACK_SCREEN.md` for detailed troubleshooting.**

---

## ⚠️ If Still Black Screen (On Device)

### Additional Checks:

1. **Check Camera:**
   - Verify `MetaOrigin` → `XRCameraRig` → `TrackingSpace` → `CenterEyeAnchor` has a Camera component
   - Camera should be enabled

2. **Check Passthrough:**
   - Verify `Meta_Passthrough` GameObject is active
   - Check if passthrough is enabled in OVRManager

3. **Check OVRManager:**
   - Select `MetaOrigin` → `XRCameraRig`
   - In Inspector, find **OVRManager** component
   - Verify settings are correct

4. **Check Build Settings:**
   - File → Build Settings
   - Platform should be **Android**
   - **Quest** should be selected as target device

---

## 🚀 Quick Fix Summary

**Most Important:**
1. ✅ Enable **Meta XR SDK** in Project Settings → XR Plug-in Management
2. ✅ Add **Rigidbody** to `[BuildingBlock] HandGrabInstallationRoutine` (Is Kinematic = true)
3. ✅ Disable **EnvironmentDepthManager** (doesn't work in editor)

**For Editor Testing:**
- Disable Environment Raycast Manager (optional)

**For Device:**
- Everything should work once XR is enabled

---

## 📚 Related Guides

- See `CAMERA_RIG_SETUP_GUIDE.md` for Camera Rig setup
- See `SIMPLE_META_BUILDING_BLOCKS.md` for building blocks overview

