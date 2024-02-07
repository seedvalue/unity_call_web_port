using UnityEngine;

namespace Colorful
{
	[ExecuteInEditMode]
	[AddComponentMenu("Colorful FX/Artistic Effects/LoFi Palette")]
	[HelpURL("http://www.thomashourdel.com/colorful/doc/artistic-effects/lofi-palette.html")]
	public class LoFiPalette : LookupFilter3D
	{
		public enum Preset
		{
			None = 0,
			AmstradCPC = 2,
			CGA = 3,
			Commodore64 = 4,
			CommodorePlus = 5,
			EGA = 6,
			GameBoy = 7,
			MacOS16 = 8,
			MacOS256 = 9,
			MasterSystem = 10,
			RiscOS16 = 11,
			Teletex = 12,
			Windows16 = 13,
			Windows256 = 14,
			ZXSpectrum = 15,
			Andrae = 17,
			Anodomani = 18,
			Crayolo = 19,
			DB16 = 20,
			DB32 = 21,
			DJinn = 22,
			DrazileA = 23,
			DrazileB = 24,
			DrazileC = 25,
			Eggy = 26,
			FinlalA = 27,
			FinlalB = 28,
			Hapiel = 29,
			PavanzA = 30,
			PavanzB = 31,
			Peyton = 32,
			SpeedyCube = 33
		}

		public Preset Palette;

		[Tooltip("Pixelize the display.")]
		public bool Pixelize = true;

		[Tooltip("The display height in pixels.")]
		public float PixelSize = 128f;

		protected Preset m_CurrentPreset;

		protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (Palette != m_CurrentPreset)
			{
				m_CurrentPreset = Palette;
				if (Palette == Preset.None)
				{
					LookupTexture = null;
				}
				else
				{
					LookupTexture = Resources.Load<Texture2D>("LoFiPalettes/" + Palette);
				}
			}
			if (LookupTexture == null || Amount <= 0f)
			{
				Graphics.Blit(source, destination);
			}
			else if (m_Use2DLut || ForceCompatibility)
			{
				RenderLut2D(source, destination);
			}
			else
			{
				RenderLut3D(source, destination);
			}
		}

		protected override void RenderLut2D(RenderTexture source, RenderTexture destination)
		{
			float num = Mathf.Sqrt(LookupTexture.width);
			base.Material.SetTexture("_LookupTex", LookupTexture);
			base.Material.SetVector("_Params1", new Vector3(1f / (float)LookupTexture.width, 1f / (float)LookupTexture.height, num - 1f));
			base.Material.SetVector("_Params2", new Vector2(Amount, PixelSize));
			int pass = ((!Pixelize) ? 4 : 6) + (CLib.IsLinearColorSpace() ? 1 : 0);
			Graphics.Blit(source, destination, base.Material, pass);
		}

		protected override void RenderLut3D(RenderTexture source, RenderTexture destination)
		{
			if (LookupTexture.name != m_BaseTextureName)
			{
				ConvertBaseTexture();
			}
			if (m_Lut3D == null)
			{
				SetIdentityLut();
			}
			m_Lut3D.filterMode = FilterMode.Point;
			base.Material.SetTexture("_LookupTex", m_Lut3D);
			float num = m_Lut3D.width;
			base.Material.SetVector("_Params", new Vector4((num - 1f) / (1f * num), 1f / (2f * num), Amount, PixelSize));
			int pass = (Pixelize ? 2 : 0) + (CLib.IsLinearColorSpace() ? 1 : 0);
			Graphics.Blit(source, destination, base.Material, pass);
		}
	}
}
