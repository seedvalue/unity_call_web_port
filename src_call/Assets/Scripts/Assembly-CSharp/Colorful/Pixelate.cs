using UnityEngine;

namespace Colorful
{
	[HelpURL("http://www.thomashourdel.com/colorful/doc/other-effects/pixelate.html")]
	[ExecuteInEditMode]
	[AddComponentMenu("Colorful FX/Other Effects/Pixelate")]
	public class Pixelate : BaseEffect
	{
		public enum SizeMode
		{
			ResolutionIndependent = 0,
			PixelPerfect = 1
		}

		[Tooltip("Scale of an individual pixel. Depends on the Mode used.")]
		[Range(1f, 1024f)]
		public float Scale = 80f;

		[Tooltip("Turn this on to automatically compute the aspect ratio needed for squared pixels.")]
		public bool AutomaticRatio = true;

		[Tooltip("Custom aspect ratio.")]
		public float Ratio = 1f;

		[Tooltip("Used for the Scale field.")]
		public SizeMode Mode;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			float x = Scale;
			if (Mode == SizeMode.PixelPerfect)
			{
				x = (float)source.width / Scale;
			}
			base.Material.SetVector("_Params", new Vector2(x, (!AutomaticRatio) ? Ratio : ((float)source.width / (float)source.height)));
			Graphics.Blit(source, destination, base.Material);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Pixelate";
		}
	}
}
