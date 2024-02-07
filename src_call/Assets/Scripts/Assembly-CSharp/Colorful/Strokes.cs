using UnityEngine;

namespace Colorful
{
	[ExecuteInEditMode]
	[AddComponentMenu("Colorful FX/Artistic Effects/Strokes")]
	[HelpURL("http://www.thomashourdel.com/colorful/doc/artistic-effects/strokes.html")]
	public class Strokes : BaseEffect
	{
		public enum ColorMode
		{
			BlackAndWhite = 0,
			WhiteAndBlack = 1,
			ColorAndWhite = 2,
			ColorAndBlack = 3,
			WhiteAndColor = 4,
			BlackAndColor = 5
		}

		public ColorMode Mode;

		[Tooltip("Stroke rotation, or wave pattern amplitude.")]
		[Range(0f, 0.04f)]
		public float Amplitude = 0.025f;

		[Tooltip("Wave pattern frequency (higher means more waves).")]
		[Range(0f, 20f)]
		public float Frequency = 10f;

		[Range(4f, 12f)]
		[Tooltip("Global scaling.")]
		public float Scaling = 7.5f;

		[Tooltip("Stroke maximum thickness.")]
		[Range(0.1f, 0.5f)]
		public float MaxThickness = 0.2f;

		[Range(0f, 1f)]
		[Tooltip("Contribution threshold (higher means more continous strokes).")]
		public float Threshold = 0.7f;

		[Range(-0.3f, 0.3f)]
		[Tooltip("Stroke pressure.")]
		public float Harshness;

		[Range(0f, 1f)]
		[Tooltip("Amount of red to contribute to the strokes.")]
		public float RedLuminance = 0.299f;

		[Range(0f, 1f)]
		[Tooltip("Amount of green to contribute to the strokes.")]
		public float GreenLuminance = 0.587f;

		[Range(0f, 1f)]
		[Tooltip("Amount of blue to contribute to the strokes.")]
		public float BlueLuminance = 0.114f;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			float num = Scaling / (float)source.height;
			base.Material.SetVector("_Params1", new Vector4(Amplitude, Frequency, num, MaxThickness * num));
			base.Material.SetVector("_Params2", new Vector3(RedLuminance, GreenLuminance, BlueLuminance));
			base.Material.SetVector("_Params3", new Vector2(Threshold, Harshness));
			Graphics.Blit(source, destination, base.Material, (int)Mode);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Strokes";
		}
	}
}
