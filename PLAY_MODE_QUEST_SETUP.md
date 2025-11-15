# Play Mode on Quest Setup - Quick Guide

## 🎯 Goal: Press Play → Test on Quest (No Build Needed!)

This guide will help you set up Unity Play mode to automatically deploy to your Quest device.

---

## ✅ Prerequisites Checklist

Before starting, make sure you have:

- [ ] Quest connected via USB cable
- [ ] Quest Link enabled on Quest device
- [ ] Developer Mode enabled on Quest
- [ ] USB Debugging enabled on Quest
- [ ] USB debugging prompt accepted on Quest

---

## 🔧 Step 1: Install ADB (Android Debug Bridge)

**This is REQUIRED for Play mode to work!**

### Option A: Download Platform Tools (Recommended)

1. **Download Android SDK Platform Tools:**
   - Go to: https://dl.google.com/android/repository/platform-tools-latest-windows.zip
   - Or: https://developer.android.com/tools/releases/platform-tools

2. **Extract the ZIP:**
   - Extract to a folder (e.g., `C:\platform-tools`)
   - You should see `adb.exe` in that folder

3. **Add to PATH:**
   - Press **Windows Key + X** → **System** → **Advanced system settings**
   - Click **Environment Variables**
   - Under **System variables**, find **Path** → Click **Edit**
   - Click **New** → Add: `C:\platform-tools` (or your folder path)
   - Click **OK** on all windows

4. **Verify Installation:**
   - **Close ALL Command Prompt/PowerShell windows**
   - Open **NEW** Command Prompt or PowerShell
   - Type: `adb version`
   - You should see ADB version (not "not recognized" error)

---

## 🔧 Step 2: Connect Quest and Verify ADB

1. **Connect Quest to PC via USB**
2. **Put on Quest headset** and accept USB debugging prompt
3. **Check "Always allow from this computer"**
4. **Test ADB connection:**
   - Open Command Prompt/PowerShell
   - Type: `adb devices`
   - You should see your Quest listed:
     ```
     List of devices attached
     ABC123XYZ    device
     ```

**If you see "unauthorized":**
- Put on Quest headset
- Accept USB debugging prompt
- Check "Always allow from this computer"

**If you see "offline":**
- Unplug and replug USB cable
- Run: `adb kill-server && adb start-server`
- Try `adb devices` again

---

## 🔧 Step 3: Configure Unity Build Settings

1. **File → Build Settings**
2. **Select "Android"** platform
3. **If not already selected:**
   - Click **"Switch Platform"** (may take a few minutes)
4. **Check "Run Device" dropdown** at bottom:
   - Your Quest should appear here
   - If not, click **"Refresh"**

---

## 🔧 Step 4: Configure Unity Editor Preferences

1. **Edit → Preferences → External Tools** (Windows)
2. **Android section:**
   - **Android SDK Tools:** Should point to your Android SDK folder
     - If you installed Platform Tools to `C:\platform-tools`, you can leave this blank or point to that folder
   - **JDK:** Should be set (required for Android builds)
   - **NDK:** Optional, but recommended

**Note:** Unity will use ADB from PATH if Android SDK Tools is not set, so having ADB in PATH is important!

---

## 🔧 Step 5: Configure XR Settings

1. **Edit → Project Settings → XR Plug-in Management**
2. **Enable "Meta XR SDK"** (check the box)
3. **Enable "Initialize XR on Startup"**
4. **For Android tab:**
   - Verify **Meta XR SDK** is checked

---

## 🎮 Step 6: Use Play Mode

1. **Connect Quest via USB**
2. **Verify Quest appears in Build Settings → Run Device dropdown**
3. **In Unity Editor, look at the top toolbar:**
   - Next to the **Play button**, there should be a **device selector dropdown**
   - Click it → Your Quest should appear
   - **Select your Quest** from the dropdown
4. **Press Play!**
   - Unity will automatically deploy to Quest
   - App should launch on Quest
   - You can test in real-time!

---

## ⚠️ Troubleshooting

### Issue: Quest doesn't appear in device selector

**Solutions:**
1. **Check ADB connection:**
   - Run `adb devices` in terminal
   - Quest should be listed
2. **Restart Unity** after connecting Quest
3. **Check Build Settings → Run Device** dropdown
4. **Verify USB debugging is enabled** on Quest

### Issue: "No devices found" when pressing Play

**Solutions:**
1. **Accept USB debugging prompt** on Quest (put on headset)
2. **Check ADB:** `adb devices` should show Quest
3. **Restart ADB server:**
   ```bash
   adb kill-server
   adb start-server
   adb devices
   ```
4. **Restart Unity**

### Issue: Play mode still doesn't work

**Fallback:** Use Build and Run (slower but more reliable)
- File → Build Settings → Build and Run
- This always works if ADB is set up

---

## 💡 Tips for Faster Development

### Workflow:
1. **Make code changes**
2. **Press Play** → Deploys to Quest automatically
3. **Test on Quest**
4. **Stop Play** → Make more changes
5. **Repeat!**

### Advantages of Play Mode:
- ✅ **Faster iteration** - No full build needed
- ✅ **Real-time testing** - See changes immediately
- ✅ **Unity debugger** - Full debugging support
- ✅ **Console logs** - See Unity Console on PC

### Limitations:
- ⚠️ **First deployment** may take a minute
- ⚠️ **Requires USB connection** (can't be wireless)
- ⚠️ **AR features** work better on device than in editor

---

## 📋 Quick Checklist

Before pressing Play:

- [ ] ADB installed and in PATH
- [ ] `adb devices` shows your Quest
- [ ] Quest connected via USB
- [ ] USB debugging enabled and accepted
- [ ] Build Settings → Platform: Android
- [ ] Build Settings → Run Device: Shows Quest
- [ ] Device selector (next to Play): Shows Quest
- [ ] XR Plug-in Management → Meta XR SDK enabled

**If all checked, press Play and it should work!** 🚀

---

## 🆘 Still Not Working?

1. **Check Unity Console** for error messages
2. **Verify ADB works:** `adb devices` in terminal
3. **Try Build and Run** as a test (if this works, ADB is fine)
4. **Restart everything:** Unity, Quest, PC
5. **Check USB cable** - Use a data cable (not just charging)

---

## 📚 Related Guides

- **`TROUBLESHOOTING_QUEST_CONNECTION.md`** - Detailed connection troubleshooting
- **`BUILD_AND_RUN_GUIDE.md`** - Alternative: Build and Run method

---

## ✅ Summary

**To use Play mode on Quest:**

1. ✅ Install ADB and add to PATH
2. ✅ Connect Quest and verify `adb devices` works
3. ✅ Configure Unity Build Settings (Android platform)
4. ✅ Select Quest in device selector (next to Play button)
5. ✅ Press Play!

**Once set up, you can just press Play and test on Quest instantly!** 🎮

