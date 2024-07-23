using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace CalamityVanilla.Content.Items.Weapons.Melee
{
    public class Icebreaker : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToSword(40, 8, 5);
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.shoot = ModContent.ProjectileType<Projectiles.Melee.Icebreaker>();
            Item.shootSpeed = 10;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }
    }
}
