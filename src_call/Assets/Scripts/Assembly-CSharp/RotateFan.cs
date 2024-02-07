using UnityEngine;

public class RotateFan : MonoBehaviour
{
	public float speed;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Rotate(0f, 0f, speed);
	}
}
