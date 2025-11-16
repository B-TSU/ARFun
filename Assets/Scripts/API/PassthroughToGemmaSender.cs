using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;
#if META_XR_SDK_AVAILABLE
using Meta.XR;
#endif

/// <summary>
/// Captures images from passthrough camera and sends them to Google Gemma 3 27B via OpenRouter
/// </summary>
public class PassthroughToGemmaSender : MonoBehaviour
{
    public static PassthroughToGemmaSender Instance { get; private set; }

    #region Constants
    private const string DEFAULT_PROMPT = "Analyze this image and describe what you see.";
    private const string JSON_CONTENT_TYPE = "application/json";
    private const string IMAGE_DATA_URI_PREFIX = "data:image/png;base64,";
    #endregion

    #region Serialized Fields
    [Header("OpenRouter API Settings")]
    [Tooltip("API key is stored securely. Use SetApiKey() method or SecretsManager to set it.")]
    private string openRouterApiKey;
    [SerializeField] private string modelName = "google/gemma-3-27b-it:free";
    [SerializeField] private string apiUrl = "https://openrouter.ai/api/v1/chat/completions";

    [Header("Camera Settings")]
    [SerializeField] private int captureWidth = 1280;
    [SerializeField] private int captureHeight = 960;
#if META_XR_SDK_AVAILABLE
    [SerializeField] private PassthroughCameraAccess passthroughCameraAccess;
#else
    [SerializeField] private MonoBehaviour passthroughCameraAccess; // Placeholder when Meta SDK not available
#endif

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    #endregion

    #region Private Fields
    private Camera arCamera;
    private bool isCapturing = false;
    #endregion

    #region Events
    public event Action<string> OnResponseReceived;
    public event Action<string> OnErrorOccurred;
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
        LoadApiKey();
        FindARCamera();
        FindPassthroughCameraAccess();
    }

    private void LoadApiKey()
    {
        openRouterApiKey = SecretsManager.GetOpenRouterApiKey();
        
        if (string.IsNullOrEmpty(openRouterApiKey))
        {
            Debug.LogWarning("PassthroughToGemmaSender: No API key found! " +
                "Please set it using PassthroughToGemmaSender.Instance.SetApiKey() or SecretsManager.SetOpenRouterApiKey()");
        }
        else if (debugMode)
        {
            Debug.Log("PassthroughToGemmaSender: API key loaded from secure storage");
        }
    }

    private void FindARCamera()
    {
        // Try to find camera from Camera Rig
        GameObject cameraRig = GameObject.Find("[BuildingBlock] Camera Rig");
        if (cameraRig != null)
        {
            Transform centerEye = cameraRig.transform.Find("TrackingSpace/CenterEyeAnchor");
            if (centerEye != null)
            {
                arCamera = centerEye.GetComponent<Camera>();
            }
        }

        // Fallback to main camera
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }

        if (arCamera == null)
        {
            Debug.LogError("PassthroughToGemmaSender: Could not find AR camera!");
        }
    }

    private void FindPassthroughCameraAccess()
    {
#if META_XR_SDK_AVAILABLE
        if (passthroughCameraAccess == null)
        {
            passthroughCameraAccess = FindFirstObjectByType<PassthroughCameraAccess>();
            
            if (passthroughCameraAccess == null)
            {
                Debug.LogWarning("PassthroughToGemmaSender: PassthroughCameraAccess component not found. " +
                    "Please add it to a GameObject in the scene or assign it in the Inspector.");
            }
            else if (debugMode)
            {
                Debug.Log("PassthroughToGemmaSender: Found PassthroughCameraAccess component");
            }
        }
#else
        if (debugMode)
        {
            Debug.LogWarning("PassthroughToGemmaSender: Meta XR SDK not available. PassthroughCameraAccess requires Meta SDK.");
        }
#endif
    }
    #endregion

    #region Public API
    /// <summary>
    /// Captures an image from the passthrough camera and sends it to Gemma
    /// </summary>
    public void CaptureAndSendToGemma(string prompt = DEFAULT_PROMPT)
    {
        if (isCapturing)
        {
            Debug.LogWarning("PassthroughToGemmaSender: Already capturing, please wait...");
            return;
        }

        if (string.IsNullOrEmpty(openRouterApiKey))
        {
            Debug.LogError("PassthroughToGemmaSender: API key not set!");
            OnErrorOccurred?.Invoke("API key not set");
            return;
        }

        StartCoroutine(CaptureAndSendCoroutine(prompt));
    }

    /// <summary>
    /// Enables the passthrough camera access
    /// </summary>
    public void StartPassthroughCamera()
    {
#if META_XR_SDK_AVAILABLE
        if (passthroughCameraAccess != null)
        {
            passthroughCameraAccess.enabled = true;
            LogDebug("Passthrough camera access enabled");
        }
        else
        {
            Debug.LogWarning("PassthroughToGemmaSender: PassthroughCameraAccess component not found");
        }
#else
        Debug.LogWarning("PassthroughToGemmaSender: Meta XR SDK not available");
#endif
    }

    /// <summary>
    /// Disables the passthrough camera access
    /// </summary>
    public void StopPassthroughCamera()
    {
#if META_XR_SDK_AVAILABLE
        if (passthroughCameraAccess != null)
        {
            passthroughCameraAccess.enabled = false;
            LogDebug("Passthrough camera access disabled");
        }
#else
        Debug.LogWarning("PassthroughToGemmaSender: Meta XR SDK not available");
#endif
    }

    /// <summary>
    /// Sets the OpenRouter API key securely
    /// </summary>
    public void SetApiKey(string apiKey)
    {
        openRouterApiKey = apiKey;
        SecretsManager.SetOpenRouterApiKey(apiKey);
        LogDebug("API key set and saved securely");
    }

    /// <summary>
    /// Checks if API key is set
    /// </summary>
    public bool HasApiKey()
    {
        return !string.IsNullOrEmpty(openRouterApiKey) && 
               openRouterApiKey != "YOUR_OPENROUTER_API_KEY";
    }

    /// <summary>
    /// Gets the current passthrough camera texture (for preview)
    /// Returns null if PassthroughCameraAccess is not available or permission not granted
    /// </summary>
    public Texture GetPassthroughTexture()
    {
#if META_XR_SDK_AVAILABLE
        if (IsPassthroughCameraAvailable())
        {
            return passthroughCameraAccess.GetTexture();
        }
#endif
        return null;
    }
    #endregion

    #region Capture Logic
    private IEnumerator CaptureAndSendCoroutine(string prompt)
    {
        isCapturing = true;

        try
        {
            // Capture image
            Texture2D capturedImage = CaptureImage();
            if (capturedImage == null)
            {
                HandleCaptureError("Failed to capture image");
                yield break;
            }

            // Convert to base64
            string base64Image = ConvertTextureToBase64(capturedImage);
            LogDebug($"Image captured and converted. Size: {capturedImage.width}x{capturedImage.height}");

            // Send to OpenRouter
            yield return StartCoroutine(SendToOpenRouter(base64Image, prompt));

            // Cleanup
            Destroy(capturedImage);
        }
        finally
        {
            isCapturing = false;
        }
    }

    private Texture2D CaptureImage()
    {
#if META_XR_SDK_AVAILABLE
        if (IsPassthroughCameraAvailable())
        {
            return CaptureFromPassthroughCamera();
        }
#endif
        
        if (arCamera != null)
        {
            return CaptureFromCamera();
        }

        Debug.LogError("PassthroughToGemmaSender: No camera available for capture!");
        return null;
    }

    private Texture2D CaptureFromPassthroughCamera()
    {
#if META_XR_SDK_AVAILABLE
        if (!IsPassthroughCameraAvailable())
        {
            Debug.LogError("PassthroughToGemmaSender: PassthroughCameraAccess is not available or not enabled");
            return null;
        }

        Texture sourceTexture = passthroughCameraAccess.GetTexture();
        
        if (sourceTexture == null)
        {
            Debug.LogWarning("PassthroughToGemmaSender: PassthroughCameraAccess.GetTexture() returned null. " +
                "Make sure 'horizonos.permission.HEADSET_CAMERA' permission has been granted.");
            return null;
        }

        return ConvertTextureToTexture2D(sourceTexture);
#else
        return null;
#endif
    }

    private Texture2D CaptureFromCamera()
    {
        if (arCamera == null)
        {
            return null;
        }

        RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        arCamera.targetTexture = renderTexture;
        Texture2D screenshot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);

        arCamera.Render();
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        screenshot.Apply();

        arCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        return screenshot;
    }

    private Texture2D ConvertTextureToTexture2D(Texture sourceTexture)
    {
        if (sourceTexture == null)
        {
            return null;
        }

        // Handle RenderTexture
        if (sourceTexture is RenderTexture renderTexture)
        {
            return ReadFromRenderTexture(renderTexture);
        }

        // Handle Texture2D
        if (sourceTexture is Texture2D texture2D)
        {
            return CopyTexture2D(texture2D);
        }

        // Handle other texture types by blitting to RenderTexture
        return ConvertViaBlit(sourceTexture);
    }

    private Texture2D ReadFromRenderTexture(RenderTexture renderTexture)
    {
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        return texture;
    }

    private Texture2D CopyTexture2D(Texture2D source)
    {
        Texture2D texture = new Texture2D(source.width, source.height, TextureFormat.RGB24, false);
        texture.SetPixels(source.GetPixels());
        texture.Apply();
        return texture;
    }

    private Texture2D ConvertViaBlit(Texture sourceTexture)
    {
        RenderTexture tempRT = RenderTexture.GetTemporary(captureWidth, captureHeight, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(sourceTexture, tempRT);
        RenderTexture.active = tempRT;
        Texture2D texture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(tempRT);
        return texture;
    }
    #endregion

    #region API Communication
    private IEnumerator SendToOpenRouter(string base64Image, string prompt)
    {
        string jsonPayload = BuildJsonPayload(base64Image, prompt);
        LogDebug("Sending request to OpenRouter...");

        using (UnityWebRequest request = CreateWebRequest(jsonPayload))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                HandleApiSuccess(request.downloadHandler.text);
            }
            else
            {
                HandleApiError(request.error, request.downloadHandler.text);
            }
        }
    }

    private UnityWebRequest CreateWebRequest(string jsonPayload)
    {
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        
        request.SetRequestHeader("Content-Type", JSON_CONTENT_TYPE);
        request.SetRequestHeader("Authorization", $"Bearer {openRouterApiKey}");
        request.SetRequestHeader("HTTP-Referer", Application.absoluteURL);
        request.SetRequestHeader("X-Title", Application.productName);
        
        return request;
    }

    private string BuildJsonPayload(string base64Image, string prompt)
    {
        string escapedPrompt = EscapeJsonString(prompt);
        string imageUrl = IMAGE_DATA_URI_PREFIX + base64Image;
        
        return $@"{{
            ""model"": ""{modelName}"",
            ""messages"": [
                {{
                    ""role"": ""user"",
                    ""content"": [
                        {{
                            ""type"": ""text"",
                            ""text"": ""{escapedPrompt}""
                        }},
                        {{
                            ""type"": ""image_url"",
                            ""image_url"": {{
                                ""url"": ""{imageUrl}""
                            }}
                        }}
                    ]
                }}
            ]
        }}";
    }

    private void HandleApiSuccess(string responseText)
    {
        LogDebug($"Response received: {responseText}");
        ParseOpenRouterResponse(responseText);
    }

    private void HandleApiError(string error, string responseBody)
    {
        string errorMessage = $"Error: {error} - {responseBody}";
        Debug.LogError($"PassthroughToGemmaSender: {errorMessage}");
        OnErrorOccurred?.Invoke(errorMessage);
    }

    private void HandleCaptureError(string message)
    {
        Debug.LogError($"PassthroughToGemmaSender: {message}");
        OnErrorOccurred?.Invoke(message);
    }
    #endregion

    #region Response Parsing
    private void ParseOpenRouterResponse(string jsonResponse)
    {
        try
        {
            string content = ExtractContentFromResponse(jsonResponse);
            
            if (!string.IsNullOrEmpty(content))
            {
                LogDebug($"Gemma response: {content}");
                OnResponseReceived?.Invoke(content);
            }
            else
            {
                // Fallback: return full response
                OnResponseReceived?.Invoke(jsonResponse);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"PassthroughToGemmaSender: Error parsing response: {e.Message}");
            OnErrorOccurred?.Invoke($"Parse error: {e.Message}");
        }
    }

    private string ExtractContentFromResponse(string jsonResponse)
    {
        const string contentMarker = "\"content\":\"";
        int contentStart = jsonResponse.IndexOf(contentMarker);
        
        if (contentStart < 0)
        {
            return null;
        }

        contentStart += contentMarker.Length;
        int contentEnd = jsonResponse.IndexOf("\"", contentStart);
        
        if (contentEnd <= contentStart)
        {
            return null;
        }

        string content = jsonResponse.Substring(contentStart, contentEnd - contentStart);
        return UnescapeJsonString(content);
    }

    private string UnescapeJsonString(string input)
    {
        return input.Replace("\\n", "\n")
                    .Replace("\\\"", "\"")
                    .Replace("\\\\", "\\");
    }
    #endregion

    #region Utility Methods
    private string ConvertTextureToBase64(Texture2D texture)
    {
        byte[] imageBytes = texture.EncodeToPNG();
        return Convert.ToBase64String(imageBytes);
    }

    private string EscapeJsonString(string input)
    {
        return input.Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\n", "\\n");
    }

    private void LogDebug(string message)
    {
        if (debugMode)
        {
            Debug.Log($"PassthroughToGemmaSender: {message}");
        }
    }

#if META_XR_SDK_AVAILABLE
    private bool IsPassthroughCameraAvailable()
    {
        return passthroughCameraAccess != null && passthroughCameraAccess.enabled;
    }
#endif
    #endregion
}
