using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonWeapons : MonoBehaviour
{
	[Tooltip("List of weapon objects that correspond with the Weapon Order list of PlayerWeapons.cs.")]
	public List<MultiDimensionalWeapons> thirdPersonWeaponModels = new List<MultiDimensionalWeapons>();
}
