using UnityEngine;

public class QT_DistanceDropout : MonoBehaviour
{
	public float DistanceFromCamera = 25f;

	public Camera CustomCamera;

	private void FixedUpdate()
	{
		if ((bool)CustomCamera)
		{
			if (Vector3.Distance(CustomCamera.transform.position, base.transform.position) >= DistanceFromCamera)
			{
				base.gameObject.GetComponent<Renderer>().enabled = false;
			}
			else
			{
				base.gameObject.GetComponent<Renderer>().enabled = true;
			}
		}
		else if (Vector3.Distance(Camera.main.transform.position, base.transform.position) >= DistanceFromCamera)
		{
			base.gameObject.GetComponent<Renderer>().enabled = false;
		}
		else
		{
			base.gameObject.GetComponent<Renderer>().enabled = true;
		}
	}
}
