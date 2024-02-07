using UnityEngine;

namespace Colorful
{
	[HelpURL("http://www.thomashourdel.com/colorful/doc/color-correction/grayscale.html")]
	[AddComponentMenu("Colorful FX/Color Correction/Grayscale")]
	[ExecuteInEditMode]
	public class Grayscale : BaseEffect
	{
		[Range(0f, 1f)]
		[Tooltip("Amount of red to contribute to the luminosity.")]
		public float RedLuminance = 0.299f;

		[Range(0f, 1f)]
		[Tooltip("Amount of green to contribute to the luminosity.")]
		public float GreenLuminance = 0.587f;

		[Tooltip("Amount of blue to contribute to the luminosity.")]
		[Range(0f, 1f)]
		public float BlueLuminance = 0.114f;

		[Range(0f, 1f)]
		[Tooltip("Blending factor.")]
		public float Amount = 1f;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (Amount <= 0f)
			{
				Graphics.Blit(source, destination);
				return;
			}
			base.Material.SetVector("_Params", new Vector4(RedLuminance, GreenLuminance, BlueLuminance, Amount));
			Graphics.Blit(source, destination, base.Material);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Grayscale";
		}
	}
}
