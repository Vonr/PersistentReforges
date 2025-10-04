using System.ComponentModel;
using System.Text.Json.Serialization;
using Terraria.ModLoader.Config;

namespace PersistentReforges
{
    public class PersistentReforgesConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(SelectionMode.Rarest)]
        public SelectionMode SelectMode { get; set; }

        [Slider]
        [DefaultValue(1.0)]
        public float Chance { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter<SelectionMode>))]
        public enum SelectionMode
        {
            Rarest,
            LeastRare,
            Random,
        }
    }
}

