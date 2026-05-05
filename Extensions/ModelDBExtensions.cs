using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Extensions;

/// <summary>
/// Extension on ModelDB
/// </summary>
public static class ModelDbExtensions
{
    extension(ModelDb)
    {
        /// <summary>
        /// Get Model from ModelDB
        /// </summary>
        /// <typeparam name="T">Type of the model</typeparam>
        /// <returns>Instance of model in ModelDB</returns>
        public static T GetModel<T>() where T : AbstractModel
        {
            return ModelDb.GetById<T>(ModelDb.GetId<T>());
        }

        /// <summary>
        /// Get Model from ModelDB
        /// </summary>
        /// <param name="type">The type to get</param>
        /// <typeparam name="T">Type or base type of the model</typeparam>
        /// <returns>Instance of model in ModelDB</returns>
        public static T GetModel<T>(Type type) where T : AbstractModel
        {
            return ModelDb.GetById<T>(ModelDb.GetId(type));
        }
    }
}