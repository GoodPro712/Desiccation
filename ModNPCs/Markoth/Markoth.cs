using Terraria.ID;
using Terraria.ModLoader;

namespace Desiccation.ModNPCs.Markoth
{
	public class Markoth : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Markoth, The Fallen Magician");
		}

		public override void SetDefaults()
		{
			npc.boss = true;
			npc.width = 18;
			npc.height = 40;
			npc.damage = 14;
			npc.defense = 6;
			npc.lifeMax = 200;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath2;
			npc.value = 60f;
			npc.knockBackResist = 0.5f;
			npc.behindTiles = false;
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.625 * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.6f);
		}
	}
}