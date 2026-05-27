using CalamityVanilla.Common;
using CalamityVanilla.Common.Blessings;
using CalamityVanilla.Common.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest;

// [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
[AutoloadHead]
public class Priest : ModNPC
{
    public const string ShopName = "Shop";
    private static int ShimmerHeadIndex;
    private static Profiles.StackedNPCProfile NPCProfile;

    public static LocalizedText BlessingText1 { get; private set; }
    public static LocalizedText BlessingText2 { get; private set; }
    public static LocalizedText BlessingText3 { get; private set; }

    // Sets a unique message when the NPC dies.
    // See also NPCID.Sets.IsTownChild if you just want the message used by Angler and Princess.
    // See ModifyDeathMessage() way below for more details
    //public override LocalizedText DeathMessage => this.GetLocalization("DeathMessage");

    public override void Load()
    {
        // Adds our Shimmer Head to the NPCHeadLoader.
        ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 23; // The total amount of frames the NPC has

        NPCID.Sets.ExtraFramesCount[Type] = 7; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs. This is the remaining frames after the walking frames.
        NPCID.Sets.AttackFrameCount[Type] = 3; // The amount of frames in the attacking animation.
        NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the NPC that it tries to attack enemies.
        NPCID.Sets.AttackType[Type] = 2; // The type of attack the Town NPC performs. 0 = throwing, 1 = shooting, 2 = magic, 3 = melee
        NPCID.Sets.AttackTime[Type] = 180; // The amount of time it takes for the NPC's attack animation to be over once it starts.
        NPCID.Sets.AttackAverageChance[Type] = 20; // The denominator for the chance for a Town NPC to attack. Lower numbers make the Town NPC appear more aggressive.
        NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset.
        NPCID.Sets.ShimmerTownTransform[Type] = true; // This set says that the Town NPC has a Shimmered form. Otherwise, the Town NPC will become transparent when touching Shimmer like other enemies.

        // Connects this NPC with a custom emote.
        // This makes it when the NPC is in the world, other NPCs will "talk about him".
        // By setting this you don't have to override the PickEmote method for the emote to appear.
        NPCID.Sets.FaceEmote[Type] = ModContent.EmoteBubbleType<Emotes.PriestEmote>();

        // Influences how the NPC looks in the Bestiary
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            Direction = -1 // -1 is left and 1 is right. NPCs are drawn facing the left by default but Priest will be drawn facing the right
                          // Rotation = MathHelper.ToRadians(180) // You can also change the rotation of an NPC. Rotation is measured in radians
                          // If you want to see an example of manually modifying these when the NPC is drawn, see PreDraw
        };

        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

        // Set Example Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an example in ExampleMod/Localization/en-US.lang).
        // NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.
        NPC.Happiness
            .SetBiomeAffection<HallowBiome>(AffectionLevel.Love)
            .SetNPCAffection(NPCID.Dryad, AffectionLevel.Love)
            .SetNPCAffection(NPCID.Princess, AffectionLevel.Like)
            .SetNPCAffection(NPCID.Clothier, AffectionLevel.Like)
            .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.Guide, AffectionLevel.Hate)
            .SetBiomeAffection<SnowBiome>(AffectionLevel.Hate)
            .SetBiomeAffection<Common.Personalities.Underworld>(AffectionLevel.Hate); // < Mind the semicolon!

        // This creates a "profile" for Priest, which allows for different textures during a party and/or while the NPC is shimmered.
        NPCProfile = new Profiles.StackedNPCProfile(
            new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture), Texture),
            new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex, Texture + "_Shimmer")
        );

        ContentSamples.NpcBestiaryRarityStars[Type] = 3; // We can override the default bestiary star count calculation by setting this.

        BlessingText1 = this.GetLocalization("BlessingText1");
        BlessingText2 = this.GetLocalization("BlessingText2");
        BlessingText3 = this.GetLocalization("BlessingText3");
    }

    public override void SetDefaults()
    {
        NPC.townNPC = true; // Sets NPC to be a Town NPC
        NPC.friendly = true; // NPC Will not attack player
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = NPCAIStyleID.Passive;
        NPC.damage = 10;
        NPC.defense = 15;
        NPC.lifeMax = 250;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;

        AnimationType = NPCID.Clothier;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
        bestiaryEntry.Info.AddRange([
            // Sets the preferred biomes of this town NPC listed in the bestiary.
            // With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,

				// Sets your NPC's flavor text in the bestiary. (use localization keys)
				new FlavorTextBestiaryInfoElement("Mods.CalamityVanilla.NPCs.Priest.Bestiary"),
        ]);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        int num = NPC.life > 0 ? 1 : 5;

        for (int k = 0; k < num; k++)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
        }

        // Create gore when the NPC is killed.
        if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
        {
            // Retrieve the gore types. This NPC has shimmer and party variants for head, arm, and leg gore. (12 total gores)
            //string variant = "";
            //if (NPC.IsShimmerVariant)
            //    variant += "_Shimmer";
            //if (NPC.altTexture == 1)
            //    variant += "_Party";
            //int hatGore = NPC.GetPartyHatGore();
            //int headGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Head").Type;
            //int armGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Arm").Type;
            //int legGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Leg").Type;

            //// Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
            //if (hatGore > 0)
            //{
            //    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, hatGore);
            //}
            //Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
            //Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
            //Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
            //Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
            //Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
        }
    }

    public override bool CanTownNPCSpawn(int numTownNPCs)
    { // Requirements for the town NPC to spawn.
        if (BossDownedSystem.DownedCryogen)
        {
            // If Priest has spawned in this world/Cryogen has been defeated before, we don't require the user satisfying this condition again for a respawn.
            return true;
        }
        return false;
    }

    public override ITownNPCProfile TownNPCProfile()
    {
        return NPCProfile;
    }

    public override List<string> SetNPCNameList()
    {
        return new List<string>() {
                "Francis",
                "Alvin",
                "Thomas",
                "Ward",
                "Gabriel",
                "Noah",
                "Tobias",
                "Izaiah",
                "Peter",
                "Leo",
                "Gregory",
                "Paul",
                "Alexander",
                "Elijah",
                "Solomon",
                "Martin",
                "Luther",
                "Quinn",
                "Job",
                "Ezekiel"
            };
    }

    public override void FindFrame(int frameHeight)
    {
        /*npc.frame.Width = 40;
        if (((int)Main.time / 10) % 2 == 0)
        {
            npc.frame.X = 40;
        }
        else
        {
            npc.frame.X = 0;
        }*/
    }

    public override string GetChat()
    {
        WeightedRandom<string> chat = new WeightedRandom<string>();

        int taxCollector = NPC.FindFirstNPC(NPCID.TaxCollector);
        int painter = NPC.FindFirstNPC(NPCID.Painter);
        int cyborg = NPC.FindFirstNPC(NPCID.Cyborg);
        if (taxCollector >= 0 && Main.rand.NextBool(4))
        {
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.TaxCollectorChat", Main.npc[taxCollector].GivenName));
        }
        if (painter >= 0 && Main.rand.NextBool(4))
        {
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.TaxCollectorChat", Main.npc[painter].GivenName));
        }
        if (cyborg >= 0 && Main.rand.NextBool(4))
        {
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.TaxCollectorChat", Main.npc[cyborg].GivenName));
        }
        // These are things that the NPC has a chance of telling you when you talk to it.
        chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.DayChat1"));
        chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.DayChat2"));
        chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.DayChat3", Main.LocalPlayer.name));
        chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.DayChat4"));
        chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.DayChat5"), 0.5);
        
        if (!Main.dayTime){
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.NightChat1"));
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.NightChat2"));
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.NightChat3"));
        }

        if (Main.IsItAHappyWindyDay)
        {
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.WindyChat1"));
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.WindyChat2"));
        }

        if (Main.raining)
        {
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.RainChat1"));
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.RainChat2"));
        }

        if (Main.IsItStorming)
        {
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.StormChat1"));
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.StormChat2"));
        }

        if (Main.bloodMoon)
        {
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.BloodMoonChat1"));
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.BloodMoonChat2"));
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.BloodMoonChat3"));
        }

        if (NPC.downedMartians)
        {
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.MartianChat1"));
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.MartianChat2"));
        }

        if (Main.LocalPlayer.ZoneGraveyard)
        {
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.GraveyardChat1"));
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.GraveyardChat2"));
        }

        if (BirthdayParty.PartyIsUp)
        {
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.PartyChat1"));
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.PartyChat2"));
        }

        if (NPC.homeless)
        {
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.HomelessChat1"));
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.HomelessChat2"));
            chat.Add(Language.GetTextValue("Mods.CalamityVanilla.Dialogue.Priest.HomelessChat3"));
        }

        string chosenChat = chat; // chat is implicitly cast to a string. This is where the random choice is made.

        return chosenChat;
    }

    public bool blessingTabOpen;
    public override void SetChatButtons(ref string button, ref string button2)
    { // What the chat buttons are when you open up the chat UI
        button = Language.GetTextValue("LegacyInterface.28"); // This is the key to the word "Shop"
        if (!CVTownNPCUI.PriestUIOpen)
        {
            button2 = "Blessing";
        } else
        {
            button2 = "Close Blessing";
        }
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shop)
    {
        if (firstButton)
        {
            shop = ShopName; // Name of the shop tab we want to open.
            CVTownNPCUI.ClosePanel();
        } 
        else // clicked Blessing button
        {
            if (!CVTownNPCUI.PriestUIOpen)
            {
                switch (Main.rand.Next(1, 4))
                {
                    case 1:
                        Main.npcChatText = BlessingText1.Value;
                        break;
                    case 2:
                        Main.npcChatText = BlessingText2.Format(Main.LocalPlayer.name);
                        break;
                    case 3:
                        Main.npcChatText = BlessingText3.Value;
                        break;
                    default:
                        Main.npcChatText = BlessingText1.Value;
                        break;
                }

                CVTownNPCUI.ShowPriestUI(NPC);

                return;
            }
            else
            {
                CVTownNPCUI.ClosePanel();
            }
        }
    }

    // Not completely finished, but below is what the NPC will sell
    public override void AddShops()
    {
        var npcShop = new NPCShop(Type, ShopName)
            .Add<Corruption.Items.DarkPrismStaff.DarkPrismStaff>()
            .Add(new Item(ModContent.ItemType<Bosses.Cryogen.Drops.FrostGuardStaff.FrostGuardStaff>()) { shopCustomPrice = Item.buyPrice(gold: 15) }) // This example sets a custom price, ExampleNPCShop.cs has more info on custom prices and currency.
            .Add(ItemID.AcornAxe); // Here is an example of how to sell an existing vanilla item.
            //.Add<Items.Consumables.ExampleHealingPotion>(new Condition("Mods.ExampleMod.Conditions.PlayerHasLifeforceBuff", () => Main.LocalPlayer.HasBuff(BuffID.Lifeforce)))
            //.Add<Items.Weapons.ExampleSword>(Condition.MoonPhasesQuarter0)
            ////.Add<ExampleGun>(Condition.MoonPhasesQuarter1)
            //.Add<Items.Ammo.ExampleBullet>(Condition.MoonPhasesQuarter1)
            //.Add<Items.Weapons.ExampleStaff>(ExampleConditions.DownedMinionBoss)
            //.Add<ExampleOnBuyItem>()
            //.Add<EquipMaterial>()
            //.Add<BossItem>();

        //if (ModContent.GetInstance<ExampleModConfig>().ExampleWingsToggle)
        //{
        //    npcShop.Add<ExampleWings>(ExampleConditions.InExampleBiome);
        //}

        //if (ModContent.TryFind("SummonersAssociation/BloodTalisman", out ModItem bloodTalisman))
        //{
        //    npcShop.Add(bloodTalisman.Type);
        //}
        npcShop.Register(); // Name of this shop tab
    }

    public override void ModifyActiveShop(string shopName, Item[] items)
    {
        foreach (Item item in items)
        {
            // Skip 'air' items and null items.
            if (item == null || item.type == ItemID.None)
            {
                continue;
            }

            //// If NPC is shimmered then reduce all prices by 50%.
            //if (NPC.IsShimmerVariant)
            //{
            //    int value = item.shopCustomPrice ?? item.value;
            //    item.shopCustomPrice = value / 2;
            //}
        }
    }

    //public override void ModifyNPCLoot(NPCLoot npcLoot)
    //{
    //    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ExampleCostume>()));
    //}

    // Make this Town NPC teleport to the King and/or Queen statue when triggered. Return toKingStatue for only King Statues. Return !toKingStatue for only Queen Statues. Return true for both.
    public override bool CanGoToStatue(bool toKingStatue) => true;

    //public override bool ModifyDeathMessage(ref NetworkText customText, ref Color color)
    //{
    //    // This example shows how you would further customize the message, in this case just for the shimmer variant.
    //    if (NPC.IsShimmerVariant)
    //    {
    //        customText = NetworkText.FromKey(this.GetLocalizationKey("DeathMessageAlt"), NPC.GetFullNetName());
    //        color = Color.Yellow;
    //    }
    //    return true;
    //}

    public override void TownNPCAttackStrength(ref int damage, ref float knockback)
    {
        damage = 20;
        knockback = 4f;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
    {
        cooldown = 180;
        randExtraCooldown = 30;
    }

    public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
    {
        projType = ModContent.ProjectileType<Corruption.Items.DarkPrismStaff.DarkPrism>();
        attackDelay = 200;
    }

    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
    {
        multiplier = 12f;
        randomOffset = 2f;
        // SparklingBall is not affected by gravity, so gravityCorrection is left alone.
    }

    //public override void LoadData(TagCompound tag)
    //{
    //    NumberOfTimesTalkedTo = tag.GetInt("numberOfTimesTalkedTo");
    //}

    //public override void SaveData(TagCompound tag)
    //{
    //    tag["numberOfTimesTalkedTo"] = NumberOfTimesTalkedTo;
    //}

    // Let the NPC "talk about" minion boss
    public override int? PickEmote(Player closestPlayer, List<int> emoteList, WorldUIAnchor otherAnchor)
    {
        // By default this NPC will have a chance to use the Minion Boss Emote even if Minion Boss is not downed yet
        int type = ModContent.EmoteBubbleType<Emotes.CryogenEmote>();
        // If the NPC is talking to the TaxCollector, it will be more likely to react with angry emote
        if (otherAnchor.entity is NPC { type: NPCID.TaxCollector })
        {
            type = EmoteID.EmotionAnger;
        }

        // Make the selection more likely by adding it to the list multiple times
        for (int i = 0; i < 4; i++)
        {
            emoteList.Add(type);
        }

        // Use this or return null if you don't want to override the emote selection totally
        return base.PickEmote(closestPlayer, emoteList, otherAnchor);
    }
}