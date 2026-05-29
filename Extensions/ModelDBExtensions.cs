using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Extensions;

/// <summary>
/// Extension on ModelDB
/// </summary>
public static class ModelDbExtensions
{
    private static Dictionary<Type, AbstractModel> AllModelsDictionary
    {
        get
        {
            field ??= InitDictionary();
            return field;
        }
    }

    private static Dictionary<Type, AbstractModel> InitDictionary()
    {
        Dictionary<ModelId, AbstractModel> dic = AccessTools.DeclaredField(typeof(ModelDb), "_contentById")
            .GetValue(null) as Dictionary<ModelId, AbstractModel> ?? throw new InvalidOperationException();

        return dic.Values.ToDictionary(v => v.GetType(), v => v);
    }

    internal static void PreInit()
    {
        _ = AllModelsDictionary;
    }

    extension(ModelDb)
    {
        /// <summary>
        /// Get Model from ModelDB
        /// </summary>
        /// <typeparam name="T">Type of the model</typeparam>
        /// <returns>Instance of model in ModelDB</returns>
        public static T GetModel<T>() where T : AbstractModel
        {
            return (T)AllModelsDictionary[typeof(T)];
        }

        /// <summary>
        /// Get Model from ModelDB
        /// </summary>
        /// <param name="type">The type to get</param>
        /// <typeparam name="T">Type or base type of the model</typeparam>
        /// <returns>Instance of model in ModelDB</returns>
        public static T GetModel<T>(Type type) where T : AbstractModel
        {
            return (T)AllModelsDictionary[type];
        }

        /// <summary>
        /// Get every model in ModelDB
        /// </summary>
        public static IEnumerable<AbstractModel> AllModels => AllModelsDictionary.Values;
    }
}