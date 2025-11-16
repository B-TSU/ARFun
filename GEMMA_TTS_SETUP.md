# Gemma Text-to-Speech Setup Guide

## Overview

This guide explains how to set up the `GemmaTextToSpeech` script to convert Gemma API responses to speech using Meta Voice SDK.

---

## 🚀 Quick Setup

### 1. **Add GemmaTextToSpeech Component**

1. Create an empty GameObject (e.g., "GemmaTTS")
2. Add the `GemmaTextToSpeech` component
3. The script will automatically find `PassthroughToGemmaSender` and subscribe to its events

### 2. **Set Up TTSSpeaker Component**

1. Create a GameObject for TTS (e.g., "TTSSpeaker")
2. Add `TTSSpeaker` component (from Meta Voice SDK)
3. Configure TTS settings:
   - **Voice Settings**: Choose voice preset
   - **TTS Service**: Set up Wit.ai TTS service (if using cloud TTS)
   - **Audio Source**: Assign an AudioSource component

### 3. **Assign TTSSpeaker Reference**

1. Select the GameObject with `GemmaTextToSpeech`
2. In Inspector, drag the `TTSSpeaker` GameObject to the **TTS Speaker** field
   - Or leave it empty - the script will auto-find it

### 4. **Configure Settings**

- **Auto Speak**: ✅ Enable to automatically speak Gemma responses
- **Queue Speeches**: Enable to queue multiple speeches
- **Speech Rate**: Adjust speech speed (0.1 - 3.0)

---

## 📝 Usage

### Automatic (Default)

When `autoSpeak` is enabled, the script automatically speaks Gemma responses:

```csharp
// No code needed - it works automatically!
// When PassthroughToGemmaSender receives a response, it's automatically spoken
```

### Manual Control

```csharp
// Speak text manually
GemmaTextToSpeech.Instance.SpeakText("Hello, this is a test");

// Stop current speech
GemmaTextToSpeech.Instance.StopSpeaking();

// Check if speaking
bool isSpeaking = GemmaTextToSpeech.Instance.IsSpeaking();

// Adjust speech rate
GemmaTextToSpeech.Instance.SetSpeechRate(1.5f); // 1.5x speed
```

### Event-Based Usage

```csharp
// Subscribe to TTS events
GemmaTextToSpeech.Instance.OnSpeechStarted += (text) => {
    Debug.Log($"Started speaking: {text}");
};

GemmaTextToSpeech.Instance.OnSpeechCompleted += (text) => {
    Debug.Log($"Finished speaking: {text}");
};
```

---

## ⚙️ Configuration

### Inspector Settings

| Setting | Description | Default |
|---------|-------------|---------|
| **TTS Speaker** | TTSSpeaker component reference | (Auto-finds) |
| **Auto Speak** | Automatically speak Gemma responses | ✅ true |
| **Queue Speeches** | Queue multiple speeches | false |
| **Speech Rate** | Speech speed multiplier | 1.0 |
| **Debug Mode** | Enable debug logs | true |

---

## 🔧 TTSSpeaker Setup

### Option 1: Local TTS (No Internet Required)

1. Add `TTSSpeaker` component
2. Use built-in TTS voices
3. No additional setup needed

### Option 2: Cloud TTS (Wit.ai)

1. **Create Wit.ai App:**
   - Go to [wit.ai](https://wit.ai/)
   - Create a new app
   - Get your Server Access Token

2. **Set Up TTSWitService:**
   - Add `TTSWitService` component
   - Enter your Wit.ai Server Access Token
   - Assign to TTSSpeaker's TTS Service field

3. **Configure Voice:**
   - Choose voice preset in TTSSpeaker
   - Test voice in Inspector

---

## 🎯 Integration Flow

```
1. User captures image → PassthroughToGemmaSender
2. Image sent to Gemma API → OpenRouter
3. Gemma responds with text → OnResponseReceived event
4. GemmaTextToSpeech receives text → Automatically speaks
5. User hears the response → TTS audio playback
```

---

## 📊 Events

### GemmaTextToSpeech Events

| Event | Description |
|-------|-------------|
| `OnSpeechStarted` | Fired when speech starts |
| `OnSpeechCompleted` | Fired when speech finishes |
| `OnSpeechCancelled` | Fired when speech is cancelled |

### Usage Example

```csharp
void Start()
{
    GemmaTextToSpeech.Instance.OnSpeechStarted += OnSpeechStarted;
    GemmaTextToSpeech.Instance.OnSpeechCompleted += OnSpeechCompleted;
}

void OnSpeechStarted(string text)
{
    // Show "Speaking..." UI
    Debug.Log($"Speaking: {text}");
}

void OnSpeechCompleted(string text)
{
    // Hide "Speaking..." UI
    Debug.Log("Speech finished");
}
```

---

## 🔍 Troubleshooting

### No Speech Output

**Problem:** Text is received but no speech plays

**Solutions:**
1. Check `TTSSpeaker` is assigned and enabled
2. Verify `AudioSource` is assigned to TTSSpeaker
3. Check audio volume settings
4. Verify Meta Voice SDK is installed
5. Check console for errors

### Speech Cuts Off

**Problem:** Long responses get cut off

**Solutions:**
1. Enable `Queue Speeches` for long texts
2. Split text into chunks manually
3. Increase audio buffer size

### Wrong Voice

**Problem:** Voice doesn't match expectations

**Solutions:**
1. Check TTSSpeaker voice settings
2. Verify TTS service configuration
3. Try different voice presets

### Auto-Speak Not Working

**Problem:** Responses not automatically spoken

**Solutions:**
1. Check `Auto Speak` is enabled
2. Verify `PassthroughToGemmaSender` is initialized
3. Check event subscription in console logs
4. Verify `GemmaTextToSpeech` is initialized before responses arrive

---

## 💡 Advanced Usage

### Custom Text Processing

```csharp
// Process text before speaking
void OnGemmaResponseReceived(string text)
{
    // Clean up text
    string cleanedText = text.Trim();
    
    // Add pauses for better speech
    cleanedText = cleanedText.Replace(".", ". ");
    
    // Speak processed text
    GemmaTextToSpeech.Instance.SpeakText(cleanedText);
}
```

### Queue Management

```csharp
// Queue multiple speeches
GemmaTextToSpeech.Instance.SetQueueSpeeches(true);
GemmaTextToSpeech.Instance.SpeakText("First message");
GemmaTextToSpeech.Instance.SpeakText("Second message");
GemmaTextToSpeech.Instance.SpeakText("Third message");
// All will play in sequence
```

### Speech Rate Control

```csharp
// Slow speech for important information
GemmaTextToSpeech.Instance.SetSpeechRate(0.8f);
GemmaTextToSpeech.Instance.SpeakText("Important announcement");

// Fast speech for quick updates
GemmaTextToSpeech.Instance.SetSpeechRate(1.5f);
GemmaTextToSpeech.Instance.SpeakText("Quick update");
```

---

## 🎮 For Your AR Ikebana App

### Example Integration

```csharp
// In your flower arrangement script
public void AnalyzeArrangement()
{
    // Capture and send to Gemma
    PassthroughToGemmaSender.Instance.CaptureAndSendToGemma(
        "Analyze this ikebana arrangement and provide feedback."
    );
    
    // TTS will automatically speak the response!
}
```

### Custom Feedback Flow

```csharp
void Start()
{
    // Subscribe to get feedback
    PassthroughToGemmaSender.Instance.OnResponseReceived += OnArrangementFeedback;
    GemmaTextToSpeech.Instance.OnSpeechCompleted += OnFeedbackComplete;
}

void OnArrangementFeedback(string feedback)
{
    // Display feedback in UI
    feedbackText.text = feedback;
    
    // TTS automatically speaks it (if autoSpeak enabled)
}

void OnFeedbackComplete(string text)
{
    // Show "Feedback complete" message
    Debug.Log("User has heard the feedback");
}
```

---

## 📚 Resources

- [Meta Voice SDK Documentation](https://developer.oculus.com/documentation/unity/unity-voice-sdk-overview/)
- [TTSSpeaker API Reference](https://developer.oculus.com/documentation/unity/unity-voice-sdk-tts/)
- [Wit.ai Setup Guide](https://wit.ai/docs)

---

## ⚠️ Notes

- **Voice SDK Required:** TTS requires Meta Voice SDK package
- **Audio Source:** TTSSpeaker needs an AudioSource component
- **Permissions:** May require microphone permissions for some features
- **Network:** Cloud TTS requires internet connection
- **Performance:** Long texts may take time to generate speech

---

## ✅ Quick Checklist

- [ ] Add `GemmaTextToSpeech` component to GameObject
- [ ] Add `TTSSpeaker` component to GameObject
- [ ] Assign TTSSpeaker to GemmaTextToSpeech (or let it auto-find)
- [ ] Enable `Auto Speak` if desired
- [ ] Test by calling `CaptureAndSendToGemma()`
- [ ] Verify audio output works
- [ ] Adjust speech rate if needed

---

Your Gemma responses will now be automatically converted to speech! 🎤

