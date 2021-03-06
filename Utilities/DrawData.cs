﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Desiccation.Utilities
{
	/// <summary>
	/// Used for drawing related data, rectangle related data, and vector2 related data
	/// </summary>
	internal static class DrawData
	{
		/// <summary>
		/// Returns true if thr cursor is colliding with the qualifing rectangle
		/// </summary>
		/// <param name="rectangle">The rectangle.</param>
		/// <returns></returns>
		public static bool ContainsCursor(this Rectangle rectangle)
			=> rectangle.Contains(new Point(Main.mouseX, Main.mouseY)) ? true : false;

		public static void DrawRectangle(this Rectangle rectangle)
		{
			//TODO: Needs Testing
			//rectangle.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
			Main.spriteBatch.Draw(Main.magicPixel, rectangle, Color.White);
		}

		public static float CenterStringXOnScreen(string text, DynamicSpriteFont font)
			=> (Main.screenWidth / 2f) - font.MeasureString(text).X / 2;

		public static Texture2D BlankTexture => ModContent.GetTexture("Desiccation/UI/Textures/Blank");
	}
}