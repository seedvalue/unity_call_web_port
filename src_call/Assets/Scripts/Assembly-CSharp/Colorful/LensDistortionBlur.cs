using UnityEngine;

namespace Colorful
{
	[HelpURL("http://www.thomashourdel.com/colorful/doc/blur-effects/lens-distortion-blur.html")]
	[AddComponentMenu("Colorful FX/Blur Effects/Lens Distortion Blur")]
	[ExecuteInEditMode]
	public class LensDistortionBlur : BaseEffect
	{
		public enum QualityPreset
		{
			Low = 4,
			Medium = 8,
			High = 12,
			Custom = 13
		}

		[Tooltip("Quality preset. Higher means better quality but slower processing.")]
		public QualityPreset Quality = QualityPreset.Medium;

		[Range(2f, 32f)]
		[Tooltip("Sample count. Higher means better quality but slower processing.")]
		public int Samples = 10;

		[Tooltip("Spherical distortion factor.")]
		[Range(-2f, 2f)]
		public float Distortion = 0.2f;

		[Tooltip("Cubic distortion factor.")]
		[Range(-2f, 2f)]
		public float CubicDistortion = 0.6f;

		[Range(0.01f, 2f)]
		[Tooltip("Helps avoid screen streching on borders when working with heavy distortions.")]
		public float Scale = 0.8f;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			int num = ((Quality != QualityPreset.Custom) ? ((int)Quality) : Samples);
			base.Material.SetVector("_Params", new Vector4(num, Distortion / (float)num, CubicDistortion / (float)num, Scale));
			Graphics.Blit(source, destination, base.Material);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/LensDistortionBlur";
		}
	}
}
