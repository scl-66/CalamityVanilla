using CalamityVanilla.Content.Vanity.JonaDevSet;
using CalamityVanilla.Content.Vanity.SixtyDevSet;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityVanilla.Common;

public class AddDevSetsToTreasureBags : ILoadable
{
    public void Load(Mod mod)
    {
        // this makes the dev drops guaranteed.
        //IL_Player.TryGettingDevArmor += IL_Player_TryGettingDevArmor;
        On_Player.TryGettingDevArmor += On_Player_TryGettingDevArmor;
    }
    //private void IL_Player_TryGettingDevArmor(ILContext il)
    //{
    //    var c = new ILCursor(il);

    //    ILLabel l = il.DefineLabel();
    //    c.GotoNext(i => i.MatchLdcI4(16));
    //    c.Remove();
    //    c.Emit(OpCodes.Ldc_I4, 1);
    //}
    private void On_Player_TryGettingDevArmor(On_Player.orig_TryGettingDevArmor orig, Player self, IEntitySource source)
    {
        if (Main.rand.Next(Main.tenthAnniversaryWorld ? 8 : 16) == 0)
        {
            switch (Main.rand.Next(2))
            {
                case 0:
                    self.QuickSpawnItem(source, ModContent.ItemType<JonaWig>());
                    self.QuickSpawnItem(source, ModContent.ItemType<JonaBody>());
                    self.QuickSpawnItem(source, ModContent.ItemType<JonaLegs>());
                    self.QuickSpawnItem(source, ModContent.ItemType<JonaWings>());
                    break;
                case 1:
                    self.QuickSpawnItem(source, ModContent.ItemType<SixtyHead>());
                    self.QuickSpawnItem(source, ModContent.ItemType<SixtyBody>());
                    self.QuickSpawnItem(source, ModContent.ItemType<SixtyLegs>());
                    self.QuickSpawnItem(source, ModContent.ItemType<SixtyWings>());
                    break;
            }
        }
        orig(self, source);
    }
    public void Unload()
    {
    }
}
