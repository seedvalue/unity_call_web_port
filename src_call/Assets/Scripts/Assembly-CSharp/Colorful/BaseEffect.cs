using UnityEngine;

namespace Colorful
{
	[AddComponentMenu("")]
	[RequireComponent(typeof(Camera))]
	public class BaseEffect : MonoBehaviour
	{
		public Shader Shader;

		protected Material m_Material;

		public Shader ShaderSafe
		{
			get
			{
				if (Shader == null)
				{
					Shader = Shader.Find(GetShaderName());
				}
				return Shader;
			}
		}

		public Material Material
		{
			get
			{
				if (m_Material == null)
				{
					m_Material = new Material(ShaderSafe);
					m_Material.hideFlags = HideFlags.HideAndDontSave;
				}
				return m_Material;
			}
		}

		protected virtual void Start()
		{
			if (!SystemInfo.supportsImageEffects)
			{
				Debug.LogWarning("Image effects aren't supported on this device");
				base.enabled = false;
			}
			else if (!ShaderSafe || !Shader.isSupported)
			{
				Debug.LogWarning("The shader is null or unsupported on this device");
				base.enabled = false;
			}
		}

		protected virtual void OnDisable()
		{
			if ((bool)m_Material)
			{
				Object.DestroyImmediate(m_Material);
			}
			m_Material = null;
		}

		public void Apply(Texture source, RenderTexture destination)
		{
			if (source is RenderTexture)
			{
				OnRenderImage(source as RenderTexture, destination);
				return;
			}
			RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height);
			Graphics.Blit(source, temporary);
			OnRenderImage(temporary, destination);
			RenderTexture.ReleaseTemporary(temporary);
		}

		protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
		}

		protected virtual string GetShaderName()
		{
			return "null";
		}
	}
}
