using UnityEngine;

namespace Colorful
{
	[AddComponentMenu("Colorful FX/Blur Effects/Grainy Blur")]
	[ExecuteInEditMode]
	[HelpURL("http://www.thomashourdel.com/colorful/doc/blur-effects/grainy-blur.html")]
	public class GrainyBlur : BaseEffect
	{
		[Tooltip("Blur radius.")]
		[Min(0f)]
		public float Radius = 32f;

		[Tooltip("Sample count. Higher means better quality but slower processing.")]
		[Range(1f, 32f)]
		public int Samples = 16;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (CLib.Approximately(Radius, 0f, 0.001f))
			{
				Graphics.Blit(source, destination);
				return;
			}
			base.Material.SetVector("_Params", new Vector2(Radius, Samples));
			Graphics.Blit(source, destination, base.Material);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/GrainyBlur";
		}
	}
}
