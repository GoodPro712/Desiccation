using Desiccation.Projectiles.EyeOfCthulhu;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Desiccation.Items.EyeOfCuthulu
{
	public class EyeOfCthulhuStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("'It's very gaze has the power to kill'");
			Item.staff[item.type] = true; //this makes the useStyle animate as a staff instead of as a gun
		}

		public override void SetDefaults()
		{
			item.damage = 20;
			item.summon = true;
			item.mana = 12;
			item.width = 40;
			item.height = 40;
			item.useTime = 25;
			item.useAnimation = 25;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.knockBack = 5;
			item.value = Item.sellPrice(silver: 20);
			item.rare = ItemRarityID.Green;
			item.UseSound = SoundID.Item20;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<EyeOfCthulhuStaffProjectile>();
			item.shootSpeed = 16f;
		}
	}
}