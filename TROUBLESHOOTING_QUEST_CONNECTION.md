# Troubleshooting: Cannot Connect to Meta Quest When Pressing Play

## ⚠️ Common Issue: Unity Not Connecting to Quest

When you press the **Play** button in Unity, it should connect to your Quest device if it's properly set up. If it doesn't connect, follow these steps:

### 🔌 WiFi is NOT Required for USB Connection

**Important:** If you're using a USB cable, you do NOT need WiFi. USB provides the connection directly. WiFi is only needed for wireless ADB (which is optional and advanced).

---

## ✅ Quick Check: If You Already Have These Set Up

If you already have:
- ✅ USB cable connected
- ✅ Meta XR SDK installed
- ✅ Developer mode enabled
- ✅ Quest Link enabled

**Skip to Solution 6** to check Unity Editor settings and verify ADB connection.

---

## 🔧 Solution 1: Enable Quest Link (Required for Play Mode)

### Step 1: Enable Quest Link on Your Quest Device

1. **Put on your Quest headset**
2. **Open Settings** (gear icon in the menu)
3. **Go to System → Quest Link** (or **Settings → Link**)
4. **Enable "Quest Link"** (toggle it ON)
5. **Enable "USB Connection Dialog"** (so you can accept connection prompts)

### Step 2: Connect Quest to PC via USB

1. **Use a USB-C cable** (preferably USB 3.0 or better)
2. **Connect Quest to your PC**
3. **Put on Quest headset** - you should see a prompt asking to "Allow USB Debugging"
4. **Select "Allow"** and check **"Always allow from this computer"**
5. **Select "Enable Link"** when prompted

---

## 🔧 Solution 2: Enable USB Debugging (Required for ADB)

### On Quest Device:

1. **Enable Developer Mode:**
   - Open **Settings → System → About**
   - Find **"Software Version"** and click it **7 times** (you'll see "You are now a developer!")
   - Go back to **Settings → Developer**
   - Enable **"USB Connection Dialog"**
   - Enable **"USB Debugging"**

2. **Connect Quest to PC:**
   - Connect via USB cable
   - Put on headset and **accept the USB debugging prompt**
   - Check **"Always allow from this computer"**

---

## 🔧 Solution 3: Install ADB (Android Debug Bridge)

**If you get `'adb' is not recognized` error, ADB is not installed or not in PATH.**

### Why Do You Need ADB?

**For Unity Play Mode:**
- When you press **Play** button, Unity needs to deploy your app to Quest
- Unity uses ADB to communicate with Android devices
- Even if Unity has its own ADB, it may not find it if not in PATH

**You might not have needed it before if:**
- You were using **Build and Run** instead of Play mode (Unity uses its own ADB)
- You had Android Studio installed (ADB was already in PATH)
- Unity had ADB bundled and accessible before
- You were just using Quest Link for VR (doesn't need ADB)

**For Play Mode to work, you need ADB accessible.**

### Option A: Install Android SDK Platform Tools (Recommended)

1. **Download Android SDK Platform Tools:**
   - Go to: https://developer.android.com/tools/releases/platform-tools
   - Or direct download: https://dl.google.com/android/repository/platform-tools-latest-windows.zip
   - Download the ZIP file for Windows

2. **Extract ADB:**
   - Extract the ZIP to a folder (e.g., `C:\platform-tools`)
   - You should see `adb.exe` in that folder

3. **Add to PATH (Windows):**
   - Press **Windows Key + X** → **System** → **Advanced system settings**
   - Click **Environment Variables**
   - Under **System variables**, find **Path** and click **Edit**
   - Click **New** and add: `C:\platform-tools` (or wherever you extracted it)
   - Click **OK** on all windows
   - **Close and reopen Command Prompt/PowerShell** (PATH changes require restart)

4. **Verify Installation:**
   - Open **NEW** Command Prompt or PowerShell
   - Type: `adb version`
   - You should see ADB version number (not an error)

### Option B: Use Unity's Built-in ADB (If Android SDK is Installed)

If you have Android SDK installed through Unity Hub or Android Studio:

1. **Find Unity's ADB:**
   - Usually located at: `C:\Users\<YourName>\AppData\Local\Android\Sdk\platform-tools\adb.exe`
   - Or check: `C:\Program Files\Unity\Hub\Editor\<Version>\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\`

2. **Add to PATH:**
   - Add the `platform-tools` folder path to your system PATH (same steps as Option A)

### Option C: Install Android Studio (Full SDK)

1. **Download Android Studio:**
   - Go to: https://developer.android.com/studio
   - Install Android Studio
   - During installation, make sure **Android SDK Platform-Tools** is selected

2. **Find ADB:**
   - Usually at: `C:\Users\<YourName>\AppData\Local\Android\Sdk\platform-tools\`

3. **Add to PATH:**
   - Add the `platform-tools` folder to PATH (same steps as Option A)

### Quick Test After Installing ADB:

1. **Close ALL Command Prompt/PowerShell windows** (PATH changes need new windows)
2. **Open NEW Command Prompt or PowerShell**
3. **Type:** `adb devices`
4. **You should see:**
   ```
   List of devices attached
   ```
   (Even if no devices, you shouldn't get "not recognized" error)

5. **Connect Quest and run `adb devices` again** - Quest should appear

---

## 🔧 Solution 4: Verify Unity Build Settings

### Check Android Build Settings:

1. **File → Build Settings**
2. **Select "Android"** platform
3. **Click "Switch Platform"** if needed
4. **Verify settings:**
   - **Target Device:** Quest (or Quest 2/3/Pro)
   - **Minimum API Level:** Android 7.0 (API 24) or higher
   - **Target API Level:** Latest (API 33+ recommended)

### Check XR Settings:

1. **Edit → Project Settings → XR Plug-in Management**
2. **Enable "Meta XR SDK"** (check the box)
3. **Enable "Initialize XR on Startup"**
4. **For Android:**
   - Go to **Android** tab
   - Verify **Meta XR SDK** is checked

---

## 🔧 Solution 5: Test ADB Connection

### Verify Quest is Detected:

1. **Open Command Prompt/PowerShell**
2. **Type:** `adb devices`
3. **You should see:**
   ```
   List of devices attached
   ABC123XYZ    device
   ```
   - If you see **"unauthorized"**, accept the USB debugging prompt on Quest
   - If you see **"offline"**, unplug and replug the USB cable
   - If nothing appears, check USB cable and drivers

### Common ADB Issues:

- **"adb: command not found"** → ADB not in PATH (see Solution 3)
- **"unauthorized"** → Accept USB debugging prompt on Quest
- **"offline"** → Unplug/replug USB cable, restart ADB server: `adb kill-server && adb start-server`
- **No devices** → Check USB cable, enable USB debugging, enable Quest Link

---

## 🔧 Solution 6: Unity Editor Settings (Most Important for Your Situation)

Since you already have Quest Link and Developer mode enabled, this is likely where the issue is:

### Step 1: Verify ADB Can See Your Quest

1. **Open Command Prompt or PowerShell**
2. **Type:** `adb devices`
3. **You should see:**
   ```
   List of devices attached
   ABC123XYZ    device
   ```
   - If you see **"unauthorized"** → Put on Quest headset and accept USB debugging prompt
   - If you see **"offline"** → Unplug and replug USB cable, then run `adb kill-server && adb start-server`
   - If nothing appears → Quest not detected (check USB cable/port)

### Step 2: Check Unity Device Selector

1. **In Unity Editor**, look at the top toolbar
2. **Next to the Play button**, there should be a device selector dropdown
3. **Click it** - your Quest should appear in the list
4. **If no devices appear:**
   - Unity can't see Quest via ADB
   - Check if ADB works (Step 1 above)
   - Restart Unity after connecting Quest

### Step 3: Check Unity Editor Preferences

1. **Edit → Preferences → External Tools** (Windows) or **Unity → Preferences** (Mac)
2. **Android:**
   - **Android SDK Tools:** Should point to your Android SDK folder
   - If blank, Unity might not be able to use ADB
   - **JDK:** Should point to your JDK installation
   - **NDK:** Optional, but recommended for Quest

### Step 4: Verify Build Settings

1. **File → Build Settings**
2. **Platform should be "Android"** (not PC, Mac, etc.)
3. **If not Android:**
   - Select **Android**
   - Click **"Switch Platform"** (this may take a few minutes)
4. **Check "Run Device"** dropdown at bottom - Quest should appear here too

---

## 🔧 Solution 7: Alternative - Use Quest Link Desktop App

### If USB connection doesn't work:

1. **Install Oculus Desktop App:**
   - Download from: https://www.meta.com/quest/setup/
   - Install and sign in

2. **Enable Quest Link in Oculus App:**
   - Open Oculus Desktop App
   - Go to **Settings → General**
   - Enable **"Quest Link"**

3. **Connect Quest:**
   - Connect via USB
   - Put on headset
   - Select **"Enable Link"** when prompted

4. **In Unity:**
   - Unity should now detect Quest as a connected device
   - Select it from the device dropdown next to Play button

---

## 🎯 Quick Checklist

Before pressing Play, verify:

- [ ] **Quest Link is enabled** on Quest device
- [ ] **USB Debugging is enabled** on Quest device
- [ ] **Quest is connected** via USB cable
- [ ] **USB debugging prompt accepted** on Quest
- [ ] **ADB is installed** and in PATH
- [ ] **`adb devices`** shows your Quest
- [ ] **Unity Build Settings** → Platform is **Android**
- [ ] **XR Plug-in Management** → **Meta XR SDK** is enabled
- [ ] **Device selector** (next to Play button) shows your Quest

---

## ⚠️ Important Notes

### When Do You Need ADB?

**You need ADB in your PATH for:**
- **Unity Play Mode** - When you press Play button and want Unity to run on Quest
- **Manual ADB commands** - Testing connection, installing APKs manually, etc.

**You DON'T need ADB in PATH for:**
- **Build & Run** - Unity uses its own bundled ADB (if Android SDK is configured in Unity)
- **Quest Link VR streaming** - Just for VR desktop experience (not Unity development)
- **Manual APK installation** - If you install APK files directly on Quest

### Play Mode vs Build & Run:

- **Play Mode (Press Play button):** 
  - Requires Quest Link + USB connection + ADB accessible
  - Unity needs to find ADB to deploy to device
  - If ADB not in PATH, Unity might not find it (even if it has its own ADB)
  
- **Build & Run (File → Build and Run):**
  - Uses Unity's bundled ADB (if Android SDK configured)
  - May work even without ADB in PATH
  - Builds APK and installs automatically

### Editor vs Device:

- **In Editor:** You'll see a black screen (normal for AR apps)
- **On Device:** Full AR experience with passthrough

### Wireless Connection (Advanced - Optional):

**Note:** WiFi is NOT needed when using USB cable. Only use this if you want wireless connection.

For wireless connection (no USB cable):
1. Enable **Wireless ADB** in Quest Developer settings
2. Connect Quest and PC to same Wi-Fi network
3. Use `adb connect <Quest-IP-Address>` in terminal
4. Unity should detect Quest wirelessly

**For USB connection (your current setup): WiFi is NOT required!**

---

## 🐛 Still Not Working?

### Try These Steps:

1. **Restart everything:**
   - Close Unity
   - Unplug Quest
   - Restart Quest (hold power button)
   - Restart PC
   - Reconnect Quest
   - Open Unity

2. **Check USB cable:**
   - Use a **data cable** (not just charging cable)
   - Try a different USB port (preferably USB 3.0)
   - Try a different cable

3. **Check Windows drivers:**
   - Open **Device Manager**
   - Look for **"Android Device"** or **"Quest"**
   - If there's a yellow warning, update drivers

4. **Reset ADB:**
   ```bash
   adb kill-server
   adb start-server
   adb devices
   ```

5. **Check Unity Console:**
   - Look for error messages about device connection
   - Check for ADB-related errors

---

## 📚 Related Guides

- **`TROUBLESHOOTING_BLACK_SCREEN.md`** - For black screen issues
- **`OPENXR_FEATURES_SETUP.md`** - For OpenXR configuration
- **`SETUP_CHECKLIST.md`** - For general setup verification

---

## 💡 Pro Tips

1. **Always accept USB debugging prompt** on Quest (check "Always allow")
2. **Keep Quest Link enabled** while developing
3. **Use a good USB 3.0 cable** for stable connection
4. **Check device selector** in Unity before pressing Play
5. **Test with `adb devices`** first before opening Unity

---

## 🆘 Need More Help?

If you're still having issues:
1. Check Unity Console for specific error messages
2. Verify all steps in the checklist above
3. Try building and running (File → Build and Run) instead of Play mode
4. Check Meta Quest Developer documentation: https://developer.oculus.com/

