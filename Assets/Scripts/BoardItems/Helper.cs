using System;

namespace BoardItems
{
    public class Helper
    {
        public static event Action<string, string> OnItemActivated;

        public static void DoGlobalCallItemActivated(string itemName, string abilityName) => OnItemActivated?.Invoke(itemName, abilityName);
    }
}