# Script Attachment Guide

## 📋 Where to Attach Each Script

### ✅ **Managers GameObject** (Recommended Location)

All manager scripts should be attached to the **"Managers"** GameObject:

```
Managers (Empty GameObject)
├── GameStateManager
├── UIManager
├── ARManagerMetaSDK
├── PlateManager
├── FlowerManager
├── TrimmingManager
├── PassthroughToGemmaSender  ← Add this
└── GemmaTextToSpeech          ← Add this
```

---

## 🎯 Step-by-Step Setup

### Step 1: Find or Create "Managers" GameObject

1. **Open your scene** (IkebanAR.unity)
2. **Look in Hierarchy** for "Managers" GameObject
   - If it exists: Select it
   - If it doesn't exist:
     - Right-click in Hierarchy → **Create Empty**
     - Name it: `Managers`
     - Position: (0, 0, 0)

### Step 2: Attach PassthroughToGemmaSender

1. **Select "Managers" GameObject**
2. **Add Component** → Search: `PassthroughToGemmaSender`
3. **Configure in Inspector:**
   - **Model Name**: `google/gemma-3-27b-it:free` (already set)
   - **API Url**: `https://openrouter.ai/api/v1/chat/completions` (already set)
   - **Capture Width**: 1280 (default)
   - **Capture Height**: 960 (default)
   - **Passthrough Camera Access**: Leave empty (auto-finds) OR drag `PassthroughCameraAccess` component
   - **Debug Mode**: ✅ Enabled (for testing)

### Step 3: Attach GemmaTextToSpeech

1. **Select "Managers" GameObject** (same one)
2. **Add Component** → Search: `GemmaTextToSpeech`
3. **Configure in Inspector:**
   - **TTS Speaker**: Leave empty (auto-finds) OR drag `TTSSpeaker` component
   - **Auto Speak**: ✅ Enabled (automatically speaks responses)
   - **Queue Speeches**: ❌ Disabled (unless you want queuing)
   - **Speech Rate**: 1.0 (normal speed)
   - **Debug Mode**: ✅ Enabled (for testing)

### Step 4: Set Up PassthroughCameraAccess (Required)

1. **Create GameObject** for passthrough camera:
   - Right-click Hierarchy → **Create Empty**
   - Name: `PassthroughCameraAccess`
2. **Add Component**:
   - Add `PassthroughCameraAccess` component (from Meta Voice SDK)
3. **Configure**:
   - Enable the component
   - Ensure camera permissions are set
4. **Optional**: Assign to PassthroughToGemmaSender in Inspector

### Step 5: Set Up TTSSpeaker (Required for TTS)

1. **Create GameObject** for TTS:
   - Right-click Hierarchy → **Create Empty**
   - Name: `TTSSpeaker`
2. **Add Components**:
   - Add `AudioSource` component
   - Add `TTSSpeaker` component (from Meta Voice SDK)
3. **Configure TTSSpeaker**:
   - Assign AudioSource to TTSSpeaker
   - Set up voice settings
   - Configure TTS service (if using cloud TTS)
4. **Optional**: Assign to GemmaTextToSpeech in Inspector

---

## 📁 Complete Scene Structure

```
IkebanAR Scene
│
├── Managers (Empty GameObject)
│   ├── GameStateManager
│   ├── UIManager
│   ├── ARManagerMetaSDK
│   ├── PlateManager
│   ├── FlowerManager
│   ├── TrimmingManager
│   ├── PassthroughToGemmaSender  ← NEW
│   └── GemmaTextToSpeech          ← NEW
│
├── PassthroughCameraAccess (Empty GameObject)  ← NEW
│   └── PassthroughCameraAccess (Component)
│
├── TTSSpeaker (Empty GameObject)  ← NEW
│   ├── AudioSource (Component)
│   └── TTSSpeaker (Component)
│
├── [BuildingBlock] Camera Rig
├── Meta_Passthrough
├── UI Canvas
└── ... (other scene objects)
```

---

## 🔧 Optional: PassthroughGemmaExample

If you want to use the example UI script:

1. **Create GameObject**:
   - Right-click Hierarchy → **Create Empty**
   - Name: `GemmaExampleUI`
2. **Attach Script**:
   - Add Component → `PassthroughGemmaExample`
3. **Assign UI References**:
   - **Capture Button**: Drag your capture button
   - **Response Text**: Drag TextMeshPro text for responses
   - **Status Text**: Drag TextMeshPro text for status
   - **Preview Image**: Drag RawImage for camera preview (optional)
4. **Configure**:
   - **Custom Prompt**: Set your default prompt

---

## ✅ Quick Checklist

### Required Setup:
- [ ] Find/Create "Managers" GameObject
- [ ] Attach `PassthroughToGemmaSender` to Managers
- [ ] Attach `GemmaTextToSpeech` to Managers
- [ ] Create `PassthroughCameraAccess` GameObject
- [ ] Add `PassthroughCameraAccess` component
- [ ] Create `TTSSpeaker` GameObject
- [ ] Add `TTSSpeaker` component
- [ ] Add `AudioSource` to TTSSpeaker GameObject
- [ ] Set up `secrets.json` with API key

### Optional Setup:
- [ ] Create `GemmaExampleUI` GameObject
- [ ] Attach `PassthroughGemmaExample` script
- [ ] Assign UI references

---

## 🎯 Why Managers GameObject?

**Benefits:**
- ✅ All managers in one place
- ✅ Easy to find and manage
- ✅ Clean scene hierarchy
- ✅ Follows Unity best practices
- ✅ Matches your existing setup

**Alternative Locations:**
- You can attach to separate GameObjects if preferred
- Scripts will work the same way
- Just make sure they're active in the scene

---

## ⚠️ Important Notes

1. **Both scripts can be on the same GameObject** (Managers)
2. **PassthroughCameraAccess must be separate** - It's a Meta SDK component
3. **TTSSpeaker must be separate** - It needs AudioSource
4. **Scripts auto-find components** - You don't need to assign if components are in scene
5. **SecretsManager is static** - No GameObject needed, just edit `secrets.json`

---

## 🚀 After Setup

Once attached:

1. **API Key**: Edit `secrets.json` with your OpenRouter API key
2. **Test**: Call `PassthroughToGemmaSender.Instance.CaptureAndSendToGemma()`
3. **Verify**: Check Console for debug messages
4. **Listen**: TTS should automatically speak responses (if autoSpeak enabled)

---

Your scripts are now ready to use! 🎉


