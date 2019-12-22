﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Desiccation.Projectiles.Developer.Chem
{
    class HyperShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hyper Shot");
        }
        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 42;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 20;
            projectile.timeLeft = 600;
        }
        public override void AI()
        { //Thanks, Fargo
            for (int i = 0; i < 10; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * 21);
                offset.Y += (float)(Math.Cos(angle) * 21);
                Dust dust = Main.dust[Dust.NewDust(
                    projectile.Center + offset - new Vector2(4, 4), 0, 0,
                    DustID.Electric, 0, 0, 100, Color.White, 0.5f
                    )];
                dust.velocity = projectile.velocity;
                if (Main.rand.Next(3) == 0)
                    dust.velocity += Vector2.Normalize(offset) * -5f;
                dust.noGravity = true;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; //Makes the projectile unaffected by light
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            target.AddBuff(BuffID.Frostburn, 300);
            target.AddBuff(BuffID.ShadowFlame, 300);
        }
        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 300); //Only Frostburn because Shadowflame has no effect on players (you can make it deal damage to players though, let me know if you do)
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 40; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(projectile.Center, 0, 0, DustID.Electric, 0, 0, 0, Color.White, 1f)];
                dust.noGravity = true;
                dust.velocity *= 3f;
            }
        }
    }
}