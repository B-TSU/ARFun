# Simple Meta Building Blocks Guide - Quick Drag & Drop

## 🎯 Using Meta Building Blocks (Camera Rig Approach)

This guide covers using Meta's building blocks (Grab Interaction, Real Hands, Poke Interaction) with the **Camera Rig approach**.

---

## ✅ What to Keep/Add (Meta Building Blocks Approach)

### 1. **`[BuildingBlock] Camera Rig`** - ✅ REQUIRED!
   - Provides tracking and camera
   - Required for all Meta building blocks
   - **Keep this - it's essential!**

### 2. **`[BuildingBlock] Real Hands`** - ✅ REQUIRED!
   - Provides hand tracking
   - Already in your scene
   - **Keep this!**

### 3. **`[BuildingBlock] OVRInteraction`** - ✅ REQUIRED!
   - Provides grab and poke interactions
   - You have this selected in your screenshot
   - **Keep this!**

### 4. **`[BuildingBlock] Passthrough`** - ✅ REQUIRED!
   - Provides AR passthrough view
   - **Keep this!**

### 5. **`[BuildingBlock] Ray Interaction`** - ✅ RECOMMENDED!
   - Provides ray-based interaction for pointing/selecting
   - Useful for UI interaction and object selection
   - **Add this if not already present!**

---

## ❌ What to Remove/Disable (Meta Building Blocks Approach)

### 1. **`XROrigin`** - ❌ Remove/Disable
   - **Why**: Conflicts with Camera Rig
   - **Why**: Meta building blocks require Camera Rig, not XROrigin
   - **Action**: Right-click → Disable (or Delete)

### 2. **`Main Camera` (under XROrigin)** - ❌ Remove/Disable
   - **Why**: Camera Rig has its own camera (CenterEyeAnchor)
   - **Action**: Will be removed when you remove XROrigin

### 3. **AR Foundation Components** - ❌ Remove/Disable
   - AR_PlaneManager, AR_RaycastManager, etc.
   - **Why**: Not compatible with Camera Rig approach
   - **Action**: Will be removed with XROrigin

---

## 🎯 Quick Setup Checklist (Meta Building Blocks)

### Step 1: Clean Up (2 minutes)
- [ ] Disable/Remove `XROrigin` (conflicts with Camera Rig)
- [ ] Keep `[BuildingBlock] Camera Rig` ✅
- [ ] Keep `[BuildingBlock] Real Hands` ✅
- [ ] Keep `[BuildingBlock] OVRInteraction` ✅
- [ ] Keep `[BuildingBlock] Passthrough` ✅

### Step 2: Verify Building Blocks
- [ ] Check `[BuildingBlock] OVRInteraction` dependencies in Inspector
- [ ] Verify Camera Rig is installed (should show green checkmark)
- [ ] Verify Real Hands is installed
- [ ] Verify Grab Interaction is installed
- [ ] Verify Poke Interaction is installed

### Step 3: Add Ray Interaction (Optional but Recommended)
- [ ] Add `[BuildingBlock] Ray Interaction` for ray-based interactions
- [ ] Useful for UI interaction and object selection

### Step 4: Add Your App Components
- [ ] Create "Managers" GameObject → Add manager scripts
- [ ] Create "AR Content" → PlateAnchor, FlowerContainer
- [ ] Create UI Canvas with panels

---

## 📦 What Building Blocks Are Available?

If you want to see what Meta building blocks exist:
1. In Unity, look for **Meta XR SDK** menu items
2. Or check: **GameObject → XR** menu
3. Common building blocks:
   - Camera Rig (OVRCameraRig) - ❌ Don't need (use XROrigin)
   - Passthrough - ❌ Don't need (AR Foundation handles it)
   - Hand Tracking - ⚠️ Optional (use XR Hands instead)

---

## 🎯 Final Answer: What to Drag Into Hierarchy (Meta Building Blocks)

### Keep (Already Have):
- ✅ **`[BuildingBlock] Camera Rig`** - Required for all building blocks!
- ✅ **`[BuildingBlock] Real Hands`** - Hand tracking
- ✅ **`[BuildingBlock] OVRInteraction`** - Grab & poke interactions
- ✅ **`[BuildingBlock] Passthrough`** - AR view

### Drag Into Hierarchy (If Missing):
1. **`[BuildingBlock] Ray Interaction`** (Optional but Recommended)
   - Provides ray-based interaction for pointing/selecting
   - Right-click Hierarchy → Look for building blocks menu
   - Or use Meta XR Tools → Building Blocks

2. **Your Custom Components**
   - Managers GameObject
   - AR Content container
   - UI Canvas

### Remove:
- ❌ **XROrigin** - Conflicts with Camera Rig

---

## ⚠️ Important Notes

1. **You DON'T need Meta building blocks** when using AR Foundation
2. **AR Foundation handles passthrough automatically** - no separate component
3. **XROrigin is all you need** for AR setup
4. **Building blocks are for pure Meta SDK** - you're using AR Foundation (better!)

---

## Quick Reference

| Component | Need It? | Why |
|-----------|----------|-----|
| [BuildingBlock] Camera Rig | ✅ YES | **REQUIRED** - All building blocks depend on it! |
| [BuildingBlock] Real Hands | ✅ YES | Hand tracking for interactions |
| [BuildingBlock] OVRInteraction | ✅ YES | Provides grab & poke interactions |
| [BuildingBlock] Passthrough | ✅ YES | AR passthrough view |
| [BuildingBlock] Ray Interaction | ⚠️ RECOMMENDED | Ray-based interaction for pointing/selecting |
| XROrigin (AR Foundation) | ❌ NO | Conflicts with Camera Rig - remove it! |
| Main Camera (under XROrigin) | ❌ NO | Camera Rig has its own camera |

---

**Bottom Line (Meta Building Blocks Approach)**: 
- ✅ **KEEP Camera Rig** - Required for all building blocks!
- ✅ **KEEP Real Hands** - Hand tracking
- ✅ **KEEP OVRInteraction** - Grab & poke interactions
- ✅ **KEEP Passthrough** - AR view
- ❌ **Remove XROrigin** - Conflicts with Camera Rig!
- ✅ **Add Ray Interaction** - For ray-based interactions (optional)

**Your setup is correct! Just remove XROrigin to avoid conflicts.** 🎉

**For more details**, see `CAMERA_RIG_SETUP_GUIDE.md`.

