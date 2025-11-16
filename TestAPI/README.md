# API Test Console Application

Standalone C# console application to test the OpenRouter Gemma API without Unity dependencies.

## 🚀 Quick Start

### Prerequisites
- .NET 6.0 or later SDK
- OpenRouter API key

### Running the Tests

1. **Navigate to TestAPI directory:**
   ```bash
   cd TestAPI
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Run the test program:**
   ```bash
   dotnet run
   ```

### Setting API Key

The program will look for API key in this order:
1. `secrets.json` file in current directory
2. `secrets.json` file in user home directory
3. `OPENROUTER_API_KEY` environment variable
4. Prompt user to enter key

**Create secrets.json:**
```json
{
  "openRouterApiKey": "sk-or-v1-your-api-key-here"
}
```

Or set environment variable:
```bash
export OPENROUTER_API_KEY="sk-or-v1-your-api-key-here"
```

## 📋 Test Menu Options

1. **Test API with sample image** - Sends a test image (1x1 pixel) to API
2. **Test API with text-only prompt** - Tests text-only requests
3. **Test JSON payload building** - Validates JSON construction and escaping
4. **Test response parsing** - Tests parsing of API responses
5. **Test error handling** - Tests error scenarios
6. **Run all tests** - Executes all tests sequentially
7. **Change API key** - Update stored API key
0. **Exit** - Quit program

## 🧪 What Gets Tested

### JSON Payload Building
- Proper JSON structure
- String escaping
- Special characters handling (quotes, newlines, etc.)

### API Communication
- HTTP request construction
- Header setting
- Response handling
- Error handling

### Response Parsing
- JSON parsing
- Content extraction
- Escaped character handling
- Fallback parsing methods

## 📊 Example Output

```
=== Gemma API Test Console ===

✅ API Key loaded: sk-or-v1-abc123...

=== Test Menu ===
1. Test API with sample image (base64)
2. Test API with text-only prompt
...

Select option: 1

=== Test: API Call with Image ===
Prompt: What do you see in this image?
Image size: 68 bytes (base64)
Sending request...

Request headers:
  Authorization: Bearer sk-or-v1-abc123...
  Content-Type: application/json
  HTTP-Referer: https://test-app.com
  X-Title: Gemma API Test

✅ Response received in 3.21 seconds
Response length: 245 characters

Response content:
I can see a red pixel in this image. It appears to be a simple 1x1 pixel test image.
```

## 🔧 Building

```bash
dotnet build
```

## 📝 Notes

- This is a standalone test application, independent of Unity
- Uses standard .NET HttpClient for API calls
- Tests the same logic as Unity scripts but without Unity dependencies
- Outputs directly to console/terminal
- Can be integrated into CI/CD pipelines

## 🐛 Troubleshooting

### API Key Not Found
- Create `secrets.json` file with your API key
- Or set `OPENROUTER_API_KEY` environment variable
- Or enter key when prompted

### Network Errors
- Check internet connection
- Verify OpenRouter API is accessible
- Check firewall settings

### JSON Parsing Errors
- Verify API response format
- Check for malformed JSON
- Review error messages in output

