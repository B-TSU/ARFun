# Unity App Crash Diagnostics

Common crash causes and fixes for the ARFun Unity app.

---

## 🔍 Common Crash Causes

### 1. **Null Reference Exceptions**

**Problem:** Accessing `Instance` properties before initialization.

**Symptoms:**
- App crashes on startup
- NullReferenceException in console
- Scripts not working

**Fix:** Always check for null before accessing Instance:
```csharp
if (PassthroughToGemmaSender.Instance != null)
{
    // Safe to use
}
```

**Files to check:**
- `GemmaAPIDebugger.cs` - Lines 204-205 (event subscription)
- `GemmaAPITester.cs` - Lines 40-41 (event subscription)
- `GemmaTextToSpeech.cs` - Line 105 (event subscription)

---

### 2. **Event Subscription Issues**

**Problem:** Subscribing to events when Instance is null, or not unsubscribing properly.

**Symptoms:**
- Crashes when components are destroyed
- Memory leaks
- Events firing after object destruction

**Fix:** Ensure proper null checks and cleanup:
```csharp
private void OnDestroy()
{
    if (PassthroughToGemmaSender.Instance != null)
    {
        PassthroughToGemmaSender.Instance.OnResponseReceived -= OnResponseReceived;
    }
}
```

---

### 3. **SecretsManager File Access**

**Problem:** File I/O operations can throw exceptions.

**Symptoms:**
- Crashes when loading/saving secrets
- Permission errors
- File not found errors

**Fix:** Already handled with try-catch, but verify:
- File permissions are correct
- Directory exists
- No concurrent access

---

### 4. **Camera Access Issues**

**Problem:** Camera not found or null when trying to capture.

**Symptoms:**
- Crashes when calling `CaptureAndSendToGemma()`
- NullReferenceException in camera code

**Fix:** Check camera initialization:
```csharp
if (arCamera == null)
{
    Debug.LogError("Camera not found!");
    return; // Don't proceed
}
```

---

### 5. **Coroutine Issues**

**Problem:** Starting coroutines on destroyed objects.

**Symptoms:**
- Crashes when retrying subscriptions
- MissingReferenceException

**Fix:** Check if object is destroyed before starting coroutines.

---

## 🛠️ Quick Fixes

### Fix 1: Add Null Checks to Event Subscriptions

**In GemmaAPIDebugger.cs:**
```csharp
private void SubscribeToEvents()
{
    if (PassthroughToGemmaSender.Instance != null)
    {
        PassthroughToGemmaSender.Instance.OnResponseReceived += OnResponseReceived;
        PassthroughToGemmaSender.Instance.OnErrorOccurred += OnErrorOccurred;
    }
    else
    {
        // Retry later or log warning
        Debug.LogWarning("PassthroughToGemmaSender.Instance is null. Will retry...");
    }
}
```

### Fix 2: Ensure Proper Cleanup

**In all scripts with event subscriptions:**
```csharp
private void OnDestroy()
{
    UnsubscribeFromEvents();
}

private void OnDisable()
{
    // Also unsubscribe when disabled
    UnsubscribeFromEvents();
}
```

### Fix 3: Add Safety Checks Before API Calls

**In PassthroughToGemmaSender.cs:**
```csharp
public void CaptureAndSendToGemma(string prompt = DEFAULT_PROMPT)
{
    // Add more safety checks
    if (isCapturing)
    {
        Debug.LogWarning("Already capturing, please wait...");
        return;
    }

    if (string.IsNullOrEmpty(openRouterApiKey))
    {
        Debug.LogError("API key not set!");
        OnErrorOccurred?.Invoke("API key not set");
        return;
    }

    if (arCamera == null && !IsPassthroughCameraAvailable())
    {
        Debug.LogError("No camera available!");
        OnErrorOccurred?.Invoke("No camera available");
        return;
    }

    StartCoroutine(CaptureAndSendCoroutine(prompt));
}
```

---

## 📋 Diagnostic Checklist

Run through this checklist to identify the crash:

- [ ] **Check Unity Console** for error messages
- [ ] **Check Logs** folder for detailed error logs
- [ ] **Verify all scripts are attached** to GameObjects
- [ ] **Check script execution order** (Edit → Project Settings → Script Execution Order)
- [ ] **Verify API key is set** (check secrets.json)
- [ ] **Check camera setup** (AR camera exists in scene)
- [ ] **Verify Input System** (new vs old Input system conflicts)
- [ ] **Check for duplicate instances** (multiple singletons)
- [ ] **Verify scene setup** (all required GameObjects present)

---

## 🔧 Step-by-Step Debugging

### Step 1: Check Console for Errors

1. Open Unity Console (Window → General → Console)
2. Look for red error messages
3. Note the script name and line number
4. Check stack trace

### Step 2: Enable Detailed Logging

1. In each script, ensure `debugMode = true`
2. Check Console for warning messages
3. Look for initialization messages

### Step 3: Verify Component Setup

1. Check Hierarchy for required GameObjects:
   - Managers GameObject (with PassthroughToGemmaSender)
   - Camera Rig
   - Other required components

2. Verify scripts are enabled:
   - Check Inspector for each component
   - Ensure checkboxes are checked

### Step 4: Test in Isolation

1. Disable all API-related scripts
2. Enable one at a time
3. Identify which script causes the crash

### Step 5: Check Script Execution Order

1. Edit → Project Settings → Script Execution Order
2. Ensure managers initialize before scripts that use them
3. Set PassthroughToGemmaSender to execute early

---

## 🚨 Most Common Issues

### Issue 1: Instance Not Initialized

**Error:** `NullReferenceException: Object reference not set to an instance of an object`

**Cause:** Script trying to access `Instance` before `Awake()` runs.

**Fix:** 
- Check script execution order
- Add null checks
- Use `Start()` instead of `Awake()` for initialization-dependent code

### Issue 2: Event Subscription on Null

**Error:** `NullReferenceException` when subscribing to events

**Cause:** Trying to subscribe when Instance is null.

**Fix:** Always check for null before subscribing:
```csharp
if (PassthroughToGemmaSender.Instance != null)
{
    PassthroughToGemmaSender.Instance.OnResponseReceived += Handler;
}
```

### Issue 3: Camera Not Found

**Error:** `NullReferenceException` in camera capture code

**Cause:** Camera not found or not initialized.

**Fix:** 
- Verify camera exists in scene
- Check camera path: `[BuildingBlock] Camera Rig/TrackingSpace/CenterEyeAnchor`
- Add fallback to Camera.main

### Issue 4: File Access Errors

**Error:** `UnauthorizedAccessException` or `DirectoryNotFoundException`

**Cause:** Secrets file access issues.

**Fix:**
- Check file permissions
- Verify directory exists
- Use Application.persistentDataPath for cross-platform compatibility

---

## 📝 How to Report Crashes

When reporting crashes, include:

1. **Error Message** (from Console)
2. **Stack Trace** (full stack trace)
3. **When it happens** (on startup, during API call, etc.)
4. **Unity Version**
5. **Platform** (Editor, Quest, etc.)
6. **Script Name** and line number
7. **Scene** where crash occurs

---

## 🔍 Advanced Debugging

### Enable Exception Breakpoints

1. Visual Studio: Debug → Windows → Exception Settings
2. Check "Common Language Runtime Exceptions"
3. Run in debug mode
4. App will break on exceptions

### Use Unity Profiler

1. Window → Analysis → Profiler
2. Check for memory leaks
3. Monitor performance
4. Identify bottlenecks

### Check Player Logs

**Windows:**
```
%USERPROFILE%\AppData\LocalLow\<CompanyName>\<ProductName>\Player.log
```

**Mac:**
```
~/Library/Logs/Unity/Player.log
```

---

## ✅ Prevention Checklist

Before building/deploying:

- [ ] All null checks in place
- [ ] Event subscriptions properly cleaned up
- [ ] No duplicate singleton instances
- [ ] Camera setup verified
- [ ] API key configured
- [ ] Script execution order set
- [ ] All required GameObjects in scene
- [ ] Input System properly configured
- [ ] No compiler errors
- [ ] Tested in Play Mode

---

## 🆘 Still Crashing?

If crashes persist:

1. **Create minimal test scene** with just the crashing script
2. **Check Unity version compatibility**
3. **Update all packages** (Window → Package Manager)
4. **Clear Library folder** and reimport
5. **Check for conflicting scripts**
6. **Review recent changes** (git diff)

---

## 📚 Related Files

- `Assets/Scripts/API/PassthroughToGemmaSender.cs`
- `Assets/Scripts/API/GemmaTextToSpeech.cs`
- `Assets/Scripts/API/GemmaAPIDebugger.cs`
- `Assets/Scripts/API/GemmaAPITester.cs`
- `Assets/Scripts/API/SecretsManager.cs`

