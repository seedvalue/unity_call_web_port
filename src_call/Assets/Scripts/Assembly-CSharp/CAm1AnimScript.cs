using UnityEngine;

public class CAm1AnimScript : MonoBehaviour
{
	public GameObject[] Enemy;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void ActiveEnemy1(int N)
	{
		Enemy[N].SetActive(true);
	}
}
