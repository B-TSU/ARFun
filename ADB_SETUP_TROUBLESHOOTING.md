# ADB Setup Troubleshooting: "Cannot Find C:\platform-tools"

## 🔍 Where Are You Seeing This Error?

The error could appear in different places. Let's fix each one:

---

## Issue 1: ZIP Extraction Error

**Error:** "Cannot find C:\platform-tools" when extracting ZIP

**Solution:**

### Method A: Extract to Desktop First (Easiest)

1. **Download the ZIP file** to your Desktop
2. **Right-click the ZIP** → **Extract All...**
3. **Extract to Desktop** (default location)
4. **Rename the extracted folder** to `platform-tools`
5. **Move it to C:\**:
   - Cut the folder (Ctrl+X)
   - Go to `C:\` in File Explorer
   - Paste (Ctrl+V)
6. **Now you have:** `C:\platform-tools`

### Method B: Create Folder First, Then Extract

1. **Open File Explorer**
2. **Navigate to C:\** (type `C:\` in address bar)
3. **Right-click empty space** → **New** → **Folder**
4. **Name it:** `platform-tools`
5. **Download the ZIP** to Desktop
6. **Right-click ZIP** → **Extract All...**
7. **Click "Browse"** and navigate to `C:\platform-tools`
8. **Extract here**

---

## Issue 2: PATH Environment Variable Error

**Error:** "Cannot find C:\platform-tools" when adding to PATH

**Solution:**

1. **First, make sure the folder exists:**
   - Open File Explorer
   - Go to `C:\`
   - Check if `platform-tools` folder exists
   - If not, create it (see Issue 1)

2. **When adding to PATH:**
   - Don't include quotes: Use `C:\platform-tools` (not `"C:\platform-tools"`)
   - Make sure there are no spaces
   - Use backslashes `\` (not forward slashes `/`)

3. **Verify the folder has adb.exe:**
   - Go to `C:\platform-tools` in File Explorer
   - You should see `adb.exe` file
   - If not, the ZIP wasn't extracted correctly

---

## Issue 3: Command Prompt Can't Find ADB

**Error:** `'adb' is not recognized` after adding to PATH

**Solution:**

1. **Verify folder exists:**
   - Open File Explorer → Go to `C:\platform-tools`
   - Check that `adb.exe` is there

2. **Verify PATH was added correctly:**
   - Windows Key + X → System → Advanced system settings
   - Environment Variables → System variables → Path → Edit
   - Look for `C:\platform-tools` in the list
   - If not there, add it again

3. **Restart Command Prompt:**
   - **Close ALL Command Prompt/PowerShell windows**
   - **Open a NEW Command Prompt**
   - Type: `adb version`
   - Should work now!

4. **If still not working, test directly:**
   - In Command Prompt, type: `C:\platform-tools\adb.exe version`
   - If this works, PATH isn't set correctly
   - If this doesn't work, the folder/file doesn't exist

---

## ✅ Step-by-Step: Complete Setup (No Errors)

### Step 1: Create the Folder

1. **Open File Explorer**
2. **Click "This PC"** in left sidebar
3. **Double-click "Local Disk (C:)"**
4. **Right-click empty space** → **New** → **Folder**
5. **Type:** `platform-tools` and press Enter
6. **Verify:** You should see `C:\platform-tools` folder

### Step 2: Download and Extract

1. **Download:** https://dl.google.com/android/repository/platform-tools-latest-windows.zip
2. **Save to Desktop** (or Downloads folder)
3. **Right-click the ZIP file** → **Extract All...**
4. **Click "Browse"**
5. **Navigate to:** `C:\`
6. **Select the `platform-tools` folder** you just created
7. **Click "Extract"**
8. **Verify:** Go to `C:\platform-tools` - you should see `adb.exe`

### Step 3: Add to PATH

1. **Press Windows Key + X** → **System**
2. **Click "Advanced system settings"** (right side)
3. **Click "Environment Variables"** (bottom)
4. **Under "System variables"**, find **Path** → Click **Edit**
5. **Click "New"**
6. **Type:** `C:\platform-tools` (no quotes, no spaces)
7. **Click OK** on all windows

### Step 4: Test

1. **Close ALL Command Prompt/PowerShell windows**
2. **Open NEW Command Prompt** (Windows Key + R → type `cmd` → Enter)
3. **Type:** `adb version`
4. **Should see:** Version number (not an error!)

---

## 🔍 Quick Verification Checklist

Before adding to PATH, verify:

- [ ] Folder exists: `C:\platform-tools` (check in File Explorer)
- [ ] `adb.exe` is inside the folder (check in File Explorer)
- [ ] No spaces in folder name
- [ ] Using backslashes `\` (not `/`)

After adding to PATH:

- [ ] Closed all Command Prompt windows
- [ ] Opened NEW Command Prompt
- [ ] `adb version` works
- [ ] `adb devices` works (with Quest connected)

---

## 🆘 Still Not Working?

### Alternative: Use Full Path

Instead of adding to PATH, you can use the full path:

```bash
C:\platform-tools\adb.exe devices
```

This works from anywhere, but you need to type the full path each time.

### Alternative: Extract to Different Location

If `C:\` is protected, extract to:

- `C:\Users\YourName\platform-tools`
- `D:\platform-tools` (if you have D drive)
- `C:\Android\platform-tools`

Then add that path to PATH instead.

---

## 💡 Common Mistakes

1. **Creating folder in wrong location** - Make sure it's `C:\platform-tools`, not `C:\Users\...\platform-tools`
2. **Not extracting ZIP correctly** - Make sure `adb.exe` is directly in `C:\platform-tools`
3. **Not restarting Command Prompt** - PATH changes require new terminal windows
4. **Typing path wrong in PATH** - Use `C:\platform-tools` exactly (no quotes, backslashes)

---

## ✅ Final Check

Run these commands in NEW Command Prompt:

```bash
# Check if folder exists
dir C:\platform-tools

# Check if adb.exe exists
dir C:\platform-tools\adb.exe

# Test adb (should work if PATH is set)
adb version
```

If all three work, you're good to go! 🎉

