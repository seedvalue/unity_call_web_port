using UnityEngine;

public class HealthPickup : MonoBehaviour
{
	private Transform myTransform;

	private FPSPlayer FPSPlayerComponent;

	[Tooltip("Amount of health this pickup should restore on use.")]
	public float healthToAdd = 25f;

	[Tooltip("True if this pickup should disappear when used/activated by player.")]
	public bool removeOnUse = true;

	[Tooltip("Sound to play when picking up this item.")]
	public AudioClip pickupSound;

	[Tooltip("Sound to play when health is full and item cannot be used.")]
	public AudioClip fullSound;

	[Tooltip("If not null, this texture used for the pick up crosshair of this item.")]
	public Sprite healthPickupReticle;

	private void Start()
	{
		myTransform = base.transform;
		FPSPlayerComponent = Camera.main.transform.GetComponent<CameraControl>().playerObj.GetComponent<FPSPlayer>();
		Physics.IgnoreCollision(myTransform.GetComponent<Collider>(), FPSPlayerComponent.FPSWalkerComponent.capsule, true);
	}

	private void PickUpItem(GameObject user)
	{
		FPSPlayerComponent = user.GetComponent<FPSPlayer>();
		if (FPSPlayerComponent.hitPoints < FPSPlayerComponent.maximumHitPoints)
		{
			FPSPlayerComponent.HealPlayer(healthToAdd);
			if ((bool)pickupSound)
			{
				PlayAudioAtPos.PlayClipAt(pickupSound, myTransform.position, 0.75f);
			}
			if (removeOnUse)
			{
				FreePooledObjects();
				Object.Destroy(base.gameObject);
			}
		}
		else
		{
			if ((bool)fullSound)
			{
				PlayAudioAtPos.PlayClipAt(fullSound, myTransform.position, 0.75f);
			}
			if (removeOnUse)
			{
				FreePooledObjects();
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void FreePooledObjects()
	{
		FadeOutDecals[] componentsInChildren = base.gameObject.GetComponentsInChildren<FadeOutDecals>(true);
		FadeOutDecals[] array = componentsInChildren;
		foreach (FadeOutDecals fadeOutDecals in array)
		{
			fadeOutDecals.parentObjTransform.parent = AzuObjectPool.instance.transform;
			fadeOutDecals.parentObj.SetActive(false);
		}
		ArrowObject[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<ArrowObject>(true);
		ArrowObject[] array2 = componentsInChildren2;
		foreach (ArrowObject arrowObject in array2)
		{
			arrowObject.transform.parent = null;
			arrowObject.myRigidbody.isKinematic = false;
			arrowObject.myBoxCol.isTrigger = false;
			arrowObject.gameObject.tag = "Usable";
			arrowObject.falling = true;
		}
	}
}
