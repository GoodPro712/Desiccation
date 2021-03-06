using Desiccation.UI.UIStates;
using Desiccation.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Audio;
using Terraria.UI;
using Terraria.UI.Chat;
using static Desiccation.Utilities.PlayerData;

namespace Desiccation
{
	public class Desiccation : Mod
	{
		//--Stuff for GoodPro712 to do:
		//TODO: Replace eerie messages with ambient sounds
		//TODO: Change the main menu rework into "themes"
		//TODO: Make vanity music boxes its own mod
		//TODO: Make banner rework unique
		//TODO: world layer id
		//TODO: Rework flails to find a reason for players to use them?
		//TODO: 0.11.6 All items don't burn via GlobalItem.CanBurnInLava() return false; bruh turns out i need to il edit it
		//TODO: Make squirrels fall out of trees in random ammounts and sometimes none. Maybe make use of NPCData.SpawnMoreThanOneNPCOfTheSameType
		//TODO: Piggybank UI when nurse npc chat is active
		//TODO: Music Box ID thing
		//TODO: Show info on vanity accessories. Code for this in antisocial
		//TODO: Search for player names and worlds ui
		//TODO: Save but dont exit button
		//TODO: Statue enemies drop loot if requirements are met
		//TODO: Ammo notice bottom left
		//TODO: Rework dev weapons. balanace out
		//TODO: Overequipping
		//TODO: Quick stack ammo and coins
		//TODO: Multitool Rework with select ui and random thing. Ideas in #concept-discussion. fix multitool sprites aswell
		//TODO: Able to hold right click to open bags and crates etc. IL Edit
		//TODO: Main menu replacement Texture2D's fade in and out when loaded and unloaded. check #coding for a discord link on some info on how to do this
		//TODO: discord tags for credits
		//TODO: IL the main menu music possibly tmod contribution
		//TODO: change the color of the hover main menu tabs. have to redraw them all posssibly IL
		//TODO: Create desiccation email, youtube and twitter and twitter discord webhook
		//TODO: Add new boss checklist calls to bosses when coded.
		//TODO: Make Pumpkins spwn at any time.

		//--Stuff for Lemmy to do:
		//TODO: Rework sifting pan. the stats are in #stating
		//TODO: Rework overbright torch. Mak eit burn out after a certain amount of time of it being selected. Maybe progress bar under sprite? for goodpro to do? idk
		//TODO: Finish the flails off
		//TODO: Code the dino stuff
		//TODO: Code the Timelost items
		//TODO: Code the random gun

		//--Stuff for Reb to do:
		//TODO: Enchanted flail
		//TODO: Banner rework? stats in the checklist google doc.

		//--Stuff for Nobody to do:
		//TODO: Constructer potion. stats pinned in #stating

		private bool DebugMode => false;

		#region Fields

		private Texture2D vanillaLogoDay;
		private Texture2D vanillaLogoNight;
		private Texture2D vanillaSkyBackground;
		public Texture2D vanillaFrontMainMenuBackground;
		public Texture2D vanillaMiddleMainMenuBackground;
		public Texture2D vanillaBackMainMenuBackground;
		public Texture2D[] vanillaCloud = new Texture2D[22];
		internal static CreditMenu creditMenuUI;
		private bool unloadCalled;
		private bool isInVersionRectangle;
		private bool isInDiscordRectangle;
		private bool isInGithubRectangle;
		private bool isInPatreonRectangle;
		private bool isInCreditRectangle;
		private bool isInLinkMenuRectangle;
		private volatile bool swap = false;
		private volatile bool stop = false;
		private bool linksOpen;
		private bool lastMouseLeft;
		public float fadePercent = 0;
		private static readonly string releaseSuffix = "Beta Release!";
		public static DesiccationGlobalConfig GlobalConfig = ModContent.GetInstance<DesiccationGlobalConfig>();
		public static DesiccationClientsideConfig ClientConfig = ModContent.GetInstance<DesiccationClientsideConfig>();
		private ManualResetEvent swapComplete = new ManualResetEvent(false);

		#endregion Fields

		#region tModLoader Hooks

		public override void Load()
		{
			#region Main Menu Backups

			vanillaFrontMainMenuBackground = Main.backgroundTexture[173];
			vanillaMiddleMainMenuBackground = Main.backgroundTexture[172];
			vanillaBackMainMenuBackground = Main.backgroundTexture[171];
			for (int i = 0; i < vanillaCloud.Length; i++)
			{
				vanillaCloud[i] = Main.cloudTexture[i];
			}
			vanillaSkyBackground = Main.backgroundTexture[0];
			vanillaLogoDay = Main.logoTexture;
			vanillaLogoNight = Main.logo2Texture;

			#endregion Main Menu Backups

			unloadCalled = false;
			Main.OnTick += OnUpdate;
			Main.OnPostDraw += OnPostDrawEvent;
			IL.Terraria.Main.DrawInterface_14_EntityHealthBars += HookDrawInterface_14_EntityHealthBars;
		}

		public override void Unload()
		{
			Main.logoTexture = vanillaLogoDay;
			Main.logo2Texture = vanillaLogoNight;
			Main.backgroundTexture[0] = vanillaSkyBackground;
			for (int i = 0; i < vanillaCloud.Length; i++)
			{
				Main.cloudTexture[i] = vanillaCloud[i];
			}
			Main.backgroundTexture[173] = vanillaFrontMainMenuBackground;
			Main.backgroundTexture[172] = vanillaMiddleMainMenuBackground;
			Main.backgroundTexture[171] = vanillaBackMainMenuBackground;

			Main.OnTick -= OnUpdate;
			Main.OnPostDraw -= OnPostDrawEvent;
			unloadCalled = true;
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int deathText = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Death Text"));
			if (deathText != -1)
			{
				layers.Insert(deathText, new LegacyGameInterfaceLayer("Desiccation: Respawn Timer", delegate
				{
					if (MyPlayer.dead && ModContent.GetInstance<DesiccationClientsideConfig>().RespawnTimer)
					{
						DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontDeathText, string.Format("{0:f" + ModContent.GetInstance<DesiccationClientsideConfig>().RespawnTimerDecimal + "}", MyPlayer.respawnTimer / 60f), new Vector2((Main.screenWidth / 2) - Main.fontDeathText.MeasureString(string.Format("{0:f" + ModContent.GetInstance<DesiccationClientsideConfig>().RespawnTimerDecimal + "}", MyPlayer.respawnTimer / 60f)).X / 2f, Main.screenHeight / 2 - 70), MyPlayer.GetDeathAlpha(Color.Transparent));
					}
					return true;
				},
					InterfaceScaleType.UI)
				);
			}
			int mouseText = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseText != -1)
			{
				layers.Insert(mouseText, new LegacyGameInterfaceLayer("Desiccation: Player Name", delegate
				{
					if (!Main.gameMenu && ModContent.GetInstance<DesiccationClientsideConfig>().NameInfo && !DUtils.IsInventoryOpen)
					{
						string text = $"{MyName} in {Main.worldName}";
						Vector2 size = Utils.DrawBorderString(Main.spriteBatch, text, new Vector2(DrawData.CenterStringXOnScreen(text, Main.fontMouseText), 2f), Color.WhiteSmoke);
						Rectangle rectangle = new Rectangle((int)DrawData.CenterStringXOnScreen(text, Main.fontMouseText), 2, (int)size.X + 2, (int)size.Y - 10);
						if (rectangle.ContainsCursor())
						{
							Main.hoverItemName = "Type in chat to change names. '/playername NEW NAME' to change player name, '/worldname NEW NAME' to change world name.";
						}
					}
					return true;
				},
					InterfaceScaleType.UI)
				);
			}
		}

		public override void PreSaveAndQuit()
		{
			WorldGen.setBG(0, 6);
		}

		public override void PostSetupContent()
		{
			if (DebugMode)
			{
				try
				{
					FieldInfo texturesField = typeof(Mod).GetField("textures", BindingFlags.Instance | BindingFlags.NonPublic);
					IDictionary<string, Texture2D> textures = (IDictionary<string, Texture2D>)texturesField?.GetValue(this);
					foreach (KeyValuePair<string, Texture2D> texture in textures)
					{
						Logger.Debug($"Texture: {texture}\nWidth * Height * 4: {texture.Value.Width * texture.Value.Height * 4}\nKey: {texture.Key}");
					}
				}
				catch (Exception e)
				{
					Logger.Error("Desiccation textures reflection error: " + e);
				}
			}
			/*
			swap = true;
			swapComplete.WaitOne();
			swapComplete.Reset();*/
		}
		/*
		public override void Close()
		{
			if (Main.music[MusicID.Title] == GetMusic("Sounds/Music/Title"))
			{
				stop = true;
				swap = true;
				swapComplete.WaitOne();
			}
			base.Close();
		}

		public static void Swap<T>(ref T a, ref T b)
		{
			var tmp = a;
			a = b;
			b = tmp;
		}

		public override void UpdateMusic(ref int music, ref MusicPriority priority)
		{
			if (swap)
			{
				int slot = GetSoundSlot(SoundType.Music, "Sounds/Music/Title");
				Swap(ref Main.music[MusicID.Title], ref Main.music[slot]);
				Swap(ref Main.musicFade[MusicID.Title], ref Main.musicFade[slot]);
				swap = false;

				if (stop)
					((MusicStreaming)Main.music[slot]).Dispose();

				swapComplete.Set();
			}
		}*/

		#endregion tModLoader Hooks

		#region Events

		private void OnUpdate()
		{
			if (Main.gameMenu)
			{
				fadePercent += MathHelper.ToRadians(1.7f * 360f / 60f);

				if (Mods.Overhaul == null)
				{
					if (ModContent.GetInstance<DesiccationClientsideConfig>().MainMenuDesert)
					{
						Main.dayTime = true;
						Main.time = 40000;
						Main.sunModY = 5;

						if (!Main.dedServ)
						{
							BGData.LoadBackgrounds(new List<int>() { 173, 171, 172, 20, 21, 22 });
							BGData.MainMenuBackgroundSwap(20, 21, 22);
							for (int vanillaCloudTextureID = 0; vanillaCloudTextureID < vanillaCloud.Length; vanillaCloudTextureID++)
							{
								Main.cloudTexture[vanillaCloudTextureID] = DrawData.BlankTexture;
							}
							Main.backgroundTexture[0] = GetTexture("UI/Textures/Sky");
							Main.logoTexture = Main.logo2Texture = GetTexture("UI/Textures/DesertLogo");
						}
					}
					else
					{
						if (!Main.dedServ)
						{
							Main.logoTexture = vanillaLogoDay;
							Main.logo2Texture = vanillaLogoNight;
							Main.backgroundTexture[0] = vanillaSkyBackground;
							for (int i = 0; i < vanillaCloud.Length; i++)
							{
								Main.cloudTexture[i] = vanillaCloud[i];
							}
							Main.backgroundTexture[173] = vanillaFrontMainMenuBackground;
							Main.backgroundTexture[172] = vanillaMiddleMainMenuBackground;
							Main.backgroundTexture[171] = vanillaBackMainMenuBackground;
						}
					}
					if (Main.menuMode == 10006 && !unloadCalled)
					{
						Unload();
						return;
					}
				}
			}
		}

		private void OnPostDrawEvent(GameTime gametime)
		{
			if (Main.gameMenu)
			{
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
				MainMenuLinkDraw("Desiccation v" + Version + " - " + releaseSuffix, 8, "https://forums.terraria.org/index.php?threads/84525/", 10, ref isInVersionRectangle, "Forum");
				MainMenuLinkDraw("Desiccation Credits ", 36, "", -18, ref isInCreditRectangle, "Credits");
				if (!linksOpen)
				{
					MainMenuLinkDraw("Useful Links", 64, "", -46, ref isInLinkMenuRectangle, "Useful Links");
				}
				else
				{
					MainMenuLinkDraw("Need support? Discord!", 70, "https://discordapp.com/invite/btQqCdt", -52, ref isInDiscordRectangle, "Discord");
					//TODO: fix github link
					MainMenuLinkDraw("Got an issue to report? Github!", 100, "https://github.com/GoodPro712/DesiccationModBrowser", -82, ref isInGithubRectangle, "Github");
					MainMenuLinkDraw("Want to support development? Patreon!", 130, "https://patreon.com/tModLoader_Desiccation", -112, ref isInPatreonRectangle, "Patreon");
				}
				//TODO: Credit Menu
				Main.DrawCursor(Main.DrawThickCursor());
				Main.spriteBatch.End();
			}
			lastMouseLeft = Main.mouseLeft;
		}

		#endregion Events

		#region Main Menu Text

		private void MainMenuLinkDraw(string text, float Y, string process, int offset, ref bool IsIn, string type)
		{
			#region Color Setting

			Color color;
			if (type == "Useful Links")
			{
				color = IsIn ? Color.White * 0.75f : Color.Lerp(Color.White * 0.50f, Color.Lerp(Color.SlateGray, Color.Black, 0.5f), (float)Math.Sin(fadePercent));
			}
			else
			{
				color = IsIn ? Color.White * 0.75f : Color.SlateGray;
			}
			Color newColor = Color.Black;
			for (int i = 0; i < 5; i++)
			{
				if (i == 4)
				{
					newColor = color;
					newColor.R = (byte)((255 + newColor.R) / 2);
					newColor.G = (byte)((255 + newColor.G) / 2);
					newColor.B = (byte)((255 + newColor.B) / 2);
				}
				newColor.A = (byte)(newColor.A * 0.5f);
			}

			#endregion Color Setting

			#region Rectangle & Hover/Click

			Vector2 size = ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, text, new Vector2(10f, Y), Color.Transparent, 0f, Vector2.Zero, new Vector2(1f, 1f));
			Rectangle rectangle = new Rectangle((int)10f, (int)Y, (int)size.X - 10, (int)size.Y + offset);
			if (rectangle.Contains(new Point(Main.mouseX, Main.mouseY)))
			{
				if (IsIn == false)
				{
					IsIn = true;
					Main.PlaySound(SoundID.MenuTick);
				}
			}
			else if (IsIn == true && !rectangle.Contains(new Point(Main.mouseX, Main.mouseY)))
			{
				IsIn = false;
			}
			if (!lastMouseLeft && Main.mouseLeft && rectangle.Contains(new Point(Main.mouseX, Main.mouseY)))
			{
				if (type == "Credits")
				{
					if (Main.menuMode == 0)
					{
						Main.PlaySound(SoundID.MenuOpen);
						creditMenuUI = new CreditMenu();
						UserInterface.ActiveInstance.SetState(creditMenuUI);
					}
				}
				else if (type == "Useful Links")
				{
					Main.PlaySound(SoundID.MenuOpen);
					linksOpen = true;
				}
				else
				{
					Main.PlaySound(SoundID.MenuOpen);
					Process.Start(process);
				}
			}

			#endregion Rectangle & Hover/Click

			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, text, new Vector2(10f, Y), newColor, 0f, Vector2.Zero, new Vector2(1f, 1f));
		}

		#endregion Main Menu Text

		#region IL Editing

		private void HookDrawInterface_14_EntityHealthBars(ILContext il)
		{
			if (ModContent.GetInstance<DesiccationGlobalConfig>().OOAWaitingTimeSkip)
			{
				ILCursor c = new ILCursor(il);
				if (!c.TryGotoNext(i => i.MatchLdstr("{0}")))
				{
					return;
				}
				c.Index++;
				c.Emit(Mono.Cecil.Cil.OpCodes.Ldstr, "{0}");
				c.EmitDelegate<Func<string, string, string>>((original, append) =>
				{
					append = "Right Click to Skip: ";
					return append + original;
				});
			}
		}

		#endregion IL Editing
	}
}