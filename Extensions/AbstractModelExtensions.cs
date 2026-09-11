using System.Reflection;
using BaseLib.Extensions;
using HarmonyLib;
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
        /// Get a "clone" of this model, preserving its mutability status.
        /// This is useful in more backend-ish areas of the codebase, where a model can either be mutable or canonical,
        /// and we don't want to create a new mutable clone if we currently have a reference to the canonical instance.
        /// </summary>
        /// <returns>
        /// If this instance of the model is canonical, this method just returns itself.
        /// If this instance of the model is mutable, it returns another mutable clone.
        /// </returns>
        public T StrongClonePreservingMutability()
        {
            return (T)instance.ClonePreservingMutability();
        }

        /// <summary>
        /// WARNING: You almost always want to use `CreateMutable()` or `StrongClonePreservingMutability()` instead, since we usually
        /// don't want to make mutable clones of already-mutable models.
        /// 
        /// Get a mutable "clone" of this model.
        /// This is useful in very generalized spots where we could either have a mutable or canonical model, and we want a
        /// mutable clone regardless.
        /// </summary>
        /// <returns>A mutable clone of this model.</returns>
        public T StrongMutableClone()
        {
            return (T)instance.MutableClone();
        }
    }
}