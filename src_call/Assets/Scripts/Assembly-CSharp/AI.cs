using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
	[HideInInspector]
	public bool spawned;

	[HideInInspector]
	public GameObject playerObj;

	[HideInInspector]
	public Transform playerTransform;

	[HideInInspector]
	public GameObject NPCMgrObj;

	[HideInInspector]
	public GameObject weaponObj;

	[HideInInspector]
	public FPSRigidBodyWalker FPSWalker;

	[HideInInspector]
	public NPCAttack NPCAttackComponent;

	[HideInInspector]
	public PlayerWeapons PlayerWeaponsComponent;

	private WeaponBehavior WeaponBehaviorComponent;

	[HideInInspector]
	public CharacterDamage TargetCharacterDamageComponent;

	[HideInInspector]
	public CharacterDamage CharacterDamageComponent;

	[HideInInspector]
	public NPCSpawner NPCSpawnerComponent;

	[HideInInspector]
	public NPCRegistry NPCRegistryComponent;

	[HideInInspector]
	public AI TargetAIComponent;

	[HideInInspector]
	public NavMeshAgent agent;

	[HideInInspector]
	public Collider[] colliders;

	private bool collisionState;

	[HideInInspector]
	public Animation AnimationComponent;

	[HideInInspector]
	public Animator AnimatorComponent;

	private float animSpeed;

	[HideInInspector]
	public bool recycleNpcObjOnDeath;

	[Tooltip("The object with the Animation/Animator component which will be accessed by AI.cs to play NPC animations. If none, this root object will be checked for the Animator/Animations component.")]
	public Transform objectWithAnims;

	[Range(0f, 1f)]
	[Tooltip("Chance between 0 and 1 that NPC will spawn. Used to randomize NPC locations and surprise the player.")]
	public float randomSpawnChance = 1f;

	[Tooltip("Running speed of the NPC.")]
	public float runSpeed = 6f;

	[Tooltip("Walking speed of the NPC.")]
	public float walkSpeed = 1f;

	[Tooltip("Speed of running animation.")]
	public float walkAnimSpeed = 1f;

	[Tooltip("Speed of walking animation.")]
	public float runAnimSpeed = 1f;

	private float speedAmt = 1f;

	private float lastRunTime;

	[Tooltip("NPC yaw angle offset when standing.")]
	public float idleYaw;

	[Tooltip("NPC yaw angle offset when moving.")]
	public float movingYaw;

	private float yawAmt;

	[Tooltip("Sets the alignment of this NPC. 1 = friendly to player and hostile to factions 2 and 3, 2 = hostile to player and factions 1 and 3, 3 = hostile to player and factions 1 and 2.")]
	public int factionNum = 1;

	[Tooltip("If false, NPC will attack any character that attacks it, regardless of faction.")]
	public bool ignoreFriendlyFire;

	[HideInInspector]
	public bool playerAttacked;

	[HideInInspector]
	public float attackedTime;

	[HideInInspector]
	public Transform myTransform;

	private Vector3 upVec;

	[Tooltip("True if NPC will hunt player accross map without needing to detect player first.")]
	public bool huntPlayer;

	[Tooltip("True if NPC should only follow patrol waypoints once.")]
	public bool patrolOnce;

	[Tooltip("True if NPC should walk on patrol, will run on patrol if false.")]
	public bool walkOnPatrol = true;

	private Transform curWayPoint;

	[Tooltip("Drag the parent waypoint object with the WaypointGroup.cs script attached here. If none, NPC will stand watch instead of patrolling.")]
	public WaypointGroup waypointGroup;

	[Tooltip("The number of the waypoint in the waypoint group which should be followed first.")]
	public int firstWaypoint = 1;

	[Tooltip("True if NPC should stand in one place and not patrol waypoint path.")]
	public bool standWatch;

	[Tooltip("True if NPC is following player.")]
	public bool followPlayer;

	[Tooltip("True if NPC can be activated with the use button and start following the player.")]
	public bool followOnUse;

	[Tooltip("True if this NPC wants player to follow them (wont take move orders from player, only from MoveTrigger.cs).")]
	public bool leadPlayer;

	[HideInInspector]
	public bool orderedMove;

	[HideInInspector]
	public bool playerFollow;

	private bool animInit;

	private bool animInitialized;

	private float commandedTime;

	private float talkedTime;

	private bool countBackwards;

	[Tooltip("Minimum distance to destination waypoint that NPC will consider their destination as reached.")]
	public float pickNextDestDist = 2.5f;

	private Vector3 startPosition;

	private float spawnTime;

	[Tooltip("Volume of NPC's vocal sound effects.")]
	public float vocalVol = 0.7f;

	[Tooltip("Sound to play when player commands NPC to stop following.")]
	public AudioClip stayFx1;

	[Tooltip("Sound to play when player commands NPC to stop following.")]
	public AudioClip stayFx2;

	[Tooltip("Sound to play when player commands NPC to start following.")]
	public AudioClip followFx1;

	[Tooltip("Sound to play when player commands NPC to start following.")]
	public AudioClip followFx2;

	[Tooltip("Sound to play when player commands NPC to move to position.")]
	public AudioClip moveToFx1;

	[Tooltip("Sound to play when player commands NPC to move to position.")]
	public AudioClip moveToFx2;

	[Tooltip("Sound to play when NPC has been activated more than joke activate times.")]
	public AudioClip jokeFx;

	[Tooltip("Sound to play when NPC has been activated more than joke activate times.")]
	public AudioClip jokeFx2;

	[Tooltip("Number of consecutive use button presses that activates joke fx.")]
	public int jokeActivate = 33;

	private float jokePlaying;

	private int jokeCount;

	[Tooltip("Sound effects to play when pursuing player.")]
	public AudioClip[] tauntSnds;

	[Tooltip("True if taunt sound shouldn't be played when attacking.")]
	public bool cancelAttackTaunt;

	private float lastTauntTime;

	[Tooltip("Delay between times to check if taunt sound should be played.")]
	public float tauntDelay = 2f;

	[Tooltip("Chance that a taunt sound will play after taunt delay.")]
	[Range(0f, 1f)]
	public float tauntChance = 0.5f;

	[Tooltip("Volume of taunt sound effects.")]
	public float tauntVol = 0.7f;

	[Tooltip("Sound effects to play when NPC discovers player.")]
	public AudioClip[] alertSnds;

	[Tooltip("Volume of alert sound effects.")]
	public float alertVol = 0.7f;

	private bool alertTaunt;

	[HideInInspector]
	public AudioSource vocalFx;

	private AudioSource footstepsFx;

	[Tooltip("Sound effects to play for NPC footsteps.")]
	public AudioClip[] footSteps;

	[Tooltip("Volume of footstep sound effects.")]
	public float footStepVol = 0.5f;

	[Tooltip("Time between footstep sound effects when walking (sync with anim).")]
	public float walkStepTime = 1f;

	[Tooltip("Time between footstep sound effects when running (sync with anim).")]
	public float runStepTime = 1f;

	private float stepInterval;

	private float stepTime;

	[Tooltip("Minimum range to target to start attack.")]
	public float shootRange = 15f;

	[Tooltip("Range that NPC will start chasing target until they are within shoot range.")]
	public float attackRange = 30f;

	[Tooltip("Range that NPC will hear player attacks.")]
	public float listenRange = 30f;

	[Tooltip("Time between shots (longer for burst weapons).")]
	public float shotDuration;

	[Tooltip("Speed of attack animation.")]
	public float shootAnimSpeed = 1f;

	[HideInInspector]
	public float attackRangeAmt = 30f;

	[Tooltip("Percentage to reduce enemy search range if player is crouching.")]
	public float sneakRangeMod = 0.4f;

	private float shootAngle = 3f;

	[Tooltip("Time before atack starts, to allow weapon to be raised before firing.")]
	public float delayShootTime = 0.35f;

	[Tooltip("Random delay between NPC attacks.")]
	public float randShotDelay = 0.75f;

	[Tooltip("Height of rayCast origin which detects targets (can be raised if NPC origin is at their feet).")]
	public float eyeHeight = 0.4f;

	[Tooltip("Draws spheres in editor for position and eye height.")]
	public bool drawDebugGizmos;

	[HideInInspector]
	public Transform target;

	[HideInInspector]
	public bool targetVisible;

	private float lastVisibleTime;

	private Vector3 targetPos;

	[HideInInspector]
	public float targetRadius;

	[HideInInspector]
	public float attackTime = -16f;

	private bool attackFinished = true;

	private bool turning;

	[HideInInspector]
	public bool cancelRotate;

	[HideInInspector]
	public bool followed;

	private float targetDistance;

	private Vector3 targetDirection;

	private RaycastHit[] hits;

	private bool sightBlocked;

	[HideInInspector]
	public bool playerIsBehind;

	[HideInInspector]
	public float targetEyeHeight;

	private bool pursueTarget;

	[HideInInspector]
	public Vector3 lastVisibleTargetPosition;

	[HideInInspector]
	public float timeout;

	[HideInInspector]
	public bool heardPlayer;

	[HideInInspector]
	public bool heardTarget;

	[HideInInspector]
	public bool damaged;

	private bool damagedState;

	[HideInInspector]
	public float lastDamageTime;

	[HideInInspector]
	public LayerMask searchMask = 0;

	[HideInInspector]
	public RaycastHit attackHit;

	private void Start()
	{
		NPCMgrObj = GameObject.Find("NPC Manager");
		NPCRegistryComponent = NPCMgrObj.GetComponent<NPCRegistry>();
		NPCRegistryComponent.Npcs.Add(myTransform.gameObject.GetComponent<AI>());
		playerObj = Camera.main.transform.GetComponent<CameraControl>().playerObj;
		Mathf.Clamp01(randomSpawnChance);
		if (Random.value > randomSpawnChance)
		{
			Object.Destroy(myTransform.gameObject);
		}
	}

	private void OnEnable()
	{
		myTransform = base.transform;
		upVec = Vector3.up;
		startPosition = myTransform.position;
		timeout = 0f;
		attackedTime = -16f;
		if ((bool)jokeFx)
		{
			jokePlaying = jokeFx.length * -2f;
		}
		collisionState = false;
		footstepsFx = myTransform.gameObject.AddComponent<AudioSource>();
		footstepsFx.spatialBlend = 1f;
		footstepsFx.volume = footStepVol;
		footstepsFx.pitch = 1f;
		footstepsFx.dopplerLevel = 0f;
		footstepsFx.bypassEffects = true;
		footstepsFx.bypassListenerEffects = true;
		footstepsFx.bypassReverbZones = true;
		footstepsFx.maxDistance = 10f;
		footstepsFx.rolloffMode = AudioRolloffMode.Linear;
		footstepsFx.playOnAwake = false;
		vocalFx = myTransform.gameObject.AddComponent<AudioSource>();
		vocalFx.spatialBlend = 1f;
		vocalFx.volume = vocalVol;
		vocalFx.pitch = 1f;
		vocalFx.dopplerLevel = 0f;
		vocalFx.bypassEffects = true;
		vocalFx.bypassListenerEffects = true;
		vocalFx.bypassReverbZones = true;
		vocalFx.maxDistance = 10f;
		vocalFx.rolloffMode = AudioRolloffMode.Linear;
		vocalFx.playOnAwake = false;
		searchMask = 1059841;
		if (objectWithAnims == null)
		{
			objectWithAnims = base.transform;
		}
		AnimatorComponent = objectWithAnims.GetComponent<Animator>();
		playerObj = Camera.main.transform.GetComponent<CameraControl>().playerObj;
		playerTransform = playerObj.transform;
		PlayerWeaponsComponent = Camera.main.transform.GetComponent<CameraControl>().weaponObj.GetComponentInChildren<PlayerWeapons>();
		FPSWalker = playerObj.GetComponent<FPSRigidBodyWalker>();
		NPCAttackComponent = GetComponent<NPCAttack>();
		CharacterDamageComponent = GetComponent<CharacterDamage>();
		agent = GetComponent<NavMeshAgent>();
		agent.speed = runSpeed;
		agent.acceleration = 60f;
		colliders = GetComponentsInChildren<Collider>();
		attackRangeAmt = attackRange;
		AnimatorComponent.SetInteger("AnimState", 0);
		if (!spawned)
		{
			SpawnNPC();
		}
	}

	public void SpawnNPC()
	{
		StopAllCoroutines();
		if (agent.isOnNavMesh)
		{
			spawnTime = Time.time;
			StartCoroutine(PlayFootSteps());
			if (objectWithAnims != myTransform)
			{
				StartCoroutine(UpdateModelYaw());
			}
			if (!huntPlayer)
			{
				if (!standWatch && (bool)waypointGroup && (bool)waypointGroup.wayPoints[firstWaypoint - 1])
				{
					curWayPoint = waypointGroup.wayPoints[firstWaypoint - 1];
					speedAmt = runSpeed;
					startPosition = curWayPoint.position;
					TravelToPoint(curWayPoint.position);
					StartCoroutine(Patrol());
				}
				else
				{
					TravelToPoint(startPosition);
					StartCoroutine(StandWatch());
				}
			}
			else
			{
				factionNum = 2;
				target = playerTransform;
				targetEyeHeight = FPSWalker.capsule.height * 0.25f;
				lastVisibleTargetPosition = target.position + target.up * targetEyeHeight;
				attackRange = 2048f;
				StartCoroutine(AttackTarget());
				speedAmt = runSpeed;
				TravelToPoint(playerObj.transform.position);
			}
		}
		else
		{
			Debug.Log("<color=red>NPC can't find Navmesh:</color> Please bake Navmesh for this scene or reposition NPC closer to navmesh.");
		}
	}

	private void OnDrawGizmos()
	{
		if (drawDebugGizmos)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(base.transform.position, 0.2f);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.position + new Vector3(0f, eyeHeight, 0f), 0.2f);
			Vector3 vector = base.transform.position + base.transform.up * eyeHeight;
			Vector3 vector2 = lastVisibleTargetPosition;
			Vector3 normalized = (vector2 - vector).normalized;
			float num = Vector3.Distance(vector, vector2);
			Vector3 vector3 = vector + normalized * num;
			if (Physics.Linecast(vector, vector2))
			{
				Gizmos.color = Color.red;
			}
			else
			{
				Gizmos.color = Color.green;
			}
			Gizmos.DrawLine(vector, vector3);
			Gizmos.DrawSphere(vector3, 0.2f);
			if ((bool)target)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(vector, target.position + base.transform.up * targetEyeHeight);
			}
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(base.transform.position + base.transform.forward * 0.6f + base.transform.up * eyeHeight, 0.2f);
			Vector3 forward = base.transform.forward;
			forward = Quaternion.Euler(0f, -90f, 0f) * forward;
			agent = GetComponent<NavMeshAgent>();
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(base.transform.position + base.transform.up * eyeHeight + forward * agent.radius, 0.2f);
			Gizmos.DrawSphere(base.transform.position + base.transform.up * eyeHeight - forward * agent.radius, 0.2f);
		}
	}

	private void InitializeAnim()
	{
		if (!animInit && !animInitialized)
		{
			animInit = true;
			animInitialized = true;
		}
		else
		{
			animInit = false;
		}
	}

	private IEnumerator StandWatch()
	{
		while (!huntPlayer)
		{
			if (attackedTime + 6f > Time.time)
			{
				attackRangeAmt = attackRange * 6f;
			}
			else
			{
				attackRangeAmt = attackRange;
			}
			if (playerObj.activeInHierarchy && !collisionState && (bool)FPSWalker.capsule)
			{
				Collider[] array = colliders;
				foreach (Collider collider in array)
				{
					Physics.IgnoreCollision(collider, FPSWalker.capsule, true);
				}
				collisionState = true;
			}
			CanSeeTarget();
			if (((bool)target && targetVisible) || heardPlayer || heardTarget)
			{
				yield return StartCoroutine(AttackTarget());
			}
			else if ((bool)NPCRegistryComponent)
			{
				NPCRegistryComponent.FindClosestTarget(myTransform.gameObject, this, myTransform.position, attackRangeAmt, factionNum);
			}
			if (attackTime < Time.time)
			{
				if ((!followPlayer || orderedMove) && Vector3.Distance(startPosition, myTransform.position) > pickNextDestDist)
				{
					if (!orderedMove)
					{
						speedAmt = walkSpeed;
					}
					else
					{
						speedAmt = runSpeed;
					}
					InitializeAnim();
					TravelToPoint(startPosition);
				}
				else if (followPlayer && !orderedMove && Vector3.Distance(playerObj.transform.position, myTransform.position) > pickNextDestDist)
				{
					if (followPlayer && Vector3.Distance(playerObj.transform.position, myTransform.position) > pickNextDestDist * 2f)
					{
						speedAmt = runSpeed;
						lastRunTime = Time.time;
					}
					else if (lastRunTime + 2f < Time.time)
					{
						speedAmt = walkSpeed;
					}
					InitializeAnim();
					TravelToPoint(playerObj.transform.position);
				}
				else
				{
					speedAmt = 0f;
					agent.Stop();
					SetSpeed(speedAmt);
					if (attackFinished && attackTime < Time.time)
					{
						AnimatorComponent.SetInteger("AnimState", 0);
					}
				}
			}
			if (animInit)
			{
				yield return null;
			}
			else
			{
				yield return new WaitForSeconds(0.3f);
			}
		}
		SpawnNPC();
	}

	private IEnumerator Patrol()
	{
		while (true)
		{
			if (huntPlayer)
			{
				SpawnNPC();
				yield break;
			}
			if (!curWayPoint || !waypointGroup)
			{
				break;
			}
			Vector3 waypointPosition = curWayPoint.position;
			float waypointDist = Vector3.Distance(waypointPosition, myTransform.position);
			int waypointNumber = waypointGroup.wayPoints.IndexOf(curWayPoint);
			if (patrolOnce && waypointNumber == waypointGroup.wayPoints.Count - 1)
			{
				if (waypointDist < pickNextDestDist)
				{
					speedAmt = 0f;
					startPosition = waypointPosition;
					StartCoroutine(StandWatch());
					yield break;
				}
			}
			else if (waypointDist < pickNextDestDist)
			{
				if (waypointGroup.wayPoints.Count == 1)
				{
					speedAmt = 0f;
					startPosition = waypointPosition;
					StartCoroutine(StandWatch());
					yield break;
				}
				curWayPoint = PickNextWaypoint(curWayPoint, waypointNumber);
				if (spawned && Vector3.Distance(waypointPosition, myTransform.position) < pickNextDestDist)
				{
					walkOnPatrol = true;
				}
			}
			if (attackedTime + 6f > Time.time)
			{
				attackRangeAmt = attackRange * 6f;
			}
			else
			{
				attackRangeAmt = attackRange;
			}
			if (playerObj.activeInHierarchy && !collisionState && (bool)FPSWalker.capsule)
			{
				Collider[] array = colliders;
				foreach (Collider collider in array)
				{
					Physics.IgnoreCollision(collider, FPSWalker.capsule, true);
				}
				collisionState = true;
			}
			CanSeeTarget();
			if (((bool)target && targetVisible) || heardPlayer || heardTarget)
			{
				yield return StartCoroutine(AttackTarget());
			}
			else
			{
				if ((bool)NPCRegistryComponent)
				{
					NPCRegistryComponent.FindClosestTarget(myTransform.gameObject, this, myTransform.position, attackRangeAmt, factionNum);
				}
				if (attackTime < Time.time)
				{
					if (orderedMove && !followPlayer)
					{
						if (!(Vector3.Distance(startPosition, myTransform.position) > pickNextDestDist))
						{
							speedAmt = 0f;
							agent.Stop();
							SetSpeed(speedAmt);
							if (attackFinished && attackTime < Time.time)
							{
								AnimatorComponent.SetInteger("AnimState", 0);
							}
							StartCoroutine(StandWatch());
							yield break;
						}
						speedAmt = runSpeed;
						TravelToPoint(startPosition);
					}
					else if (!orderedMove && followPlayer)
					{
						if (Vector3.Distance(playerObj.transform.position, myTransform.position) > pickNextDestDist)
						{
							if (Vector3.Distance(playerObj.transform.position, myTransform.position) > pickNextDestDist * 2f)
							{
								speedAmt = runSpeed;
								lastRunTime = Time.time;
							}
							else if (lastRunTime + 2f < Time.time)
							{
								speedAmt = walkSpeed;
							}
							TravelToPoint(playerObj.transform.position);
						}
						else
						{
							speedAmt = 0f;
							agent.Stop();
							SetSpeed(speedAmt);
							if (attackFinished && attackTime < Time.time)
							{
								AnimatorComponent.SetInteger("AnimState", 0);
							}
						}
					}
					else
					{
						if (walkOnPatrol)
						{
							speedAmt = walkSpeed;
						}
						else
						{
							speedAmt = runSpeed;
						}
						TravelToPoint(waypointPosition);
					}
				}
			}
			yield return new WaitForSeconds(0.3f);
		}
		StartCoroutine(StandWatch());
	}

	private void CanSeeTarget()
	{
		if (spawnTime + 1f > Time.time)
		{
			return;
		}
		if (((bool)TargetAIComponent && !TargetAIComponent.enabled) || ((bool)target && !target.gameObject.activeInHierarchy))
		{
			target = null;
			TargetAIComponent = null;
			targetVisible = false;
			heardTarget = false;
			return;
		}
		if ((factionNum != 1 || playerAttacked) && (bool)FPSWalker.capsule)
		{
			float num = Vector3.Distance(myTransform.position + upVec * eyeHeight, playerTransform.position + upVec * FPSWalker.capsule.height * 0.25f);
			if (!heardPlayer && !huntPlayer && FPSWalker.dropTime + 2.5f < Time.time && num < listenRange && (target == playerTransform || target == FPSWalker.leanObj.transform || target == null) && (bool)PlayerWeaponsComponent && (bool)PlayerWeaponsComponent.CurrentWeaponBehaviorComponent)
			{
				WeaponBehaviorComponent = PlayerWeaponsComponent.CurrentWeaponBehaviorComponent;
				if (WeaponBehaviorComponent.shootStartTime + 2f > Time.time && !WeaponBehaviorComponent.silentShots)
				{
					if (target == FPSWalker.leanObj.transform)
					{
						targetEyeHeight = 0f;
						target = FPSWalker.leanObj.transform;
						pursueTarget = true;
					}
					else
					{
						targetEyeHeight = FPSWalker.capsule.height * 0.25f;
						target = playerTransform;
					}
					timeout = Time.time + 6f;
					heardPlayer = true;
					return;
				}
			}
			if (huntPlayer)
			{
				targetEyeHeight = FPSWalker.capsule.height * 0.25f;
				target = playerTransform;
			}
			if (num < attackRangeAmt)
			{
				if (Mathf.Abs(FPSWalker.leanAmt) > 0.1f && num > 2f && target == playerTransform && (attackedTime + 6f > Time.time || heardPlayer))
				{
					targetEyeHeight = 0f;
					target = FPSWalker.leanObj.transform;
				}
				if ((Mathf.Abs(FPSWalker.leanAmt) < 0.1f || num < 2f) && target == FPSWalker.leanObj.transform)
				{
					targetEyeHeight = FPSWalker.capsule.height * 0.25f;
					target = playerTransform;
				}
			}
		}
		if (target == playerTransform || target == FPSWalker.leanObj.transform || ((bool)TargetAIComponent && TargetAIComponent.enabled && target != null))
		{
			Vector3 vector = myTransform.position + upVec * eyeHeight;
			targetPos = target.position + target.up * targetEyeHeight;
			targetDistance = Vector3.Distance(vector, targetPos);
			targetDirection = (targetPos - vector).normalized;
			Vector3.Cross(targetDirection, Vector3.forward).Normalize();
			if (targetDistance > attackRangeAmt)
			{
				sightBlocked = true;
				targetVisible = false;
				return;
			}
			hits = Physics.RaycastAll(vector, targetDirection, targetDistance, searchMask);
			sightBlocked = false;
			if (!huntPlayer && timeout < Time.time && attackedTime + 6f < Time.time && (target == playerTransform || target == FPSWalker.leanCol) && !heardPlayer && !FPSWalker.sprintActive)
			{
				Vector3 normalized = (targetPos - vector).normalized;
				if (Vector3.Dot(normalized, base.transform.forward) < -0.45f)
				{
					sightBlocked = true;
					playerIsBehind = true;
					targetVisible = false;
					return;
				}
			}
			playerIsBehind = false;
			for (int i = 0; i < hits.Length; i++)
			{
				if ((!hits[i].transform.IsChildOf(target) && !hits[i].transform.IsChildOf(myTransform)) || (!playerAttacked && factionNum == 1 && target != playerObj && (hits[i].collider == FPSWalker.capsule || hits[i].collider == FPSWalker.leanCol)))
				{
					sightBlocked = true;
					break;
				}
				if (hits[i].transform.IsChildOf(target))
				{
					attackHit = hits[i];
					break;
				}
			}
			if (!sightBlocked)
			{
				if (target != FPSWalker.leanObj.transform)
				{
					pursueTarget = false;
					targetVisible = true;
				}
				else
				{
					pursueTarget = true;
					targetVisible = true;
				}
			}
			else
			{
				if ((bool)TargetAIComponent && !huntPlayer && TargetAIComponent.attackTime > Time.time && Vector3.Distance(myTransform.position, target.position) < listenRange)
				{
					timeout = Time.time + 6f;
					heardTarget = true;
				}
				targetVisible = false;
			}
		}
		else
		{
			targetVisible = false;
		}
	}

	private IEnumerator Shoot()
	{
		attackFinished = false;
		speedAmt = 0f;
		SetSpeed(speedAmt);
		agent.Stop();
		AnimatorComponent.SetInteger("AnimState", 3);
		AnimatorComponent.SetTrigger("Attack");
		yield return new WaitForSeconds(delayShootTime);
		NPCAttackComponent.Fire();
		if (cancelAttackTaunt)
		{
			vocalFx.Stop();
		}
		attackTime = Time.time + 2f;
		yield return new WaitForSeconds(delayShootTime + Random.Range(shotDuration, shotDuration + 0.75f));
		attackFinished = true;
		AnimatorComponent.SetInteger("AnimState", 0);
	}

	private IEnumerator AttackTarget()
	{
		while (true)
		{
			if (Time.timeSinceLevelLoad < 1f)
			{
				yield return new WaitForSeconds(1f);
			}
			if (target == null || ((bool)TargetAIComponent && !TargetAIComponent.enabled && !huntPlayer))
			{
				timeout = 0f;
				heardPlayer = false;
				heardTarget = false;
				damaged = false;
				TargetAIComponent = null;
				yield break;
			}
			if (lastTauntTime + tauntDelay < Time.time && Random.value < tauntChance && (alertTaunt || alertSnds.Length <= 0) && tauntSnds.Length > 0)
			{
				vocalFx.volume = tauntVol;
				vocalFx.pitch = Random.Range(0.94f, 1f);
				vocalFx.spatialBlend = 1f;
				vocalFx.clip = tauntSnds[Random.Range(0, tauntSnds.Length)];
				vocalFx.PlayOneShot(vocalFx.clip);
				lastTauntTime = Time.time;
			}
			if (!alertTaunt && alertSnds.Length > 0)
			{
				vocalFx.volume = alertVol;
				vocalFx.pitch = Random.Range(0.94f, 1f);
				vocalFx.spatialBlend = 1f;
				vocalFx.clip = alertSnds[Random.Range(0, alertSnds.Length)];
				vocalFx.PlayOneShot(vocalFx.clip);
				lastTauntTime = Time.time;
				alertTaunt = true;
			}
			float distance = Vector3.Distance(myTransform.position, target.position);
			if (!huntPlayer)
			{
				if (heardPlayer && (target == playerTransform || target == FPSWalker.leanObj.transform))
				{
					InitializeAnim();
					speedAmt = runSpeed;
					SearchTarget(lastVisibleTargetPosition);
				}
				if (heardTarget)
				{
					InitializeAnim();
					speedAmt = runSpeed;
					SearchTarget(lastVisibleTargetPosition);
				}
				if (distance > attackRangeAmt)
				{
					speedAmt = walkSpeed;
					target = null;
					yield break;
				}
			}
			else
			{
				InitializeAnim();
				target = playerTransform;
				speedAmt = runSpeed;
				TravelToPoint(target.position);
			}
			if (pursueTarget)
			{
				lastVisibleTargetPosition = FPSWalker.leanObj.transform.position;
			}
			else
			{
				lastVisibleTargetPosition = target.position + target.up * targetEyeHeight;
			}
			CanSeeTarget();
			if (targetVisible)
			{
				timeout = Time.time + 6f;
				if (distance > shootRange)
				{
					if (!huntPlayer)
					{
						SearchTarget(lastVisibleTargetPosition);
					}
				}
				else
				{
					if (!turning)
					{
						StopCoroutine("RotateTowards");
						StartCoroutine(RotateTowards(lastVisibleTargetPosition, 20f, 2f));
					}
					speedAmt = 0f;
					SetSpeed(speedAmt);
					agent.speed = speedAmt;
				}
				InitializeAnim();
				speedAmt = runSpeed;
				Vector3 forward = myTransform.TransformDirection(Vector3.forward);
				Vector3 targetDirection = lastVisibleTargetPosition - (myTransform.position + myTransform.up * eyeHeight);
				targetDirection.y = 0f;
				float angle = Vector3.Angle(targetDirection, forward);
				if (distance < shootRange && angle < shootAngle)
				{
					if (attackFinished)
					{
						yield return StartCoroutine(Shoot());
					}
					else
					{
						speedAmt = 0f;
						SetSpeed(speedAmt);
						agent.Stop();
					}
				}
			}
			else if (!huntPlayer && (attackFinished || huntPlayer))
			{
				if (!(timeout > Time.time))
				{
					break;
				}
				InitializeAnim();
				speedAmt = runSpeed;
				SetSpeed(speedAmt);
				SearchTarget(lastVisibleTargetPosition);
			}
			if (animInit)
			{
				yield return null;
			}
			else
			{
				yield return new WaitForSeconds(0.3f);
			}
		}
		heardPlayer = false;
		heardTarget = false;
		alertTaunt = false;
		speedAmt = 0f;
		SetSpeed(speedAmt);
		agent.Stop();
		target = null;
	}

	private void SearchTarget(Vector3 position)
	{
		if (!attackFinished)
		{
			return;
		}
		if (target == playerTransform || target == FPSWalker.leanObj.transform || ((bool)TargetAIComponent && TargetAIComponent.enabled))
		{
			if (!huntPlayer)
			{
				speedAmt = runSpeed;
				TravelToPoint(target.position);
			}
		}
		else
		{
			timeout = 0f;
			damaged = false;
		}
	}

	public IEnumerator RotateTowards(Vector3 position, float rotationSpeed, float turnTimer, bool attacking = true)
	{
		float turnTime = Time.time;
		SetSpeed(0f);
		agent.Stop();
		while (turnTime + turnTimer > Time.time && !cancelRotate)
		{
			turning = true;
			if (pursueTarget)
			{
				position = FPSWalker.leanObj.transform.position;
			}
			else if ((bool)target && attacking && (target == playerTransform || ((bool)TargetAIComponent && TargetAIComponent.enabled)))
			{
				lastVisibleTargetPosition = target.position + target.up * targetEyeHeight;
			}
			else
			{
				lastVisibleTargetPosition = position;
			}
			Vector3 direction = lastVisibleTargetPosition - myTransform.position;
			direction.y = 0f;
			if (direction.x != 0f && direction.z != 0f)
			{
				myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
				myTransform.eulerAngles = new Vector3(0f, myTransform.eulerAngles.y, 0f);
				yield return null;
				continue;
			}
			break;
		}
		cancelRotate = false;
		turning = false;
	}

	private IEnumerator UpdateModelYaw()
	{
		while (true)
		{
			if (stepInterval > 0f)
			{
				yawAmt = Mathf.MoveTowards(yawAmt, movingYaw, Time.deltaTime * 180f);
			}
			else
			{
				yawAmt = Mathf.MoveTowards(yawAmt, idleYaw, Time.deltaTime * 180f);
			}
			objectWithAnims.transform.localRotation = Quaternion.Euler(0f, yawAmt, 0f);
			yield return null;
		}
	}

	private void TravelToPoint(Vector3 position)
	{
		if (attackFinished)
		{
			agent.SetDestination(position);
			agent.Resume();
			agent.speed = speedAmt;
			SetSpeed(speedAmt);
		}
	}

	private Transform PickNextWaypoint(Transform currentWaypoint, int curWaypointNumber)
	{
		Transform transform = currentWaypoint;
		if (!countBackwards)
		{
			if (curWaypointNumber < waypointGroup.wayPoints.Count - 1)
			{
				transform = waypointGroup.wayPoints[curWaypointNumber + 1];
			}
			else
			{
				transform = waypointGroup.wayPoints[curWaypointNumber - 1];
				countBackwards = true;
			}
		}
		else if (curWaypointNumber != 0)
		{
			transform = waypointGroup.wayPoints[curWaypointNumber - 1];
		}
		else
		{
			transform = waypointGroup.wayPoints[curWaypointNumber + 1];
			countBackwards = false;
		}
		return transform;
	}

	private void SetSpeed(float speed)
	{
		if (speed > walkSpeed && agent.hasPath)
		{
			AnimatorComponent.SetInteger("AnimState", 2);
			stepInterval = runStepTime;
			return;
		}
		if (speed > 0f && agent.hasPath)
		{
			AnimatorComponent.SetInteger("AnimState", 1);
			stepInterval = walkStepTime;
			return;
		}
		if (attackFinished && attackTime < Time.time)
		{
			AnimatorComponent.SetInteger("AnimState", 0);
		}
		stepInterval = -1f;
	}

	private IEnumerator PlayFootSteps()
	{
		while (true)
		{
			if (footSteps.Length > 0 && stepInterval > 0f)
			{
				footstepsFx.pitch = 1f;
				footstepsFx.volume = footStepVol;
				footstepsFx.clip = footSteps[Random.Range(0, footSteps.Length)];
				footstepsFx.PlayOneShot(footstepsFx.clip);
			}
			yield return new WaitForSeconds(stepInterval);
		}
	}

	public void CommandNPC()
	{
		if (factionNum == 1 && followOnUse && commandedTime + 0.5f < Time.time)
		{
			orderedMove = false;
			cancelRotate = false;
			commandedTime = Time.time;
			if (attackFinished && !turning)
			{
				StopCoroutine("RotateTowards");
				StartCoroutine(RotateTowards(playerTransform.position, 10f, 2f, false));
			}
			if (!followPlayer)
			{
				if (((bool)followFx1 || (bool)followFx2) && (((bool)jokeFx && jokePlaying + jokeFx.length < Time.time) || !jokeFx))
				{
					if (Random.value > 0.5f)
					{
						vocalFx.clip = followFx1;
					}
					else
					{
						vocalFx.clip = followFx2;
					}
					vocalFx.pitch = Random.Range(0.94f, 1f);
					vocalFx.spatialBlend = 1f;
					vocalFx.PlayOneShot(vocalFx.clip);
				}
				followPlayer = true;
			}
			else
			{
				if (((bool)stayFx1 || (bool)stayFx2) && (((bool)jokeFx && jokePlaying + jokeFx.length < Time.time) || !jokeFx))
				{
					if (Random.value > 0.5f)
					{
						vocalFx.clip = stayFx1;
					}
					else
					{
						vocalFx.clip = stayFx2;
					}
					vocalFx.pitch = Random.Range(0.94f, 1f);
					vocalFx.spatialBlend = 1f;
					vocalFx.PlayOneShot(vocalFx.clip);
				}
				startPosition = myTransform.position;
				followPlayer = false;
			}
		}
		if (!jokeFx || factionNum != 1 || !followOnUse)
		{
			return;
		}
		if (jokeCount == 0)
		{
			talkedTime = Time.time;
		}
		if (talkedTime + 0.5f > Time.time)
		{
			talkedTime = Time.time;
			jokeCount++;
			if (jokeCount > jokeActivate)
			{
				if (!jokeFx2)
				{
					vocalFx.clip = jokeFx;
				}
				else if (Random.value > 0.5f)
				{
					vocalFx.clip = jokeFx;
				}
				else
				{
					vocalFx.clip = jokeFx2;
				}
				vocalFx.pitch = Random.Range(0.94f, 1f);
				vocalFx.spatialBlend = 1f;
				vocalFx.PlayOneShot(vocalFx.clip);
				jokePlaying = Time.time;
				jokeCount = 0;
			}
		}
		else
		{
			jokeCount = 0;
		}
	}

	public void GoToPosition(Vector3 position, bool runToPos)
	{
		if (runToPos)
		{
			orderedMove = true;
		}
		else
		{
			orderedMove = false;
		}
		cancelRotate = true;
		startPosition = position;
	}

	public void ChangeFaction(int factionChange)
	{
		target = null;
		factionNum = factionChange;
	}
}
