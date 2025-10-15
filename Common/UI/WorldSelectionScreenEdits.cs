using CalamityVanilla.Common.World;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityVanilla.Common.UI;

internal sealed class WorldSelectionScreenEdits : ModSystem
{
    public override void Load()
    {
        On_UIWorldListItem.ctor += On_UIWorldListItemOnctor;
    }

    public override void Unload()
    {
        On_UIWorldListItem.ctor -= On_UIWorldListItemOnctor;
    }

    private void On_UIWorldListItemOnctor(On_UIWorldListItem.orig_ctor orig, UIWorldListItem self, WorldFileData data, int orderInList, bool canBePlayed)
    {
        orig.Invoke(self, data, orderInList, canBePlayed);
        bool commonData = self.Data.TryGetHeaderData(ModContent.GetInstance<CommonWorldFlags>(), out var _data);
        var worldIcon = (UIElement)typeof(UIWorldListItem).GetField("_worldIcon", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(self);
        var wldData = (WorldFileData)typeof(AWorldListItem).GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(self);
 
        Debug.Assert(wldData != null);

        bool normalWorld = !wldData.RemixWorld
                           && !wldData.DrunkWorld
                           && !wldData.Anniversary
                           && !wldData.DontStarve
                           && !wldData.ForTheWorthy
                           && !wldData.ZenithWorld
                           && !wldData.NotTheBees
                           && !wldData.NoTrapsWorld
                           && wldData.IsHardMode;
        
        if (commonData)
        {
            #region RegularSeedIcon
            
            if (_data.GetBool("CalamityVanilla:HasAstro") && normalWorld)
            {
                var icon = worldIcon;
                var element = new UIImage(ModContent.Request<Texture2D>("CalamityVanilla/Assets/Textures/UI/IconAstro_Default")) {
                    Top = new StyleDimension(0f, 0f),
                    Left = new StyleDimension(1f, 0f),
                    IgnoresMouseInteraction = true
                };
                
                Debug.Assert(icon != null);
                
                icon.Append(element);
            }
            #endregion
        }
    }
}