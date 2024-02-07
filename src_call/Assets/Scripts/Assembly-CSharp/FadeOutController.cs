using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutController : MonoBehaviour
{
	private void OnEnable()
	{
		StartCoroutine(fadeOut());
	}

	private IEnumerator fadeOut()
	{
		Color col = Color.black;
		GetComponent<Image>().color = col;
		while (col.a != 0f)
		{
			col.a = Mathf.MoveTowards(col.a, 0f, 0.5f * Time.deltaTime);
			GetComponent<Image>().color = col;
			yield return null;
		}
	}
}
