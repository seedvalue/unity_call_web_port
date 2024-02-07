using UnityEngine;

namespace Colorful
{
	[AddComponentMenu("Colorful FX/Color Correction/Posterize")]
	[HelpURL("http://www.thomashourdel.com/colorful/doc/color-correction/posterize.html")]
	[ExecuteInEditMode]
	public class Posterize : BaseEffect
	{
		[Range(2f, 255f)]
		[Tooltip("Number of tonal levels (brightness values) for each channel.")]
		public int Levels = 16;

		[Range(0f, 1f)]
		[Tooltip("Blending factor.")]
		public float Amount = 1f;

		[Tooltip("Only affects luminosity. Use this if you don't want any hue shifting or color changes.")]
		public bool LuminosityOnly;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			base.Material.SetVector("_Params", new Vector2(Levels, Amount));
			Graphics.Blit(source, destination, base.Material, LuminosityOnly ? 1 : 0);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Posterize";
		}
	}
}
