# Passthrough Camera to Gemma 3 27B Setup Guide

## Overview

This guide explains how to use the `PassthroughToGemmaSender` script to capture images from the Meta Quest passthrough camera and send them to Google Gemma 3 27B via OpenRouter.

---

## 🚀 Quick Start

### 1. **Get OpenRouter API Key**

1. Go to [OpenRouter.ai](https://openrouter.ai/)
2. Sign up or log in
3. Navigate to **Keys** section
4. Create a new API key
5. Copy the API key

### 2. **Setup in Unity**

1. **Add PassthroughToGemmaSender Script:**
   - Create an empty GameObject (e.g., "PassthroughGemmaManager")
   - Add the `PassthroughToGemmaSender` component
   - In the Inspector, set:
     - **Open Router Api Key**: Your API key from step 1
     - **Model Name**: `google/gemma-3-27b-it:free` (already set)
     - **Capture Width/Height**: Adjust if needed (default: 1280x960)

2. **Add Example UI (Optional):**
   - Add the `PassthroughGemmaExample` script to a UI GameObject
   - Connect UI elements:
     - Capture Button
     - Response Text (TextMeshPro)
     - Status Text (TextMeshPro)
     - Preview Image (RawImage) - optional

### 3. **Permissions Setup**

For Quest devices, ensure camera permissions are set:

**AndroidManifest.xml** (auto-generated, but verify):
```xml
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="horizonos.permission.HEADSET_CAMERA" />
```

---

## 📝 Usage

### Basic Usage

```csharp
// Capture and send with default prompt
PassthroughToGemmaSender.Instance.CaptureAndSendToGemma();

// Capture and send with custom prompt
PassthroughToGemmaSender.Instance.CaptureAndSendToGemma(
    "What objects do you see in this image? Describe them in detail."
);
```

### Event-Based Usage

```csharp
// Subscribe to events
PassthroughToGemmaSender.Instance.OnResponseReceived += OnResponseReceived;
PassthroughToGemmaSender.Instance.OnErrorOccurred += OnErrorOccurred;

// Capture
PassthroughToGemmaSender.Instance.CaptureAndSendToGemma("Analyze this image.");

// Handle response
private void OnResponseReceived(string response)
{
    Debug.Log($"Gemma says: {response}");
}

// Handle errors
private void OnErrorOccurred(string error)
{
    Debug.LogError($"Error: {error}");
}
```

### Camera Control

```csharp
// Start passthrough camera
PassthroughToGemmaSender.Instance.StartPassthroughCamera();

// Stop passthrough camera
PassthroughToGemmaSender.Instance.StopPassthroughCamera();

// Get preview texture
Texture preview = PassthroughToGemmaSender.Instance.GetPassthroughTexture();
```

---

## ⚙️ Configuration

### Inspector Settings

| Setting | Description | Default |
|---------|-------------|---------|
| **Open Router Api Key** | Your OpenRouter API key | (Required) |
| **Model Name** | Model to use | `google/gemma-3-27b-it:free` |
| **Api Url** | OpenRouter API endpoint | `https://openrouter.ai/api/v1/chat/completions` |
| **Capture Width** | Image width | 1280 |
| **Capture Height** | Image height | 960 |
| **Use Web Cam Texture** | Use WebCamTexture for capture | true |
| **Debug Mode** | Enable debug logs | true |

### Model Options

You can use different models by changing the `modelName`:

- `google/gemma-3-27b-it:free` - Free tier (recommended)
- `google/gemma-3-27b-it` - Paid tier (faster)
- Other vision models supported by OpenRouter

---

## 🎯 Use Cases

### 1. **Object Recognition**
```csharp
PassthroughToGemmaSender.Instance.CaptureAndSendToGemma(
    "What objects are visible in this image? List them."
);
```

### 2. **Scene Description**
```csharp
PassthroughToGemmaSender.Instance.CaptureAndSendToGemma(
    "Describe the scene in this image in detail."
);
```

### 3. **AR Assistance**
```csharp
PassthroughToGemmaSender.Instance.CaptureAndSendToGemma(
    "I'm arranging flowers in AR. What advice can you give about the arrangement?"
);
```

### 4. **Safety Check**
```csharp
PassthroughToGemmaSender.Instance.CaptureAndSendToGemma(
    "Are there any safety hazards visible in this image?"
);
```

---

## 🔧 Troubleshooting

### Camera Not Working

**Problem:** Camera doesn't start or capture fails

**Solutions:**
1. Check camera permissions in AndroidManifest.xml
2. Verify Quest device has camera access enabled
3. Try restarting the app
4. Check if `useWebCamTexture` is enabled

### API Errors

**Problem:** OpenRouter API returns errors

**Solutions:**
1. Verify API key is correct
2. Check internet connection
3. Verify model name is correct: `google/gemma-3-27b-it:free`
4. Check OpenRouter account has credits/quota
5. Review error message in console

### No Response

**Problem:** Request sent but no response received

**Solutions:**
1. Check `OnResponseReceived` event is subscribed
2. Enable debug mode to see logs
3. Check network connectivity
4. Verify API key has proper permissions

### Image Quality Issues

**Problem:** Captured images are blurry or low quality

**Solutions:**
1. Increase `captureWidth` and `captureHeight`
2. Ensure camera is focused
3. Check lighting conditions
4. Verify camera is playing before capture

---

## 📊 Performance Notes

- **Capture Resolution:** 1280x960 is recommended for balance of quality and speed
- **API Latency:** Free tier may have slower responses (5-30 seconds)
- **Camera FPS:** Passthrough camera runs at ~30 FPS
- **Memory:** Large images use more memory, encode as PNG for quality

---

## 🔒 Privacy & Security

### Important Considerations:

1. **Camera Access:** Users must grant camera permissions
2. **Data Privacy:** Images are sent to OpenRouter/Google servers
3. **API Key Security:** Never commit API keys to version control
4. **User Consent:** Inform users that images are sent to external services

### Best Practices:

- Store API key securely (use Unity's PlayerPrefs or secure storage)
- Add user consent dialog before capturing
- Consider local processing for sensitive content
- Implement rate limiting to prevent abuse

---

## 📚 API Reference

### Methods

| Method | Description |
|--------|-------------|
| `CaptureAndSendToGemma(string prompt)` | Captures image and sends to Gemma |
| `StartPassthroughCamera()` | Starts the passthrough camera |
| `StopPassthroughCamera()` | Stops the passthrough camera |
| `GetPassthroughTexture()` | Gets preview texture |
| `SetApiKey(string key)` | Sets API key programmatically |

### Events

| Event | Description |
|-------|-------------|
| `OnResponseReceived` | Fired when response is received |
| `OnErrorOccurred` | Fired when an error occurs |

---

## 🎮 Example Integration

### For Your AR Ikebana App:

```csharp
// In your flower arrangement script
public void AnalyzeArrangement()
{
    PassthroughToGemmaSender.Instance.CaptureAndSendToGemma(
        "I'm creating an ikebana flower arrangement. " +
        "Analyze the current arrangement and provide feedback on balance, " +
        "color harmony, and traditional ikebana principles."
    );
}

// Subscribe to get feedback
void Start()
{
    PassthroughToGemmaSender.Instance.OnResponseReceived += OnArrangementFeedback;
}

void OnArrangementFeedback(string feedback)
{
    // Display feedback to user
    Debug.Log($"Ikebana Feedback: {feedback}");
}
```

---

## 🔗 Resources

- [OpenRouter Documentation](https://openrouter.ai/docs)
- [Google Gemma Models](https://ai.google.dev/gemma)
- [Meta Passthrough Camera API](https://developer.oculus.com/documentation/unity/unity-passthrough-camera-api/)
- [Unity WebCamTexture](https://docs.unity3d.com/ScriptReference/WebCamTexture.html)

---

## ⚠️ Notes

- **Free Tier Limitations:** Free tier may have rate limits
- **Device Requirements:** Requires Quest device with passthrough support
- **Network Required:** Internet connection needed for API calls
- **Latency:** API responses may take 5-30 seconds depending on model and tier

