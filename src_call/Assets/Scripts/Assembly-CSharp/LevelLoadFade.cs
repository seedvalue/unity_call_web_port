using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoadFade : MonoBehaviour
{
	[HideInInspector]
	public GameObject LevelLoadFadeobj;

	[HideInInspector]
	public Image fadeImage;

	public void FadeAndLoadLevel(Color color, float fadeLength, bool fadeIn)
	{
		// Sergei отключил
		return;
		LevelLoadFadeobj.layer = 14;
		fadeImage.color = color;
		LevelLoadFadeobj.SetActive(true);
		if (fadeIn)
		{
			StartCoroutine(DoFadeIn(fadeLength));
		}
		else
		{
			StartCoroutine(DoFadeOut(fadeLength));
		}
	}

	private IEnumerator DoFadeIn(float fadeLength)
	{
		float time = 0f;
		while (time < fadeLength)
		{
			if (Time.timeSinceLevelLoad > 0.8f)
			{
				Color color = fadeImage.color;
				time += Time.deltaTime;
				color.a = Mathf.InverseLerp(fadeLength, 0f, time);
				fadeImage.color = color;
			}
			yield return null;
		}
		LevelLoadFadeobj.SetActive(false);
	}

	private IEnumerator DoFadeOut(float fadeLength)
	{
		Color tempColor = fadeImage.color;
		tempColor.a = 0f;
		fadeImage.color = tempColor;
		float time = 0f;
		while (time < fadeLength)
		{
			time += Time.deltaTime;
			tempColor.a = Mathf.InverseLerp(0f, fadeLength, time);
			fadeImage.color = tempColor;
			yield return null;
		}
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		yield return new WaitForSeconds(1f);
		LevelLoadFadeobj.SetActive(false);
	}
}
