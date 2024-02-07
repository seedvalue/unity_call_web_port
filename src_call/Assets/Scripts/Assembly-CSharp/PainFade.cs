using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PainFade : MonoBehaviour
{
	[HideInInspector]
	public Image painImageComponent;

	private void Start()
	{
		painImageComponent = GetComponent<Image>();
	}

	public IEnumerator FadeIn(Color color, float fadeLength)
	{
		Color tempColor = color;
		tempColor.a = 0f;
		painImageComponent.color = tempColor;
		color.a = Mathf.Clamp01(color.a);
		float time = 0f;
		while (time < fadeLength * 3f)
		{
			time += Time.deltaTime * 1.15f;
			tempColor.a = Mathf.InverseLerp(fadeLength, 0f, time) * color.a;
			painImageComponent.color = tempColor;
			yield return null;
		}
	}
}
