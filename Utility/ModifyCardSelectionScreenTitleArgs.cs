namespace Pikcube.Common.Utility;

/// <summary>
/// Event args for modifying the title of a card selection screen.
/// </summary>
/// <param name="original">The original title.</param>
public class ModifyCardSelectionScreenTitleArgs(string original)
{
    /// <summary>
    /// The original text.
    /// </summary>
    public string OriginalText { get; } = original;
    /// <summary>
    /// The text to be displayed.
    /// </summary>
    public string NewText { get; set; } = original;
}