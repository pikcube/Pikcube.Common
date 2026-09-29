using MegaCrit.Sts2.Core.Runs;

namespace Pikcube.Common.Extensions;

/// <summary>
/// Extensions of the RunState class.
/// </summary>
public static class RunStateExtensions
{
    extension(RunState)
    {
        /// <summary>
        /// Get the current RunState object from the RunManager.
        /// </summary>
        public static RunState Instance => RunManager.Instance.PrivatePropertyWrapper<RunManager, RunState>("State").Value ?? 
                                          throw new InvalidOperationException();
    }
}