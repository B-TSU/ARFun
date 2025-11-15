# Plate Components Guide

## 🎯 What Components Does the Plate Need?

### ✅ REQUIRED: Collider

**Yes, you need a Collider!**

The `FlowerPlacement` script uses `Physics.OverlapSphere()` to detect plates:

```csharp
Collider[] nearbyColliders = Physics.OverlapSphere(worldPosition, 0.2f, plateLayer);
```

**Without a Collider, the plate cannot be detected!**

**Recommended Collider Type:**
- **Box Collider** - Good for flat plates
- **Mesh Collider** - Matches plate shape exactly (more accurate but heavier)
- **Capsule Collider** - Good for round plates

**Collider Settings:**
- **Is Trigger:** `false` (unless you need trigger detection)
- **Size:** Match your plate model

---

### ❌ NOT REQUIRED: Rigidbody

**No, you typically DON'T need a Rigidbody for plates!**

**Rigidbody is only needed if:**
- ✅ Plate needs to be **grabbable/moveable** (with hands/controllers)
- ✅ Plate needs to be affected by **physics forces**
- ✅ Plate needs to **fall or move** with physics

**For static plates (most common):**
- ❌ **No Rigidbody needed** - Plate stays in place
- ✅ **Just Collider** - For detection only

---

## 📋 Plate Component Checklist

### Essential Components:
- [ ] **MeshFilter** - Contains the plate mesh (usually auto-added)
- [ ] **MeshRenderer** - Renders the plate (usually auto-added)
- [ ] **Collider** ⭐ **REQUIRED** - For flower placement detection
  - Type: Box, Mesh, or Capsule Collider
  - Is Trigger: `false`

### Optional Components:
- [ ] **Rigidbody** - Only if plate needs to be moveable/grabbable
  - If added: Is Kinematic = `true` (for AR placement)
  - If added: Use Gravity = `false`
- [ ] **GrabInteractable** - Only if users should be able to grab/move the plate
- [ ] **Tag:** `Plate` (for organization)
- [ ] **Layer:** `Plate` (for detection - see PLATE_LAYER_SETUP.md)

---

## 🎯 Two Scenarios

### Scenario 1: Static Plate (Most Common) ⭐

**Plate stays in place, flowers are placed on it:**

**Components Needed:**
- ✅ MeshFilter
- ✅ MeshRenderer
- ✅ Collider (Box/Mesh/Capsule)
- ❌ No Rigidbody
- ❌ No GrabInteractable

**Use Case:** Standard AR placement where plate is anchored and flowers are placed on it.

---

### Scenario 2: Moveable/Grabbable Plate

**Users can grab and move the plate:**

**Components Needed:**
- ✅ MeshFilter
- ✅ MeshRenderer
- ✅ Collider
- ✅ Rigidbody (Is Kinematic = `true`, Use Gravity = `false`)
- ✅ GrabInteractable (Meta XR SDK)

**Use Case:** Interactive plate that users can reposition.

---

## 🔍 How It Works

### Flower Detection on Plate:

1. **FlowerPlacement script** calls `Physics.OverlapSphere()`
2. **Unity searches for Colliders** on the "Plate" layer
3. **If Collider found** → Flower snaps to plate
4. **If no Collider** → Flower can't detect plate

**The Collider is what makes the plate "detectable" by physics queries!**

---

## ✅ Quick Setup for Static Plate

1. **Select plate prefab**
2. **Add Component → Box Collider** (or Mesh/Capsule)
3. **Adjust Collider size** to match plate
4. **Set Layer to "Plate"** (if using layers)
5. **Done!** No Rigidbody needed.

---

## 🎯 Summary

**For your plate:**

- ✅ **Collider** - REQUIRED (for detection)
- ❌ **Rigidbody** - NOT needed (unless plate is moveable)
- ❌ **Physics stuff** - NOT needed (unless plate needs physics)

**Just add a Collider and you're good to go!** 🍽️

