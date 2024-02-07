using UnityEngine;

namespace Colorful
{
	[AddComponentMenu("Colorful FX/Other Effects/LED")]
	[ExecuteInEditMode]
	[HelpURL("http://www.thomashourdel.com/colorful/doc/other-effects/led.html")]
	public class Led : BaseEffect
	{
		public enum SizeMode
		{
			ResolutionIndependent = 0,
			PixelPerfect = 1
		}

		[Range(1f, 255f)]
		[Tooltip("Scale of an individual LED. Depends on the Mode used.")]
		public float Scale = 80f;

		[Range(0f, 10f)]
		[Tooltip("LED brightness booster.")]
		public float Brightness = 1f;

		[Tooltip("LED shape, from softer to harsher.")]
		[Range(1f, 3f)]
		public float Shape = 1.5f;

		[Tooltip("Turn this on to automatically compute the aspect ratio needed for squared LED.")]
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
			base.Material.SetVector("_Params", new Vector4(x, (!AutomaticRatio) ? Ratio : ((float)source.width / (float)source.height), Brightness, Shape));
			Graphics.Blit(source, destination, base.Material);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Led";
		}
	}
}
