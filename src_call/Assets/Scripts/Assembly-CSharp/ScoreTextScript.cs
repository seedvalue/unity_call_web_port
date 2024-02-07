using UnityEngine;

public class ScoreTextScript : MonoBehaviour
{
	public float fadeTime = 1f;

	private float startTime;

	private void Start()
	{
		startTime = Time.time;
	}

	private void Update()
	{
		base.transform.Translate(0f, Time.deltaTime * 1f, 0f);
		float num = 1f - (Time.time - startTime) / fadeTime;
		GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, num);
		if (num <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
