namespace Pikcube.Common.Utility;

/// <summary>
/// Defines a custom loc table used in this project.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class CustomLocTableAttribute : Attribute
{
    /// <summary>
    /// Create a custom loc table entry.
    /// </summary>
    /// <param name="name">The name of the json file to register.</param>
    public CustomLocTableAttribute(string name)
    {
        CustomLocManager.Register(name);
    }
}