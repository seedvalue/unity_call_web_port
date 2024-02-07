using UnityEngine;

namespace Colorful
{
	[ExecuteInEditMode]
	[AddComponentMenu("Colorful FX/Color Correction/Brightness, Contrast, Gamma")]
	[HelpURL("http://www.thomashourdel.com/colorful/doc/color-correction/brightness-contrast-gamma.html")]
	public class BrightnessContrastGamma : BaseEffect
	{
		[Range(-100f, 100f)]
		[Tooltip("Moving the slider to the right increases tonal values and expands highlights, to the left decreases values and expands shadows.")]
		public float Brightness;

		[Range(-100f, 100f)]
		[Tooltip("Expands or shrinks the overall range of tonal values.")]
		public float Contrast;

		public Vector3 ContrastCoeff = new Vector3(0.5f, 0.5f, 0.5f);

		[Tooltip("Simple power function.")]
		[Range(0.1f, 9.9f)]
		public float Gamma = 1f;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (Brightness == 0f && Contrast == 0f && Gamma == 1f)
			{
				Graphics.Blit(source, destination);
				return;
			}
			base.Material.SetVector("_BCG", new Vector4((Brightness + 100f) * 0.01f, (Contrast + 100f) * 0.01f, 1f / Gamma));
			base.Material.SetVector("_Coeffs", ContrastCoeff);
			Graphics.Blit(source, destination, base.Material);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Brightness Contrast Gamma";
		}
	}
}
