using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrendingGamesRotation : MonoBehaviour
{
	private Image trenendingGameButtun;

	private int selectedIndex;

	private ObjectCreator objectCreater;

	public string SceneName;

	private void OnLevelWasLoaded()
	{
		if (!(base.transform.root.GetComponent<MoreGameSingleton>() == MoreGameSingleton.instance))
		{
			return;
		}
		try
		{
			StopAllCoroutines();
			if (SceneName == SceneManager.GetActiveScene().name)
			{
				trenendingGameButtun = GameObject.Find("TrendinGameButton").GetComponent<Image>();
				trenendingGameButtun.GetComponent<TrendingButtunClick>().trending = GetComponent<TrendingGamesRotation>();
				trenendingGameButtun.gameObject.SetActive(false);
				objectCreater = GetComponent<ObjectCreator>();
				StopAllCoroutines();
				InvokeRepeating("isImageLoaded", 3f, 10f);
			}
		}
		catch
		{
		}
	}

	private void OnEnable()
	{
		if (!(base.transform.root.GetComponent<MoreGameSingleton>() == MoreGameSingleton.instance))
		{
			return;
		}
		Debug.Log(" ");
		try
		{
			StopAllCoroutines();
			if (SceneName == SceneManager.GetActiveScene().name)
			{
				trenendingGameButtun = GameObject.Find("TrendinGameButton").GetComponent<Image>();
				trenendingGameButtun.GetComponent<TrendingButtunClick>().trending = GetComponent<TrendingGamesRotation>();
				trenendingGameButtun.gameObject.SetActive(false);
				objectCreater = GetComponent<ObjectCreator>();
				StopAllCoroutines();
				InvokeRepeating("isImageLoaded", 3f, 10f);
			}
		}
		catch
		{
		}
	}

	private void isImageLoaded()
	{
		if (objectCreater.objectOfLoadAssest.allFeatures.Length > 3 && objectCreater.objectOfLoadAssest.allFeatures[4].LargeGame.Length > 0 && (bool)objectCreater.objectOfLoadAssest.allFeatures[4].LargeGame[selectedIndex])
		{
			CancelInvoke("isImageLoaded");
			CanvasGroup component = GetComponent<CanvasGroup>();
			selectedIndex = 0;
			trenendingGameButtun.gameObject.SetActive(true);
			Texture2D texture2D = objectCreater.objectOfLoadAssest.allFeatures[4].LargeGame[selectedIndex];
			trenendingGameButtun.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0f, 0f), 100f);
			StartCoroutine(ChangeImages());
			component.alpha = 1f;
			component.interactable = true;
		}
	}

	private IEnumerator ChangeImages()
	{
		while (true)
		{
			yield return new WaitForSeconds(8f);
			selectedIndex = (selectedIndex + 1) % objectCreater.Large_Image_Buttons.Length;
			Texture2D image = objectCreater.objectOfLoadAssest.allFeatures[4].LargeGame[selectedIndex];
			trenendingGameButtun.sprite = Sprite.Create(image, new Rect(0f, 0f, image.width, image.height), new Vector2(0f, 0f), 100f);
		}
	}

	public void Click()
	{
		Application.OpenURL(objectCreater.objectOfLoadAssest.allFeatures[4].OpenURL[selectedIndex]);
	}
}
