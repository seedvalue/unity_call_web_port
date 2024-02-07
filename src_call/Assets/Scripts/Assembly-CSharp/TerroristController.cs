using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TerroristController : MonoBehaviour
{
	public enum Enemies
	{
		Select = 0,
		Chief = 1,
		Commandor = 2,
		Soldier = 3,
		Ranger = 4,
		Roof = 5,
		rocketLauncher = 6,
		Tactical = 7,
		CustomCover = 8,
		TacticalExpert = 9
	}

	public enum Behave
	{
		Select = 0,
		STANDING = 1,
		AIMING_RANDOM = 2,
		AIMING_PLAYER = 3,
		Crouching = 4,
		WALKING = 5,
		RUNING = 6,
		AIMING_WALKING = 7,
		AIMING_WALKING_PLAYER = 8,
		AIMING_RUNING = 9,
		AIMING_RUNING_PLAYER = 10,
		StandingRoof = 11,
		AggressiveBeh = 12,
		TacticalShoot = 13,
		TacticalMove = 14,
		TacticalCover = 15,
		TacticalMoveAndFire = 16
	}

	private float PreviousAnim;

	public EnemyLocator HealthLocator;

	public float bulletInaccuracy;

	public int bulletInaccuracyWhileFlashed;

	public float flashTime = 5f;

	private float initialFlashTime;

	public float Health = 100f;

	private float totalhealth;

	public float fireRate = 2f;

	public float AlertTime;

	[Space(10f)]
	public int FireClipSize = 15;

	public string MyName;

	public Renderer CurrentMat;

	public bool PingPong;

	public bool canCrouch;

	public Enemies TypeOfEnemies;

	public Behave CurrentBehave;

	public Transform[] WalkingPoints;

	public Transform[] AlertPoints;

	[Header("\t***Tactical AI Parameters***")]
	[Space(10f)]
	public Transform[] CoverPoints;

	private bool MoveAndFireWaitNext;

	private int FireBurst;

	public int[] AttackDelay;

	public int[] CoverDelay;

	public int[] FireTime;

	[Header("On Start Go to First Cover then Hide_Fire will effect this behaviour")]
	public bool OnStartGoCover;

	public bool HideFireAfterCover;

	public bool AttackCoverThenAttack;

	[Header("Integer value 0 for next cover & value 1 for hide & fire use attack delay")]
	public int[] Hide_And_Fire;

	[Space(10f)]
	[Header("\tUp & Down In Cover 0 for nothing & one for down")]
	[Header(" we have situation where cover point height is not enough for hide use 1 in these points")]
	public int[] Up_Down;

	[Header("Running & Firing is it is true")]
	public bool isFireWhileRunning;

	[Header("Go to attack then Cover Hide if it is true")]
	public bool AttackToCover_Hide;

	[Header("Define Cover In & Out 0 for left Cover 1 for Right Cover")]
	public bool isCoverInOut;

	public int[] CoverInOut;

	private bool WaitForCoverDelay;

	private bool WaitForAttackDelay;

	[Header("\t***Tactical Expert Parameters***")]
	[Space(10f)]
	[Header("\t** Move & Fire  add moving points**")]
	public Transform[] MoveWhileFire;

	private int up_down_index;

	private int coverCount;

	private int Hide_FireCount;

	private int AttackingCount;

	[Space(10f)]
	[Space(10f)]
	[Header("Currently above tactical parameters only used in tactical Enemy Type!")]
	private NavMeshAgent navAgent;

	public Animator Anim;

	public float walkSpeed = 4f;

	public float _walkSpeedAnim = 0.2f;

	public float runSpeed = 10f;

	public float _runSpeedAnim = 0.8f;

	public float[] StandingAnim;

	public bool canTakeRest;

	public float aimingX;

	public float aimingY;

	public GameObject gun;

	public int indexOfStandingAnim;

	public bool randomStanding;

	public bool randomMovement;

	public GameObject player;

	public bool Alert;

	public float alertDelay = 2f;

	public float ThreatTime = 10f;

	public bool isTarget;

	public float AttackDamage = 10f;

	public int cashReward;

	public int score;

	public bool canAlertOther;

	public GameObject bullet;

	public Transform bulletSpawnPoint;

	public AudioClip[] FireSound;

	public AudioClip[] AlertSounds;

	public AudioClip[] DeadSound;

	public AudioClip[] HitSound;

	public AudioClip[] ReloadSound;

	private AudioSource audioSouce;

	private AudioSource audioController;

	public AudioClip[] flashSound;

	private bool GetAlert = true;

	private int index;

	private bool IsPlayerWalking;

	private bool isFlashed;

	private float bulletTime;

	private int waitForSecondPoint = 5;

	private bool canThreatLevel;

	private float smoothDelta;

	private bool AlertDelayTime = true;

	private bool DontGoToAtack;

	private float valueAimX;

	private float valueAimY;

	private int walkingIndex;

	private int MoveWhileFireIndex;

	private int runningIndex;

	private int CurrentCoverPoint;

	private bool isGoToCover;

	private bool dead;

	public GameObject deadReplacement;

	private GameObject deadRagdoll;

	private RemoveBody RemoveBodyComponent;

	private Vector3 attackerPos2;

	private Vector3 attackDir2;

	private Transform myTransform;

	private LayerMask raymask = 0;

	private void Awake()
	{
		initialFlashTime = flashTime;
		myTransform = base.transform;
		totalhealth = Health;
		RemoveBodyComponent = GetComponent<RemoveBody>();
		RemoveBodyComponent.enabled = false;
		HealthLocator = GetComponent<EnemyLocator>();
		audioController = GetComponent<AudioSource>();
		audioSouce = base.gameObject.AddComponent<AudioSource>();
		FireBurst = FireClipSize;
		Anim = GetComponentInChildren<Animator>();
		if (Anim == null)
		{
			Anim = GetComponent<Animator>();
		}
		Anim.SetFloat("InputMagnitude", 0f);
		navAgent = GetComponent<NavMeshAgent>();
	}

	private void Start()
	{
		if (player == null)
		{
			player = GameObject.FindGameObjectWithTag("target");
		}
		Invoke("setup", 1f);
	}

	private void setup()
	{
		HealthLocator.updateBarValue(1f);
		canAlertOther = true;
		switch (TypeOfEnemies)
		{
		case Enemies.Chief:
			MyName = "Chiefs";
			break;
		case Enemies.Commandor:
			MyName = "Commandors";
			break;
		case Enemies.Ranger:
			MyName = "Rangers";
			break;
		case Enemies.Soldier:
			MyName = "Soldiers";
			break;
		case Enemies.Roof:
			MyName = "RoofTarget";
			break;
		case Enemies.rocketLauncher:
			MyName = "rocketLauncher";
			break;
		case Enemies.Tactical:
			MyName = "Tactical";
			break;
		case Enemies.TacticalExpert:
			MyName = "TacticalExpert";
			break;
		}
		switch (CurrentBehave)
		{
		case Behave.STANDING:
			standing();
			break;
		case Behave.AIMING_RANDOM:
			aiming();
			break;
		case Behave.WALKING:
			StartCoroutine(_roaming());
			break;
		case Behave.RUNING:
			runing("Alert");
			break;
		case Behave.AIMING_WALKING:
			aiming();
			walking();
			break;
		case Behave.AIMING_RUNING:
			aiming();
			runing("Alert");
			break;
		case Behave.AIMING_WALKING_PLAYER:
			aimingToPlayer();
			walking();
			break;
		case Behave.AIMING_RUNING_PLAYER:
			aimingToPlayer();
			runing("Alert");
			break;
		case Behave.StandingRoof:
			aiming();
			break;
		case Behave.AIMING_PLAYER:
		case Behave.Crouching:
			break;
		}
	}

	public void MakeEnemyFlash()
	{
		audioSouce.PlayOneShot(flashSound[Random.Range(0, flashSound.Length)]);
		Anim.SetBool("isFlash", true);
		isFlashed = true;
	}

	private void Update()
	{
		if (Anim.GetBool("IsDead"))
		{
			return;
		}
		if (isFlashed)
		{
			flashTime -= Time.deltaTime;
			if (flashTime <= 0f)
			{
				isFlashed = false;
				Anim.SetBool("isFlash", false);
				flashTime = initialFlashTime;
			}
		}
		if (Anim.GetBool("IsShoot"))
		{
			bulletTime += Time.deltaTime;
			if (bulletTime > fireRate)
			{
				bulletTime = 0f;
				FireBurst--;
				Quaternion quaternion = Quaternion.LookRotation(player.transform.position - bulletSpawnPoint.transform.position);
				GameObject gameObject = Object.Instantiate(bullet, bulletSpawnPoint.position, Quaternion.identity);
				gameObject.transform.LookAt(player.transform.position);
				gameObject.transform.rotation *= Quaternion.Euler(Random.Range(0f - bulletInaccuracy, bulletInaccuracy), Random.Range(0f - bulletInaccuracy, 0f - bulletInaccuracy), 0f);
				if (isFlashed)
				{
					gameObject.transform.position = new Vector3(gameObject.transform.position.x + (float)Random.Range(-bulletInaccuracyWhileFlashed, bulletInaccuracyWhileFlashed), gameObject.transform.position.y + (float)Random.Range(-bulletInaccuracyWhileFlashed, bulletInaccuracyWhileFlashed) + gameObject.transform.position.z + (float)Random.Range(-bulletInaccuracyWhileFlashed, bulletInaccuracyWhileFlashed));
				}
				if (index < FireSound.Length)
				{
					audioController.PlayOneShot(FireSound[index]);
					index++;
					if (index > FireSound.Length - 1)
					{
						index = 0;
					}
				}
			}
		}
		if (Input.GetKeyUp(KeyCode.A))
		{
			getAlert();
		}
		switch (CurrentBehave)
		{
		case Behave.AIMING_PLAYER:
			if (navAgent.remainingDistance <= 0.5f)
			{
				navAgent.Stop();
				stoped();
				aimingToPlayer();
			}
			else
			{
				Anim.SetBool("IsShoot", false);
			}
			break;
		case Behave.AggressiveBeh:
			if (navAgent.remainingDistance <= 0.5f)
			{
				navAgent.Stop();
				stoped();
			}
			aimingToPlayer();
			break;
		case Behave.TacticalShoot:
			if (navAgent.remainingDistance <= 0.5f)
			{
				navAgent.Stop();
				stoped();
				MoveAndFireWaitNext = false;
				WaitForAttackDelay = false;
			}
			if (!WaitForAttackDelay)
			{
				aimingToPlayer();
			}
			break;
		case Behave.TacticalMove:
			Anim.SetBool("IsShoot", false);
			if (navAgent.remainingDistance <= 0.5f)
			{
				WaitForCoverDelay = false;
				navAgent.Stop();
				stoped();
			}
			break;
		case Behave.TacticalMoveAndFire:
			if (navAgent.remainingDistance <= 0.5f)
			{
				navAgent.Stop();
				stoped();
				MoveAndFireWaitNext = true;
				WaitForAttackDelay = false;
			}
			aimingToPlayer();
			break;
		case Behave.WALKING:
			if (navAgent.remainingDistance <= 0.5f)
			{
				navAgent.Stop();
				stoped();
				IsPlayerWalking = false;
			}
			break;
		case Behave.STANDING:
			break;
		case Behave.StandingRoof:
			Standing_Roof_Aiming();
			break;
		case Behave.AIMING_RANDOM:
			standing();
			break;
		case Behave.Crouching:
			StandingAimingToPlayer();
			break;
		case Behave.AIMING_WALKING:
			break;
		case Behave.AIMING_RUNING:
			break;
		case Behave.AIMING_WALKING_PLAYER:
			break;
		case Behave.AIMING_RUNING_PLAYER:
			break;
		case Behave.RUNING:
		case Behave.TacticalCover:
			break;
		}
	}

	private void AlerfSoundWithDelay()
	{
		audioSouce.PlayOneShot(AlertSounds[Random.Range(0, AlertSounds.Length)]);
	}

	public void getAlert()
	{
		if (canAlertOther)
		{
			CancelInvoke("AlerfSoundWithDelay");
			Invoke("AlerfSoundWithDelay", Random.Range(0f, 3f));
			StopAllCoroutines();
			canAlertOther = false;
			if (MyName.Equals("Chiefs"))
			{
				StartCoroutine(_reload_and_shoot());
			}
			else if (MyName.Equals("Soldiers"))
			{
				StartCoroutine(_roaming_standing_shooting());
			}
			else if (MyName.Equals("Rangers"))
			{
				StartCoroutine(_standing_shooting());
			}
			else if (MyName.Equals("RoofTarget"))
			{
				StartCoroutine(_standing_shooting());
			}
			else if (MyName.Equals("rocketLauncher"))
			{
				StartCoroutine(RocketLauncherBehaviour());
			}
			else if (MyName.Equals("Commandors"))
			{
				StartCoroutine(AggressiveBehaviour());
			}
			else if (MyName.Equals("Tactical"))
			{
				StartCoroutine(TacticalEnemyBehaviour());
			}
			else if (MyName.Equals("TacticalExpert"))
			{
				StartCoroutine(TacticalExpertStatic());
			}
		}
	}

	private IEnumerator _roaming()
	{
		if (Anim.GetBool("IsDead"))
		{
			StopCoroutine(_roaming());
		}
		IsPlayerWalking = true;
		walking();
		while (IsPlayerWalking)
		{
			yield return new WaitForSeconds(0.2f);
		}
		yield return new WaitForSeconds(Random.Range(3, 5));
		StartCoroutine(_roaming());
	}

	private IEnumerator _standing_shooting()
	{
		if (Anim.GetBool("IsDead"))
		{
			StopCoroutine(_standing_shooting());
		}
		Anim.SetBool("IsShoot", false);
		stoped();
		Anim.SetBool("isCrouch", false);
		yield return new WaitForSeconds(0.5f);
		Anim.SetFloat("StandingPose", 0.1f);
		Anim.SetBool("IsShoot", true);
		CurrentBehave = Behave.StandingRoof;
		yield return new WaitForSeconds(Random.Range(4, 6));
		if (!audioSouce.isPlaying)
		{
			audioSouce.PlayOneShot(ReloadSound[Random.Range(0, ReloadSound.Length)]);
		}
		Anim.SetBool("IsShoot", false);
		Anim.SetTrigger("ReloadTrg");
		yield return new WaitForSeconds(1f);
		Anim.SetBool("isCrouch", true);
		yield return new WaitForSeconds(Random.Range(0, 5));
		StartCoroutine(_standing_shooting());
	}

	private IEnumerator RocketLauncherBehaviour()
	{
		if (Anim.GetBool("IsDead"))
		{
			StopCoroutine(RocketLauncherBehaviour());
		}
		CurrentBehave = Behave.STANDING;
		yield return new WaitForSeconds(1f);
		stoped();
		Anim.SetTrigger("ReloadTrg");
		yield return new WaitForSeconds(3f);
		Anim.SetFloat("StandingPose", 0.5f);
		yield return new WaitForSeconds(0.2f);
		yield return new WaitForSeconds(Random.Range(3, 5));
		while (smoothDelta != 0.3f)
		{
			smoothDelta = Mathf.MoveTowards(smoothDelta, 0.3f, 10f * Time.deltaTime);
			Anim.SetFloat("StandingPose", smoothDelta);
			yield return null;
		}
		yield return new WaitForSeconds(Random.Range(7, 12));
		while (smoothDelta != 0.4f)
		{
			smoothDelta = Mathf.MoveTowards(smoothDelta, 0.4f, 10f * Time.deltaTime);
			Anim.SetFloat("StandingPose", smoothDelta);
			yield return null;
		}
		yield return new WaitForSeconds(Random.Range(0.5f, 2f));
		StartCoroutine(RocketLauncherBehaviour());
	}

	private IEnumerator _standing_Roof_shooting()
	{
		if (Anim.GetBool("IsDead"))
		{
			StopCoroutine(_standing_Roof_shooting());
		}
		yield return new WaitForSeconds(Random.Range(0, 5));
		Anim.SetBool("IsShoot", false);
		if (!audioSouce.isPlaying)
		{
			audioSouce.PlayOneShot(ReloadSound[Random.Range(0, ReloadSound.Length)]);
		}
		Anim.SetTrigger("ReloadTrg");
		yield return new WaitForSeconds(1f);
		stoped();
		Anim.SetBool("isCrouch", false);
		yield return new WaitForSeconds(0.5f);
		Anim.SetFloat("StandingPose", 0.1f);
		Anim.SetBool("IsShoot", true);
		CurrentBehave = Behave.STANDING;
		yield return new WaitForSeconds(Random.Range(3, 6));
		StartCoroutine(_standing_Roof_shooting());
	}

	private IEnumerator _roaming_standing_shooting()
	{
		if (Anim.GetBool("IsDead"))
		{
			StopCoroutine(_roaming_standing_shooting());
		}
		if (canCrouch)
		{
			Anim.SetBool("IsShoot", false);
			CurrentBehave = Behave.Select;
			Anim.SetBool("isCrouch", true);
			yield return new WaitForSeconds(Random.Range(4, 7));
			stoped();
			Anim.SetBool("isCrouch", false);
			yield return new WaitForSeconds(0.5f);
			Anim.SetFloat("StandingPose", 0.1f);
			CurrentBehave = Behave.Crouching;
			Anim.SetBool("IsShoot", true);
			yield return new WaitForSeconds(Random.Range(4, 6));
			Anim.SetBool("IsShoot", false);
			CurrentBehave = Behave.Select;
			if (!audioSouce.isPlaying)
			{
				audioSouce.PlayOneShot(ReloadSound[Random.Range(0, ReloadSound.Length)]);
			}
			Anim.SetTrigger("ReloadTrg");
		}
		else
		{
			stoped();
			Anim.SetBool("isCrouch", false);
			yield return new WaitForSeconds(0.5f);
			Anim.SetFloat("StandingPose", 0.1f);
			runing("Alert");
			CurrentBehave = Behave.AIMING_PLAYER;
			yield return new WaitForSeconds(Random.Range(5, 9));
			Anim.SetBool("IsShoot", false);
			CurrentBehave = Behave.Select;
			if (!audioSouce.isPlaying)
			{
				audioSouce.PlayOneShot(ReloadSound[Random.Range(0, ReloadSound.Length)]);
			}
			Anim.SetTrigger("ReloadTrg");
			yield return new WaitForSeconds(1f);
			Anim.SetBool("isCrouch", true);
			yield return new WaitForSeconds(1f);
			Anim.SetBool("isCrouch", false);
			yield return new WaitForSeconds(0.5f);
			Anim.SetFloat("StandingPose", 0.1f);
			runing("Alert");
			CurrentBehave = Behave.AIMING_PLAYER;
			yield return new WaitForSeconds(Random.Range(6, 11));
			Anim.SetBool("IsShoot", false);
			CurrentBehave = Behave.Select;
			if (!audioSouce.isPlaying)
			{
				audioSouce.PlayOneShot(ReloadSound[Random.Range(0, ReloadSound.Length)]);
			}
			Anim.SetTrigger("ReloadTrg");
			yield return new WaitForSeconds(1f);
			Anim.SetBool("isCrouch", true);
			yield return new WaitForSeconds(Random.Range(5, 7));
		}
		StartCoroutine(_roaming_standing_shooting());
	}

	private IEnumerator AggressiveBehaviour()
	{
		if (Anim.GetBool("IsDead"))
		{
			StopCoroutine(AggressiveBehaviour());
		}
		stoped();
		Anim.SetBool("isCrouch", false);
		yield return new WaitForSeconds(0.5f);
		Anim.SetFloat("StandingPose", 0.1f);
		runing("Alert");
		CurrentBehave = Behave.AggressiveBeh;
		Anim.SetBool("IsShoot", true);
		yield return new WaitForSeconds(Random.Range(5, 9));
		if (!audioSouce.isPlaying)
		{
			audioSouce.PlayOneShot(ReloadSound[Random.Range(0, ReloadSound.Length)]);
		}
		Anim.SetTrigger("ReloadTrg");
		yield return new WaitForSeconds(1f);
		Anim.SetBool("isCrouch", true);
		yield return new WaitForSeconds(1f);
		StartCoroutine(AggressiveBehaviour());
	}

	private IEnumerator TacticalExpertStatic()
	{
		if (Anim.GetBool("IsDead"))
		{
			StopCoroutine(TacticalExpertStatic());
		}
		if (!audioSouce.isPlaying)
		{
			audioSouce.PlayOneShot(ReloadSound[Random.Range(0, ReloadSound.Length)]);
		}
		if (AlertDelayTime)
		{
			CurrentBehave = Behave.AIMING_RANDOM;
			AlertDelayTime = false;
			yield return new WaitForSeconds(AlertTime);
		}
		stoped();
		Anim.SetBool("isCrouch", false);
		yield return new WaitForSeconds(0.5f);
		Anim.SetFloat("StandingPose", 0.1f);
		runing("Alert");
		if (!isFireWhileRunning)
		{
			WaitForAttackDelay = true;
			CurrentBehave = Behave.TacticalShoot;
			while (WaitForAttackDelay)
			{
				yield return new WaitForSeconds(0.2f);
			}
		}
		else
		{
			CurrentBehave = Behave.TacticalShoot;
			WaitForAttackDelay = false;
		}
		yield return new WaitForSeconds(Random.Range(FireTime[0], FireTime[1]));
		Anim.SetBool("IsShoot", false);
		StartCoroutine(MoveStaticAndFire());
	}

	private IEnumerator MoveStaticAndFire()
	{
		stoped();
		Anim.SetFloat("Vertical", _walkSpeedAnim);
		Anim.SetFloat("Horizontal", 0f);
		Anim.SetFloat("InputMagnitude", 1f);
		Anim.SetFloat("StandingPose", 0f);
		Anim.SetBool("IsStopRU", false);
		Anim.SetBool("IsStopLU", false);
		navAgent.speed = 2f;
		navAgent.Resume();
		SetNavMoveWhileFire();
		WaitForAttackDelay = true;
		MoveAndFireWaitNext = false;
		CurrentBehave = Behave.TacticalMoveAndFire;
		while (WaitForAttackDelay)
		{
			yield return new WaitForSeconds(0.2f);
		}
		yield return new WaitForSeconds(Random.Range(FireTime[0], FireTime[1]));
		Anim.SetBool("IsShoot", false);
		if (FireBurst <= 0)
		{
			Anim.SetTrigger("ReloadTrg");
			yield return new WaitForSeconds(2f);
			FireBurst = FireClipSize;
		}
		StartCoroutine(MoveStaticAndFire());
	}

	private IEnumerator TacticalEnemyBehaviour()
	{
		if (Anim.GetBool("IsDead"))
		{
			StopCoroutine(TacticalEnemyBehaviour());
		}
		if (AlertDelayTime)
		{
			CurrentBehave = Behave.AIMING_RANDOM;
			AlertDelayTime = false;
			yield return new WaitForSeconds(AlertTime);
		}
		if (!audioSouce.isPlaying)
		{
			audioSouce.PlayOneShot(ReloadSound[Random.Range(0, ReloadSound.Length)]);
		}
		stoped();
		if (OnStartGoCover)
		{
			DontGoToAtack = true;
			runing("Cover");
		}
		else if (!DontGoToAtack)
		{
			runing("Alert");
		}
		if (AttackingCount >= AlertPoints.Length && !PingPong)
		{
			CurrentBehave = Behave.Select;
			Anim.SetBool("IsShoot", false);
			yield return new WaitForSeconds(Random.Range(AttackDelay[0], AttackDelay[1]));
		}
		AttackingCount++;
		if (!isFireWhileRunning)
		{
			if (!OnStartGoCover)
			{
				WaitForAttackDelay = true;
				CurrentBehave = Behave.TacticalShoot;
				while (WaitForAttackDelay)
				{
					yield return new WaitForSeconds(0.2f);
				}
			}
			else
			{
				CurrentBehave = Behave.TacticalMove;
				WaitForCoverDelay = true;
				while (WaitForCoverDelay)
				{
					yield return new WaitForSeconds(0.2f);
				}
			}
		}
		else if (!OnStartGoCover)
		{
			CurrentBehave = Behave.TacticalShoot;
			WaitForAttackDelay = false;
		}
		if (OnStartGoCover)
		{
			Anim.SetBool("isCrouch", true);
			OnStartGoCover = false;
			yield return new WaitForSeconds(Random.Range(CoverDelay[0], CoverDelay[1]));
		}
		else
		{
			Anim.SetBool("isCrouch", false);
			yield return new WaitForSeconds(0.5f);
			Anim.SetFloat("StandingPose", 0.1f);
			yield return new WaitForSeconds(Random.Range(FireTime[0], FireTime[1]));
		}
		Anim.SetBool("IsShoot", false);
		CurrentBehave = Behave.Select;
		bool isAttackDelay = false;
		if (isCoverInOut && !AttackToCover_Hide)
		{
			StopAllCoroutines();
			StartCoroutine(TacticalCoverInOut());
			yield break;
		}
		if (!audioSouce.isPlaying)
		{
			audioSouce.PlayOneShot(ReloadSound[Random.Range(0, ReloadSound.Length)]);
		}
		if (FireBurst <= 0)
		{
			Anim.SetTrigger("ReloadTrg");
			yield return new WaitForSeconds(2f);
			FireBurst = FireClipSize;
		}
		int tempHide_Fire = Hide_And_Fire[Hide_FireCount];
		if (tempHide_Fire == 1)
		{
			CurrentBehave = Behave.Select;
			Anim.SetBool("isCrouch", true);
			yield return new WaitForSeconds(Random.Range(AttackDelay[0], AttackDelay[1]));
			CurrentBehave = Behave.TacticalShoot;
			Anim.SetBool("isCrouch", false);
			yield return new WaitForSeconds(0.5f);
			Anim.SetFloat("StandingPose", 0.1f);
			WaitForAttackDelay = false;
			yield return new WaitForSeconds(Random.Range(FireTime[0], FireTime[1]));
			Anim.SetBool("IsShoot", false);
			isAttackDelay = true;
			CurrentBehave = Behave.Select;
		}
		else
		{
			runing("Cover");
			if (HideFireAfterCover)
			{
				DontGoToAtack = true;
			}
			CurrentBehave = Behave.TacticalMove;
			Anim.SetBool("IsShoot", false);
			WaitForCoverDelay = true;
			while (WaitForCoverDelay)
			{
				yield return new WaitForSeconds(0.2f);
			}
			if (Up_Down[up_down_index] == 1)
			{
				Anim.SetBool("isCrouch", true);
			}
			up_down_index++;
			if (up_down_index >= Up_Down.Length)
			{
				up_down_index = 0;
			}
			if (AttackToCover_Hide)
			{
				StopAllCoroutines();
				StartCoroutine(TacticalCoverInOut());
			}
			if (isGoToCover)
			{
				yield return new WaitForSeconds(Random.Range(CoverDelay[0], CoverDelay[1]));
			}
			if (AttackCoverThenAttack)
			{
				StopAllCoroutines();
				StartCoroutine(CoverThenAlertAndAttack());
			}
		}
		Hide_FireCount++;
		if (Hide_FireCount >= Hide_And_Fire.Length)
		{
			Hide_FireCount = 0;
		}
		if (FireBurst <= 0)
		{
			Anim.SetTrigger("ReloadTrg");
			yield return new WaitForSeconds(1f);
			FireBurst = FireClipSize;
		}
		Anim.SetBool("isCrouch", true);
		if (isAttackDelay)
		{
			yield return new WaitForSeconds(Random.Range(AttackDelay[0], AttackDelay[1]));
		}
		StartCoroutine(TacticalEnemyBehaviour());
	}

	private IEnumerator CoverThenAlertAndAttack()
	{
		if (Anim.GetBool("IsDead"))
		{
			StopCoroutine(CoverThenAlertAndAttack());
		}
		stoped();
		navAgent.SetDestination(AlertPoints[runningIndex].position);
		RunningAnim();
		if (!isFireWhileRunning)
		{
			WaitForAttackDelay = true;
			CurrentBehave = Behave.TacticalShoot;
			while (WaitForAttackDelay)
			{
				yield return new WaitForSeconds(0.2f);
			}
		}
		else
		{
			CurrentBehave = Behave.TacticalShoot;
			WaitForAttackDelay = false;
		}
		Anim.SetBool("isCrouch", false);
		yield return new WaitForSeconds(0.5f);
		Anim.SetFloat("StandingPose", 0.1f);
		yield return new WaitForSeconds(Random.Range(FireTime[0], FireTime[1]));
		Anim.SetBool("IsShoot", false);
		if (!audioSouce.isPlaying)
		{
			audioSouce.PlayOneShot(ReloadSound[Random.Range(0, ReloadSound.Length)]);
		}
		if (FireBurst <= 0)
		{
			Anim.SetTrigger("ReloadTrg");
			yield return new WaitForSeconds(2f);
			FireBurst = FireClipSize;
		}
		if (CoverPoints != null && CoverPoints.Length != 0)
		{
			navAgent.SetDestination(CoverPoints[CurrentCoverPoint].position);
		}
		RunningAnim();
		CurrentBehave = Behave.TacticalMove;
		Anim.SetBool("IsShoot", false);
		WaitForCoverDelay = true;
		while (WaitForCoverDelay)
		{
			yield return new WaitForSeconds(0.2f);
		}
		if (Up_Down[up_down_index] == 1)
		{
			Anim.SetBool("isCrouch", true);
		}
		up_down_index++;
		if (up_down_index >= Up_Down.Length)
		{
			up_down_index = 0;
		}
		yield return new WaitForSeconds(Random.Range(CoverDelay[0], CoverDelay[1]));
		StartCoroutine(CoverThenAlertAndAttack());
	}

	private IEnumerator TacticalCoverInOut()
	{
		if (Anim.GetBool("IsDead"))
		{
			StopCoroutine(TacticalEnemyBehaviour());
		}
		CurrentBehave = Behave.TacticalCover;
		int tempCoverValue = CoverInOut[coverCount];
		if (tempCoverValue == 0)
		{
			Anim.SetBool("isCoverLeft", true);
		}
		else
		{
			Anim.SetBool("isCoverRight", true);
		}
		yield return new WaitForSeconds(Random.Range(CoverDelay[0], CoverDelay[1]));
		if (tempCoverValue == 0)
		{
			Anim.SetBool("isCoverLeft", false);
		}
		else
		{
			Anim.SetBool("isCoverRight", false);
		}
		coverCount++;
		if (coverCount >= CoverInOut.Length)
		{
			coverCount = 0;
		}
		yield return new WaitForSeconds(1f);
		CurrentBehave = Behave.TacticalShoot;
		Anim.SetBool("isCrouch", false);
		yield return new WaitForSeconds(0.5f);
		Anim.SetFloat("StandingPose", 0.1f);
		yield return new WaitForSeconds(Random.Range(FireTime[0], FireTime[1]));
		Anim.SetBool("IsShoot", false);
		if (!audioSouce.isPlaying)
		{
			audioSouce.PlayOneShot(ReloadSound[Random.Range(0, ReloadSound.Length)]);
		}
		if (FireBurst <= 0)
		{
			Anim.SetTrigger("ReloadTrg");
			yield return new WaitForSeconds(2f);
			FireBurst = FireClipSize;
		}
		StartCoroutine(TacticalCoverInOut());
	}

	private IEnumerator _reload_and_shoot()
	{
		if (Anim.GetBool("IsDead"))
		{
			StopCoroutine(_reload_and_shoot());
		}
		CurrentBehave = Behave.Select;
		stoped();
		Anim.SetBool("isCrouch", false);
		yield return new WaitForSeconds(0.5f);
		Anim.SetFloat("StandingPose", 0.1f);
		CurrentBehave = Behave.AIMING_PLAYER;
		runing("Alert");
		yield return new WaitForSeconds(Random.Range(4, 7));
		defaultAnim();
		Anim.SetBool("IsShoot", false);
		CurrentBehave = Behave.Select;
		audioSouce.PlayOneShot(ReloadSound[Random.Range(0, ReloadSound.Length)]);
		Anim.SetTrigger("ReloadTrg");
		yield return new WaitForSeconds(1f);
		Anim.SetBool("isCrouch", true);
		yield return new WaitForSeconds(Random.Range(2, 5));
		stoped();
		Anim.SetBool("isCrouch", false);
		yield return new WaitForSeconds(0.5f);
		Anim.SetFloat("StandingPose", 0.1f);
		runing("Alert");
		CurrentBehave = Behave.AIMING_PLAYER;
		yield return new WaitForSeconds(Random.Range(3, 6));
		Anim.SetBool("IsShoot", false);
		CurrentBehave = Behave.Select;
		if (!audioSouce.isPlaying)
		{
			audioSouce.PlayOneShot(ReloadSound[Random.Range(0, ReloadSound.Length)]);
		}
		Anim.SetTrigger("ReloadTrg");
		yield return new WaitForSeconds(1f);
		Anim.SetBool("isCrouch", true);
		yield return new WaitForSeconds(Random.Range(2, 4));
		StartCoroutine(_reload_and_shoot());
	}

	public void aiming()
	{
		if (!Anim.GetBool("IsDead"))
		{
			valueAimX = Mathf.Sin(Time.time) * aimingX;
			valueAimY = (Mathf.Sin(Time.time) + 1f) / 2f * aimingY;
			Anim.SetFloat("VerAimAngle", valueAimY);
			Anim.SetFloat("HorAimAngle", valueAimX);
		}
	}

	public void aimingToPlayer()
	{
		if (Anim.GetBool("IsDead"))
		{
			return;
		}
		Vector3 normalized = (player.transform.position - base.transform.position).normalized;
		Quaternion b = Quaternion.LookRotation(normalized);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * 10f);
		if (Anim.GetCurrentAnimatorStateInfo(3).IsName("Rifle_Reload_2") && Anim.GetCurrentAnimatorStateInfo(3).normalizedTime > 0f)
		{
			return;
		}
		if (CurrentBehave.Equals(Behave.TacticalMoveAndFire))
		{
			if (MoveAndFireWaitNext)
			{
				Anim.SetBool("IsShoot", true);
			}
		}
		else
		{
			Anim.SetBool("IsShoot", true);
		}
	}

	public void StandingAimingToPlayer()
	{
		if (!Anim.GetBool("IsDead"))
		{
			Vector3 normalized = (player.transform.position - base.transform.position).normalized;
			Anim.SetFloat("VerAimAngle", Vector3.Angle(base.transform.position, player.transform.position));
			Anim.SetFloat("HorAimAngle", Vector3.Angle(base.transform.position, player.transform.position));
			normalized.y = 0f;
			Quaternion b = Quaternion.LookRotation(normalized);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * 10f);
		}
	}

	public void Standing_Roof_Aiming()
	{
		if (!Anim.GetBool("IsDead"))
		{
			Vector3 normalized = (player.transform.position - base.transform.position).normalized;
			Anim.SetFloat("VerAimAngle", Vector3.Angle(base.transform.position, player.transform.position));
			Anim.SetFloat("HorAimAngle", Vector3.Angle(base.transform.position, player.transform.position));
			normalized.y = 0f;
			Quaternion b = Quaternion.LookRotation(normalized);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * 10f);
		}
	}

	public void defaultAnim()
	{
		if (!Anim.GetBool("IsDead"))
		{
			navAgent.Stop();
			Anim.SetFloat("VerAimAngle", Mathf.Lerp(valueAimY, 0f, Time.time));
			Anim.SetFloat("HorAimAngle", Mathf.Lerp(valueAimX, 0f, Time.time));
			Anim.SetFloat("Vertical", 0f);
			Anim.SetFloat("Horizontal", 0f);
			Anim.SetFloat("InputMagnitude", 0f);
			Anim.SetBool("IsShoot", false);
			Anim.SetFloat("StandingPose", 0f);
			Anim.SetBool("IsReload", false);
		}
	}

	public void stoped()
	{
		if (!Anim.GetBool("IsDead"))
		{
			navAgent.Stop();
			Anim.SetFloat("VerAimAngle", Mathf.Lerp(valueAimY, 0f, Time.time));
			Anim.SetFloat("HorAimAngle", Mathf.Lerp(valueAimX, 0f, Time.time));
			Anim.SetFloat("Vertical", 0f);
			Anim.SetFloat("Horizontal", 0f);
			Anim.SetFloat("InputMagnitude", 0.5f);
			Anim.SetBool("IsShoot", false);
			Anim.SetBool("IsStopRU", true);
			Anim.SetBool("IsStopLU", true);
		}
	}

	public void standing()
	{
		if (!Anim.GetBool("IsDead"))
		{
			navAgent.Stop();
			Anim.SetFloat("VerAimAngle", Mathf.Lerp(valueAimY, 0f, Time.time));
			Anim.SetFloat("HorAimAngle", Mathf.Lerp(valueAimX, 0f, Time.time));
			Anim.SetFloat("Vertical", 0f);
			Anim.SetFloat("Horizontal", 0f);
			Anim.SetFloat("InputMagnitude", 0.5f);
			Anim.SetBool("IsShoot", false);
			Anim.SetBool("IsStopRU", true);
			Anim.SetBool("IsStopLU", true);
			if (randomStanding)
			{
				Anim.SetFloat("StandingPose", StandingAnim[Random.Range(0, StandingAnim.Length)]);
			}
			else
			{
				Anim.SetFloat("StandingPose", StandingAnim[indexOfStandingAnim]);
			}
		}
	}

	public void runing(string Name)
	{
		if (!Anim.GetBool("IsDead"))
		{
			if (Name.Equals("Cover"))
			{
				SetAgentCoverPoint();
			}
			else if (Name.Equals("Alert"))
			{
				SetAgentAlertPoint();
			}
		}
	}

	private void RunningAnim()
	{
		Anim.SetFloat("StandingPose", 0.1f);
		Anim.SetFloat("Vertical", _runSpeedAnim);
		Anim.SetFloat("Horizontal", 0f);
		Anim.SetFloat("InputMagnitude", 1f);
		Anim.SetBool("IsStopRU", false);
		Anim.SetBool("IsStopLU", false);
		navAgent.Resume();
		navAgent.speed = runSpeed;
		Anim.SetBool("isCrouch", false);
	}

	public void walking()
	{
		if (!Anim.GetBool("IsDead"))
		{
			Anim.SetFloat("Vertical", _walkSpeedAnim);
			Anim.SetFloat("Horizontal", 0f);
			Anim.SetFloat("InputMagnitude", 1f);
			Anim.SetFloat("StandingPose", 0f);
			Anim.SetBool("IsStopRU", false);
			Anim.SetBool("IsStopLU", false);
			Anim.SetBool("isCrouch", false);
			navAgent.Resume();
			SetNavMeshAgentWalkingPoint();
			navAgent.speed = walkSpeed;
		}
	}

	public void SetNavMeshAgentWalkingPoint()
	{
		if (WalkingPoints != null && WalkingPoints.Length != 0 && WalkingPoints != null && navAgent.destination != WalkingPoints[walkingIndex].position)
		{
			navAgent.SetDestination(WalkingPoints[walkingIndex].position);
			walkingIndex++;
			if (walkingIndex >= WalkingPoints.Length)
			{
				walkingIndex = 0;
			}
		}
	}

	public void SetNavMoveWhileFire()
	{
		if (MoveWhileFire != null && MoveWhileFire.Length != 0 && MoveWhileFire != null && navAgent.destination != MoveWhileFire[MoveWhileFireIndex].position)
		{
			navAgent.SetDestination(MoveWhileFire[MoveWhileFireIndex].position);
			MoveWhileFireIndex++;
			if (MoveWhileFireIndex >= MoveWhileFire.Length)
			{
				MoveWhileFireIndex = 0;
			}
		}
	}

	public void SetAgentCoverPoint()
	{
		if (CoverPoints == null || CoverPoints.Length == 0 || !(navAgent.destination != CoverPoints[CurrentCoverPoint].position))
		{
			return;
		}
		navAgent.SetDestination(CoverPoints[CurrentCoverPoint].position);
		RunningAnim();
		isGoToCover = true;
		if (CurrentCoverPoint <= CoverPoints.Length && PingPong)
		{
			CurrentCoverPoint++;
			if (CurrentCoverPoint >= CoverPoints.Length)
			{
				CurrentCoverPoint = 0;
			}
		}
		else
		{
			CurrentCoverPoint++;
			if (CurrentCoverPoint >= CoverPoints.Length)
			{
				CurrentCoverPoint--;
			}
		}
	}

	public void SetAgentAlertPoint()
	{
		if (AlertPoints.Length == 0 || AlertPoints == null || canCrouch || !(navAgent.destination != AlertPoints[runningIndex].position))
		{
			return;
		}
		navAgent.SetDestination(AlertPoints[runningIndex].position);
		RunningAnim();
		if (runningIndex <= AlertPoints.Length && PingPong)
		{
			runningIndex++;
			if (runningIndex >= AlertPoints.Length)
			{
				runningIndex = 0;
			}
			return;
		}
		runningIndex++;
		if (runningIndex >= AlertPoints.Length)
		{
			runningIndex--;
			canCrouch = true;
		}
	}

	public void damageHit(float damage, Vector3 attackDir, Vector3 attackerPos, Transform attacker, bool isPlayer, bool isExplosion, bool isHeadShot)
	{
		GetAlert = false;
		Anim.SetTrigger("HitTrg");
		audioSouce.PlayOneShot(HitSound[Random.Range(0, HitSound.Length)]);
		Health -= damage;
		float value = Health / totalhealth;
		HealthLocator.updateBarValue(value);
		if (canAlertOther)
		{
			TerroristController[] array = Object.FindObjectsOfType<TerroristController>();
			TerroristController[] array2 = array;
			foreach (TerroristController terroristController in array2)
			{
				terroristController.getAlert();
			}
		}
		if (Health <= 0f && !dead)
		{
			Health = 0f;
			HealthLocator.DestroyHealthBar();
			dead = true;
			attackDir2 = attackDir;
			attackerPos2 = attackerPos;
			if ((bool)LevelController.instance)
			{
				LevelController.instance.EnemyKilled(base.transform);
			}
			if (isHeadShot && (bool)GameController.instance)
			{
				GameController.instance.AddHeadShot();
			}
			Dead();
		}
	}

	public void Dead()
	{
		defaultAnim();
		audioSouce.PlayOneShot(DeadSound[Random.Range(0, DeadSound.Length)]);
		StopAllCoroutines();
		RagdollInstantiate();
	}

	private void RagdollInstantiate()
	{
		if (!deadReplacement)
		{
			return;
		}
		deadRagdoll = Object.Instantiate(deadReplacement, base.transform.position, base.transform.rotation);
		RemoveBodyComponent = deadRagdoll.GetComponent<RemoveBody>();
		if (CurrentMat != null)
		{
			deadRagdoll.GetComponent<terroristMaterial>().MaterialChange(CurrentMat.material);
		}
		CopyTransformsRecurse(base.transform, deadRagdoll.transform);
		RaycastHit hitInfo;
		if (Physics.SphereCast(attackerPos2, 0.2f, attackDir2, out hitInfo, 750f, raymask) && (bool)hitInfo.rigidbody && attackDir2.x != 0f)
		{
			hitInfo.rigidbody.AddForce(attackDir2 * 50f, ForceMode.Impulse);
		}
		else
		{
			Component[] componentsInChildren = deadRagdoll.GetComponentsInChildren<Rigidbody>();
			Component[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Rigidbody rigidbody = (Rigidbody)array[i];
				rigidbody.AddForce((myTransform.position - (attackerPos2 + Vector3.up * -2.5f)).normalized * Random.Range(5f, 10f), ForceMode.Impulse);
			}
		}
		if ((bool)RemoveBodyComponent)
		{
			RemoveBodyComponent.enabled = true;
		}
		Object.Destroy(base.transform.gameObject);
	}

	public void CopyTransformsRecurse(Transform src, Transform dst)
	{
		dst.position = src.position;
		dst.rotation = src.rotation;
		if (src.gameObject.activeSelf)
		{
			dst.gameObject.SetActive(true);
		}
		foreach (Transform item in dst)
		{
			Transform transform2 = src.Find(item.name);
			if ((bool)transform2)
			{
				CopyTransformsRecurse(transform2, item);
			}
		}
	}
}
