using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneController : MonoBehaviour
{
	public GameObject loadingScreen;

	public void startGameOnClick()
	{
		loadingScreen.SetActive(true);
		SceneManager.LoadScene("MainMenu");
	}
}
