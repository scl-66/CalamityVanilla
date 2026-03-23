using CalamityVanilla.Content.Buffs.Debuffs;
using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.Particles;
using CalamityVanilla.Content.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Underground.Items.Ores.PostHardmode;

public class UraniumOre : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<UraniumOreTile>(), 0);
        Item.value = 2050;
        Item.rare = ModContent.RarityType<CobaltRarity>();
    }
}
public class PlutoniumOre : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<PlutoniumOreTile>(), 0);
        Item.value = 2200;
        Item.rare = ModContent.RarityType<CobaltRarity>();
    }
}

public class UraniumOreTile : ModTile
{
    public float val;
    private static Asset<Texture2D> _explosionTexture;
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileLighted[Type] = true;
        MinPick = 225;
        MineResist = 5f;

        TileID.Sets.Ore[Type] = true;
        TileID.Sets.FriendlyFairyCanLureTo[Type] = true;
        Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
        Main.tileOreFinderPriority[Type] = 850; // Metal Detector value, see https://terraria.wiki.gg/wiki/Metal_Detector
        Main.tileShine2[Type] = true; // Modifies the draw color slightly.
        Main.tileShine[Type] = 10000; // How often tiny dust appear off this tile. Larger is less frequently


        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(226, 246, 87), name);

        DustType = DustID.Chlorophyte;
        VanillaFallbackOnModDeletion = TileID.LunarOre;
        HitSound = SoundID.Tink;
    }

    public override void Load()
    {
        if (Name == "UraniumOreTile")
        {
            _explosionTexture = Mod.Assets.Request<Texture2D>("Content/Underground/Items/Ores/PostHardmode/NuclearExplosion");
            On_Player.PickTile += On_Player_PickTile;
        }
    }

    private void On_Player_PickTile(On_Player.orig_PickTile orig, Player self, int x, int y, int pickPower)
    {
        orig(self, x, y, pickPower);
        if (pickPower < 225)
        {
            if (Main.tile[x, y].TileType == ModContent.TileType<UraniumOreTile>() || Main.tile[x, y].TileType == ModContent.TileType<PlutoniumOreTile>())
            {
                LocalizedText DeathText = Language.GetText($"Mods.CalamityVanilla.DeathMessage.NuclearExplosion{Main.rand.Next(1, 12)}");
                int dir = (-(int)(x * 16 - self.position.X) < 0 ? -1 : 1) * 5;
                self.ClearBuff(ModContent.BuffType<IrradiatedDebuff>());
                self.Hurt(PlayerDeathReason.ByCustomReason(DeathText.ToNetworkText(self.name)), 999999, dir, false, false, -1, false, 999999f);

                var p = AnimatedParticle.RequestAnimatedParticle();
                p.SetTypeInfo(10, 40, _explosionTexture, new Color(1, 1, 1, 0.65f));
                p.LocalPosition = new Vector2(x, y).ToWorldCoordinates() + new Vector2(0, -40);
                p.Scale = Vector2.One;
                Main.ParticleSystem_World_OverPlayers.Add(p);

                for (int i = 0; i < 75; i++)
                {
                    int w = 8;
                    Dust d = Dust.NewDustPerfect(new Vector2(x, y).ToWorldCoordinates()+Main.rand.NextVector2Circular(w/4, w/4), DustID.Smoke, Main.rand.NextVector2Circular(w, w), 128);
                    d.scale = Main.rand.NextFloat(0.75f, 1.65f);
                }
                for (int i = 0; i < 55; i++)
                {
                    int w = 10;
                    Dust d = Dust.NewDustPerfect(new Vector2(x, y).ToWorldCoordinates() + Main.rand.NextVector2Circular(w / 4, w / 4), DustID.Torch, Main.rand.NextVector2Circular(w, w));
                    d.noGravity = true;
                    d.scale = Main.rand.NextFloat(0.75f, 1.25f);
                }
                for (int i = 0; i < 15; i++)
                {
                    Gore g = Gore.NewGoreDirect(self.GetSource_FromThis(), new Vector2(x, y).ToWorldCoordinates(0, 0), Main.rand.NextVector2Circular(2, 2), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1), Main.rand.NextFloat(0.25f, 1f));
                    g.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }
                for (float i = -0.5f; i <= 0f; i += 0.1f)
                {
                    SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Pitch = i, MaxInstances = 10}, new Vector2(x, y).ToWorldCoordinates());
                    SoundEngine.PlaySound(SoundID.Item14 with { Pitch = i, MaxInstances = 10}, new Vector2(x, y).ToWorldCoordinates());
                }
                SoundEngine.PlaySound(SoundID.Item74, new Vector2(x, y).ToWorldCoordinates());
            }
        }
    }

    public override bool CanExplode(int i, int j)
    {
        return false;
    }

    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
    }

    public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
    {
        if (Main.rand.Next(0, 500) == 0)
        {
            float speed = 0.1f;
            Dust d = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<UraniumRadDust>(), Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed, speed));
            d.velocity *= 0.5f;
        }
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        if (Main.rand.Next(0, 300) == 0)
        {
            val += Main.rand.NextFloat(-0.1f, 0.1f);
        }
        val = Math.Clamp(val, 0.75f, 1.25f);
        r = 0.60f / 3f * val;
        g = 0.85f / 3f * val;
        b = 0.05f / 3f * val;
    }
}

public class PlutoniumOreTile : UraniumOreTile {
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileLighted[Type] = true;
        MinPick = 225;
        MineResist = 5f;

        TileID.Sets.Ore[Type] = true;
        TileID.Sets.FriendlyFairyCanLureTo[Type] = true;
        Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
        Main.tileOreFinderPriority[Type] = 860; // Metal Detector value, see https://terraria.wiki.gg/wiki/Metal_Detector
        Main.tileShine2[Type] = true; // Modifies the draw color slightly.
        Main.tileShine[Type] = 10000; // How often tiny dust appear off this tile. Larger is less frequently
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(232, 186, 86), name);

        DustType = DustID.Palladium;
        VanillaFallbackOnModDeletion = TileID.LunarOre;
        HitSound = SoundID.Tink;
    }
    public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
    {
        if (Main.rand.Next(0, 500) == 0)
        {
            float speed = 0f;
            Dust d = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<PlutoniumRadDust>(), Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed, speed), 1, Color.Orange);
            d.velocity *= 0.5f;
        }
    }
    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        if (Main.rand.Next(0, 300) == 0)
        {
            val += Main.rand.NextFloat(-0.1f, 0.1f);
        }
        val = Math.Clamp(val, 0.75f, 1.25f);
        r = 0.9f / 3f * val;
        g = 0.6f / 3f * val;
        b = 0.3f / 3f * val;
    }
}
