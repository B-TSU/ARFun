# Troubleshooting: Meta XR Camera Rig Falling Down in Unity

## Problem
The Meta XR camera rig appears to be "falling down" or sinking through the floor in Unity.

## Common Causes & Solutions

### ✅ Solution 1: Check for Rigidbody Component (MOST COMMON)

**The camera rig should NOT have a Rigidbody component with gravity enabled.**

1. Select the **XRCameraRig** GameObject in the Hierarchy
2. Check the Inspector for a **Rigidbody** component
3. If present:
   - **Option A (Recommended):** Remove the Rigidbody component entirely
   - **Option B:** If you must keep it:
     - ✅ Check **Is Kinematic**
     - ✅ Uncheck **Use Gravity**

**Why?** The camera rig's position is controlled by Meta's tracking system, not Unity's physics. A Rigidbody with gravity will cause it to fall.

---

### ✅ Solution 2: Check CharacterController Component

**The camera rig should NOT have a CharacterController component.**

1. Select the **XRCameraRig** GameObject
2. Check for a **CharacterController** component
3. If present, **remove it**

**Why?** CharacterController can interfere with XR tracking and cause positioning issues.

---

### ✅ Solution 3: Verify Tracking Origin Settings

1. Select the **XRCameraRig** GameObject
2. In the Inspector, find the **OVR Manager** component
3. Check **Tracking Origin Type**:
   - **Floor Level** (value 1) - Camera starts at floor height
   - **Eye Level** (value 0) - Camera starts at eye height
4. Ensure it matches your scene setup

**Current Setting:** Your scene shows `_trackingOriginType: 1` (Floor Level)

---

### ✅ Solution 4: Reset Camera Rig Transform Position

The camera rig's Transform position should typically be at (0, 0, 0) relative to its parent.

1. Select the **XRCameraRig** GameObject
2. In the Inspector, check the **Transform** component
3. If the position is not (0, 0, 0):
   - Set **Position** to: `X: 0, Y: 0, Z: 0`
   - The tracking system will handle positioning

**Current Issue:** Your XRCameraRig has position `{x: -0.36060452, y: -0.0746943, z: 0.41289046}` with a negative Y value, which could cause it to appear below the floor.

---

### ✅ Solution 5: Check Parent GameObject (MetaOrigin)

1. Select the **MetaOrigin** GameObject (parent of XRCameraRig)
2. Verify it does NOT have:
   - ❌ Rigidbody component
   - ❌ CharacterController component
3. The MetaOrigin Transform position can be adjusted for scene setup, but should not have physics components

---

### ✅ Solution 6: Verify OVRCameraRig Settings

1. Select the **XRCameraRig** GameObject
2. Find the **OVR Camera Rig** component
3. Verify these settings:
   - ✅ **Use Position Tracking**: Enabled
   - ✅ **Use Rotation Tracking**: Enabled
   - ✅ **Reset Tracker On Load**: Usually disabled (unless you want to reset on scene load)

---

### ✅ Solution 7: Check for Collision Issues

1. Ensure the camera rig is NOT colliding with other objects
2. Check if any child objects of the camera rig have Colliders that might be causing issues
3. The camera rig itself should not have Colliders (unless specifically needed)

---

### ✅ Solution 8: Verify Scene Setup

**Proper Hierarchy:**
```
MetaOrigin (or XR Origin)
└── XRCameraRig
    ├── TrackingSpace
    │   ├── LeftEyeAnchor
    │   ├── RightEyeAnchor
    │   └── CenterEyeAnchor
    └── (other tracking anchors)
```

**Components on XRCameraRig:**
- ✅ OVR Input Module (or similar)
- ✅ OVR Manager
- ✅ OVR Camera Rig
- ✅ Transform
- ❌ **NO Rigidbody**
- ❌ **NO CharacterController**

---

## Quick Fix Checklist

- [ ] Remove Rigidbody from XRCameraRig (if present)
- [ ] Remove CharacterController from XRCameraRig (if present)
- [ ] Set XRCameraRig Transform position to (0, 0, 0)
- [ ] Verify MetaOrigin has no physics components
- [ ] Check Tracking Origin Type in OVR Manager
- [ ] Ensure Use Position Tracking is enabled
- [ ] Verify no collision issues

---

## Testing

After applying fixes:

1. **Enter Play Mode**
2. **Put on your Quest headset**
3. **Check if the camera rig stays at the correct height**
4. **Move around** - the camera should track your head movement smoothly

---

## Additional Notes

- The camera rig's position is controlled by **Meta's tracking system**, not Unity's physics
- The rig should be positioned by the **OVRCameraRig** component based on headset tracking
- Any manual position changes or physics interactions will interfere with tracking
- If you need to adjust the starting height, modify the **MetaOrigin** Transform, not the XRCameraRig

---

## Still Having Issues?

If the problem persists:

1. **Check Unity Console** for any errors or warnings
2. **Verify Meta XR SDK** is properly installed and up to date
3. **Check Quest headset** - ensure tracking is working (guardian system, etc.)
4. **Try resetting tracking** - In Quest, go to Settings > Guardian > Reset Guardian

---

## Related Files

- Scene: `Assets/Scenes/IkebanAR.unity`
- XRCameraRig GameObject ID: `907922978`
- MetaOrigin GameObject ID: `968295607`

