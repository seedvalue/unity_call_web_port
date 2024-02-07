using UnityEngine;
using UnityEngine.SceneManagement;

public class Anim_Changer : MonoBehaviour
{
	public GameObject Fader;

	public GameObject Next;

	public GameObject Parent;

	public GameObject GamePlay;

	public GameObject loadingScreen;

	public AudioClip[] Sounds;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Fade_In()
	{
		Fader.GetComponent<FadeOutController>().enabled = false;
		Fader.GetComponent<Fade_InController>().enabled = true;
	}

	public void Fade_Out()
	{
		Fader.GetComponent<Fade_InController>().enabled = false;
		Fader.GetComponent<FadeOutController>().enabled = true;
	}

	public void Change_Anim()
	{
		if (Next != null)
		{
			Next.SetActive(true);
			Parent.SetActive(false);
		}
		else
		{
			loadingScreen.SetActive(true);
			SceneManager.LoadScene("AgentIntro");
		}
	}

	public void Play_Sound(int S_Num)
	{
		base.gameObject.GetComponent<AudioSource>().PlayOneShot(Sounds[S_Num]);
	}
}
