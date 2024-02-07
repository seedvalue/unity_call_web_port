using UnityEngine;

public class GunRotationcontroller : MonoBehaviour
{
	private void Update()
	{
		base.transform.Rotate(0f, 60f * Time.deltaTime, 0f);
	}
}
