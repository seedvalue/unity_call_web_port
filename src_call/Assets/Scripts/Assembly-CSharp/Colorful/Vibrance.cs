using UnityEngine;

namespace Colorful
{
	[HelpURL("http://www.thomashourdel.com/colorful/doc/color-correction/vibrance.html")]
	[ExecuteInEditMode]
	[AddComponentMenu("Colorful FX/Color Correction/Vibrance")]
	public class Vibrance : BaseEffect
	{
		[Range(-100f, 100f)]
		[Tooltip("Adjusts the saturation so that clipping is minimized as colors approach full saturation.")]
		public float Amount;

		[Range(-5f, 5f)]
		public float RedChannel = 1f;

		[Range(-5f, 5f)]
		public float GreenChannel = 1f;

		[Range(-5f, 5f)]
		public float BlueChannel = 1f;

		public bool AdvancedMode;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (Amount <= 0f)
			{
				Graphics.Blit(source, destination);
			}
			else if (AdvancedMode)
			{
				base.Material.SetFloat("_Amount", Amount * 0.01f);
				base.Material.SetVector("_Channels", new Vector3(RedChannel, GreenChannel, BlueChannel));
				Graphics.Blit(source, destination, base.Material, 1);
			}
			else
			{
				base.Material.SetFloat("_Amount", Amount * 0.02f);
				Graphics.Blit(source, destination, base.Material, 0);
			}
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Vibrance";
		}
	}
}
