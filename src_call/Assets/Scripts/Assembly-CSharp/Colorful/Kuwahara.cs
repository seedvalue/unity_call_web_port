using UnityEngine;

namespace Colorful
{
	[ExecuteInEditMode]
	[AddComponentMenu("Colorful FX/Artistic Effects/Kuwahara")]
	[HelpURL("http://www.thomashourdel.com/colorful/doc/artistic-effects/kuwahara.html")]
	public class Kuwahara : BaseEffect
	{
		[Range(1f, 6f)]
		[Tooltip("Larger radius will give a more abstract look but will lower performances.")]
		public int Radius = 3;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Radius = Mathf.Clamp(Radius, 1, 6);
			base.Material.SetVector("_PSize", new Vector2(1f / (float)source.width, 1f / (float)source.height));
			Graphics.Blit(source, destination, base.Material, Radius - 1);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Kuwahara";
		}
	}
}
