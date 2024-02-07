using UnityEngine;

public class MoreGameSingleton : MonoBehaviour
{
	public static MoreGameSingleton instance;

	private void Awake()
	{
		if (!instance)
		{
			instance = GetComponent<MoreGameSingleton>();
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (instance != GetComponent<MoreGameSingleton>())
		{
			Object.Destroy(base.gameObject);
		}
	}
}
