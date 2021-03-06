﻿using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Desiccation.ModCommands
{
	public class Kill : ModCommand
	{
		public override CommandType Type => CommandType.Chat;
		public override string Command => "kill";
		public override string Usage => "/kill";
		public override string Description => "Kills the user of the comand.";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			PlayerDeathReason damageSource;
			damageSource = PlayerDeathReason.ByCustomReason(caller.Player.name + " went suicidal.");
			caller.Player.KillMe(damageSource, 9999, 0);
		}
	}
}