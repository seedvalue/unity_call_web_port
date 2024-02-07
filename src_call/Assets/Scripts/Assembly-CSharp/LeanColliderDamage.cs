using UnityEngine;

public class LeanColliderDamage : MonoBehaviour
{
	private FPSPlayer FPSPlayerComponent;

	private void Start()
	{
		FPSPlayerComponent = Camera.main.GetComponent<CameraControl>().playerObj.GetComponent<FPSPlayer>();
	}

	public void ApplyDamage(float damage)
	{
		FPSPlayerComponent.ApplyDamage(damage);
	}
}
