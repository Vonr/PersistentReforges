using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace PersistentReforges
{
    public class PersistentReforges : Mod
    {
        public static void Init()
        {
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

                    var baseValue = recipe.createItem.value;
                    var potential = new Item[consumedItems.Count];
                    var numPrefixes = 0;
                    foreach (var component in consumedItems)
                    {
                        if (item.CanApplyPrefix(component.prefix))
                        {
                            var dummy = recipe.createItem.Clone();
                            dummy.Prefix(component.prefix);

                            if (config.PersistBadReforges && dummy.value < baseValue)
                            {
                                continue;
                            }

                            potential[numPrefixes] = dummy;
                            numPrefixes += 1;
                        }
                    }

                    if (numPrefixes <= 0)
                    {
                        return;
                    }

                    potential = potential[..numPrefixes];

                    int by(Func<Item, long> transform, Func<long, long, long> reducer)
                    {
                        var best = potential.Select(transform).Aggregate(reducer);
                        var filtered = potential.Where(p =>
                        {
                            var value = transform.Invoke(p);
                            return reducer.Invoke(value, best) == value;
                        }).ToArray();

                        return Main.rand.NextFromList(filtered).prefix;
                    }

                    var prefix = config.SelectMode switch
                    {
                        PersistentReforgesConfig.SelectionMode.HighestValue => by(static i => i.value, Math.Max),
                        PersistentReforgesConfig.SelectionMode.LowestValue => by(static i => i.value, Math.Min),
                        PersistentReforgesConfig.SelectionMode.Rarest => by(static i => i.rare, Math.Max),
                        PersistentReforgesConfig.SelectionMode.LeastRare => by(static i => i.rare, Math.Min),
                        PersistentReforgesConfig.SelectionMode.Random => Main.rand.NextFromList(potential).prefix,
                        _ => throw new NotImplementedException(),
                    };

                    item.Prefix(prefix);
                });
            }
        }
    }
}
