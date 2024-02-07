using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
	public Image bar;

	private AsyncOperation a;

	private bool isAdd;

	private void Start()
	{
	}

	public void levelName(string levelName)
	{
		a = Application.LoadLevelAsync(levelName);
	}

	public void levelName(int levelNo)
	{
		a = Application.LoadLevelAsync(levelNo);
	}

	private void Update()
	{
		bar.fillAmount = a.progress;
		if (a.progress > 0.5f && !isAdd)
		{
			isAdd = true;
		}
		if (a.isDone)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
