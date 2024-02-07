using UnityEngine;

public class MoreGameButton : MonoBehaviour
{
	private GameObject moreGameObject;

	private void Awake()
	{
		moreGameObject = GameObject.Find("MoreGamesPrefab");
	}

	public void OpenMoreGame()
	{
		if ((bool)moreGameObject)
		{
			moreGameObject.GetComponent<CanvasGroup>().alpha = 1f;
			moreGameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
			moreGameObject.GetComponent<CanvasGroup>().interactable = true;
		}
	}
}
