using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Manages secure storage of API keys and secrets
/// Secrets are stored in a file that should be gitignored
/// </summary>
public static class SecretsManager
{
    private static string secretsFilePath;
    private static SecretsData secretsData;
    private static bool isInitialized = false;

    [System.Serializable]
    private class SecretsData
    {
        public string openRouterApiKey = "";
    }

    /// <summary>
    /// Initializes the secrets manager (called automatically on first use)
    /// </summary>
    private static void Initialize()
    {
        if (isInitialized) return;
        
        // Safety check: Don't initialize file I/O in edit mode unless explicitly needed
        // This prevents crashes when scripts are attached in the editor
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // In edit mode, just create empty secrets data without file I/O
            secretsData = new SecretsData();
            isInitialized = true;
            return;
        }
        #endif
        
        try
        {
            InitializeSecretsPath();
            LoadSecrets();
            isInitialized = true;
        }
        catch (Exception e)
        {
            // If initialization fails, create empty secrets to prevent crashes
            Debug.LogWarning($"SecretsManager: Initialization failed, using empty secrets. Error: {e.Message}");
            secretsData = new SecretsData();
            isInitialized = true;
        }
    }

    /// <summary>
    /// Initializes the path to the secrets file
    /// </summary>
    private static void InitializeSecretsPath()
    {
        try
        {
            // Store in persistent data path (outside of Assets folder)
            // Only access persistentDataPath in play mode to avoid editor-time issues
            string persistentPath = Application.isPlaying ? Application.persistentDataPath : "";
            
            if (!string.IsNullOrEmpty(persistentPath))
            {
                secretsFilePath = Path.Combine(persistentPath, "secrets.json");
            }

            // For editor, also check for a local secrets file in Assets (gitignored)
            #if UNITY_EDITOR
            try
            {
                string editorSecretsPath = Path.Combine(Application.dataPath, "..", "secrets.json");
                if (File.Exists(editorSecretsPath))
                {
                    secretsFilePath = editorSecretsPath;
                }
            }
            catch (Exception e)
            {
                // Silently handle file system errors in editor
                Debug.LogWarning($"SecretsManager: Could not check editor secrets path: {e.Message}");
            }
            #endif
        }
        catch (Exception e)
        {
            Debug.LogWarning($"SecretsManager: Error initializing secrets path: {e.Message}");
            secretsFilePath = "";
        }
    }

    /// <summary>
    /// Loads secrets from file
    /// </summary>
    private static void LoadSecrets()
    {
        if (!isInitialized)
        {
            Initialize();
            return;
        }
        
        // Safety check: Don't do file I/O in edit mode unless in play mode
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (secretsData == null)
            {
                secretsData = new SecretsData();
            }
            return;
        }
        #endif
        
        if (string.IsNullOrEmpty(secretsFilePath))
        {
            InitializeSecretsPath();
        }

        if (string.IsNullOrEmpty(secretsFilePath))
        {
            secretsData = new SecretsData();
            return;
        }

        if (File.Exists(secretsFilePath))
        {
            try
            {
                string json = File.ReadAllText(secretsFilePath);
                if (!string.IsNullOrEmpty(json))
                {
                    secretsData = JsonUtility.FromJson<SecretsData>(json);
                    if (secretsData == null)
                    {
                        secretsData = new SecretsData();
                    }
                    Debug.Log("SecretsManager: Secrets loaded successfully");
                }
                else
                {
                    secretsData = new SecretsData();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"SecretsManager: Error loading secrets: {e.Message}");
                secretsData = new SecretsData();
            }
        }
        else
        {
            // Create default empty secrets
            secretsData = new SecretsData();
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Debug.LogWarning("SecretsManager: No secrets file found. Please set your API key using SetOpenRouterApiKey()");
            }
            #endif
        }
    }

    /// <summary>
    /// Saves secrets to file
    /// </summary>
    private static void SaveSecrets()
    {
        if (string.IsNullOrEmpty(secretsFilePath))
        {
            InitializeSecretsPath();
        }

        try
        {
            // Ensure directory exists
            string directory = Path.GetDirectoryName(secretsFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception dirEx)
                {
                    Debug.LogError($"SecretsManager: Error creating directory: {dirEx.Message}");
                    throw;
                }
            }

            // Validate path before writing
            if (string.IsNullOrEmpty(secretsFilePath))
            {
                throw new Exception("Secrets file path is invalid");
            }

            string json = JsonUtility.ToJson(secretsData, true);
            
            // Use a temporary file first, then rename (safer)
            string tempPath = secretsFilePath + ".tmp";
            File.WriteAllText(tempPath, json);
            
            // Replace the old file with the new one
            if (File.Exists(secretsFilePath))
            {
                File.Delete(secretsFilePath);
            }
            File.Move(tempPath, secretsFilePath);
            
            Debug.Log($"SecretsManager: Secrets saved successfully to {secretsFilePath}");
        }
        catch (UnauthorizedAccessException e)
        {
            Debug.LogError($"SecretsManager: Permission denied. Cannot write to {secretsFilePath}. Error: {e.Message}");
            throw;
        }
        catch (DirectoryNotFoundException e)
        {
            Debug.LogError($"SecretsManager: Directory not found. Error: {e.Message}");
            throw;
        }
        catch (Exception e)
        {
            Debug.LogError($"SecretsManager: Error saving secrets to {secretsFilePath}. Error: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// Gets the OpenRouter API key
    /// Safe to call in edit mode - will return null or environment variable value
    /// </summary>
    public static string GetOpenRouterApiKey()
    {
        try
        {
            if (!isInitialized)
            {
                Initialize();
            }
            
            if (secretsData == null)
            {
                LoadSecrets();
            }

            if (secretsData != null && !string.IsNullOrEmpty(secretsData.openRouterApiKey))
            {
                return secretsData.openRouterApiKey;
            }

            // Fallback: Check environment variable (safe in edit mode)
            try
            {
                string envKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");
                if (!string.IsNullOrEmpty(envKey))
                {
                    return envKey;
                }
            }
            catch (Exception e)
            {
                // Silently handle environment variable access errors
                #if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Debug.LogWarning($"SecretsManager: Error reading environment variable: {e.Message}");
                }
                #endif
            }

            return null;
        }
        catch (Exception e)
        {
            // Prevent crashes - return null if anything goes wrong
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Debug.LogWarning($"SecretsManager: Error getting API key: {e.Message}");
            }
            #endif
            return null;
        }
    }

    /// <summary>
    /// Sets the OpenRouter API key securely
    /// </summary>
    public static void SetOpenRouterApiKey(string apiKey)
    {
        if (!isInitialized)
        {
            Initialize();
        }
        
        if (secretsData == null)
        {
            LoadSecrets();
        }

        if (secretsData == null)
        {
            secretsData = new SecretsData();
        }

        secretsData.openRouterApiKey = apiKey;
        SaveSecrets();
        Debug.Log("SecretsManager: OpenRouter API key set successfully");
    }

    /// <summary>
    /// Checks if API key is set
    /// </summary>
    public static bool HasOpenRouterApiKey()
    {
        string key = GetOpenRouterApiKey();
        return !string.IsNullOrEmpty(key) && key != "YOUR_OPENROUTER_API_KEY";
    }

    /// <summary>
    /// Gets the path to the secrets file (for reference)
    /// Safe to call in edit mode
    /// </summary>
    public static string GetSecretsFilePath()
    {
        try
        {
            if (string.IsNullOrEmpty(secretsFilePath))
            {
                InitializeSecretsPath();
            }
            return secretsFilePath ?? "";
        }
        catch (Exception e)
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Debug.LogWarning($"SecretsManager: Error getting secrets file path: {e.Message}");
            }
            #endif
            return "";
        }
    }
}

