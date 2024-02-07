using UnityEngine;

namespace Colorful
{
	[AddComponentMenu("Colorful FX/Blur Effects/Radial Blur")]
	[ExecuteInEditMode]
	[HelpURL("http://www.thomashourdel.com/colorful/doc/blur-effects/radial-blur.html")]
	public class RadialBlur : BaseEffect
	{
		public enum QualityPreset
		{
			Low = 4,
			Medium = 8,
			High = 12,
			Custom = 13
		}

		[Tooltip("Blur strength.")]
		[Range(0f, 1f)]
		public float Strength = 0.1f;

		[Tooltip("Sample count. Higher means better quality but slower processing.")]
		[Range(2f, 32f)]
		public int Samples = 10;

		[Tooltip("Focus point.")]
		public Vector2 Center = new Vector2(0.5f, 0.5f);

		[Tooltip("Quality preset. Higher means better quality but slower processing.")]
		public QualityPreset Quality = QualityPreset.Medium;

		[Range(-100f, 100f)]
		[Tooltip("Smoothness of the vignette effect.")]
		public float Sharpness = 40f;

		[Range(0f, 100f)]
		[Tooltip("Amount of vignetting on screen.")]
		public float Darkness = 35f;

		[Tooltip("Should the effect be applied like a vignette ?")]
		public bool EnableVignette = true;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (Strength <= 0f)
			{
				Graphics.Blit(source, destination);
				return;
			}
			int num = ((Quality != QualityPreset.Custom) ? ((int)Quality) : Samples);
			base.Material.SetVector("_Center", Center);
			base.Material.SetVector("_Params", new Vector4(Strength, num, Sharpness * 0.01f, Darkness * 0.02f));
			Graphics.Blit(source, destination, base.Material, EnableVignette ? 1 : 0);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Radial Blur";
		}
	}
}
