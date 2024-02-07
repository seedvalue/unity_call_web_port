using System;
using AmplifyColor;
using UnityEngine;

[AddComponentMenu("")]
public class AmplifyColorBase : MonoBehaviour
{
	public Quality QualityLevel = Quality.Standard;

	public float BlendAmount;

	public Texture2D LutTexture;

	public Texture2D LutBlendTexture;

	public Texture MaskTexture;

	public bool UseVolumes;

	public float ExitVolumeBlendTime = 1f;

	public Transform TriggerVolumeProxy;

	public LayerMask VolumeCollisionMask = -1;

	private Shader shaderBase;

	private Shader shaderBlend;

	private Shader shaderBlendCache;

	private Shader shaderMask;

	private Shader shaderBlendMask;

	private RenderTexture blendCacheLut;

	private Texture2D normalLut;

	private ColorSpace colorSpace = ColorSpace.Uninitialized;

	private Quality qualityLevel = Quality.Standard;

	private bool use3d;

	private Material materialBase;

	private Material materialBlend;

	private Material materialBlendCache;

	private Material materialMask;

	private Material materialBlendMask;

	private bool blending;

	private float blendingTime;

	private float blendingTimeCountdown;

	private Action onFinishBlend;

	internal bool JustCopy;

	private Texture2D worldLUT;

	private AmplifyColorVolumeBase currentVolumeLut;

	private RenderTexture midBlendLUT;

	private bool blendingFromMidBlend;

	public bool IsBlending
	{
		get
		{
			return blending;
		}
	}

	public bool WillItBlend
	{
		get
		{
			return LutTexture != null && LutBlendTexture != null && !blending;
		}
	}

	private void ReportMissingShaders()
	{
		Debug.LogError("[AmplifyColor] Error initializing shaders. Please reinstall Amplify Color.");
		base.enabled = false;
	}

	private void ReportNotSupported()
	{
		Debug.LogError("[AmplifyColor] This image effect is not supported on this platform. Please make sure your Unity license supports Full-Screen Post-Processing Effects which is usually reserved forn Pro licenses.");
		base.enabled = false;
	}

	private bool CheckShader(Shader s)
	{
		if (s == null)
		{
			ReportMissingShaders();
			return false;
		}
		if (!s.isSupported)
		{
			ReportNotSupported();
			return false;
		}
		return true;
	}

	private bool CheckShaders()
	{
		return CheckShader(shaderBase) && CheckShader(shaderBlend) && CheckShader(shaderBlendCache) && CheckShader(shaderMask) && CheckShader(shaderBlendMask);
	}

	private bool CheckSupport()
	{
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			ReportNotSupported();
			return false;
		}
		return true;
	}

	private void OnEnable()
	{
		if (CheckSupport())
		{
			CreateMaterials();
			if ((LutTexture != null && LutTexture.mipmapCount > 1) || (LutBlendTexture != null && LutBlendTexture.mipmapCount > 1))
			{
				Debug.LogError("[AmplifyColor] Please disable \"Generate Mip Maps\" import settings on all LUT textures to avoid visual glitches. Change Texture Type to \"Advanced\" to access Mip settings.");
			}
		}
	}

	private void OnDisable()
	{
		ReleaseMaterials();
		ReleaseTextures();
	}

	public void BlendTo(Texture2D blendTargetLUT, float blendTimeInSec, Action onFinishBlend)
	{
		LutBlendTexture = blendTargetLUT;
		BlendAmount = 0f;
		this.onFinishBlend = onFinishBlend;
		blendingTime = blendTimeInSec;
		blendingTimeCountdown = blendTimeInSec;
		blending = true;
	}

	private void Start()
	{
		worldLUT = LutTexture;
	}

	private void Update()
	{
		if (blending)
		{
			BlendAmount = (blendingTime - blendingTimeCountdown) / blendingTime;
			blendingTimeCountdown -= Time.smoothDeltaTime;
			if (BlendAmount >= 1f)
			{
				LutTexture = LutBlendTexture;
				BlendAmount = 0f;
				blending = false;
				LutBlendTexture = null;
				if (blendingFromMidBlend && midBlendLUT != null)
				{
					midBlendLUT.DiscardContents();
				}
				blendingFromMidBlend = false;
				if (onFinishBlend != null)
				{
					onFinishBlend();
				}
			}
		}
		else
		{
			BlendAmount = Mathf.Clamp01(BlendAmount);
		}
		if (!UseVolumes)
		{
			return;
		}
		Transform transform = ((!(TriggerVolumeProxy == null)) ? TriggerVolumeProxy : GetComponent<Camera>().transform);
		Collider[] array = Physics.OverlapSphere(transform.position, 0.01f, VolumeCollisionMask);
		AmplifyColorVolumeBase amplifyColorVolumeBase = null;
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			AmplifyColorVolumeBase component = collider.GetComponent<AmplifyColorVolumeBase>();
			if (component != null)
			{
				amplifyColorVolumeBase = component;
				break;
			}
		}
		if (!(amplifyColorVolumeBase != currentVolumeLut))
		{
			return;
		}
		currentVolumeLut = amplifyColorVolumeBase;
		Texture2D texture2D = ((!(amplifyColorVolumeBase == null)) ? amplifyColorVolumeBase.LutTexture : worldLUT);
		float num = ((!(amplifyColorVolumeBase == null)) ? amplifyColorVolumeBase.EnterBlendTime : ExitVolumeBlendTime);
		if (IsBlending && !blendingFromMidBlend && texture2D == LutTexture)
		{
			LutTexture = LutBlendTexture;
			LutBlendTexture = texture2D;
			blendingTimeCountdown = num * ((blendingTime - blendingTimeCountdown) / blendingTime);
			blendingTime = num;
			BlendAmount = 1f - BlendAmount;
			return;
		}
		if (IsBlending)
		{
			materialBlendCache.SetFloat("_lerpAmount", BlendAmount);
			if (blendingFromMidBlend)
			{
				materialBlendCache.SetTexture("_RgbTex", midBlendLUT);
			}
			else
			{
				materialBlendCache.SetTexture("_RgbTex", LutTexture);
			}
			materialBlendCache.SetTexture("_LerpRgbTex", (!(LutBlendTexture != null)) ? normalLut : LutBlendTexture);
			Graphics.Blit(LutTexture, midBlendLUT, materialBlendCache);
			blendingFromMidBlend = true;
		}
		BlendTo(texture2D, num, null);
	}

	private void SetupShader()
	{
		Shader.EnableKeyword(string.Empty);
		colorSpace = QualitySettings.activeColorSpace;
		qualityLevel = QualityLevel;
		string text = ((colorSpace != ColorSpace.Linear) ? string.Empty : "Linear");
		string empty = string.Empty;
		if (QualityLevel == Quality.Mobile)
		{
			Shader.EnableKeyword("QUALITY_MOBILE");
			Shader.DisableKeyword("QUALITY_STANDARD");
		}
		else
		{
			Shader.DisableKeyword("QUALITY_MOBILE");
			Shader.EnableKeyword("QUALITY_STANDARD");
		}
		shaderBase = Shader.Find("Hidden/Amplify Color/Base" + text + empty);
		shaderBlend = Shader.Find("Hidden/Amplify Color/Blend" + text + empty);
		shaderBlendCache = Shader.Find("Hidden/Amplify Color/BlendCache");
		shaderMask = Shader.Find("Hidden/Amplify Color/Mask" + text + empty);
		shaderBlendMask = Shader.Find("Hidden/Amplify Color/BlendMask" + text + empty);
	}

	private void ReleaseMaterials()
	{
		if (materialBase != null)
		{
			UnityEngine.Object.DestroyImmediate(materialBase);
			materialBase = null;
		}
		if (materialBlend != null)
		{
			UnityEngine.Object.DestroyImmediate(materialBlend);
			materialBlend = null;
		}
		if (materialBlendCache != null)
		{
			UnityEngine.Object.DestroyImmediate(materialBlendCache);
			materialBlendCache = null;
		}
		if (materialMask != null)
		{
			UnityEngine.Object.DestroyImmediate(materialMask);
			materialMask = null;
		}
		if (materialBlendMask != null)
		{
			UnityEngine.Object.DestroyImmediate(materialBlendMask);
			materialBlendMask = null;
		}
	}

	private void CreateHelperTextures()
	{
		int num = 1024;
		int num2 = 32;
		ReleaseTextures();
		blendCacheLut = new RenderTexture(num, num2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		blendCacheLut.name = "BlendCacheLut";
		blendCacheLut.wrapMode = TextureWrapMode.Clamp;
		blendCacheLut.useMipMap = false;
		blendCacheLut.anisoLevel = 0;
		blendCacheLut.Create();
		midBlendLUT = new RenderTexture(num, num2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		midBlendLUT.name = "MidBlendLut";
		midBlendLUT.wrapMode = TextureWrapMode.Clamp;
		midBlendLUT.useMipMap = false;
		midBlendLUT.anisoLevel = 0;
		midBlendLUT.Create();
		normalLut = new Texture2D(num, num2, TextureFormat.RGB24, false, true)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		normalLut.name = "NormalLut";
		normalLut.hideFlags = HideFlags.DontSave;
		normalLut.anisoLevel = 1;
		normalLut.filterMode = FilterMode.Bilinear;
		Color32[] array = new Color32[num * num2];
		for (int i = 0; i < 32; i++)
		{
			int num3 = i * 32;
			for (int j = 0; j < 32; j++)
			{
				int num4 = num3 + j * num;
				for (int k = 0; k < 32; k++)
				{
					float num5 = (float)k / 31f;
					float num6 = (float)j / 31f;
					float num7 = (float)i / 31f;
					byte r = (byte)(num5 * 255f);
					byte g = (byte)(num6 * 255f);
					byte b = (byte)(num7 * 255f);
					array[num4 + k] = new Color32(r, g, b, byte.MaxValue);
				}
			}
		}
		normalLut.SetPixels32(array);
		normalLut.Apply();
	}

	private bool CheckMaterialAndShader(Material material, string name)
	{
		if (material == null || material.shader == null)
		{
			Debug.LogError("[AmplifyColor] Error creating " + name + " material. Effect disabled.");
			base.enabled = false;
		}
		else if (!material.shader.isSupported)
		{
			Debug.LogError("[AmplifyColor] " + name + " shader not supported on this platform. Effect disabled.");
			base.enabled = false;
		}
		else
		{
			material.hideFlags = HideFlags.HideAndDontSave;
		}
		return base.enabled;
	}

	private void CreateMaterials()
	{
		SetupShader();
		ReleaseMaterials();
		materialBase = new Material(shaderBase);
		materialBlend = new Material(shaderBlend);
		materialBlendCache = new Material(shaderBlendCache);
		materialMask = new Material(shaderMask);
		materialBlendMask = new Material(shaderBlendMask);
		CheckMaterialAndShader(materialBase, "BaseMaterial");
		CheckMaterialAndShader(materialBlend, "BlendMaterial");
		CheckMaterialAndShader(materialBlendCache, "BlendCacheMaterial");
		CheckMaterialAndShader(materialMask, "MaskMaterial");
		CheckMaterialAndShader(materialBlendMask, "BlendMaskMaterial");
		if (base.enabled)
		{
			CreateHelperTextures();
		}
	}

	private void ReleaseTextures()
	{
		if (blendCacheLut != null)
		{
			UnityEngine.Object.DestroyImmediate(blendCacheLut);
			blendCacheLut = null;
		}
		if (midBlendLUT != null)
		{
			UnityEngine.Object.DestroyImmediate(midBlendLUT);
			midBlendLUT = null;
		}
		if (normalLut != null)
		{
			UnityEngine.Object.DestroyImmediate(normalLut);
			normalLut = null;
		}
	}

	public static bool ValidateLutDimensions(Texture2D lut)
	{
		bool result = true;
		if (lut != null)
		{
			if (lut.width / lut.height != lut.height)
			{
				Debug.LogWarning("[AmplifyColor] Lut " + lut.name + " has invalid dimensions.");
				result = false;
			}
			else if (lut.anisoLevel != 0)
			{
				lut.anisoLevel = 0;
			}
		}
		return result;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		BlendAmount = Mathf.Clamp01(BlendAmount);
		if (colorSpace != QualitySettings.activeColorSpace || qualityLevel != QualityLevel)
		{
			CreateMaterials();
		}
		bool flag = ValidateLutDimensions(LutTexture);
		bool flag2 = ValidateLutDimensions(LutBlendTexture);
		bool flag3 = LutTexture == null && LutBlendTexture == null;
		if (JustCopy || !flag || !flag2 || flag3)
		{
			Graphics.Blit(source, destination);
			return;
		}
		Texture2D texture2D = ((!(LutTexture == null)) ? LutTexture : normalLut);
		Texture2D lutBlendTexture = LutBlendTexture;
		int pass = (GetComponent<Camera>().allowHDR ? 1 : 0);
		bool flag4 = BlendAmount != 0f || blending;
		bool flag5 = flag4 || (flag4 && lutBlendTexture != null);
		bool flag6 = flag5 && !use3d;
		Material material = (flag5 ? ((!(MaskTexture != null)) ? materialBlend : materialBlendMask) : ((!(MaskTexture != null)) ? materialBase : materialMask));
		material.SetFloat("_lerpAmount", BlendAmount);
		if (MaskTexture != null)
		{
			material.SetTexture("_MaskTex", MaskTexture);
		}
		if (flag6)
		{
			materialBlendCache.SetFloat("_lerpAmount", BlendAmount);
			if (UseVolumes && blendingFromMidBlend)
			{
				materialBlendCache.SetTexture("_RgbTex", midBlendLUT);
			}
			else
			{
				materialBlendCache.SetTexture("_RgbTex", texture2D);
			}
			materialBlendCache.SetTexture("_LerpRgbTex", (!(lutBlendTexture != null)) ? normalLut : lutBlendTexture);
			Graphics.Blit(texture2D, blendCacheLut, materialBlendCache);
			material.SetTexture("_RgbBlendCacheTex", blendCacheLut);
		}
		else if (!use3d)
		{
			if (texture2D != null)
			{
				material.SetTexture("_RgbTex", texture2D);
			}
			if (lutBlendTexture != null)
			{
				material.SetTexture("_LerpRgbTex", lutBlendTexture);
			}
		}
		Graphics.Blit(source, destination, material, pass);
		if (flag6)
		{
			blendCacheLut.DiscardContents();
		}
	}
}
