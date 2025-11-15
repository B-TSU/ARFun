# Build and Run Guide for Meta Quest

## 🚀 Quick Start: Build and Run

**Build and Run** is an alternative to Play mode that builds an APK and installs it on your Quest automatically. You don't need ADB in PATH for this!

---

## ✅ Prerequisites

Before building, make sure you have:

- [ ] **Quest connected via USB** (with Quest Link enabled)
- [ ] **USB Debugging enabled** on Quest (Settings → Developer)
- [ ] **Build Settings → Platform is Android**
- [ ] **XR Plug-in Management → Meta XR SDK enabled**
- [ ] **OpenXR features configured** (see `OPENXR_FEATURES_SETUP.md`)

---

## 📦 Step-by-Step: Build and Run

### Step 1: Configure Build Settings

1. **File → Build Settings**
2. **Select "Android"** platform
3. **If not already selected:**
   - Click **"Switch Platform"** (may take a few minutes)
4. **Check "Run Device"** dropdown at bottom:
   - Your Quest should appear here
   - If not, connect Quest and click **"Refresh"**

### Step 2: Player Settings (Important!)

1. **Click "Player Settings"** button (bottom left of Build Settings)
2. **Or go to:** Edit → Project Settings → Player
3. **Android Settings:**
   - **Package Name:** Should be unique (e.g., `com.yourname.ikebanar`)
   - **Minimum API Level:** Android 7.0 (API 24) or higher
   - **Target API Level:** Latest (API 33+ recommended)
   - **Scripting Backend:** IL2CPP (recommended for Quest)
   - **Target Architectures:** ARM64 (required for Quest)

### Step 3: Build and Run

1. **In Build Settings window:**
   - Make sure your scene is added (check the checkbox)
   - Click **"Build and Run"** button
2. **Choose save location:**
   - Select a folder to save the APK (e.g., create `Builds` folder)
   - Click **"Save"**
3. **Wait for build:**
   - First build takes 5-10 minutes
   - Subsequent builds are faster (2-5 minutes)
   - Unity will show progress in bottom-right corner

### Step 4: Automatic Installation

- Unity will automatically:
  1. Build the APK
  2. Install it on your Quest (via Unity's bundled ADB)
  3. Launch the app on Quest

**If installation fails:**
- Check that Quest is connected and USB debugging is enabled
- Accept any USB debugging prompts on Quest
- Check Unity Console for error messages

---

## 🎮 After Build and Run

### On Your Quest:

1. **The app should launch automatically**
2. **If not, find it in:**
   - Quest menu → **Unknown Sources** (if first time)
   - Or: **Library → Apps → Unknown Sources → IkebanAR**

### To Run Again (Without Rebuilding):

**You DON'T need to rebuild every time!** After the first build:

- **Option 1: Launch from Quest Menu** ⭐ **FASTEST**
  - Find **IkebanAR** in Quest menu
  - Click to launch (no rebuild needed!)
  - Use this when you just want to test, no code changes

- **Option 2: Build and Run Again** (only when you make changes)
  - Use this when you've changed code, scenes, or settings
  - Rebuilds APK and installs updated version

- **Option 3: Use ADB to Launch** (if ADB is installed)
  - `adb shell am start -n com.yourname.ikebanar/com.unity3d.player.UnityPlayerActivity`
  - Launches existing app without rebuilding

### When Do You Need to Rebuild?

**Rebuild (Build and Run) when:**
- ✅ You changed C# scripts
- ✅ You modified the scene
- ✅ You changed project settings
- ✅ You added/removed assets
- ✅ You updated packages

**Don't rebuild when:**
- ❌ You just want to test the app again
- ❌ You want to try different interactions
- ❌ You're just exploring the app
- ❌ No code/scene changes were made

---

## ⚙️ Build Settings Checklist

Before building, verify:

- [ ] **Platform:** Android
- [ ] **Scene added:** Your main scene is in "Scenes In Build" list
- [ ] **Player Settings → Android:**
  - [ ] Package Name is set (unique)
  - [ ] Minimum API Level: 24+
  - [ ] Target API Level: 33+
  - [ ] Scripting Backend: IL2CPP
  - [ ] Target Architectures: ARM64 ✅
- [ ] **XR Settings:**
  - [ ] XR Plug-in Management → Meta XR SDK enabled
  - [ ] OpenXR features configured (Passthrough, Raycasts, etc.)

---

## 🔧 Troubleshooting Build and Run

### Issue: "No devices found"

**Solution:**
1. Connect Quest via USB
2. Enable USB Debugging on Quest
3. Accept USB debugging prompt on Quest
4. Click **"Refresh"** in Build Settings
5. Check "Run Device" dropdown shows Quest

### Issue: "Build failed" or errors

**Check:**
1. **Unity Console** for specific error messages
2. **Player Settings** → Package Name is valid (no spaces, lowercase)
3. **Build Settings** → Scene is added and enabled
4. **XR Settings** → Meta XR SDK is enabled

### Issue: "Installation failed"

**Solution:**
1. Check Quest is connected and unlocked
2. Accept USB debugging prompt on Quest
3. Check Unity Console for ADB errors
4. Try unplugging and replugging USB cable

### Issue: App launches but shows black screen

**This is normal for AR apps!** See `TROUBLESHOOTING_BLACK_SCREEN.md`:
- Black screen in editor is expected
- On device, you should see passthrough AR
- If still black on device, check OpenXR features are enabled

### Issue: Build takes too long

**First build is always slow:**
- 5-10 minutes is normal for first build
- Subsequent builds are faster (2-5 minutes)
- IL2CPP compilation takes time

---

## 💡 Tips for Faster Development

### Workflow Strategy:

1. **First Time:** Build and Run (takes 5-10 minutes)
2. **Testing:** Launch from Quest menu (instant, no rebuild)
3. **After Changes:** Build and Run again (only when needed)
4. **Quick Iteration:** Make changes → Build and Run → Test → Repeat

### Development Build:

1. **Build Settings → Development Build** (checkbox)
2. **Script Debugging** (optional, for debugging)
3. **Autoconnect Profiler** (optional)
4. Builds faster, includes debugging tools

### Incremental Builds:

- Unity caches some build data
- Only changed files are rebuilt
- Subsequent builds are faster (2-5 minutes vs 5-10 minutes)

### Build Once, Run Many:

- **After first Build and Run:**
  - ✅ Launch app from Quest menu (no rebuild needed) - **Use this most!**
  - ✅ Test multiple times without rebuilding
  - ✅ Only rebuild when you make actual changes

---

## 📊 Build vs Play Mode Comparison

| Feature | Play Mode | Build and Run | Launch from Quest |
|---------|-----------|---------------|-------------------|
| **Speed** | Fast (instant) | Slow (5-10 min first, 2-5 min after) | Instant (no build) |
| **ADB in PATH** | Required | Not required | Not needed |
| **Testing** | Quick iteration | Full build test | Test existing build |
| **Debugging** | Full Unity debugger | Limited (unless Dev Build) | None |
| **AR Features** | Limited in editor | Full on device | Full on device |
| **Best For** | Quick testing | After code changes | Testing without changes |
| **When to Use** | Every code change | After making changes | Just want to test app |

---

## 🎯 Recommended Workflow

1. **Development:**
   - Use **Play Mode** for quick testing (if ADB works)
   - Or use **Build and Run** every few changes

2. **Testing:**
   - Use **Build and Run** to test on actual device
   - Test AR features, passthrough, hand tracking

3. **Deployment:**
   - Use **Build** (not Build and Run) for final APK
   - Install manually or distribute

---

## 📚 Related Guides

- **`TROUBLESHOOTING_QUEST_CONNECTION.md`** - For connection issues
- **`TROUBLESHOOTING_BLACK_SCREEN.md`** - For black screen issues
- **`OPENXR_FEATURES_SETUP.md`** - For OpenXR configuration
- **`SETUP_CHECKLIST.md`** - For general setup verification

---

## ✅ Quick Checklist Before Building

- [ ] Quest connected via USB
- [ ] USB Debugging enabled on Quest
- [ ] Build Settings → Platform: Android
- [ ] Scene added to build
- [ ] Player Settings → Package Name set
- [ ] Player Settings → Target Architectures: ARM64
- [ ] XR Plug-in Management → Meta XR SDK enabled
- [ ] OpenXR features configured

**Ready to build?** File → Build Settings → Build and Run! 🚀

