using UnityEngine;

public class Animation_Controller : MonoBehaviour
{
	public GameObject Anim1;

	public GameObject Gameplay;

	private void Start()
	{
		Anim1.SetActive(true);
		PlayerPrefs.SetString("Animation", "Played");
	}

	private void Update()
	{
	}
}
