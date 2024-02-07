using UnityEngine;

namespace Colorful
{
	[AddComponentMenu("Colorful FX/Color Correction/Photo Filter")]
	[HelpURL("http://www.thomashourdel.com/colorful/doc/color-correction/photo-filter.html")]
	[ExecuteInEditMode]
	public class PhotoFilter : BaseEffect
	{
		[Tooltip("Lens filter color.")]
		[ColorUsage(false)]
		public Color Color = new Color(1f, 0.5f, 0.2f, 1f);

		[Tooltip("Blending factor.")]
		[Range(0f, 1f)]
		public float Density = 0.35f;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (Density <= 0f)
			{
				Graphics.Blit(source, destination);
				return;
			}
			base.Material.SetColor("_RGB", Color);
			base.Material.SetFloat("_Density", Density);
			Graphics.Blit(source, destination, base.Material);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Photo Filter";
		}
	}
}
