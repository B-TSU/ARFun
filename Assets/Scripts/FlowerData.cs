using UnityEngine;

/// <summary>
/// Stores metadata about a flower for AI reference and identification
/// </summary>
public class FlowerData : MonoBehaviour
{
    [Header("Flower Identification")]
    [SerializeField] private string flowerType = "Unknown"; // e.g., "Rose", "Lily", "Tulip"
    [SerializeField] private int flowerID = -1; // Unique ID or index
    [SerializeField] private string flowerName = ""; // Display name
    
    [Header("Flower Properties")]
    [SerializeField] private Color flowerColor = Color.white;
    [SerializeField] private float flowerSize = 1f;
    [SerializeField] private string flowerCategory = ""; // e.g., "Main", "Filler", "Accent"
    
    [Header("AI Metadata")]
    [TextArea(2, 5)]
    [SerializeField] private string description = ""; // Description for AI reference
    [SerializeField] private string[] keywords = new string[0]; // Keywords for AI search/classification
    
    /// <summary>
    /// Gets the flower type
    /// </summary>
    public string GetFlowerType()
    {
        return flowerType;
    }
    
    /// <summary>
    /// Gets the flower ID
    /// </summary>
    public int GetFlowerID()
    {
        return flowerID;
    }
    
    /// <summary>
    /// Gets the flower name
    /// </summary>
    public string GetFlowerName()
    {
        return string.IsNullOrEmpty(flowerName) ? flowerType : flowerName;
    }
    
    /// <summary>
    /// Gets the flower color
    /// </summary>
    public Color GetFlowerColor()
    {
        return flowerColor;
    }
    
    /// <summary>
    /// Gets the flower size
    /// </summary>
    public float GetFlowerSize()
    {
        return flowerSize;
    }
    
    /// <summary>
    /// Gets the flower category
    /// </summary>
    public string GetFlowerCategory()
    {
        return flowerCategory;
    }
    
    /// <summary>
    /// Gets the description
    /// </summary>
    public string GetDescription()
    {
        return description;
    }
    
    /// <summary>
    /// Gets the keywords
    /// </summary>
    public string[] GetKeywords()
    {
        return keywords;
    }
    
    /// <summary>
    /// Sets the flower type
    /// </summary>
    public void SetFlowerType(string type)
    {
        flowerType = type;
    }
    
    /// <summary>
    /// Sets the flower ID
    /// </summary>
    public void SetFlowerID(int id)
    {
        flowerID = id;
    }
    
    /// <summary>
    /// Sets the flower name
    /// </summary>
    public void SetFlowerName(string name)
    {
        flowerName = name;
    }
    
    /// <summary>
    /// Gets all metadata as a formatted string for AI reference
    /// </summary>
    public string GetMetadataString()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"Type: {flowerType}");
        sb.AppendLine($"ID: {flowerID}");
        sb.AppendLine($"Name: {GetFlowerName()}");
        sb.AppendLine($"Color: {flowerColor}");
        sb.AppendLine($"Size: {flowerSize}");
        sb.AppendLine($"Category: {flowerCategory}");
        if (!string.IsNullOrEmpty(description))
        {
            sb.AppendLine($"Description: {description}");
        }
        if (keywords != null && keywords.Length > 0)
        {
            sb.AppendLine($"Keywords: {string.Join(", ", keywords)}");
        }
        return sb.ToString();
    }
}

