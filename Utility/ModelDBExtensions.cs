using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Utility;

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
    }
}