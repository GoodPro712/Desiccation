﻿using Desiccation.ModProjectiles.DD2.Ogre;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Desiccation.ModItems.DD2.Ogre
{
	public class EtherianJavelin : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Non consumable throwing javelin.");
		}

		public override void SetDefaults()
		{
			item.value = Item.sellPrice(gold: 1);
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTurn = true;
			item.autoReuse = false;
			item.useTime = item.useAnimation = 25;
			item.rare = ItemRarityID.Pink;
			item.width = item.height = 32;
			item.UseSound = SoundID.DD2_GoblinBomberThrow;
			item.damage = 32;
			item.knockBack = 2;
			item.shoot = ModContent.ProjectileType<EtherianJavelinProjectile>();
			item.shootSpeed = 12;
			item.thrown = true;
		}
	}
}