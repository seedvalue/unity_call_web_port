using UnityEngine;

public class Cam4_Controller : MonoBehaviour
{
	public GameObject Player;

	private bool M_Down;

	private void Start()
	{
	}

	private void Update()
	{
		if (M_Down)
		{
			Player.transform.Translate(Vector3.down * Time.deltaTime * 1.5f);
		}
	}

	public void SlideDown()
	{
		M_Down = true;
	}

	public void Jump()
	{
		Player.transform.parent = base.gameObject.transform.parent;
		Player.GetComponent<Animator>().SetTrigger("Jump");
	}

	public void StopSlideDown()
	{
		M_Down = false;
	}
}
