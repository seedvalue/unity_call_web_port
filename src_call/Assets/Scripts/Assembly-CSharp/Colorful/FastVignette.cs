using UnityEngine;

namespace Colorful
{
	[HelpURL("http://www.thomashourdel.com/colorful/doc/camera-effects/fast-vignette.html")]
	[AddComponentMenu("Colorful FX/Camera Effects/Fast Vignette")]
	[ExecuteInEditMode]
	public class FastVignette : BaseEffect
	{
		public enum ColorMode
		{
			Classic = 0,
			Desaturate = 1,
			Colored = 2
		}

		[Tooltip("Vignette type.")]
		public ColorMode Mode;

		[ColorUsage(false)]
		[Tooltip("The color to use in the vignette area.")]
		public Color Color = Color.red;

		[Tooltip("Center point.")]
		public Vector2 Center = new Vector2(0.5f, 0.5f);

		[Range(-100f, 100f)]
		[Tooltip("Smoothness of the vignette effect.")]
		public float Sharpness = 10f;

		[Range(0f, 100f)]
		[Tooltip("Amount of vignetting on screen.")]
		public float Darkness = 30f;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			base.Material.SetVector("_Params", new Vector4(Center.x, Center.y, Sharpness * 0.01f, Darkness * 0.02f));
			base.Material.SetColor("_Color", Color);
			Graphics.Blit(source, destination, base.Material, (int)Mode);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Fast Vignette";
		}
	}
}
