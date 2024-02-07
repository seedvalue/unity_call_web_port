using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelBtnAnimationController : MonoBehaviour
{
	private void OnEnable()
	{
		StartCoroutine(decreaseOpacity());
	}

	private IEnumerator increasEOpacity()
	{
		Color col = GetComponent<Image>().color;
		while (col.a != 1f)
		{
			col.a = Mathf.MoveTowards(col.a, 1f, 1.5f * Time.deltaTime);
			GetComponent<Image>().color = col;
			yield return null;
		}
		StartCoroutine(decreaseOpacity());
	}

	private IEnumerator decreaseOpacity()
	{
		Color col = GetComponent<Image>().color;
		while (col.a != 0.3f)
		{
			col.a = Mathf.MoveTowards(col.a, 0.3f, 1.5f * Time.deltaTime);
			GetComponent<Image>().color = col;
			yield return null;
		}
		StartCoroutine(increasEOpacity());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}
}
