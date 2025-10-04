using Terraria.ModLoader;

namespace PersistentReforges
{
    public class PersistentReforgesPlayer : ModPlayer
    {
        public override void Initialize()
        {
            PersistentReforges.Init();
        }
    }
}

