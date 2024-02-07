using UnityEngine;

namespace Colorful
{
	[AddComponentMenu("Colorful FX/Other Effects/Sharpen")]
	[ExecuteInEditMode]
	[HelpURL("http://www.thomashourdel.com/colorful/doc/other-effects/sharpen.html")]
	public class Sharpen : BaseEffect
	{
		public enum Algorithm
		{
			TypeA = 0,
			TypeB = 1
		}

		[Tooltip("Sharpening algorithm to use.")]
		public Algorithm Mode = Algorithm.TypeB;

		[Range(0f, 5f)]
		[Tooltip("Sharpening Strength.")]
		public float Strength = 0.6f;

		[Tooltip("Limits the amount of sharpening a pixel will receive.")]
		[Range(0f, 1f)]
		public float Clamp = 0.05f;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (Strength == 0f || Clamp == 0f)
			{
				Graphics.Blit(source, destination);
				return;
			}
			base.Material.SetVector("_Params", new Vector4(Strength, Clamp, 1f / (float)source.width, 1f / (float)source.height));
			Graphics.Blit(source, destination, base.Material, (int)Mode);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Sharpen";
		}
	}
}
