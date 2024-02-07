using UnityEngine;

public class SelectedIndicatorBehavior : MonoBehaviour
{
	private RaycastHit target;

	public LayerMask IgnoreLayerMask;

	private void Update()
	{
		Ray ray = new Ray(base.transform.position, Vector3.down);
		if (Physics.Raycast(ray, out target, 10f, ~(int)IgnoreLayerMask) && (target.transform.name.Contains("Terrain") || target.transform.name.Contains("PlatformSloped")))
		{
			base.transform.rotation = Quaternion.FromToRotation(Vector3.up, target.normal);
		}
	}
}
