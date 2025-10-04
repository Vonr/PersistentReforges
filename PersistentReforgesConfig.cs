using System.ComponentModel;
using System.Text.Json.Serialization;
using Terraria.ModLoader.Config;

namespace PersistentReforges
{
    public class PersistentReforgesConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(SelectionMode.HighestValue)]
        public SelectionMode SelectMode { get; set; }

        [Slider]
        [DefaultValue(1.0)]
        public float Chance { get; set; }

        [DefaultValue(false)]
        public bool PersistBadReforges { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter<SelectionMode>))]
        public enum SelectionMode
        {
            HighestValue,
            LowestValue,
            Rarest,
            LeastRare,
            Random,
        }
    }
}

