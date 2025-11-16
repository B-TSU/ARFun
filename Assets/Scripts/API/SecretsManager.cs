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
        
        InitializeSecretsPath();
        LoadSecrets();
        isInitialized = true;
    }

    /// <summary>
    /// Initializes the path to the secrets file
    /// </summary>
    private static void InitializeSecretsPath()
    {
        // Store in persistent data path (outside of Assets folder)
        string persistentPath = Application.persistentDataPath;
        secretsFilePath = Path.Combine(persistentPath, "secrets.json");

        // For editor, also check for a local secrets file in Assets (gitignored)
        #if UNITY_EDITOR
        string editorSecretsPath = Path.Combine(Application.dataPath, "..", "secrets.json");
        if (File.Exists(editorSecretsPath))
        {
            secretsFilePath = editorSecretsPath;
        }
        #endif
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
        
        if (string.IsNullOrEmpty(secretsFilePath))
        {
            InitializeSecretsPath();
        }

        if (File.Exists(secretsFilePath))
        {
            try
            {
                string json = File.ReadAllText(secretsFilePath);
                secretsData = JsonUtility.FromJson<SecretsData>(json);
                Debug.Log("SecretsManager: Secrets loaded successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"SecretsManager: Error loading secrets: {e.Message}");
                secretsData = new SecretsData();
            }
        }
        else
        {
            // Create default empty secrets
            secretsData = new SecretsData();
            Debug.LogWarning("SecretsManager: No secrets file found. Please set your API key using SetOpenRouterApiKey()");
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
    /// </summary>
    public static string GetOpenRouterApiKey()
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

        // Fallback: Check environment variable
        string envKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");
        if (!string.IsNullOrEmpty(envKey))
        {
            return envKey;
        }

        return null;
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
    /// </summary>
    public static string GetSecretsFilePath()
    {
        if (string.IsNullOrEmpty(secretsFilePath))
        {
            InitializeSecretsPath();
        }
        return secretsFilePath;
    }
}

