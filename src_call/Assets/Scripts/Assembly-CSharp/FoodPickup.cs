using UnityEngine;

public class FoodPickup : MonoBehaviour
{
	private GameObject playerObj;

	private Transform myTransform;

	[Tooltip("True if this pickup should disappear when used/activated by player.")]
	public bool removeOnUse = true;

	[Tooltip("Sound to play when picking up this item.")]
	public AudioClip pickupSound;

	[Tooltip("Sound to play when hunger is zero and item cannot be used.")]
	public AudioClip fullSound;

	[Tooltip("Amount of hunger to remove when picking up this food item.")]
	public int hungerToRemove = 15;

	[Tooltip("Amount of health to restore when picking up this food item.")]
	public int healthToRestore = 5;

	private FPSPlayer FPSPlayerComponent;

	private void Start()
	{
		myTransform = base.transform;
		playerObj = Camera.main.transform.GetComponent<CameraControl>().playerObj;
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		Physics.IgnoreCollision(myTransform.GetComponent<Collider>(), FPSPlayerComponent.FPSWalkerComponent.capsule, true);
	}

	public void PickUpItem()
	{
		if (FPSPlayerComponent.hungerPoints > 0f && FPSPlayerComponent.usePlayerHunger)
		{
			if ((double)(FPSPlayerComponent.hungerPoints - (float)hungerToRemove) > 0.0)
			{
				FPSPlayerComponent.UpdateHunger(-hungerToRemove);
			}
			else
			{
				FPSPlayerComponent.UpdateHunger(0f - FPSPlayerComponent.hungerPoints);
			}
			if (FPSPlayerComponent.hitPoints + (float)healthToRestore < FPSPlayerComponent.maximumHitPoints)
			{
				FPSPlayerComponent.HealPlayer(healthToRestore);
			}
			else
			{
				FPSPlayerComponent.HealPlayer(FPSPlayerComponent.maximumHitPoints - FPSPlayerComponent.hitPoints);
			}
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
		else if ((bool)fullSound)
		{
			PlayAudioAtPos.PlayClipAt(fullSound, myTransform.position, 0.75f);
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
