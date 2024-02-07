using UnityEngine;

public class MovePlayerAndCamera : MonoBehaviour
{
	[Tooltip("Position and angles of main camera will be set to this one's when toggling main camera.")]
	public GameObject CinemaCameraObj;

	[Tooltip("Position that player will be teleported to.")]
	public Transform movePos;

	[Tooltip("Pitch to set camera to after moving player.")]
	public float moveCamPitch;

	[Tooltip("Yaw to set camera to after moving player.")]
	public float moveCamYaw;

	private bool toggleState;

	private bool toggleGuiState;

	private bool noCrosshair;

	[HideInInspector]
	public GameObject FPSWeaponsObj;

	[HideInInspector]
	public GameObject FPSPlayerObj;

	[HideInInspector]
	public GameObject FPSCameraObj;

	[HideInInspector]
	public GameObject MainCameraObj;

	[HideInInspector]
	public GameObject PlayerCharacterObj;

	[HideInInspector]
	public PlayerCharacter PlayerCharacterComponent;

	private CameraControl CameraControlComponent;

	private SmoothMouseLook MouseLookComponent;

	private FPSPlayer FPSPlayerComponent;

	private FPSRigidBodyWalker FPSWalkerComponent;

	private PlayerWeapons PlayerWeaponsComponent;

	private Ironsights IronsightsComponent;

	private bool playerModelToggleState;

	private void Start()
	{
		if ((bool)CinemaCameraObj)
		{
			CinemaCameraObj.SetActive(false);
		}
		CameraControlComponent = MainCameraObj.GetComponent<CameraControl>();
		FPSPlayerObj = CameraControlComponent.playerObj;
		FPSWeaponsObj = CameraControlComponent.weaponObj;
		FPSCameraObj = CameraControlComponent.transform.parent.transform.gameObject;
		MainCameraObj = Camera.main.transform.gameObject;
		MouseLookComponent = FPSCameraObj.GetComponent<SmoothMouseLook>();
		FPSPlayerComponent = FPSPlayerObj.GetComponent<FPSPlayer>();
		FPSWalkerComponent = FPSPlayerObj.GetComponent<FPSRigidBodyWalker>();
		PlayerWeaponsComponent = FPSWeaponsObj.GetComponent<PlayerWeapons>();
		IronsightsComponent = FPSPlayerObj.GetComponent<Ironsights>();
		PlayerCharacterComponent = CameraControlComponent.PlayerCharacterComponent;
		PlayerCharacterObj = CameraControlComponent.PlayerCharacterObj;
		if (!FPSPlayerComponent.crosshairEnabled)
		{
			noCrosshair = true;
		}
	}

	private void Update()
	{
		if (Time.timeSinceLevelLoad > 0.2f)
		{
			if ((bool)movePos && Input.GetKeyDown(KeyCode.Insert))
			{
				MovePlayer(movePos.position + Vector3.up * FPSWalkerComponent.capsule.height * 0.5f, moveCamYaw, moveCamPitch);
			}
			if (Input.GetKeyDown(KeyCode.Delete) && (bool)CinemaCameraObj)
			{
				ReleaseMainCamera();
			}
			if ((bool)movePos && Input.GetKeyDown(KeyCode.End) && (bool)CinemaCameraObj)
			{
				ReleaseMainCameraAndMovePlayer();
			}
		}
	}

	public void MovePlayer(Vector3 position, float camYaw = 0f, float camPitch = 0f)
	{
		if (FPSPlayerObj.activeInHierarchy)
		{
			CancelPlayerActions();
			CameraControlComponent.movingTime = Time.time;
			MouseLookComponent.playerMovedTime = Time.time;
			Vector3 vector = position + Vector3.up * FPSWalkerComponent.standingCamHeight;
			CameraControlComponent.tempLerpPos = vector;
			CameraControlComponent.camPos = vector;
			CameraControlComponent.targetPos = vector;
			FPSPlayerObj.transform.position = position;
			FPSCameraObj.transform.position = vector;
			FPSWeaponsObj.transform.position = FPSCameraObj.transform.position;
			PlayerCharacterObj.transform.position = position - Vector3.up * FPSWalkerComponent.capsule.height * 0.5f;
			PlayerCharacterComponent.tempBodyPosition = position - Vector3.up * FPSWalkerComponent.capsule.height * 0.5f;
			PlayerCharacterComponent.transform.rotation = FPSPlayerObj.transform.rotation;
			PlayerCharacterComponent.verticalPos = position.y;
			if (camYaw != 0f)
			{
				FPSCameraObj.transform.rotation = Quaternion.Euler(0f - camPitch, camYaw, 0f);
				FPSWeaponsObj.transform.rotation = FPSCameraObj.transform.rotation;
				MouseLookComponent.rotationX = camYaw;
				MouseLookComponent.rotationY = camPitch;
				MouseLookComponent.horizontalDelta = 0f;
				MouseLookComponent.recoilY = 0f;
				MouseLookComponent.recoilX = 0f;
				MouseLookComponent.xQuaternion = Quaternion.Euler(0f, 0f, 0f);
				MouseLookComponent.yQuaternion = Quaternion.Euler(0f, 0f, 0f);
				MouseLookComponent.originalRotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
	}

	public void ToggleGuiDisplay()
	{
		if (!toggleGuiState)
		{
			toggleGuiState = true;
			return;
		}
		if (!noCrosshair)
		{
			FPSPlayerComponent.crosshairUiImage.enabled = true;
		}
		toggleGuiState = false;
	}

	public void ToggleCinemaCamera()
	{
		if (!toggleState)
		{
			ToggleGuiDisplay();
			FPSWeaponsObj.SetActive(false);
			FPSPlayerObj.SetActive(false);
			FPSCameraObj.SetActive(false);
			TogglePlayerCharacterVisibility();
			PlayerCharacterComponent.fpBodyObj.SetActive(false);
			CinemaCameraObj.SetActive(true);
			toggleState = true;
		}
		else
		{
			CinemaCameraObj.SetActive(false);
			CameraControlComponent.movingTime = Time.time;
			MouseLookComponent.playerMovedTime = Time.time;
			FPSCameraObj.SetActive(true);
			FPSWeaponsObj.SetActive(true);
			FPSPlayerObj.SetActive(true);
			TogglePlayerCharacterVisibility();
			PlayerCharacterComponent.fpBodyObj.SetActive(true);
			CancelPlayerActions();
			ToggleGuiDisplay();
			toggleState = false;
		}
	}

	public void ReleaseMainCamera()
	{
		if (!toggleState)
		{
			ToggleGuiDisplay();
			FPSWeaponsObj.SetActive(false);
			FPSPlayerObj.SetActive(false);
			TogglePlayerCharacterVisibility();
			PlayerCharacterComponent.fpBodyObj.SetActive(false);
			CameraControlComponent.enabled = false;
			MouseLookComponent.enabled = false;
			MainCameraObj.transform.position = CinemaCameraObj.transform.position;
			MainCameraObj.transform.rotation = CinemaCameraObj.transform.rotation;
			Camera.main.fieldOfView = IronsightsComponent.defaultFov;
			toggleState = true;
		}
		else
		{
			MainCameraObj.transform.rotation = FPSCameraObj.transform.rotation;
			MainCameraObj.transform.position = FPSCameraObj.transform.position;
			CameraControlComponent.enabled = true;
			MouseLookComponent.enabled = true;
			FPSWeaponsObj.SetActive(true);
			FPSPlayerObj.SetActive(true);
			TogglePlayerCharacterVisibility();
			PlayerCharacterComponent.fpBodyObj.SetActive(true);
			CancelPlayerActions();
			ToggleGuiDisplay();
			toggleState = false;
		}
	}

	public void ReleaseMainCameraAndMovePlayer()
	{
		if (!toggleState)
		{
			ToggleGuiDisplay();
			FPSWeaponsObj.SetActive(false);
			FPSPlayerObj.SetActive(false);
			TogglePlayerCharacterVisibility();
			PlayerCharacterComponent.fpBodyObj.SetActive(false);
			CameraControlComponent.enabled = false;
			MouseLookComponent.enabled = false;
			MainCameraObj.transform.position = CinemaCameraObj.transform.position;
			MainCameraObj.transform.rotation = CinemaCameraObj.transform.rotation;
			Camera.main.fieldOfView = IronsightsComponent.defaultFov;
			toggleState = true;
		}
		else
		{
			MainCameraObj.transform.rotation = FPSCameraObj.transform.rotation;
			MainCameraObj.transform.position = FPSCameraObj.transform.position;
			CameraControlComponent.enabled = true;
			MouseLookComponent.enabled = true;
			FPSWeaponsObj.SetActive(true);
			FPSPlayerObj.SetActive(true);
			TogglePlayerCharacterVisibility();
			PlayerCharacterComponent.fpBodyObj.SetActive(true);
			CancelPlayerActions();
			ToggleGuiDisplay();
			MovePlayer(movePos.position + Vector3.up * FPSWalkerComponent.capsule.height * 0.5f, moveCamYaw, moveCamPitch);
			toggleState = false;
		}
	}

	private void CancelPlayerActions()
	{
		WeaponBehavior currentWeaponBehaviorComponent = PlayerWeaponsComponent.CurrentWeaponBehaviorComponent;
		PlayerWeaponsComponent.StopAllCoroutines();
		currentWeaponBehaviorComponent.StopAllCoroutines();
		FPSWalkerComponent.jumping = false;
		FPSWalkerComponent.landState = false;
		FPSWalkerComponent.jumpfxstate = true;
		FPSWalkerComponent.crouched = false;
		FPSWalkerComponent.crouchState = false;
		FPSWalkerComponent.crouchRisen = true;
		FPSWalkerComponent.prone = false;
		FPSWalkerComponent.proneState = false;
		FPSWalkerComponent.proneRisen = true;
		currentWeaponBehaviorComponent.gunAnglesTarget = Vector3.zero;
		if (PlayerWeaponsComponent.currentWeapon == PlayerWeaponsComponent.grenadeWeapon && !currentWeaponBehaviorComponent.cycleSelect)
		{
			PlayerWeaponsComponent.grenadeThrownState = true;
		}
		else
		{
			PlayerWeaponsComponent.grenadeThrownState = false;
		}
		PlayerWeaponsComponent.offhandThrowActive = false;
		PlayerWeaponsComponent.pullGrenadeState = false;
		currentWeaponBehaviorComponent.pullAnimState = false;
		currentWeaponBehaviorComponent.fireOnReleaseState = false;
		currentWeaponBehaviorComponent.doReleaseFire = false;
		currentWeaponBehaviorComponent.releaseAnimState = false;
		currentWeaponBehaviorComponent.fireHoldTimer = 0f;
		IronsightsComponent.switchMove = 0f;
		IronsightsComponent.reloading = false;
		FPSPlayerComponent.zoomed = false;
		IronsightsComponent.newFov = IronsightsComponent.defaultFov;
		IronsightsComponent.nextFov = IronsightsComponent.defaultFov;
		FPSWalkerComponent.cancelSprint = true;
		FPSWalkerComponent.sprintActive = false;
		FPSWalkerComponent.jumping = false;
		FPSWalkerComponent.landState = false;
		FPSWalkerComponent.jumpfxstate = true;
		PlayerWeaponsComponent.cameraToggleState = true;
		PlayerWeaponsComponent.cameraToggleTime = Time.time;
		PlayerCharacterComponent.lastWeapon = -1;
		PlayerCharacterComponent.fireTime = -16f;
		PlayerCharacterComponent.reloadStartTime = -16f;
		currentWeaponBehaviorComponent.CancelWeaponPull();
		IronsightsComponent.zPosRecNext = 0f;
		IronsightsComponent.zPosRec = 0f;
		currentWeaponBehaviorComponent.fireHoldMult = 0f;
		PlayerCharacterComponent.SelectCurrentWeapon();
	}

	private void TogglePlayerCharacterVisibility()
	{
		if (!playerModelToggleState)
		{
			MeshRenderer[] allMeshRenderers = PlayerCharacterComponent.AllMeshRenderers;
			foreach (MeshRenderer meshRenderer in allMeshRenderers)
			{
				meshRenderer.gameObject.SetActive(false);
			}
			SkinnedMeshRenderer[] allSkinnedMeshes = PlayerCharacterComponent.AllSkinnedMeshes;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in allSkinnedMeshes)
			{
				skinnedMeshRenderer.enabled = false;
			}
			playerModelToggleState = true;
		}
		else
		{
			MeshRenderer[] allMeshRenderers2 = PlayerCharacterComponent.AllMeshRenderers;
			foreach (MeshRenderer meshRenderer2 in allMeshRenderers2)
			{
				meshRenderer2.gameObject.SetActive(true);
			}
			SkinnedMeshRenderer[] allSkinnedMeshes2 = PlayerCharacterComponent.AllSkinnedMeshes;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer2 in allSkinnedMeshes2)
			{
				skinnedMeshRenderer2.enabled = true;
			}
			playerModelToggleState = false;
		}
	}
}
