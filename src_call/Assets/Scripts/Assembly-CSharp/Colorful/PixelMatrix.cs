using UnityEngine;

namespace Colorful
{
	[ExecuteInEditMode]
	[HelpURL("http://www.thomashourdel.com/colorful/doc/other-effects/pixel-matrix.html")]
	[AddComponentMenu("Colorful FX/Other Effects/Pixel Matrix")]
	public class PixelMatrix : BaseEffect
	{
		[Tooltip("Tile size. Works best with multiples of 3.")]
		[Min(3f)]
		public int Size = 9;

		[Tooltip("Tile brightness booster.")]
		[Range(0f, 10f)]
		public float Brightness = 1.4f;

		[Tooltip("Show / hide black borders on every tile.")]
		public bool BlackBorder = true;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			base.Material.SetVector("_Params", new Vector4(Size, Mathf.Floor((float)Size / 3f), (float)Size - Mathf.Floor((float)Size / 3f), Brightness));
			Graphics.Blit(source, destination, base.Material, BlackBorder ? 1 : 0);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/PixelMatrix";
		}
	}
}
