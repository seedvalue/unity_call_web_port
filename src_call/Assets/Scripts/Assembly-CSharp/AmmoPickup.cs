using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
	private GameObject weaponObj;

	private WeaponBehavior WeaponBehaviorComponent;

	private PlayerWeapons PlayerWeaponsComponent;

	private Transform myTransform;

	[Tooltip("The number that corresponds with this weapon's index in the PlayerWeapons script weaponOrder array.")]
	public int weaponNumber;

	[Tooltip("If this is a pooled object, set this number to its index in the object pool (to return object like an arrow back to pool after use).")]
	public int objectPoolIndex;

	[Tooltip("True if this pickup should disappear when used/activated by player.")]
	public bool removeOnUse = true;

	[Tooltip("Sound to play when picking up this item.")]
	public AudioClip pickupSound;

	[Tooltip("Sound to play when ammo is full and item cannot be used.")]
	public AudioClip fullSound;

	[Tooltip("Amount of ammo to add when picking up this ammo item.")]
	public int ammoToAdd = 1;

	[Tooltip("If not null, this texture used for the pick up crosshair of this item.")]
	public Sprite ammoPickupReticle;

	private void Start()
	{
		myTransform = base.transform;
		weaponObj = Camera.main.transform.GetComponent<CameraControl>().weaponObj;
		PlayerWeaponsComponent = weaponObj.GetComponentInChildren<PlayerWeapons>();
		Physics.IgnoreCollision(myTransform.GetComponent<Collider>(), PlayerWeaponsComponent.FPSPlayerComponent.FPSWalkerComponent.capsule, true);
		for (int i = 0; i < PlayerWeaponsComponent.weaponOrder.Length; i++)
		{
			if (PlayerWeaponsComponent.weaponOrder[i].GetComponent<WeaponBehavior>().weaponNumber == weaponNumber)
			{
				weaponObj = PlayerWeaponsComponent.weaponOrder[i];
				WeaponBehaviorComponent = weaponObj.GetComponent<WeaponBehavior>();
				break;
			}
		}
	}

	public void PickUpItem()
	{
		if (WeaponBehaviorComponent.ammo < WeaponBehaviorComponent.maxAmmo)
		{
			if (WeaponBehaviorComponent.ammo + ammoToAdd > WeaponBehaviorComponent.maxAmmo)
			{
				WeaponBehaviorComponent.ammo = WeaponBehaviorComponent.maxAmmo;
			}
			else
			{
				WeaponBehaviorComponent.ammo += ammoToAdd;
			}
			if ((bool)pickupSound)
			{
				PlayAudioAtPos.PlayClipAt(pickupSound, myTransform.position, 0.75f);
			}
			if (!WeaponBehaviorComponent.doReload && !WeaponBehaviorComponent.haveWeapon && !WeaponBehaviorComponent.nonReloadWeapon)
			{
				WeaponBehaviorComponent.haveWeapon = true;
				PlayerWeaponsComponent.StartCoroutine(PlayerWeaponsComponent.SelectWeapon(WeaponBehaviorComponent.weaponNumber));
			}
			if (removeOnUse)
			{
				FreePooledObjects();
				if (objectPoolIndex == 0)
				{
					Object.Destroy(base.gameObject);
				}
				else
				{
					AzuObjectPool.instance.RecyclePooledObj(objectPoolIndex, myTransform.gameObject);
				}
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
