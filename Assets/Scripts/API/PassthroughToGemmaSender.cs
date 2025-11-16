using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;

/// <summary>
/// Captures passthrough camera image and sends it to OpenRouter API (Gemma 3 27B)
/// to get tips on how to produce a better bouquet
/// </summary>
public class PassthroughToGemmaSender : MonoBehaviour
{
    public static PassthroughToGemmaSender Instance { get; private set; }

    #region Serialized Fields
    [Header("OpenRouter API Settings")]
    [SerializeField] private string modelName = "google/gemma-3-27b-it:free";
    [SerializeField] private string apiUrl = "https://openrouter.ai/api/v1/chat/completions";

    [Header("Camera Settings")]
    [SerializeField] private int captureWidth = 1024;
    [SerializeField] private int captureHeight = 768;
    [SerializeField] private Camera passthroughCamera;
    
    [Header("Passthrough Camera Access")]
    [SerializeField] private MonoBehaviour passthroughCameraAccess; // PassthroughCameraAccess component reference

    [Header("Prompt Settings")]
    [SerializeField] private string promptText = "Please analyze this image of my ikebana bouquet and provide tips on how to produce a better bouquet. Focus on flower arrangement, balance, color harmony, and traditional ikebana principles.";

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    #endregion

    #region Private Fields
    private RenderTexture captureTexture;
    private bool isCapturing = false;
    private bool isProcessing = false;
    #endregion

    #region Events
    public event Action<string> OnResponseReceived;
    public event Action<string> OnError;
    public event Action OnCaptureStarted;
    public event Action OnCaptureCompleted;
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
            return;
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        CleanupRenderTexture();
    }
    #endregion

    #region Initialization
    private void Initialize()
    {
        FindPassthroughCamera();
        CreateRenderTexture();
    }

    private void FindPassthroughCamera()
    {
        if (passthroughCamera != null)
        {
            LogDebug("Passthrough camera already assigned");
            return;
        }

        // Try to find camera from Meta Building Block Camera Rig
        GameObject cameraRig = GameObject.Find("[BuildingBlock] Camera Rig");
        if (cameraRig != null)
        {
            Transform trackingSpace = cameraRig.transform.Find("TrackingSpace");
            if (trackingSpace != null)
            {
                Transform centerEyeAnchor = trackingSpace.Find("CenterEyeAnchor");
                if (centerEyeAnchor != null)
                {
                    passthroughCamera = centerEyeAnchor.GetComponent<Camera>();
                    if (passthroughCamera != null)
                    {
                        LogDebug("Found passthrough camera from Camera Rig");
                        return;
                    }
                }
            }
        }

        // Fallback: Try to find by name
        GameObject passthroughObj = GameObject.Find("Meta_Passthrough");
        if (passthroughObj != null)
        {
            passthroughCamera = passthroughObj.GetComponent<Camera>();
            if (passthroughCamera != null)
            {
                LogDebug("Found passthrough camera by name");
                return;
            }
        }

        // Fallback: Use main camera
        passthroughCamera = Camera.main;
        if (passthroughCamera != null)
        {
            LogDebug("Using Main Camera as fallback");
        }
        else
        {
            Debug.LogError("PassthroughToGemmaSender: Could not find passthrough camera!");
        }
    }

    private void CreateRenderTexture()
    {
        if (captureTexture != null)
        {
            return;
        }

        captureTexture = new RenderTexture(captureWidth, captureHeight, 24, RenderTextureFormat.ARGB32);
        captureTexture.Create();
        LogDebug($"Created render texture: {captureWidth}x{captureHeight}");
    }

    private void CleanupRenderTexture()
    {
        if (captureTexture != null)
        {
            captureTexture.Release();
            Destroy(captureTexture);
            captureTexture = null;
        }
    }
    #endregion

    #region Public API
    /// <summary>
    /// Captures image from passthrough camera and sends to Gemma API
    /// </summary>
    public void CaptureAndSend()
    {
        if (isCapturing || isProcessing)
        {
            LogDebug("Already capturing or processing. Please wait.");
            return;
        }

        if (passthroughCamera == null)
        {
            Debug.LogError("PassthroughToGemmaSender: Passthrough camera not found!");
            OnError?.Invoke("Passthrough camera not found");
            return;
        }

        string apiKey = SecretsManager.GetOpenRouterApiKey();
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("PassthroughToGemmaSender: OpenRouter API key not set! Use SecretsManager.SetOpenRouterApiKey()");
            OnError?.Invoke("API key not set");
            return;
        }

        StartCoroutine(CaptureAndSendCoroutine(apiKey));
    }

    /// <summary>
    /// Manually trigger capture and send with custom prompt
    /// </summary>
    public void CaptureAndSendWithPrompt(string customPrompt)
    {
        if (isCapturing || isProcessing)
        {
            LogDebug("Already capturing or processing. Please wait.");
            return;
        }

        if (passthroughCamera == null)
        {
            Debug.LogError("PassthroughToGemmaSender: Passthrough camera not found!");
            OnError?.Invoke("Passthrough camera not found");
            return;
        }

        string apiKey = SecretsManager.GetOpenRouterApiKey();
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("PassthroughToGemmaSender: OpenRouter API key not set!");
            OnError?.Invoke("API key not set");
            return;
        }

        StartCoroutine(CaptureAndSendCoroutine(apiKey, customPrompt));
    }
    #endregion

    #region Capture and Send Coroutine
    private IEnumerator CaptureAndSendCoroutine(string apiKey, string customPrompt = null)
    {
        isCapturing = true;
        OnCaptureStarted?.Invoke();
        LogDebug("Starting image capture...");

        // Wait for end of frame to ensure camera has rendered
        yield return new WaitForEndOfFrame();

        // Capture the camera's current render
        if (captureTexture == null)
        {
            CreateRenderTexture();
        }

        // Render camera to texture
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture previousTarget = passthroughCamera.targetTexture;
        string base64Image = null;
        
        try
        {
            // Set render texture as target
            passthroughCamera.targetTexture = captureTexture;
            passthroughCamera.Render();
            
            // Set active render texture before reading pixels
            RenderTexture.active = captureTexture;

            // Read pixels from render texture
            // Use RGBA32 format for better compatibility with PNG encoding
            Texture2D texture2D = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBA32, false);
            texture2D.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
            texture2D.Apply();

            LogDebug("Image captured, converting to base64...");

            // Convert to PNG bytes then base64
            byte[] pngBytes = texture2D.EncodeToPNG();
            
            if (pngBytes == null || pngBytes.Length == 0)
            {
                string errorMsg = "Failed to encode texture to PNG - pngBytes is null or empty";
                Debug.LogError($"PassthroughToGemmaSender: {errorMsg}");
                OnError?.Invoke(errorMsg);
                yield break;
            }

            base64Image = Convert.ToBase64String(pngBytes);
            
            if (string.IsNullOrEmpty(base64Image))
            {
                string errorMsg = "Failed to convert PNG bytes to base64 string";
                Debug.LogError($"PassthroughToGemmaSender: {errorMsg}");
                OnError?.Invoke(errorMsg);
                yield break;
            }
            
            // Cleanup texture
            Destroy(texture2D);

            isCapturing = false;
            OnCaptureCompleted?.Invoke();
            LogDebug($"Image converted to base64 successfully ({base64Image.Length} characters, {pngBytes.Length} bytes)");
        }
        catch (Exception e)
        {
            string errorMsg = $"Error during image capture/encoding: {e.Message}";
            Debug.LogError($"PassthroughToGemmaSender: {errorMsg}");
            Debug.LogException(e);
            OnError?.Invoke(errorMsg);
            yield break;
        }
        finally
        {
            // Always restore camera settings
            RenderTexture.active = previousActive;
            passthroughCamera.targetTexture = previousTarget;
            // Reset capturing flag if not already reset
            if (isCapturing && string.IsNullOrEmpty(base64Image))
            {
                isCapturing = false;
            }
        }

        // Send to API if base64 conversion was successful
        if (!string.IsNullOrEmpty(base64Image))
        {
            isProcessing = true;
            string prompt = customPrompt ?? promptText;
            yield return StartCoroutine(SendToOpenRouterAPI(apiKey, base64Image, prompt));
            isProcessing = false;
        }
    }
    #endregion

    #region API Communication
    private IEnumerator SendToOpenRouterAPI(string apiKey, string base64Image, string prompt)
    {
        LogDebug("Sending request to OpenRouter API...");

        // Build JSON payload
        string jsonPayload = BuildJsonPayload(base64Image, prompt);

        // Create UnityWebRequest
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("HTTP-Referer", "https://unity-app.com");
            request.SetRequestHeader("X-Title", "Ikebana AR App");

            // Send request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseBody = request.downloadHandler.text;
                LogDebug("API response received");
                
                string responseText = ExtractContentFromResponse(responseBody);
                if (!string.IsNullOrEmpty(responseText))
                {
                    LogDebug($"Response extracted: {responseText.Substring(0, Mathf.Min(100, responseText.Length))}...");
                    OnResponseReceived?.Invoke(responseText);
                }
                else
                {
                    string errorMsg = "Failed to extract content from API response";
                    Debug.LogError($"PassthroughToGemmaSender: {errorMsg}");
                    LogDebug($"Full response: {responseBody}");
                    OnError?.Invoke(errorMsg);
                }
            }
            else
            {
                string errorMsg = $"API Error: {request.error} - {request.downloadHandler.text}";
                Debug.LogError($"PassthroughToGemmaSender: {errorMsg}");
                OnError?.Invoke(errorMsg);
            }
        }
    }

    private string BuildJsonPayload(string base64Image, string prompt)
    {
        string escapedPrompt = EscapeJsonString(prompt);
        string imageUrl = $"data:image/png;base64,{base64Image}";

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

    private string EscapeJsonString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return input.Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t");
    }

    private string ExtractContentFromResponse(string jsonResponse)
    {
        if (string.IsNullOrEmpty(jsonResponse))
            return null;

        try
        {
            // Look for "content" field in choices[0].message.content
            // Try to find the content value with proper JSON string handling
            const string contentMarker = "\"content\":\"";
            int contentStart = jsonResponse.IndexOf(contentMarker);
            
            if (contentStart < 0)
            {
                // Try alternative format without quotes (null content)
                const string contentMarkerAlt = "\"content\":null";
                if (jsonResponse.Contains(contentMarkerAlt))
                {
                    return null;
                }
                return null;
            }

            contentStart += contentMarker.Length;
            int contentEnd = FindStringEnd(jsonResponse, contentStart);
            
            if (contentEnd > contentStart)
            {
                string content = jsonResponse.Substring(contentStart, contentEnd - contentStart);
                return UnescapeJsonString(content);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"PassthroughToGemmaSender: Error parsing JSON response: {e.Message}");
        }

        return null;
    }

    private int FindStringEnd(string json, int startIndex)
    {
        bool escaped = false;
        for (int i = startIndex; i < json.Length; i++)
        {
            char c = json[i];
            if (escaped)
            {
                escaped = false;
                continue;
            }
            if (c == '\\')
            {
                escaped = true;
                continue;
            }
            if (c == '"')
            {
                return i;
            }
        }
        return json.Length;
    }

    private string UnescapeJsonString(string escaped)
    {
        if (string.IsNullOrEmpty(escaped))
            return escaped;

        StringBuilder sb = new StringBuilder(escaped.Length);
        bool escapedChar = false;
        
        for (int i = 0; i < escaped.Length; i++)
        {
            char c = escaped[i];
            
            if (escapedChar)
            {
                switch (c)
                {
                    case '"': sb.Append('"'); break;
                    case '\\': sb.Append('\\'); break;
                    case 'n': sb.Append('\n'); break;
                    case 'r': sb.Append('\r'); break;
                    case 't': sb.Append('\t'); break;
                    case 'u': // Unicode escape sequence
                        if (i + 4 < escaped.Length)
                        {
                            string hex = escaped.Substring(i + 1, 4);
                            try
                            {
                                int unicode = Convert.ToInt32(hex, 16);
                                sb.Append((char)unicode);
                                i += 4;
                            }
                            catch
                            {
                                sb.Append('\\').Append('u');
                            }
                        }
                        else
                        {
                            sb.Append('\\').Append('u');
                        }
                        break;
                    default:
                        sb.Append('\\').Append(c);
                        break;
                }
                escapedChar = false;
            }
            else if (c == '\\')
            {
                escapedChar = true;
            }
            else
            {
                sb.Append(c);
            }
        }
        
        return sb.ToString();
    }
    #endregion

    #region Utility Methods
    private void LogDebug(string message)
    {
        if (debugMode)
        {
            Debug.Log($"PassthroughToGemmaSender: {message}");
        }
    }

    /// <summary>
    /// Checks if the component is ready to capture
    /// </summary>
    public bool IsReady()
    {
        return passthroughCamera != null && 
               !string.IsNullOrEmpty(SecretsManager.GetOpenRouterApiKey()) &&
               !isCapturing && 
               !isProcessing;
    }

    /// <summary>
    /// Gets the current passthrough camera
    /// </summary>
    public Camera GetPassthroughCamera()
    {
        return passthroughCamera;
    }
    #endregion
}

