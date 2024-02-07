using UnityEngine;

namespace Colorful
{
	[AddComponentMenu("Colorful FX/Other Effects/Noise")]
	[ExecuteInEditMode]
	[HelpURL("http://www.thomashourdel.com/colorful/doc/other-effects/noise.html")]
	public class Noise : BaseEffect
	{
		public enum ColorMode
		{
			Monochrome = 0,
			RGB = 1
		}

		[Tooltip("Black & white or colored noise.")]
		public ColorMode Mode;

		[Tooltip("Automatically increment the seed to animate the noise.")]
		public bool Animate = true;

		[Tooltip("A number used to initialize the noise generator.")]
		public float Seed = 0.5f;

		[Range(0f, 1f)]
		[Tooltip("Strength used to apply the noise. 0 means no noise at all, 1 is full noise.")]
		public float Strength = 0.12f;

		[Tooltip("Reduce the noise visibility in luminous areas.")]
		[Range(0f, 1f)]
		public float LumContribution;

		protected virtual void Update()
		{
			if (Animate)
			{
				if (Seed > 1000f)
				{
					Seed = 0.5f;
				}
				Seed += Time.deltaTime * 0.25f;
			}
		}

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			base.Material.SetVector("_Params", new Vector3(Seed, Strength, LumContribution));
			int num = ((Mode != 0) ? 1 : 0);
			num += ((LumContribution > 0f) ? 2 : 0);
			Graphics.Blit(source, destination, base.Material, num);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Noise";
		}
	}
}
