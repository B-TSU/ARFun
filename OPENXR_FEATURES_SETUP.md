# OpenXR Features Setup for Meta Quest 3 - AR Ikebana

## 🎯 What You Need for Your AR Ikebana App

Based on your app requirements (passthrough, hand tracking, poke interaction, object placement), here's what to enable:

---

## ✅ Required Features (Check These)

### Already Checked (Keep These):
- ✅ **Meta Quest Support** - Required for Quest
- ✅ **Meta XR Feature** - Required for Meta features
- ✅ **Meta Quest: Session** - Required for Quest session
- ✅ **Composition Layers Support** - Required for rendering

### Need to Check (Add These):

1. **Meta Quest: Camera (Passthrough)** ⭐ **REQUIRED**
   - **Why:** Enables AR passthrough (seeing real world)
   - **For:** Your AR view where users see real world + virtual objects
   - **Status:** Currently unchecked - **CHECK THIS!**

2. **Hand Tracking Subsystem** ⭐ **REQUIRED**
   - **Why:** Enables hand tracking for Real Hands building block
   - **For:** Poke interaction, grab interaction, hand tracking
   - **Status:** Currently unchecked - **CHECK THIS!**

3. **Meta Quest: Raycasts** ⭐ **REQUIRED**
   - **Why:** Enables surface detection for object placement
   - **For:** Placing plates and flowers on real surfaces
   - **Status:** Currently unchecked - **CHECK THIS!**

---

## ⚠️ Optional Features (Recommended)

### Recommended to Check:

4. **Meta Quest: Planes** ⚠️ **RECOMMENDED**
   - **Why:** Better surface detection (tables, floors)
   - **For:** More accurate plate/flower placement
   - **Status:** Currently unchecked - **Consider checking**

5. **Meta Quest: Meshing** ⚠️ **OPTIONAL**
   - **Why:** 3D mesh of environment for better understanding
   - **For:** Advanced surface detection
   - **Status:** Currently unchecked - **Optional**

---

## ❌ Not Needed (Keep Unchecked)

These are fine to leave unchecked for your app:

- **D-Pad Binding** - For controller D-pad (not needed)
- **Foveated Rendering** - Performance optimization (optional)
- **Hand Interaction Poses** - Advanced hand poses (not needed)
- **Meta Hand Tracking Aim** - Advanced hand tracking (not needed)
- **Meta Quest: Anchors** - Persistent anchors (not needed for basic AR)
- **Meta Quest: Boundary Visibility** - Guardian boundary (not needed)
- **Meta Quest: Bounding Boxes** - Scene understanding (not needed)
- **Meta Quest: Colocation Discovery** - Multi-user (not needed)
- **Meta Quest: Display Utilities** - Display settings (not needed)
- **Meta XR Eye Tracked Foveation** - Eye tracking (Quest 3 Pro feature)

---

## 🎮 Enabled Interaction Profiles

### Current:
- ✅ **Oculus Touch Controller Profile** - Already added (good!)

### Add This:

**Hand Tracking Profile** ⭐ **REQUIRED**
- **Why:** Enables hand tracking input
- **For:** Real Hands building block, poke interaction
- **How to add:**
  1. Click the **"+"** button next to "Enabled Interaction Profiles"
  2. Select **"Hand Tracking Profile"** or **"Meta Hand Tracking"**
  3. This enables hand tracking as an input method

---

## 📋 Quick Checklist

### OpenXR Feature Groups - Check These:
- [x] Meta Quest Support (already checked)
- [x] Meta XR Feature (already checked)
- [x] Meta Quest: Session (already checked)
- [x] Composition Layers Support (already checked)
- [ ] **Meta Quest: Camera (Passthrough)** ← **CHECK THIS!**
- [ ] **Hand Tracking Subsystem** ← **CHECK THIS!**
- [ ] **Meta Quest: Raycasts** ← **CHECK THIS!**
- [ ] Meta Quest: Planes (optional, recommended)
- [ ] Meta Quest: Meshing (optional)

### Enabled Interaction Profiles - Add:
- [x] Oculus Touch Controller Profile (already added)
- [ ] **Hand Tracking Profile** ← **ADD THIS!**

---

## 🎯 Step-by-Step Instructions

### Step 1: Enable Required Features

1. **Meta Quest: Camera (Passthrough):**
   - Find "Meta Quest: Camera (Passthrough)" in the list
   - ✅ **Check the box**
   - This enables AR passthrough

2. **Hand Tracking Subsystem:**
   - Find "Hand Tracking Subsystem" in the list
   - ✅ **Check the box**
   - This enables hand tracking

3. **Meta Quest: Raycasts:**
   - Find "Meta Quest: Raycasts" in the list
   - ✅ **Check the box**
   - This enables surface detection

### Step 2: Add Hand Tracking Profile

1. In **"Enabled Interaction Profiles"** section
2. Click the **"+"** button
3. Look for:
   - **"Hand Tracking Profile"** OR
   - **"Meta Hand Tracking"** OR
   - **"Hand Tracking"**
4. Select and add it

### Step 3: Optional - Enable Planes

1. Find "Meta Quest: Planes" in the list
2. ✅ **Check the box** (recommended for better surface detection)

---

## ⚠️ Important Notes

### About Warnings:
- **Yellow warning triangles** are usually just warnings, not errors
- They might indicate the feature needs additional setup
- For basic functionality, you can ignore most warnings

### About Passthrough:
- **Meta Quest: Camera (Passthrough)** is essential for AR
- Without it, you won't see the real world
- This is what makes your app AR instead of VR

### About Hand Tracking:
- **Hand Tracking Subsystem** + **Hand Tracking Profile** = Full hand tracking
- Both are needed for Real Hands building block to work
- Without these, hand tracking won't work

### About Raycasts:
- **Meta Quest: Raycasts** enables Environment Raycast Manager
- This is needed for placing objects on surfaces
- Without it, you can't detect where to place plates/flowers

---

## 🚀 After Enabling Features

1. **Save your project** (Ctrl+S / Cmd+S)
2. **Restart Unity** (recommended)
3. **Test in Play mode** or on device

The black screen issue should be resolved once:
- XR Plug-in Management is enabled
- Required OpenXR features are checked
- Hand Tracking Profile is added

---

## 📝 Summary

**Must Check:**
- ✅ Meta Quest: Camera (Passthrough)
- ✅ Hand Tracking Subsystem
- ✅ Meta Quest: Raycasts

**Must Add:**
- ✅ Hand Tracking Profile (in Interaction Profiles)

**Optional:**
- Meta Quest: Planes (recommended)
- Meta Quest: Meshing (optional)

**Everything else can stay unchecked!**

