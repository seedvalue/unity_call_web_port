using UnityEngine;

public class playSounds : MonoBehaviour
{
	public AudioSource audioSource;

	private void OnEnable()
	{
		if (PlayerPrefs.GetInt("Sound", 1) == 1)
		{
			audioSource.Play();
		}
	}
}
