using System.Collections;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
	[HideInInspector]
	public GameObject playerObj;

	[HideInInspector]
	public GameObject cameraObj;

	private InputControl InputComponent;

	private FPSRigidBodyWalker FPSWalkerComponent;

	[HideInInspector]
	public FPSPlayer FPSPlayerComponent;

	[HideInInspector]
	public WeaponBehavior CurrentWeaponBehaviorComponent;

	private Ironsights IronsightsComponent;

	[HideInInspector]
	public CameraControl CameraControlComponent;

	private Animator WeaponObjAnimatorComponent;

	[HideInInspector]
	public Animator CurWeaponObjAnimatorComponent;

	private Animator CameraAnimatorComponent;

	[Tooltip("The weaponOrder index of the first weapon that will be selected when the map loads.")]
	public int firstWeapon;

	[Tooltip("Maximum number of weapons that the player can carry.")]
	public int maxWeapons = 10;

	[Tooltip("The weaponOrder index of a weapon like fists, a knife, or sidearm that player will select when all other weapons are dropped.")]
	public int backupWeapon = 1;

	[HideInInspector]
	public int grenadeWeapon;

	[HideInInspector]
	public int totalWeapons;

	[HideInInspector]
	public int currentWeapon;

	[HideInInspector]
	public int currentGrenade;

	[Tooltip("Array for storing order of weapons. This array is created by dragging and dropping weapons from under the FPS Weapons Object in the FPS Prefab. Weapon 0 should always be the unarmed/null weapon.")]
	public GameObject[] weaponOrder;

	private WeaponBehavior[] weaponBehaviors;

	[Tooltip("Array for storing order of grenades. This array is created by dragging and dropping grenade weapons from under the FPS Weapons Object in the FPS Prefab.")]
	public GameObject[] grenadeOrder;

	private Transform myTransform;

	private Transform mainCamTransform;

	[HideInInspector]
	public Color waterMuzzleFlashColor;

	[HideInInspector]
	public float switchTime;

	[HideInInspector]
	public float sprintSwitchTime;

	[HideInInspector]
	public bool switching;

	[HideInInspector]
	public bool displayingGrenade;

	private float grenDisplayTime;

	private bool sprintSwitching;

	private bool proneMove;

	[HideInInspector]
	public bool cameraToggleState;

	[HideInInspector]
	public float cameraToggleTime;

	private bool dropWeapon;

	private bool deadDropped;

	public AudioClip changesnd;

	private bool audioPaused;

	private AudioSource[] aSources;

	private AudioSource aSource;

	[Tooltip("Directional sun light object checked by raycast for weapon shading in shadows.")]
	public Transform sunLightObj;

	[Tooltip("Albedo color of weapon material when in shade.")]
	public Color shadeColor;

	private int prevWepToGrenIndex;

	[HideInInspector]
	public bool pullGrenadeState;

	[HideInInspector]
	public bool grenadeThrownState;

	[HideInInspector]
	public bool offhandThrowActive;

	public WeaponBehavior GrenadeWeaponBehaviorComponent;

	[Tooltip("Amount of time for bullet shell to stay parented to weapon object (causes shell to inherit weapon angular velocity, decrease value if shells stick with weapon model too long).")]
	public float shellParentTime = 0.5f;

	[Tooltip("Amount of time for bullet shell to stay parented to weapon object when deadzone aiming (causes shell to inherit weapon angular velocity, decrease value if shells stick with weapon model too long).")]
	public float shellDzParentTime = 0.1f;

	[Tooltip("Amount of time for bullet shell to stay parented to weapon object when bullet time is active (causes shell to inherit weapon angular velocity, decrease value if shells stick with weapon model too long).")]
	public float shellBtParentTime = 0.2f;

	public GameController gc;

	private void Start()
	{
		myTransform = base.transform;
		mainCamTransform = Camera.main.transform;
		CameraControlComponent = mainCamTransform.GetComponent<CameraControl>();
		playerObj = CameraControlComponent.playerObj;
		cameraObj = CameraControlComponent.transform.parent.transform.gameObject;
		InputComponent = playerObj.GetComponent<InputControl>();
		FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		IronsightsComponent = playerObj.GetComponent<Ironsights>();
		CurrentWeaponBehaviorComponent = weaponOrder[firstWeapon].GetComponent<WeaponBehavior>();
		WeaponBehavior component = weaponOrder[backupWeapon].GetComponent<WeaponBehavior>();
		weaponBehaviors = myTransform.GetComponentsInChildren<WeaponBehavior>(true);
		aSources = base.transform.GetComponents<AudioSource>();
		aSource = playerObj.AddComponent<AudioSource>();
		aSource.spatialBlend = 0f;
		aSource.playOnAwake = false;
		for (int i = 0; i < weaponOrder.Length; i++)
		{
			weaponOrder[i].GetComponent<WeaponBehavior>().weaponNumber = i;
		}
		currentGrenade = 0;
		GrenadeWeaponBehaviorComponent = grenadeOrder[currentGrenade].GetComponent<WeaponBehavior>();
		grenadeWeapon = GrenadeWeaponBehaviorComponent.weaponNumber;
		if (weaponOrder[firstWeapon].GetComponent<WeaponBehavior>().haveWeapon)
		{
			StartCoroutine(SelectWeapon(firstWeapon));
		}
		else
		{
			StartCoroutine(SelectWeapon(0));
		}
		component.droppable = false;
		component.addsToTotalWeaps = false;
		UpdateTotalWeapons();
		if ((bool)sunLightObj)
		{
			return;
		}
		Light[] array = Object.FindObjectsOfType(typeof(Light)) as Light[];
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].type == LightType.Directional && array[j].gameObject.activeInHierarchy)
			{
				sunLightObj = array[j].transform;
				break;
			}
		}
	}

	private void Update()
	{
		if (cameraToggleState)
		{
			SelectWeapon(CurrentWeaponBehaviorComponent.weaponNumber);
			CurWeaponObjAnimatorComponent = weaponOrder[CurrentWeaponBehaviorComponent.weaponNumber].GetComponent<Animator>();
			CurWeaponObjAnimatorComponent.SetTrigger("IdleForward");
			CurrentWeaponBehaviorComponent.gunAnglesTarget = Vector3.zero;
			CameraControlComponent.CameraAnglesAnim = Vector3.zero;
			if (CurrentWeaponBehaviorComponent.bulletsToReload <= 1 && CurrentWeaponBehaviorComponent.weaponNumber != 0)
			{
				CurrentWeaponBehaviorComponent.WeaponAnimatorComponent.SetTrigger("Neutral");
			}
			CurrentWeaponBehaviorComponent.CameraAnimatorComponent.speed = 1f;
			CurrentWeaponBehaviorComponent.CameraAnimatorComponent.SetTrigger("CamIdle");
			cameraToggleState = false;
		}
		if (grenadeThrownState)
		{
			StartCoroutine(SelectWeapon(prevWepToGrenIndex, false, true));
			pullGrenadeState = false;
			offhandThrowActive = false;
			grenadeThrownState = false;
		}
		if (Time.timeSinceLevelLoad > 2f && Time.timeScale > 0f && (FPSWalkerComponent.grounded || !FPSWalkerComponent.sprintActive) && !switching && !InputComponent.toggleCameraHold && (!CurrentWeaponBehaviorComponent.shooting || FPSPlayerComponent.hitPoints < 1f) && !FPSWalkerComponent.holdingObject)
		{
			if (InputComponent.selectGrenPress && !displayingGrenade)
			{
				if (currentGrenade + 1 <= grenadeOrder.Length - 1)
				{
					if (grenadeOrder[currentGrenade + 1].GetComponent<WeaponBehavior>().haveWeapon && grenadeOrder[currentGrenade + 1].GetComponent<WeaponBehavior>().ammo > 0)
					{
						currentGrenade++;
					}
				}
				else if (grenadeOrder[0].GetComponent<WeaponBehavior>().haveWeapon && grenadeOrder[0].GetComponent<WeaponBehavior>().ammo > 0)
				{
					currentGrenade = 0;
				}
				GrenadeWeaponBehaviorComponent = grenadeOrder[currentGrenade].GetComponent<WeaponBehavior>();
				if (GrenadeWeaponBehaviorComponent.haveWeapon && GrenadeWeaponBehaviorComponent.ammo > 0)
				{
					grenadeWeapon = GrenadeWeaponBehaviorComponent.weaponNumber;
					prevWepToGrenIndex = currentWeapon;
					displayingGrenade = true;
					grenDisplayTime = Time.time;
					StartCoroutine(SelectWeapon(grenadeWeapon, true));
					StartCoroutine(DisplayGrenadeSwitch());
				}
			}
			if (currentWeapon != grenadeWeapon && GrenadeWeaponBehaviorComponent.ammo > 0 && !displayingGrenade && InputComponent.grenadeHold && !pullGrenadeState && !CameraControlComponent.rotating)
			{
				CurrentWeaponBehaviorComponent.CancelWeaponPull();
				offhandThrowActive = true;
				prevWepToGrenIndex = currentWeapon;
				StartCoroutine(SelectWeapon(grenadeWeapon, true));
				grenadeThrownState = false;
				pullGrenadeState = true;
			}
			if (!sprintSwitching)
			{
				if ((InputComponent.dropPress || InputComponent.xboxDpadDownPress) && currentWeapon != 0 && !pullGrenadeState && !FPSWalkerComponent.sprintActive && CurrentWeaponBehaviorComponent.droppable && !CurrentWeaponBehaviorComponent.dropWillDupe)
				{
					DropWeapon(currentWeapon);
				}
				if (FPSPlayerComponent.hitPoints < 1f && !deadDropped)
				{
					CurrentWeaponBehaviorComponent.droppable = true;
					if ((bool)CurrentWeaponBehaviorComponent.muzzleFlash)
					{
						CurrentWeaponBehaviorComponent.muzzleFlash.GetComponent<Renderer>().enabled = false;
					}
					deadDropped = true;
					DropWeapon(currentWeapon);
				}
				if (Time.timeScale > 0f)
				{
					if ((InputComponent.mouseWheel < 0f && !CameraControlComponent.rotating) || InputComponent.selectPrevPress || InputComponent.xboxDpadLeftPress)
					{
						if (currentWeapon != 0)
						{
							for (int num = currentWeapon; num > -1; num--)
							{
								WeaponBehavior component = weaponOrder[num].GetComponent<WeaponBehavior>();
								if (component.haveWeapon && component.cycleSelect && num != currentWeapon)
								{
									StartCoroutine(SelectWeapon(num));
									break;
								}
								if (num == 0)
								{
									for (int num2 = weaponOrder.Length - 1; num2 > -1; num2--)
									{
										WeaponBehavior component2 = weaponOrder[num2].GetComponent<WeaponBehavior>();
										if (component2.haveWeapon && component2.cycleSelect && num2 != currentWeapon)
										{
											StartCoroutine(SelectWeapon(num2));
											break;
										}
									}
								}
							}
						}
						else
						{
							for (int num3 = weaponOrder.Length - 1; num3 > -1; num3--)
							{
								WeaponBehavior component3 = weaponOrder[num3].GetComponent<WeaponBehavior>();
								if (component3.haveWeapon && component3.cycleSelect && num3 != currentWeapon)
								{
									StartCoroutine(SelectWeapon(num3));
									break;
								}
							}
						}
					}
					else if ((InputComponent.mouseWheel > 0f && !CameraControlComponent.rotating) || InputComponent.selectNextPress || InputComponent.xboxDpadRightPress || (dropWeapon && totalWeapons != 0))
					{
						if (currentWeapon < weaponOrder.Length - 1)
						{
							for (int i = currentWeapon; i < weaponOrder.Length; i++)
							{
								WeaponBehavior component4 = weaponOrder[i].GetComponent<WeaponBehavior>();
								if ((component4.haveWeapon && component4.cycleSelect && i != currentWeapon && !dropWeapon) || (component4.haveWeapon && component4.cycleSelect && i != currentWeapon && i != backupWeapon && dropWeapon))
								{
									StartCoroutine(SelectWeapon(i));
									break;
								}
								if (i != weaponOrder.Length - 1)
								{
									continue;
								}
								for (int j = 0; j < weaponOrder.Length - 1; j++)
								{
									WeaponBehavior component5 = weaponOrder[j].GetComponent<WeaponBehavior>();
									if ((component5.haveWeapon && component5.cycleSelect && j != currentWeapon && !dropWeapon) || (component5.haveWeapon && component5.cycleSelect && j != currentWeapon && j != backupWeapon && dropWeapon))
									{
										StartCoroutine(SelectWeapon(j));
										break;
									}
								}
							}
						}
						else
						{
							for (int k = 0; k < weaponOrder.Length - 1; k++)
							{
								WeaponBehavior component6 = weaponOrder[k].GetComponent<WeaponBehavior>();
								if ((component6.haveWeapon && component6.cycleSelect && k != currentWeapon && !dropWeapon) || (component6.haveWeapon && component6.cycleSelect && k != currentWeapon && k != backupWeapon && dropWeapon))
								{
									StartCoroutine(SelectWeapon(k));
									break;
								}
							}
						}
					}
				}
				if (InputComponent.holsterPress)
				{
					if (currentWeapon != 0)
					{
						StartCoroutine(SelectWeapon(0));
					}
				}
				else if (InputComponent.selectWeap1Press && weaponOrder.Length - 1 > 0)
				{
					if (currentWeapon != 1)
					{
						StartCoroutine(SelectWeapon(1));
					}
				}
				else if (InputComponent.selectWeap2Press && weaponOrder.Length - 1 > 1)
				{
					if (currentWeapon != 2)
					{
						StartCoroutine(SelectWeapon(2));
					}
				}
				else if (InputComponent.selectWeap3Press && weaponOrder.Length - 1 > 2)
				{
					if (currentWeapon != 3)
					{
						StartCoroutine(SelectWeapon(3));
					}
				}
				else if (InputComponent.selectWeap4Press && weaponOrder.Length - 1 > 3)
				{
					if (currentWeapon != 4)
					{
						StartCoroutine(SelectWeapon(4));
					}
				}
				else if (InputComponent.selectWeap5Press && weaponOrder.Length - 1 > 4)
				{
					if (currentWeapon != 5)
					{
						StartCoroutine(SelectWeapon(5));
					}
				}
				else if (InputComponent.selectWeap6Press && weaponOrder.Length - 1 > 5)
				{
					if (currentWeapon != 6)
					{
						StartCoroutine(SelectWeapon(6));
					}
				}
				else if (InputComponent.selectWeap7Press && weaponOrder.Length - 1 > 6)
				{
					if (currentWeapon != 7)
					{
						StartCoroutine(SelectWeapon(7));
					}
				}
				else if (InputComponent.selectWeap8Press && weaponOrder.Length - 1 > 7)
				{
					if (currentWeapon != 8)
					{
						StartCoroutine(SelectWeapon(8));
					}
				}
				else if (InputComponent.selectWeap9Press && weaponOrder.Length - 1 > 8 && currentWeapon != 9)
				{
					StartCoroutine(SelectWeapon(9));
				}
			}
		}
		if (switchTime + 0.87f > Time.time)
		{
			switching = true;
		}
		else
		{
			switching = false;
		}
		if (grenDisplayTime + 2f < Time.time)
		{
			displayingGrenade = false;
		}
		if (sprintSwitchTime + 0.44f > Time.time)
		{
			sprintSwitching = true;
		}
		else
		{
			sprintSwitching = false;
		}
		if (Time.timeScale > 0f)
		{
			if (audioPaused)
			{
				aSources[1].Play();
				audioPaused = false;
			}
		}
		else if (!audioPaused && aSources[1].isPlaying)
		{
			aSources[1].Pause();
			audioPaused = true;
		}
	}

	private void LateUpdate()
	{
		Vector3 position = new Vector3(mainCamTransform.position.x, mainCamTransform.position.y, mainCamTransform.position.z);
		myTransform.position = position;
	}

	private IEnumerator DisplayGrenadeSwitch()
	{
		yield return new WaitForSeconds(1f);
		StartCoroutine(SelectWeapon(prevWepToGrenIndex, false, true));
	}

	public void DropWeapon(int weapon)
	{
		if (!weaponOrder[weapon].GetComponent<WeaponBehavior>().haveWeapon)
		{
			return;
		}
		weaponOrder[weapon].GetComponent<WeaponBehavior>().haveWeapon = false;
		aSources[1].Stop();
		float num = ((!deadDropped) ? ((FPSWalkerComponent.inputY > 0f) ? 8f : ((!(FPSWalkerComponent.inputY < 0f)) ? 4f : 2f)) : ((!(FPSWalkerComponent.inputY > 0f)) ? 2f : 4f));
		if ((currentWeapon != backupWeapon || deadDropped) && currentWeapon != 0)
		{
			dropWeapon = true;
		}
		UpdateTotalWeapons();
		if ((bool)weaponOrder[weapon].GetComponent<WeaponBehavior>().weaponDropObj)
		{
			GameObject gameObject = (CameraControlComponent.thirdPersonActive ? Object.Instantiate(weaponOrder[weapon].GetComponent<WeaponBehavior>().weaponDropObj, playerObj.transform.position + playerObj.transform.forward * 0.25f + Vector3.up * -0.25f, mainCamTransform.rotation) : Object.Instantiate(weaponOrder[weapon].GetComponent<WeaponBehavior>().weaponDropObj, mainCamTransform.position + playerObj.transform.forward * 0.25f + Vector3.up * -0.25f, mainCamTransform.rotation));
			gameObject.GetComponent<Rigidbody>().AddForce(playerObj.transform.forward * num, ForceMode.Impulse);
			float num2 = ((!(Random.value > 0.5f)) ? (-7f) : 7f);
			gameObject.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * num2, ForceMode.Impulse);
			num2 = ((!(Random.value > 0.5f)) ? (-7f) : 7f);
			gameObject.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.right * num2, ForceMode.Impulse);
		}
		if (!deadDropped)
		{
			if (totalWeapons == 0 && currentWeapon != 0 && currentWeapon != backupWeapon)
			{
				if (weaponOrder[backupWeapon].GetComponent<WeaponBehavior>().haveWeapon && FPSPlayerComponent.hitPoints > 1f)
				{
					StartCoroutine(SelectWeapon(backupWeapon));
				}
				else
				{
					StartCoroutine(SelectWeapon(0));
				}
			}
		}
		else
		{
			StartCoroutine(SelectWeapon(0));
		}
	}

	public void UpdateTotalWeapons()
	{
		totalWeapons = 0;
		for (int i = 1; i < weaponOrder.Length; i++)
		{
			if (weaponOrder[i].GetComponent<WeaponBehavior>().haveWeapon && weaponOrder[i].GetComponent<WeaponBehavior>().addsToTotalWeaps)
			{
				totalWeapons++;
			}
		}
	}

	public IEnumerator SelectWeapon(int index, bool isOffhandThrow = false, bool endOffhandThrow = false)
	{
		CameraAnimatorComponent = Camera.main.GetComponent<Animator>();
		WeaponObjAnimatorComponent = weaponOrder[currentWeapon].GetComponent<Animator>();
		dropWeapon = false;
		WeaponBehavior ThisWeaponBehavior = weaponOrder[index].GetComponent<WeaponBehavior>();
		if ((!ThisWeaponBehavior.haveWeapon && index != 0) || (!ThisWeaponBehavior.cycleSelect && !isOffhandThrow) || FPSWalkerComponent.hideWeapon || pullGrenadeState)
		{
			yield break;
		}
		if (index != 0)
		{
			weaponOrder[0].GetComponent<WeaponBehavior>().haveWeapon = false;
		}
		FPSPlayerComponent.zoomed = false;
		if (CurrentWeaponBehaviorComponent.useLight)
		{
			if ((bool)CurrentWeaponBehaviorComponent.lightConeMesh)
			{
				CurrentWeaponBehaviorComponent.lightConeMesh.enabled = false;
			}
			if ((bool)CurrentWeaponBehaviorComponent.spot)
			{
				CurrentWeaponBehaviorComponent.spot.enabled = false;
			}
			if ((bool)CurrentWeaponBehaviorComponent.point)
			{
				CurrentWeaponBehaviorComponent.point.enabled = false;
			}
		}
		if (CurrentWeaponBehaviorComponent.bulletsToReload != CurrentWeaponBehaviorComponent.bulletsPerClip && IronsightsComponent.reloading)
		{
			CurrentWeaponBehaviorComponent.WeaponAnimatorComponent.SetTrigger("Neutral");
			CurrentWeaponBehaviorComponent.bulletsReloaded = 0;
		}
		IronsightsComponent.reloading = false;
		CurrentWeaponBehaviorComponent.StopCoroutine("Reload");
		switchTime = Time.time;
		if (Time.timeSinceLevelLoad > 1f)
		{
			if (!offhandThrowActive && !displayingGrenade)
			{
				aSource.clip = changesnd;
				aSource.volume = 1f;
				aSource.Play();
			}
			CameraAnimatorComponent.speed = 1f;
			CameraAnimatorComponent.SetTrigger("CamSwitch");
		}
		if (WeaponObjAnimatorComponent.gameObject.activeSelf)
		{
			if (!FPSWalkerComponent.canRun && !FPSWalkerComponent.proneMove)
			{
				WeaponObjAnimatorComponent.SetTrigger("Switch");
			}
			else
			{
				WeaponObjAnimatorComponent.SetTrigger("SprintBack");
				CurrentWeaponBehaviorComponent.sprintAnimState = true;
			}
		}
		if (Time.timeSinceLevelLoad > 2f)
		{
			if (!CurrentWeaponBehaviorComponent.verticalWeapon || isOffhandThrow)
			{
				IronsightsComponent.switchMove = -0.4f;
			}
			else
			{
				IronsightsComponent.switchMove = -1.2f;
			}
			yield return new WaitForSeconds(0.2f);
		}
		for (int i = 0; i < weaponOrder.Length; i++)
		{
			CurWeaponObjAnimatorComponent = weaponOrder[i].GetComponent<Animator>();
			if (i == index)
			{
				IronsightsComponent.gun = weaponOrder[i].transform;
				IronsightsComponent.gunObj = weaponOrder[i];
				IronsightsComponent.WeaponBehaviorComponent = weaponOrder[i].GetComponent<WeaponBehavior>();
				FPSPlayerComponent.WeaponBehaviorComponent = weaponOrder[i].GetComponent<WeaponBehavior>();
				CameraControlComponent.gun = weaponOrder[i];
				CurrentWeaponBehaviorComponent = weaponOrder[i].GetComponent<WeaponBehavior>();
				weaponOrder[i].SetActive(true);
				if (endOffhandThrow)
				{
					CurrentWeaponBehaviorComponent.isOffhandThrow = true;
					switchTime = Time.time - 0.5f;
				}
				CurrentWeaponBehaviorComponent.InitializeWeapon();
				currentWeapon = index;
				weaponOrder[i].transform.localPosition = weaponOrder[i].transform.localPosition + new Vector3(0f, weaponOrder[i].transform.localPosition.y - 0.3f, 0f);
				if (Time.timeSinceLevelLoad > 2f)
				{
					IronsightsComponent.switchMove = 0f;
					if (!FPSWalkerComponent.canRun && !FPSWalkerComponent.proneMove)
					{
						CurWeaponObjAnimatorComponent.SetTrigger("SwitchReverse");
						continue;
					}
					CurWeaponObjAnimatorComponent.SetTrigger("SprintBack");
					CurrentWeaponBehaviorComponent.sprintAnimState = true;
				}
				else
				{
					CurWeaponObjAnimatorComponent.SetTrigger("IdleForward");
				}
			}
			else
			{
				weaponOrder[i].transform.position = myTransform.position;
				if (CurWeaponObjAnimatorComponent.gameObject.activeSelf)
				{
					CurWeaponObjAnimatorComponent.SetTrigger("SprintBack");
					CurrentWeaponBehaviorComponent.sprintAnimState = true;
				}
				weaponOrder[i].GetComponent<WeaponBehavior>().sprintState = true;
				weaponOrder[i].SetActive(false);
			}
		}
	}

	public void GiveAllWeaponsAndAmmo()
	{
		WeaponBehavior[] array = weaponBehaviors;
		foreach (WeaponBehavior weaponBehavior in array)
		{
			weaponBehavior.haveWeapon = true;
			weaponBehavior.ammo = weaponBehavior.maxAmmo;
		}
	}
}
