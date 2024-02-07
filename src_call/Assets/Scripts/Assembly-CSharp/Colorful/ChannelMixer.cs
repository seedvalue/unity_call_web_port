using UnityEngine;

namespace Colorful
{
	[AddComponentMenu("Colorful FX/Color Correction/Channel Mixer")]
	[ExecuteInEditMode]
	[HelpURL("http://www.thomashourdel.com/colorful/doc/color-correction/channel-mixer.html")]
	public class ChannelMixer : BaseEffect
	{
		public Vector3 Red = new Vector3(100f, 0f, 0f);

		public Vector3 Green = new Vector3(0f, 100f, 0f);

		public Vector3 Blue = new Vector3(0f, 0f, 100f);

		public Vector3 Constant = new Vector3(0f, 0f, 0f);

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			base.Material.SetVector("_Red", new Vector4(Red.x * 0.01f, Green.x * 0.01f, Blue.x * 0.01f));
			base.Material.SetVector("_Green", new Vector4(Red.y * 0.01f, Green.y * 0.01f, Blue.y * 0.01f));
			base.Material.SetVector("_Blue", new Vector4(Red.z * 0.01f, Green.z * 0.01f, Blue.z * 0.01f));
			base.Material.SetVector("_Constant", new Vector4(Constant.x * 0.01f, Constant.y * 0.01f, Constant.z * 0.01f));
			Graphics.Blit(source, destination, base.Material);
		}

		protected override string GetShaderName()
		{
			return "Hidden/Colorful/Channel Mixer";
		}
	}
}
