using System.Collections;
using UnityEngine;

public class TempAudioTimer : MonoBehaviour
{
	[HideInInspector]
	public AudioSource aSource;

	private GameObject obj;

	private void Awake()
	{
		obj = base.transform.gameObject;
	}

	public IEnumerator DeactivateTimer()
	{
		yield return new WaitForSeconds(aSource.clip.length);
		obj.SetActive(false);
	}
}
