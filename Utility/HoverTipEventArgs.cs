using MegaCrit.Sts2.Core.HoverTips;

namespace Pikcube.Common.Utility;

/// <summary>
/// Event Args for Modifying Hover Tips.
/// </summary>
/// <param name="original"></param>
public class HoverTipEventArgs(IReadOnlyCollection<IHoverTip> original)
{
    /// <summary>
    /// The original unmodified Hover Tips.
    /// </summary>
    public IReadOnlyCollection<IHoverTip> Original { get; } = original;
    /// <summary>
    /// The working value for the object's Hover Tips. Modify this list to change what appears.
    /// </summary>
    public List<IHoverTip> NewHoverTips { get; } = [.. original];
}