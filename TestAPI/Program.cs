using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace TestAPI
{
    class Program
    {
        private const string API_URL = "https://openrouter.ai/api/v1/chat/completions";
        private const string MODEL_NAME = "google/gemma-3-27b-it:free";
        private const string JSON_CONTENT_TYPE = "application/json";
        private const string IMAGE_DATA_URI_PREFIX = "data:image/png;base64,";
        
        private static string apiKey = "";
        private static HttpClient httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Gemma API Test Console ===\n");

            // Load API key
            if (!LoadApiKey())
            {
                Console.WriteLine("❌ API key not found!");
                Console.WriteLine("Please set your API key:");
                Console.Write("Enter API key: ");
                apiKey = Console.ReadLine() ?? "";
                
                if (string.IsNullOrEmpty(apiKey))
                {
                    Console.WriteLine("No API key provided. Exiting.");
                    return;
                }
                
                SaveApiKey(apiKey);
            }

            Console.WriteLine($"✅ API Key loaded: {apiKey.Substring(0, Math.Min(20, apiKey.Length))}...\n");

            // Test menu
            while (true)
            {
                Console.WriteLine("\n=== Test Menu ===");
                Console.WriteLine("1. Test API with sample image (base64)");
                Console.WriteLine("2. Test API with text-only prompt");
                Console.WriteLine("3. Test JSON payload building");
                Console.WriteLine("4. Test response parsing");
                Console.WriteLine("5. Test error handling");
                Console.WriteLine("6. Run all tests");
                Console.WriteLine("7. Change API key");
                Console.WriteLine("8. Send custom message to API");
                Console.WriteLine("0. Exit");
                Console.Write("\nSelect option: ");

                string? choice = Console.ReadLine();
                if (string.IsNullOrEmpty(choice)) continue;

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await TestWithImage();
                            break;
                        case "2":
                            await TestTextOnly();
                            break;
                        case "3":
                            TestJsonBuilding();
                            break;
                        case "4":
                            TestResponseParsing();
                            break;
                        case "5":
                            await TestErrorHandling();
                            break;
                        case "6":
                            await RunAllTests();
                            break;
                        case "7":
                            ChangeApiKey();
                            break;
                        case "8":
                            await SendCustomMessage();
                            break;
                        case "0":
                            Console.WriteLine("Exiting...");
                            return;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            }
        }

        static bool LoadApiKey()
        {
            string secretsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "secrets.json");
            
            // Also check current directory
            if (!File.Exists(secretsPath))
            {
                secretsPath = Path.Combine(Directory.GetCurrentDirectory(), "secrets.json");
            }

            if (File.Exists(secretsPath))
            {
                try
                {
                    string json = File.ReadAllText(secretsPath);
                    var secrets = JsonSerializer.Deserialize<SecretsData>(json);
                    if (secrets != null && !string.IsNullOrEmpty(secrets.openRouterApiKey))
                    {
                        apiKey = secrets.openRouterApiKey;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Could not load secrets file: {ex.Message}");
                }
            }

            // Check environment variable
            apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ?? "";
            return !string.IsNullOrEmpty(apiKey);
        }

        static void SaveApiKey(string key)
        {
            string secretsPath = Path.Combine(Directory.GetCurrentDirectory(), "secrets.json");
            var secrets = new SecretsData { openRouterApiKey = key };
            string json = JsonSerializer.Serialize(secrets, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(secretsPath, json);
            Console.WriteLine($"✅ API key saved to {secretsPath}");
        }

        static void ChangeApiKey()
        {
            Console.Write("Enter new API key: ");
            string? newKey = Console.ReadLine();
            if (!string.IsNullOrEmpty(newKey))
            {
                apiKey = newKey;
                SaveApiKey(newKey);
                Console.WriteLine("✅ API key updated!");
            }
        }

        static async Task TestWithImage()
        {
            Console.WriteLine("\n=== Test: API Call with Image ===");
            
            // Create a simple test image (1x1 red pixel PNG in base64)
            // This is a minimal valid PNG for testing
            string testImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
            
            string prompt = "What do you see in this image?";
            
            Console.WriteLine($"Prompt: {prompt}");
            Console.WriteLine($"Image size: {testImageBase64.Length} bytes (base64)");
            Console.WriteLine("Sending request...\n");

            var startTime = DateTime.Now;
            string? response = await SendApiRequest(testImageBase64, prompt);
            var duration = (DateTime.Now - startTime).TotalSeconds;

            Console.WriteLine($"\n✅ Response received in {duration:F2} seconds");
            Console.WriteLine($"Response length: {response?.Length ?? 0} characters");
            Console.WriteLine($"\nResponse content:\n{response ?? "null"}");
        }

        static async Task TestTextOnly()
        {
            Console.WriteLine("\n=== Test: Text-Only Request ===");
            
            // Note: Some models support text-only, but Gemma vision model expects image
            // This will test error handling
            string prompt = "Hello, this is a test without an image.";
            
            Console.WriteLine($"Prompt: {prompt}");
            Console.WriteLine("Sending request (may fail if model requires image)...\n");

            var startTime = DateTime.Now;
            string? response = await SendApiRequest(null, prompt);
            var duration = (DateTime.Now - startTime).TotalSeconds;

            Console.WriteLine($"\nResponse received in {duration:F2} seconds");
            Console.WriteLine($"Response: {response ?? "null"}");
        }

        static void TestJsonBuilding()
        {
            Console.WriteLine("\n=== Test: JSON Payload Building ===");
            
            string testImage = "test_base64_image_data";
            string prompt = "Test prompt with \"quotes\" and\nnewlines";
            
            string json = BuildJsonPayload(testImage, prompt);
            
            Console.WriteLine("Generated JSON payload:");
            Console.WriteLine("---");
            Console.WriteLine(FormatJson(json));
            Console.WriteLine("---");
            
            // Test JSON escaping
            Console.WriteLine("\nTesting JSON escaping:");
            Console.WriteLine($"Original prompt: {prompt}");
            string escaped = EscapeJsonString(prompt);
            Console.WriteLine($"Escaped: {escaped}");
        }

        static void TestResponseParsing()
        {
            Console.WriteLine("\n=== Test: Response Parsing ===");
            
            // Sample OpenRouter response
            string sampleResponse = @"{
                ""id"": ""gen-123"",
                ""model"": ""google/gemma-3-27b-it:free"",
                ""created"": 1234567890,
                ""choices"": [
                    {
                        ""index"": 0,
                        ""message"": {
                            ""role"": ""assistant"",
                            ""content"": ""I can see a red pixel in this image. It appears to be a simple 1x1 pixel test image.""
                        },
                        ""finish_reason"": ""stop""
                    }
                ],
                ""usage"": {
                    ""prompt_tokens"": 100,
                    ""completion_tokens"": 20,
                    ""total_tokens"": 120
                }
            }";
            
            Console.WriteLine("Sample response JSON:");
            Console.WriteLine(FormatJson(sampleResponse));
            Console.WriteLine("\nParsing content...");
            
            string? content = ExtractContentFromResponse(sampleResponse);
            Console.WriteLine($"\n✅ Extracted content: {content ?? "null"}");
            
            // Test with escaped characters
            string responseWithEscapes = @"{
                ""choices"": [{
                    ""message"": {
                        ""content"": ""He said \""Hello\"" and went to the store.\nIt was a nice day.""
                    }
                }]
            }";
            
            Console.WriteLine("\nTesting with escaped characters:");
            string? escapedContent = ExtractContentFromResponse(responseWithEscapes);
            Console.WriteLine($"Extracted: {escapedContent ?? "null"}");
        }

        static async Task TestErrorHandling()
        {
            Console.WriteLine("\n=== Test: Error Handling ===");
            
            // Test with invalid API key
            Console.WriteLine("Testing with invalid API key...");
            string originalKey = apiKey;
            apiKey = "invalid-key-test";
            
            try
            {
                await SendApiRequest("test", "test");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✅ Error caught: {ex.Message}");
            }
            finally
            {
                apiKey = originalKey;
            }
            
            // Test with invalid JSON
            Console.WriteLine("\nTesting with malformed JSON...");
            try
            {
                string malformedJson = "{ invalid json }";
                JsonSerializer.Deserialize<object>(malformedJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✅ JSON error caught: {ex.Message}");
            }
        }

        static async Task RunAllTests()
        {
            Console.WriteLine("\n=== Running All Tests ===\n");
            
            Console.WriteLine("1. Testing JSON building...");
            TestJsonBuilding();
            await Task.Delay(1000);
            
            Console.WriteLine("\n2. Testing response parsing...");
            TestResponseParsing();
            await Task.Delay(1000);
            
            Console.WriteLine("\n3. Testing API call...");
            await TestWithImage();
        }

        static async Task SendCustomMessage()
        {
            Console.WriteLine("\n=== Send Custom Message to API ===");
            Console.WriteLine("Enter your custom prompt/message:");
            Console.Write("> ");
            string? prompt = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(prompt))
            {
                Console.WriteLine("❌ No prompt entered. Cancelling.");
                return;
            }

            Console.WriteLine("\nImage options:");
            Console.WriteLine("1. Use test image (1x1 pixel)");
            Console.WriteLine("2. Enter custom base64 image");
            Console.WriteLine("3. Load image from file");
            Console.WriteLine("4. Send text-only (no image)");
            Console.Write("Select option (1-4): ");
            
            string? imageChoice = Console.ReadLine();
            string? base64Image = null;

            switch (imageChoice)
            {
                case "1":
                    // Use test image
                    base64Image = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
                    Console.WriteLine("✅ Using test image");
                    break;
                case "2":
                    Console.WriteLine("\nEnter base64 encoded image (PNG format):");
                    Console.Write("> ");
                    base64Image = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(base64Image))
                    {
                        Console.WriteLine("⚠️ No image provided. Sending text-only.");
                        base64Image = null;
                    }
                    else
                    {
                        Console.WriteLine($"✅ Image loaded ({base64Image.Length} characters)");
                    }
                    break;
                case "3":
                    Console.WriteLine("\nEnter image file path (PNG, JPG, etc.):");
                    Console.Write("> ");
                    string? filePath = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                    {
                        try
                        {
                            byte[] imageBytes = File.ReadAllBytes(filePath);
                            base64Image = Convert.ToBase64String(imageBytes);
                            Console.WriteLine($"✅ Image loaded from file ({imageBytes.Length} bytes, {base64Image.Length} base64 chars)");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Error loading image: {ex.Message}");
                            Console.WriteLine("⚠️ Sending text-only request instead.");
                            base64Image = null;
                        }
                    }
                    else
                    {
                        Console.WriteLine("❌ File not found. Sending text-only request.");
                        base64Image = null;
                    }
                    break;
                case "4":
                    Console.WriteLine("✅ Sending text-only request");
                    base64Image = null;
                    break;
                default:
                    Console.WriteLine("⚠️ Invalid choice. Using test image.");
                    base64Image = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
                    break;
            }

            Console.WriteLine($"\n📤 Sending request...");
            Console.WriteLine($"Prompt: {prompt}");
            if (base64Image != null)
            {
                Console.WriteLine($"Image: {(base64Image.Length > 50 ? base64Image.Substring(0, 50) + "..." : base64Image)}");
            }
            Console.WriteLine();

            try
            {
                var startTime = DateTime.Now;
                string? response = await SendApiRequest(base64Image, prompt);
                var duration = (DateTime.Now - startTime).TotalSeconds;

                Console.WriteLine($"\n✅ Response received in {duration:F2} seconds");
                Console.WriteLine($"Response length: {response?.Length ?? 0} characters");
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("RESPONSE:");
                Console.WriteLine(new string('=', 60));
                Console.WriteLine(response ?? "null");
                Console.WriteLine(new string('=', 60));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"   Inner: {ex.InnerException.Message}");
                }
            }
        }

        static async Task<string?> SendApiRequest(string? base64Image, string prompt)
        {
            string jsonPayload = BuildJsonPayload(base64Image, prompt);
            
            var request = new HttpRequestMessage(HttpMethod.Post, API_URL);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Headers.Add("HTTP-Referer", "https://test-app.com");
            request.Headers.Add("X-Title", "Gemma API Test");
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, JSON_CONTENT_TYPE);
            
            Console.WriteLine("Request headers:");
            Console.WriteLine($"  Authorization: Bearer {apiKey.Substring(0, Math.Min(20, apiKey.Length))}...");
            Console.WriteLine($"  Content-Type: {JSON_CONTENT_TYPE}");
            Console.WriteLine($"  HTTP-Referer: https://test-app.com");
            Console.WriteLine($"  X-Title: Gemma API Test");
            
            var response = await httpClient.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                string? content = ExtractContentFromResponse(responseBody);
                return content ?? responseBody;
            }
            else
            {
                throw new Exception($"API Error: {response.StatusCode} - {responseBody}");
            }
        }

        static string BuildJsonPayload(string? base64Image, string prompt)
        {
            string escapedPrompt = EscapeJsonString(prompt);
            
            if (string.IsNullOrEmpty(base64Image))
            {
                // Text-only request (may not work with vision models)
                return $@"{{
                    ""model"": ""{MODEL_NAME}"",
                    ""messages"": [
                        {{
                            ""role"": ""user"",
                            ""content"": ""{escapedPrompt}""
                        }}
                    ]
                }}";
            }
            else
            {
                string imageUrl = IMAGE_DATA_URI_PREFIX + base64Image;
                return $@"{{
                    ""model"": ""{MODEL_NAME}"",
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
        }

        static string EscapeJsonString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("\n", "\\n")
                        .Replace("\r", "\\r")
                        .Replace("\t", "\\t");
        }

        static string? ExtractContentFromResponse(string jsonResponse)
        {
            if (string.IsNullOrEmpty(jsonResponse))
                return null;

            try
            {
                // Try to parse as JSON first
                using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                {
                    if (doc.RootElement.TryGetProperty("choices", out JsonElement choices) &&
                        choices.GetArrayLength() > 0)
                    {
                        var firstChoice = choices[0];
                        if (firstChoice.TryGetProperty("message", out JsonElement message) &&
                            message.TryGetProperty("content", out JsonElement content))
                        {
                            return content.GetString();
                        }
                    }
                }
            }
            catch
            {
                // Fallback to string search if JSON parsing fails
                const string contentMarker = "\"content\":\"";
                int contentStart = jsonResponse.IndexOf(contentMarker);
                
                if (contentStart >= 0)
                {
                    contentStart += contentMarker.Length;
                    int contentEnd = FindStringEnd(jsonResponse, contentStart);
                    
                    if (contentEnd > contentStart)
                    {
                        string content = jsonResponse.Substring(contentStart, contentEnd - contentStart);
                        return UnescapeJsonString(content);
                    }
                }
            }

            return null;
        }

        static int FindStringEnd(string json, int startIndex)
        {
            for (int i = startIndex; i < json.Length; i++)
            {
                if (json[i] == '"')
                {
                    // Check if this quote is escaped
                    int backslashCount = 0;
                    for (int j = i - 1; j >= startIndex && json[j] == '\\'; j--)
                    {
                        backslashCount++;
                    }
                    
                    // If even number (or zero) of backslashes, this is an unescaped quote
                    if (backslashCount % 2 == 0)
                    {
                        return i;
                    }
                }
            }
            
            return -1;
        }

        static string? UnescapeJsonString(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.Replace("\\\"", "\"")
                        .Replace("\\n", "\n")
                        .Replace("\\r", "\r")
                        .Replace("\\t", "\t")
                        .Replace("\\\\", "\\");
        }

        static string FormatJson(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    return JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true });
                }
            }
            catch
            {
                return json; // Return original if parsing fails
            }
        }

        class SecretsData
        {
            public string? openRouterApiKey { get; set; }
        }
    }
}

