using UnityEngine;

namespace Colorful
{
	[ExecuteInEditMode]
	[AddComponentMenu("Colorful FX/Other Effects/Convolution Matrix 3x3")]
	[HelpURL("http://www.thomashourdel.com/colorful/doc/other-effects/convolution-3x3.html")]
	public class Convolution3x3 : BaseEffect
	{
		public Vector3 KernelTop = Vector3.zero;

		public Vector3 KernelMiddle = Vector3.up;

		public Vector3 KernelBottom = Vector3.zero;

		[Tooltip("Used to normalize the kernel.")]
		public float Divisor = 1f;

		[Tooltip("Blending factor.")]
		[Range(0f, 1f)]
		public float Amount = 1f;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (Amount <= 0f)
			{
				Graphics.Blit(source, destination);
				return;
			}
			base.Material.SetVector("_PSize", new Vector2(1f / (float)source.width, 1f / (float)source.height));
			base.Material.SetVector("_KernelT", KernelTop / Divisor);
			base.Material.SetVector("_KernelM", KernelMiddle / Divisor);
			base.Material.SetVector("_KernelB", KernelBottom / Divisor);
			base.Material.SetFloat("_Amount", Amount);
			Graphics.Blit(source, destination, base.Material);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Convolution 3x3";
		}
	}
}
