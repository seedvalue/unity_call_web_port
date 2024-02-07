using System.Collections;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
	private GameObject weaponObj;

	private Transform myTransform;

	[Tooltip("The number that corresponds with this weapon's index in the PlayerWeapons.cs script weapon order array.")]
	public int weaponNumber;

	private int weaponToDrop;

	[Tooltip("True if this pickup disappears when used/activated by player.")]
	public bool removeOnUse = true;

	[Tooltip("Remove weapon pickup after this time if it is greater than zero..")]
	public float removeTime;

	private float startTime;

	[Tooltip("Sound to play when picking up this weapon.")]
	public AudioClip pickupSound;

	[Tooltip("Sound to play when ammo is full and weapon cannot be used.")]
	public AudioClip fullSound;

	[Tooltip("If not null, this texture used for the pick up crosshair of this item.")]
	public Sprite weaponPickupReticle;

	private void Start()
	{
		myTransform = base.transform;
		PlayerWeapons component = Camera.main.transform.GetComponent<CameraControl>().weaponObj.GetComponent<PlayerWeapons>();
		for (int i = 0; i < component.weaponOrder.Length; i++)
		{
			if (component.weaponOrder[i].GetComponent<WeaponBehavior>().weaponNumber == weaponNumber)
			{
				weaponObj = component.weaponOrder[i];
				break;
			}
		}
		if (component.playerObj.activeInHierarchy && (bool)myTransform.GetComponent<Collider>())
		{
			Physics.IgnoreCollision(myTransform.GetComponent<Collider>(), component.FPSPlayerComponent.FPSWalkerComponent.capsule, true);
		}
	}

	public IEnumerator DestroyWeapon(float waitTime)
	{
		startTime = Time.time;
		while (!(startTime + waitTime < Time.time))
		{
			yield return null;
		}
		FreePooledObjects();
		Object.Destroy(base.gameObject);
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

	private void PickUpItem()
	{
		PlayerWeapons component = Camera.main.transform.GetComponent<CameraControl>().weaponObj.GetComponent<PlayerWeapons>();
		WeaponBehavior component2 = weaponObj.GetComponent<WeaponBehavior>();
		WeaponBehavior currentWeaponBehaviorComponent = component.CurrentWeaponBehaviorComponent;
		if (!component2.haveWeapon)
		{
			if (component.totalWeapons == component.maxWeapons && component2.addsToTotalWeaps)
			{
				if (currentWeaponBehaviorComponent.droppable)
				{
					if (removeOnUse && !component2.dropWillDupe && !currentWeaponBehaviorComponent.dropWillDupe)
					{
						component.DropWeapon(component.currentWeapon);
					}
					else
					{
						currentWeaponBehaviorComponent.haveWeapon = false;
						currentWeaponBehaviorComponent.dropWillDupe = false;
						component2.dropWillDupe = true;
					}
				}
				else
				{
					for (int i = component.currentWeapon; i < component.weaponOrder.Length; i++)
					{
						if (component.weaponOrder[i].GetComponent<WeaponBehavior>().haveWeapon && component.weaponOrder[i].GetComponent<WeaponBehavior>().droppable)
						{
							weaponToDrop = i;
							break;
						}
						if (i != component.weaponOrder.Length - 1)
						{
							continue;
						}
						for (int j = 0; j < component.weaponOrder.Length; j++)
						{
							if (component.weaponOrder[j].GetComponent<WeaponBehavior>().haveWeapon && component.weaponOrder[j].GetComponent<WeaponBehavior>().droppable)
							{
								weaponToDrop = j;
								break;
							}
						}
					}
					if (removeOnUse && !component2.dropWillDupe)
					{
						component.DropWeapon(weaponToDrop);
					}
					else
					{
						component.weaponOrder[weaponToDrop].GetComponent<WeaponBehavior>().haveWeapon = false;
						component.weaponOrder[weaponToDrop].GetComponent<WeaponBehavior>().dropWillDupe = false;
						component2.dropWillDupe = true;
					}
				}
			}
			component2.haveWeapon = true;
			if (!removeOnUse)
			{
				component2.dropWillDupe = true;
			}
			else
			{
				component2.dropWillDupe = false;
			}
			component.StartCoroutine(component.SelectWeapon(component2.weaponNumber));
			component.UpdateTotalWeapons();
			RemovePickup();
		}
		else if (component2.ammo < component2.maxAmmo && removeOnUse && component2.meleeSwingDelay == 0f)
		{
			if (component2.ammo + component2.bulletsPerClip > component2.maxAmmo)
			{
				component2.ammo = component2.maxAmmo;
			}
			else
			{
				component2.ammo += component2.bulletsPerClip;
			}
			RemovePickup();
		}
		else if ((bool)fullSound)
		{
			PlayAudioAtPos.PlayClipAt(fullSound, myTransform.position, 0.75f);
		}
	}

	private void RemovePickup()
	{
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
}
