using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade_InController : MonoBehaviour
{
	private void Awake()
	{
	}

	private void OnEnable()
	{
		StartCoroutine(fadeIn());
		Debug.Log("Fading in");
	}

	private IEnumerator fadeIn()
	{
		Color col = Color.black;
		col.a = 0f;
		while (col.a != 1f)
		{
			col.a = Mathf.MoveTowards(col.a, 1f, 0.75f * Time.deltaTime);
			GetComponent<Image>().color = col;
			yield return null;
		}
	}
}
