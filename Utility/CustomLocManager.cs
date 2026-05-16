namespace Pikcube.Common.Utility;

internal static class CustomLocManager
{
    private static readonly List<string> LocTables = [];
    internal static IEnumerable<string> GetCustomLocTables(IEnumerable<string> original)
    {
        return [.. original, ..LocTables];
    }

    internal static void Register(string name)
    {
        if (!name.EndsWith(".json"))
        {
            name += ".json";
        }

        if (LocTables.Contains(name))
        {
            return;
        }
        LocTables.Add(name);
    }
}