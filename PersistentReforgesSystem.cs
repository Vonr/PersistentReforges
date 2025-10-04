using Terraria.ModLoader;

namespace PersistentReforges
{
    public class PersistentReforgesSystem : ModSystem
    {
        public override void PostAddRecipes()
        {
            PersistentReforges.Init();
        }
    }
}

