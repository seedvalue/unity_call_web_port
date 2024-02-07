using System.Collections;
using UnityEngine;

public class AtmoXfade : MonoBehaviour
{
	public enum FadeState
	{
		FadeDark = 0,
		FadeBright = 1
	}

	public Material skyMat;

	public Color skyBright = Color.grey;

	public Color skyDark = Color.black;

	public Light dirLight;

	public Color lightBright = Color.grey;

	public Color lightDark = Color.black;

	public float minLightIntensity = 0.2f;

	public float maxLightIntensity = 0.85f;

	private float curIntensity;

	public bool useRenderFog = true;

	public Color fogBright = Color.grey;

	public Color fogDark = Color.black;

	public float minFog = 0.004f;

	public float maxFog = 0.02f;

	public FadeState fadeState = FadeState.FadeBright;

	public float fadeTime = 80f;

	private void Start()
	{
		if ((bool)skyMat)
		{
			skyMat.SetColor("_Tint", skyBright);
		}
		if ((bool)dirLight)
		{
			dirLight.color = lightBright;
		}
		if (useRenderFog)
		{
			RenderSettings.fog = true;
			RenderSettings.fogColor = fogBright;
		}
		else
		{
			RenderSettings.fog = false;
		}
		curIntensity = maxLightIntensity;
	}

	private void OnTriggerEnter(Collider c)
	{
		if (c.sharedMaterial != null && c.sharedMaterial.name == "Player")
		{
			fadeState = FadeState.FadeDark;
			StartCoroutine(FadeDark());
		}
	}

	private void OnTriggerExit(Collider c)
	{
		if (c.sharedMaterial != null && c.sharedMaterial.name == "Player")
		{
			fadeState = FadeState.FadeBright;
			StartCoroutine(FadeBright());
		}
	}

	private IEnumerator FadeDark()
	{
		float t = 1E-05f;
		while (fadeState == FadeState.FadeDark && curIntensity > minLightIntensity)
		{
			skyMat.SetColor("_Tint", Color.Lerp(skyMat.GetColor("_Tint"), skyDark, t));
			dirLight.color = Color.Lerp(dirLight.color, lightDark, t);
			curIntensity = dirLight.intensity;
			dirLight.intensity = Mathf.SmoothStep(curIntensity, minLightIntensity, t);
			if (useRenderFog)
			{
				RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogDark, t);
				RenderSettings.fogDensity = Mathf.SmoothStep(RenderSettings.fogDensity, maxFog, t);
			}
			yield return null;
			t += Time.deltaTime / fadeTime;
		}
	}

	private IEnumerator FadeBright()
	{
		float t = 1E-05f;
		while (fadeState == FadeState.FadeBright && curIntensity < maxLightIntensity)
		{
			skyMat.SetColor("_Tint", Color.Lerp(skyMat.GetColor("_Tint"), skyBright, t));
			dirLight.color = Color.Lerp(dirLight.color, lightBright, t);
			curIntensity = dirLight.intensity;
			dirLight.intensity = Mathf.SmoothStep(curIntensity, maxLightIntensity, t);
			if (useRenderFog)
			{
				RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogBright, t);
				RenderSettings.fogDensity = Mathf.SmoothStep(RenderSettings.fogDensity, minFog, t);
			}
			yield return null;
			t += Time.deltaTime / fadeTime;
		}
	}
}
