using System;
using UnityEngine;

[Serializable]
public class MultiDimensionalWeapons
{
	[Tooltip("Main weapon object that player holds in third person for this weapon.")]
	public GameObject weaponObject;

	[Tooltip("Weapon object that player holds in their left hand in third person.")]
	public GameObject weaponObject2;

	[Tooltip("Weapon object that player holds on their back when in third person.")]
	public GameObject weaponObjectBack;

	[Tooltip("The muzzle flash object to display for this weapon in third person when firing.")]
	public Renderer muzzleFlashRenderer;

	[Tooltip("The position to emit smoke effects for this weapon.")]
	public Transform muzzleSmokePos;

	[Tooltip("The position to emit shells for this weapon.")]
	public Transform shellEjectPos;
}
