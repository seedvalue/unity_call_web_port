using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDTextEffectController : MonoBehaviour
{
	private Text text;

	private Color col;

	private AudioSource sound;

	private void Awake()
	{
		text = GetComponent<Text>();
		sound = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		if (sound != null)
		{
			sound.Play();
		}
		StopCoroutine(moveToTop());
		col = text.color;
		col.a = 1f;
		text.color = col;
		base.transform.localPosition = new Vector3(0f, 50f, 0f);
		StartCoroutine(moveToTop());
	}

	private IEnumerator moveToTop()
	{
		float alpha = 1f;
		while (base.transform.localPosition.y < 140f)
		{
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, new Vector3(0f, 150f, 0f), 1.6f * Time.deltaTime);
			alpha = Mathf.MoveTowards(alpha, 0f, 1.3f * Time.deltaTime);
			col.a = alpha;
			text.color = col;
			yield return null;
		}
		base.gameObject.SetActive(false);
	}
}
