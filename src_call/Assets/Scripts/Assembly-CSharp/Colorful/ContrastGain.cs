using UnityEngine;

namespace Colorful
{
	[HelpURL("http://www.thomashourdel.com/colorful/doc/color-correction/contrast-gain.html")]
	[ExecuteInEditMode]
	[AddComponentMenu("Colorful FX/Color Correction/Contrast Gain")]
	public class ContrastGain : BaseEffect
	{
		[Tooltip("Steepness of the contrast curve. 1 is linear, no contrast change.")]
		[Range(0.001f, 2f)]
		public float Gain = 1f;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			base.Material.SetFloat("_Gain", Gain);
			Graphics.Blit(source, destination, base.Material);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Contrast Gain";
		}
	}
}
