using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashEffectController : MonoBehaviour
{
	private void OnEnable()
	{
		StopAllCoroutines();
		StartCoroutine(fadeIn());
		StartCoroutine(fadeOut());
	}

	private IEnumerator fadeIn()
	{
		Color col = GetComponent<Image>().color;
		while (col.a != 1f)
		{
			col.a = Mathf.MoveTowards(col.a, 1f, 100f * Time.deltaTime);
			GetComponent<Image>().color = col;
			yield return null;
		}
	}

	private IEnumerator fadeOut()
	{
		yield return new WaitForSeconds(0.7f);
		Color col = GetComponent<Image>().color;
		while (col.a != 0f)
		{
			col.a = Mathf.MoveTowards(col.a, 0f, 2f * Time.deltaTime);
			GetComponent<Image>().color = col;
			yield return null;
		}
		base.transform.parent.gameObject.SetActive(false);
	}
}
