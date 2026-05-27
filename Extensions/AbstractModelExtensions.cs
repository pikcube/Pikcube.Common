using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Extensions;

/// <summary>
/// Extensions on all Abstract Models
/// </summary>
public static class AbstractModelExtensions
{
    extension<T>(T instance) where T : AbstractModel
    {
        /// <summary>
        /// Create a strongly typed Mutable Clone of the model
        /// </summary>
        /// <returns>A mutable clone of the model</returns>
        public T StrongMutableClone()
        {
            return (T)instance.MutableClone();
        }
    }
}