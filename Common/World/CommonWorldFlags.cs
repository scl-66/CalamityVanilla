using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityVanilla.Common.World;

internal sealed class CommonWorldFlags : ModSystem
{
    public static bool AstroOrHallow;
    
    #if DEBUG
    public override void PostUpdateEverything()
    {   
        /*
        if (Main.keyState.IsKeyDown(Keys.L) && !Main.oldKeyState.IsKeyDown(Keys.L))
        {
            AstroOrHallow = !AstroOrHallow;

            string status = AstroOrHallow ? "enabled" : "disabled";
            Main.NewText($"Astro/Hallow flag is now {status}.");
        }
        
        if (Main.keyState.IsKeyDown(Keys.K) && !Main.oldKeyState.IsKeyDown(Keys.K))
        {
            string status = AstroOrHallow ? "enabled" : "disabled";
            Main.NewText($"Astro/Hallow flag is {status}.");
        }
        */
    }
#endif
    
    public override void OnWorldLoad()
    {
        AstroOrHallow = false;
    }
    
    public override void OnWorldUnload() 
    {
        AstroOrHallow = false;
    }
    
    public override void SaveWorldData(TagCompound tag)
    {
        tag["CalamityVanilla:HasAstro"] = AstroOrHallow; 
    }
    
    public override void SaveWorldHeader(TagCompound tag)
    {
        tag["CalamityVanilla:HasAstro"] = AstroOrHallow;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        AstroOrHallow = tag.GetBool("CalamityVanilla:HasAstro");
    }
}