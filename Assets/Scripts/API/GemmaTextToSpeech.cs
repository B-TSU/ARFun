using UnityEngine;
using System;
#if META_VOICE_SDK_AVAILABLE
using Meta.Voice;
using Meta.WitAi.TTS;
using Meta.WitAi.TTS.Utilities;
#endif

/// <summary>
/// Converts Gemma API text responses to speech using Meta Voice SDK TTS
/// </summary>
public class GemmaTextToSpeech : MonoBehaviour
{
    public static GemmaTextToSpeech Instance { get; private set; }

    #region Serialized Fields
    [Header("TTS Settings")]
#if META_VOICE_SDK_AVAILABLE
    [SerializeField] private TTSSpeaker ttsSpeaker;
#else
    [SerializeField] private MonoBehaviour ttsSpeaker; // Placeholder when Voice SDK not available
#endif
    [SerializeField] private bool autoSpeak = true;
    [SerializeField] private bool queueSpeeches = false;
    [SerializeField] private float speechRate = 1.0f;

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    #endregion

    #region Private Fields
    private bool isInitialized = false;
    private int subscriptionRetryCount = 0;
    private const int MAX_SUBSCRIPTION_RETRIES = 10;
    #endregion

    #region Events
    public event Action<string> OnSpeechStarted;
    public event Action<string> OnSpeechCompleted;
    public event Action<string> OnSpeechCancelled;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Initialize();
    }
    #endregion

    #region Initialization
    private void Initialize()
    {
        FindTTSSpeaker();
        SubscribeToGemmaEvents();
        isInitialized = true;
    }

    private void FindTTSSpeaker()
    {
#if META_VOICE_SDK_AVAILABLE
        if (ttsSpeaker == null)
        {
            ttsSpeaker = FindFirstObjectByType<TTSSpeaker>();
            
            if (ttsSpeaker == null)
            {
                Debug.LogWarning("GemmaTextToSpeech: TTSSpeaker component not found. " +
                    "Please add TTSSpeaker to a GameObject in the scene or assign it in the Inspector.");
            }
            else if (debugMode)
            {
                Debug.Log("GemmaTextToSpeech: Found TTSSpeaker component");
            }
        }

        if (ttsSpeaker != null)
        {
            SubscribeToTTSEvents();
            SetSpeechRate(speechRate);
        }
#else
        if (debugMode)
        {
            Debug.LogWarning("GemmaTextToSpeech: Meta Voice SDK not available. TTS requires Meta Voice SDK.");
        }
#endif
    }

    private void SubscribeToGemmaEvents()
    {
        if (PassthroughToGemmaSender.Instance != null)
        {
            PassthroughToGemmaSender.Instance.OnResponseReceived += OnGemmaResponseReceived;
            subscriptionRetryCount = 0; // Reset retry count on success
            if (debugMode)
            {
                Debug.Log("GemmaTextToSpeech: Subscribed to PassthroughToGemmaSender events");
            }
        }
        else
        {
            // Retry subscription if PassthroughToGemmaSender hasn't initialized yet
            if (subscriptionRetryCount < MAX_SUBSCRIPTION_RETRIES)
            {
                subscriptionRetryCount++;
                if (debugMode)
                {
                    Debug.LogWarning($"GemmaTextToSpeech: PassthroughToGemmaSender.Instance not found. " +
                        $"Retrying subscription ({subscriptionRetryCount}/{MAX_SUBSCRIPTION_RETRIES})...");
                }
                // Retry in next frame
                StartCoroutine(RetrySubscriptionCoroutine());
            }
            else
            {
                Debug.LogError("GemmaTextToSpeech: PassthroughToGemmaSender.Instance not found after multiple retries. " +
                    "TTS will not work until PassthroughToGemmaSender is initialized.");
            }
        }
    }

    private System.Collections.IEnumerator RetrySubscriptionCoroutine()
    {
        yield return null; // Wait one frame
        SubscribeToGemmaEvents(); // Retry subscription
    }

#if META_VOICE_SDK_AVAILABLE
    private void SubscribeToTTSEvents()
    {
        if (ttsSpeaker != null)
        {
            // Subscribe to TTS events if available
            // Note: TTSSpeaker events may vary by SDK version
            // These are common events in Meta Voice SDK
            ttsSpeaker.Events.OnPlaybackStart.AddListener(OnTTSSpeechStarted);
            ttsSpeaker.Events.OnPlaybackComplete.AddListener(OnTTSSpeechCompleted);
            ttsSpeaker.Events.OnPlaybackCancelled.AddListener(OnTTSSpeechCancelled);
        }
    }
#endif
    #endregion

    #region Event Handlers
    private void OnGemmaResponseReceived(string text)
    {
        if (autoSpeak && !string.IsNullOrEmpty(text))
        {
            SpeakText(text);
        }
    }

#if META_VOICE_SDK_AVAILABLE
    private void OnTTSSpeechStarted(string text)
    {
        LogDebug($"Speech started: {text}");
        OnSpeechStarted?.Invoke(text);
    }

    private void OnTTSSpeechCompleted(string text)
    {
        LogDebug($"Speech completed: {text}");
        OnSpeechCompleted?.Invoke(text);
    }

    private void OnTTSSpeechCancelled(string text)
    {
        LogDebug($"Speech cancelled: {text}");
        OnSpeechCancelled?.Invoke(text);
    }
#endif
    #endregion

    #region Public API
    /// <summary>
    /// Speaks the given text using TTS
    /// </summary>
    public void SpeakText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("GemmaTextToSpeech: Cannot speak empty text");
            return;
        }

        // Safety check: Ensure component is active
        if (!isActiveAndEnabled)
        {
            Debug.LogWarning("GemmaTextToSpeech: Component is not active. Cannot speak.");
            return;
        }

#if META_VOICE_SDK_AVAILABLE
        if (ttsSpeaker == null)
        {
            Debug.LogError("GemmaTextToSpeech: TTSSpeaker not available. Cannot speak text.");
            return;
        }

        // Safety check: Ensure TTS speaker GameObject is still valid
        if (ttsSpeaker.gameObject == null)
        {
            Debug.LogError("GemmaTextToSpeech: TTSSpeaker GameObject is destroyed. Cannot speak text.");
            return;
        }

        try
        {
            if (queueSpeeches)
            {
                ttsSpeaker.SpeakQueued(text);
                LogDebug($"Queued speech: {text}");
            }
            else
            {
                ttsSpeaker.Speak(text);
                LogDebug($"Speaking: {text}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"GemmaTextToSpeech: Error speaking text: {e.Message}");
        }
#else
        Debug.LogWarning("GemmaTextToSpeech: Meta Voice SDK not available. Cannot speak text.");
#endif
    }

    /// <summary>
    /// Stops the current speech
    /// </summary>
    public void StopSpeaking()
    {
#if META_VOICE_SDK_AVAILABLE
        if (ttsSpeaker != null)
        {
            ttsSpeaker.Stop();
            LogDebug("Speech stopped");
        }
#else
        Debug.LogWarning("GemmaTextToSpeech: Meta Voice SDK not available.");
#endif
    }

    /// <summary>
    /// Sets the speech rate (speed)
    /// </summary>
    public void SetSpeechRate(float rate)
    {
        speechRate = Mathf.Clamp(rate, 0.1f, 3.0f);
        
#if META_VOICE_SDK_AVAILABLE
        if (ttsSpeaker != null)
        {
            // Set speech rate if supported by TTSSpeaker
            // Note: This may vary by SDK version
            if (ttsSpeaker.VoiceSettings != null)
            {
                ttsSpeaker.VoiceSettings.Speed = speechRate;
            }
            LogDebug($"Speech rate set to: {speechRate}");
        }
#endif
    }

    /// <summary>
    /// Sets whether to automatically speak Gemma responses
    /// </summary>
    public void SetAutoSpeak(bool enabled)
    {
        autoSpeak = enabled;
    }

    /// <summary>
    /// Sets whether to queue multiple speeches
    /// </summary>
    public void SetQueueSpeeches(bool enabled)
    {
        queueSpeeches = enabled;
    }

    /// <summary>
    /// Checks if TTS is currently speaking
    /// </summary>
    public bool IsSpeaking()
    {
#if META_VOICE_SDK_AVAILABLE
        return ttsSpeaker != null && ttsSpeaker.IsSpeaking;
#else
        return false;
#endif
    }
    #endregion

    #region Utility Methods
    private void LogDebug(string message)
    {
        if (debugMode)
        {
            Debug.Log($"GemmaTextToSpeech: {message}");
        }
    }
    #endregion

    #region Cleanup
    private void OnDisable()
    {
        // Unsubscribe when disabled to prevent crashes
        UnsubscribeFromEvents();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        UnsubscribeFromEvents();
    }

    private void UnsubscribeFromEvents()
    {
        // Unsubscribe from PassthroughToGemmaSender events
        if (PassthroughToGemmaSender.Instance != null)
        {
            PassthroughToGemmaSender.Instance.OnResponseReceived -= OnGemmaResponseReceived;
        }

#if META_VOICE_SDK_AVAILABLE
        // Unsubscribe from TTS events
        if (ttsSpeaker != null)
        {
            try
            {
                ttsSpeaker.Events.OnPlaybackStart.RemoveListener(OnTTSSpeechStarted);
                ttsSpeaker.Events.OnPlaybackComplete.RemoveListener(OnTTSSpeechCompleted);
                ttsSpeaker.Events.OnPlaybackCancelled.RemoveListener(OnTTSSpeechCancelled);
            }
            catch (Exception e)
            {
                // Silently handle if events are already removed or object is destroyed
                if (debugMode)
                {
                    Debug.LogWarning($"GemmaTextToSpeech: Error unsubscribing from TTS events: {e.Message}");
                }
            }
        }
#endif
    }
    #endregion
}

