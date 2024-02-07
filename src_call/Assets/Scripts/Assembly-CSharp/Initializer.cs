using UnityEngine;

public class Initializer : MonoBehaviour
{
	public GameObject CanvasObj;

	public GameObject LoadingPrefab;

	public string YourFirstGameLevelName = "MainMenu";

	private void Start()
	{
		Invoke("goToMainMenu", 1.5f);
	}

	private void goToMainMenu()
	{
		GameObject gameObject = Object.Instantiate(LoadingPrefab);
		gameObject.GetComponent<Loading>().levelName(YourFirstGameLevelName);
		gameObject.transform.SetParent(CanvasObj.transform, false);
	}
}
