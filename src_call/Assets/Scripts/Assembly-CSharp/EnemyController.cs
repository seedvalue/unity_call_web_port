using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public int HP;

	public Transform target;

	public bool isDead;

	public LevelController levelController;

	private Animator animator;

	public float fireSpeed;

	public int bulletDamage;

	public LayerMask enemyShootLayerMask;

	public Transform bulletPoint;

	public AudioClip fireSound;

	public BoxCollider[] colliders;

	public float flashDuration;

	public bool isFlashed;

	public float fireDurationMin = 1f;

	public float fireDurationMax = 1.5f;

	public float moveDurationMin = 1.5f;

	public float moveDurationMax = 2f;

	public EnemyMovementController enemyMovement;

	public bool cantMove;

	public bool isAboveGround;

	public Transform centerPoint;

	public bool hasRagdoll;

	public EnemyRunningController enemyRunning;

	private bool isRunning;

	private void Start()
	{
		animator = GetComponent<Animator>();
		animator.Play("IdleUnAware", 0, Random.Range(0f, 1f));
		if (hasRagdoll)
		{
			dsiableColliders();
		}
	}

	private void Update()
	{
		if (target != null && !isRunning)
		{
			Vector3 forward = target.position - base.transform.position;
			if (!isAboveGround)
			{
				forward.y = 0f;
			}
			Quaternion b = Quaternion.LookRotation(forward);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * 10f);
		}
		else if (enemyRunning == null)
		{
			enemyMovement.canMove = false;
		}
	}

	public void sawPlayerInRange(Transform player)
	{
		if (!isFlashed)
		{
			target = player;
			animator.SetBool("aimPlayer", true);
			if (enemyMovement != null && !cantMove)
			{
				StartCoroutine(enemyMovement.startChangingPos());
				Invoke("stopAndFire", Random.Range(1.5f, 2f));
			}
			else if (enemyRunning == null)
			{
				startFiringAfterLocking();
			}
			else if (enemyRunning != null)
			{
				enemyRunning.moveToNextPoint();
				isRunning = true;
			}
		}
	}

	private void stopAndFire()
	{
		enemyMovement.stopMoving();
		InvokeRepeating("fireAtPlayer", 0.3f, fireSpeed);
		Invoke("startMovingAgain", Random.Range(fireDurationMin, fireDurationMax));
	}

	private void startMovingAgain()
	{
		CancelInvoke("fireAtPlayer");
		enemyMovement.startMoving();
		Invoke("stopAndFire", Random.Range(moveDurationMin, moveDurationMax));
	}

	public void startFiringAfterLocking()
	{
		InvokeRepeating("fireAtPlayer", fireSpeed, fireSpeed);
	}

	private void fireAtPlayer()
	{
		if (target == null)
		{
			return;
		}
		RaycastHit hitInfo;
		if (!isAboveGround)
		{
			if (Physics.Raycast(bulletPoint.position, base.transform.forward, out hitInfo, 100f, enemyShootLayerMask) && hitInfo.transform.tag == "Player")
			{
				hitInfo.transform.GetComponent<FPSPlayer>().ApplyDamage(bulletDamage);
				AudioSource.PlayClipAtPoint(fireSound, target.position);
				bulletPoint.GetChild(0).gameObject.SetActive(true);
				animator.SetTrigger("Fire");
			}
		}
		else if (Physics.Raycast(centerPoint.position, base.transform.forward, out hitInfo, 100f, enemyShootLayerMask))
		{
			Debug.DrawRay(centerPoint.position, base.transform.forward * 100f, Color.red, 5f);
			if (hitInfo.transform.tag == "Player")
			{
				hitInfo.transform.GetComponent<FPSPlayer>().ApplyDamage(bulletDamage);
			}
			AudioSource.PlayClipAtPoint(fireSound, target.position);
			bulletPoint.GetChild(0).gameObject.SetActive(true);
			animator.SetTrigger("Fire");
		}
	}

	public void takeHit(int hitStrength, GameObject enemyObj, Vector3 direction)
	{
		if (!isDead)
		{
			HP -= hitStrength;
			if (HP <= 0)
			{
				dieEnemy(enemyObj, direction);
			}
		}
	}

	private void dieEnemy(GameObject hitOnObj, Vector3 direction)
	{
		isDead = true;
		enemyMovement.stopMoving();
		CancelInvoke();
		StopAllCoroutines();
		if (!hasRagdoll)
		{
			animator.SetTrigger("dieByBullet");
		}
		else
		{
			animator.enabled = false;
			Rigidbody[] componentsInChildren = base.transform.GetComponentsInChildren<Rigidbody>();
			Rigidbody[] array = componentsInChildren;
			foreach (Rigidbody rigidbody in array)
			{
				rigidbody.isKinematic = false;
			}
			hitOnObj.GetComponent<Rigidbody>().AddForce(direction * 5f, ForceMode.Impulse);
		}
		levelController.EnemyKilled(base.transform);
		dsiableColliders();
		target = null;
	}

	public void hitByGrenade(float damage, Vector3 grenadePos)
	{
		int num = 0;
		float num2 = Vector3.Distance(base.transform.position, grenadePos);
		if (num2 < 10f)
		{
			num = 120;
		}
		float num3 = 120f * (10f / num2);
		num = 120 - (int)num3;
		Debug.Log(num);
		decreadeHPByGrebade(Mathf.Abs(num));
	}

	private void decreadeHPByGrebade(int val)
	{
		HP -= val;
		if (HP <= 0)
		{
			enemyMovement.stopMoving();
			isDead = true;
			HP = 0;
			CancelInvoke();
			StopAllCoroutines();
			animator.applyRootMotion = true;
			dsiableColliders();
			if (Random.Range(0, 2) == 0)
			{
				animator.SetTrigger("dieByGrenade1");
			}
			else
			{
				animator.SetTrigger("dieByGrenade2");
			}
			levelController.EnemyKilled(base.transform);
			target = null;
		}
	}

	public void hitByFlash(Vector3 flashPos)
	{
		isFlashed = true;
		if (target != null)
		{
			target = null;
			CancelInvoke("fireAtPlayer");
		}
		enemyMovement.stopMoving();
		enemyMovement.CancelInvoke();
		if (Globals.currentLevelNumber == 5)
		{
			flashDuration = 10f;
		}
		else
		{
			flashDuration = 2f;
			float num = Vector3.Distance(base.transform.position, flashPos);
			if (num < 6f)
			{
				flashDuration = 10f;
			}
			else
			{
				flashDuration = 10 - ((int)num - 6);
			}
			flashDuration = Mathf.Clamp(flashDuration, 2f, 10f);
		}
		Invoke("flashEnemy", Random.Range(0f, 1.5f));
		Invoke("removeFlash", flashDuration);
	}

	private void flashEnemy()
	{
		if (target != null)
		{
			target = null;
			CancelInvoke("fireAtPlayer");
		}
		isFlashed = true;
		Debug.Log("EnemyFlashed");
		levelController.gc.showFlashEffect();
		animator.SetBool("aimPlayer", false);
		animator.SetTrigger("flashed");
	}

	private void removeFlash()
	{
		isFlashed = false;
		animator.SetBool("aimPlayer", true);
		CancelInvoke();
		target = levelController.gc.fpsPlayer;
		if (!cantMove)
		{
			startMovingAgain();
		}
		else
		{
			startFiringAfterLocking();
		}
	}

	private void dsiableColliders()
	{
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = false;
		}
	}

	public void stopRunningAndAim()
	{
		animator.SetBool("Run", false);
		isRunning = false;
	}
}
