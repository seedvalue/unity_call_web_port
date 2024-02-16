using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInController : MonoBehaviour
{
	private Text text;

	private Image image;

	private Color col;

	private float targetAlpha;

	private void OnEnable()
	{
		return;
		if (GetComponent<Image>() != null)
		{
			image = GetComponent<Image>();
			col = image.color;
			targetAlpha = col.a;
			col.a = 0f;
			image.color = col;
			StartCoroutine(FadeInImage());
		}
		else
		{
			text = GetComponent<Text>();
			col = text.color;
			targetAlpha = col.a;
			col.a = 0f;
			text.color = col;
			StartCoroutine(FadeInText());
		}
	}

	private IEnumerator FadeInText()
	{
		while (col.a != targetAlpha)
		{
			col.a = Mathf.MoveTowards(col.a, targetAlpha, 0.9f * Time.deltaTime);
			text.color = col;
			yield return null;
		}
	}

	private IEnumerator FadeInImage()
	{
		while (col.a != targetAlpha)
		{
			col.a = Mathf.MoveTowards(col.a, targetAlpha, 0.9f * Time.deltaTime);
			image.color = col;
			yield return null;
		}
	}
}
