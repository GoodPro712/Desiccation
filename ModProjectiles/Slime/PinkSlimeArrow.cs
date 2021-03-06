﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Desiccation.ModProjectiles.Slime
{
    class PinkSlimeArrow : ModProjectile
    {
        int Bounces;
        bool Washed;

        //TODO: Make gel on arrow pink (Crim's already on it)

        public override void SetDefaults()
        {
            projectile.arrow = true;
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.noDropItem = true;
            projectile.penetrate = 3;
            Bounces = 2;
            Washed = false;
        }

        public override void AI()
        {
            if (projectile.wet && !projectile.lavaWet)
            {
                if (projectile.ai[1] == 0 || projectile.ai[1] == 1)
                {
                    if (projectile.ai[1] == 1)
                    {
                        Main.PlaySound(SoundID.LiquidsWaterLava, projectile.position);
                    }
                    Projectile.NewProjectile(projectile.position, projectile.velocity, ProjectileID.WoodenArrowFriendly, projectile.damage, 0, projectile.owner);
                }
                else if (projectile.ai[1] == 2)
                {
                    Projectile.NewProjectile(projectile.position, projectile.velocity, ProjectileID.CursedArrow, projectile.damage, 0, projectile.owner);
                }
                else if (projectile.ai[1] == 3)
                {
                    Projectile.NewProjectile(projectile.position, projectile.velocity, ProjectileID.FrostburnArrow, projectile.damage, 0, projectile.owner);
                }
                Washed = true;
                projectile.Kill();
            }
            else if (projectile.ai[1] == 0 && projectile.lavaWet)
            {
                projectile.ai[1] = 1;
            }
            if (projectile.ai[1] == 1)
            { //Flaming
                int flame = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100);
                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[flame].noGravity = true;
                    Main.dust[flame].scale *= 2f;
                }
            }
            else if (projectile.ai[1] == 2)
            { //Cursed
                int cursedflame = Dust.NewDust(projectile.position, projectile.width, projectile.height, 75, 0f, 0f, 100);
                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[cursedflame].noGravity = true;
                    Main.dust[cursedflame].scale *= 2f;
                }
            }
            else if (projectile.ai[1] == 3)
            { //Frostburn
                int frostburn = Dust.NewDust(projectile.position, projectile.width, projectile.height, 135, 0f, 0f, 100);
                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[frostburn].noGravity = true;
                    Main.dust[frostburn].scale *= 2f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Bounces > 0)
            {
                Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
                Main.PlaySound(SoundID.Item56, projectile.position);
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X * 0.75f;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y * 0.75f;
                }
                projectile.penetrate -= 1;
                Bounces -= 1;
                if (Bounces <= 0)
                {
                    if (projectile.ai[1] == 0)
                    {
                        Projectile.NewProjectile(projectile.position, projectile.velocity * 0.75f, ProjectileID.WoodenArrowFriendly, projectile.damage, 0, projectile.owner);
                    }
                    else if (projectile.ai[1] == 1)
                    {
                        Projectile.NewProjectile(projectile.position, projectile.velocity * 0.75f, ProjectileID.FireArrow, projectile.damage, 0, projectile.owner);
                    }
                    else if (projectile.ai[1] == 2)
                    {
                        Projectile.NewProjectile(projectile.position, projectile.velocity * 0.75f, ProjectileID.CursedArrow, projectile.damage, 0, projectile.owner);
                    }
                    else if (projectile.ai[1] == 3)
                    {
                        Projectile.NewProjectile(projectile.position, projectile.velocity * 0.75f, ProjectileID.FrostburnArrow, projectile.damage, 0, projectile.owner);
                    }
                    projectile.Kill();
                }
                return false;
            }
            return base.OnTileCollide(oldVelocity);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.ai[1] == 1)
            {
                target.AddBuff(BuffID.OnFire, 180);
            }
            else if (projectile.ai[1] == 2)
            {
                target.AddBuff(BuffID.CursedInferno, 180);
            }
            else if (projectile.ai[1] == 3)
            {
                target.AddBuff(BuffID.Frostburn, 180);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Bounces > 0)
            {
                Main.PlaySound(SoundID.Item56, projectile.position);
                projectile.velocity.X = -projectile.oldVelocity.X * 0.75f;
                if (projectile.oldVelocity.Y > 0)
                {
                    projectile.velocity.Y = -projectile.oldVelocity.Y * 0.5f;
                }
                else
                {
                    projectile.velocity.Y = -projectile.oldVelocity.Y * 1.5f;
                }
                if (projectile.ai[1] > 0)
                {
                    projectile.ai[1] = 0;
                    Main.PlaySound(SoundID.LiquidsWaterLava, projectile.position);
                }
                projectile.damage /= 2;
                Bounces -= 1;
                if (Bounces <= 0)
                {
                    target.immune[projectile.owner] = 20;
                    if (projectile.ai[1] == 0)
                    {
                        Projectile.NewProjectile(projectile.position, projectile.velocity * 0.5f, ProjectileID.WoodenArrowFriendly, projectile.damage, 0, projectile.owner);
                    }
                    else if (projectile.ai[1] == 1)
                    {
                        Projectile.NewProjectile(projectile.position, projectile.velocity * 0.5f, ProjectileID.FireArrow, projectile.damage, 0, projectile.owner);
                    }
                    else if (projectile.ai[1] == 2)
                    {
                        Projectile.NewProjectile(projectile.position, projectile.velocity * 0.5f, ProjectileID.CursedArrow, projectile.damage, 0, projectile.owner);
                    }
                    else if (projectile.ai[1] == 3)
                    {
                        Projectile.NewProjectile(projectile.position, projectile.velocity * 0.5f, ProjectileID.FrostburnArrow, projectile.damage, 0, projectile.owner);
                    }
                    projectile.Kill();
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCDeath1.WithVolume(0.5f), projectile.position);
            int num;
            for (int i = 0; i < 10; i = num + 1)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.PinkSlime, 0f, 0f, 100);
                num = i;
            }
        }
    }
}
