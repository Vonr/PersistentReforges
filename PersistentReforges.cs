using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace PersistentReforges
{
    public class PersistentReforges : Mod
    {
        private static bool init;

        public static void Init()
        {
            if (init)
            {
                return;
            }
            init = true;

            foreach (var recipe in Main.recipe)
            {
                if (!recipe.createItem.CanHavePrefixes() || recipe.requiredItem.All(static i => !i.CanHavePrefixes()))
                {
                    continue;
                }

                recipe.AddOnCraftCallback((recipe, item, consumedItems, destinationStack) =>
                {
                    var config = ModContent.GetInstance<PersistentReforgesConfig>();
                    Main.rand ??= new UnifiedRandom();
                    if (Main.rand.NextFloat() >= config.Chance)
                    {
                        return;
                    }

                    var prefixes = new (int, float)[consumedItems.Count];
                    var numPrefixes = 0;
                    foreach (var component in consumedItems)
                    {
                        if (item.CanApplyPrefix(component.prefix))
                        {
                            var dummy = recipe.createItem.Clone();
                            dummy.Prefix(component.prefix);
                            prefixes[numPrefixes] = (component.prefix, dummy.rare);
                            numPrefixes += 1;
                        }
                    }

                    if (numPrefixes <= 0)
                    {
                        return;
                    }

                    prefixes = prefixes[..numPrefixes];

                    int prefix;
                    switch (config.SelectMode)
                    {
                        case PersistentReforgesConfig.SelectionMode.Rarest:
                            {
                                var highestRarity = prefixes.Max(static p => p.Item2);
                                var filtered = prefixes.Where(p => p.Item2 >= highestRarity).ToArray();
                                prefix = filtered[Main.rand.Next(filtered.Length)].Item1;

                                break;
                            }
                        case PersistentReforgesConfig.SelectionMode.LeastRare:
                            {
                                var lowestRarity = prefixes.Min(static p => p.Item2);
                                var filtered = prefixes.Where(p => p.Item2 <= lowestRarity).ToArray();
                                prefix = filtered[Main.rand.Next(filtered.Length)].Item1;

                                break;
                            }
                        case PersistentReforgesConfig.SelectionMode.Random:
                            {
                                prefix = prefixes[Main.rand.Next(prefixes.Length)].Item1;
                                break;
                            }
                        default:
                            {
                                throw new System.NotImplementedException();
                            }
                    }

                    item.Prefix(prefix);
                });
            }
        }
    }
}
