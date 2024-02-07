using UnityEngine;

public class EnemyBase : MonoBehaviour
{
	public bool hasRagdoll;

	public BoxCollider[] colliders;

	public bool isAboveGround;

	public bool isStationary;

	public bool isStraffer;

	public bool isRunner;

	public bool isHider;

	public float fireSpeed;

	public int fireStrength;

	public LayerMask enemyShootLayerMask;

	public Transform bulletPoint;

	public AudioClip fireSound;

	public int HP;

	[HideInInspector]
	public Animator animator;

	[HideInInspector]
	public Transform target;

	[HideInInspector]
	public bool lookAtTarget;

	[HideInInspector]
	public LevelController levelController;

	[HideInInspector]
	public bool isDead;

	[HideInInspector]
	public bool isFlashed;

	public Transform centerPoint;

	private float flashDuration;

	private void OnEnable()
	{
		animator = GetComponent<Animator>();
		animator.Play("IdleUnAware", 0, Random.Range(0f, 1f));
		if (hasRagdoll)
		{
			disableColliders();
		}
		levelController = base.transform.parent.GetComponent<LevelController>();
	}

	public void sawPlayerInRange(Transform player)
	{
		if (!isFlashed)
		{
			target = player;
			animator.SetBool("aimPlayer", true);
			if (isStationary)
			{
				GetComponent<EnemyStationary>().activateEnemy();
			}
			else if (isStraffer)
			{
				GetComponent<EnemyStrafer>().activateEnemy();
			}
			else if (isRunner)
			{
				GetComponent<EnemyRunner>().activateEnemy();
			}
			else if (isHider)
			{
				GetComponent<EnemyHider>().activateEnemy();
			}
		}
	}

	public void fireAtPlayer()
	{
		if (target == null)
		{
			return;
		}
		RaycastHit hitInfo;
		if (isAboveGround)
		{
			if (Physics.Raycast(centerPoint.position, base.transform.forward, out hitInfo, 100f, enemyShootLayerMask) && hitInfo.transform.tag == "Player")
			{
				hitInfo.transform.GetComponent<FPSPlayer>().ApplyDamage(fireStrength);
				AudioSource.PlayClipAtPoint(fireSound, target.position);
				bulletPoint.GetChild(0).gameObject.SetActive(true);
				animator.SetTrigger("Fire");
			}
		}
		else if (Physics.Raycast(bulletPoint.position, base.transform.forward, out hitInfo, 100f, enemyShootLayerMask) && hitInfo.transform.tag == "Player")
		{
			hitInfo.transform.GetComponent<FPSPlayer>().ApplyDamage(fireStrength);
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
		if (isStationary)
		{
			GetComponent<EnemyStationary>().deadAction();
		}
		else if (isStraffer)
		{
			GetComponent<EnemyStrafer>().deadAction();
		}
		else if (isRunner)
		{
			GetComponent<EnemyRunner>().deadAction();
		}
		else if (isHider)
		{
			GetComponent<EnemyHider>().deadAction();
		}
		isDead = true;
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
		disableColliders();
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
		decreadeHPByGrenade(Mathf.Abs(num));
	}

	private void decreadeHPByGrenade(int val)
	{
		HP -= val;
		if (HP <= 0)
		{
			if (isStationary)
			{
				GetComponent<EnemyStationary>().deadAction();
			}
			else if (isStraffer)
			{
				GetComponent<EnemyStrafer>().deadAction();
			}
			else if (isRunner)
			{
				GetComponent<EnemyRunner>().deadAction();
			}
			else if (isHider)
			{
				GetComponent<EnemyHider>().deadAction();
			}
			isDead = true;
			HP = 0;
			CancelInvoke();
			StopAllCoroutines();
			animator.applyRootMotion = true;
			disableColliders();
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
		Debug.Log("Hit by flash");
		CancelInvoke("fireAtPlayer");
		isFlashed = true;
		if (isStationary)
		{
			GetComponent<EnemyStationary>().flashedEnemy();
		}
		else if (isStraffer)
		{
			GetComponent<EnemyStrafer>().flashedEnemy();
		}
		else if (isRunner)
		{
			GetComponent<EnemyRunner>().flashedEnemy();
		}
		else if (isHider)
		{
			GetComponent<EnemyHider>().flashedEnemy();
		}
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
		Invoke("flashEnemy", Random.Range(0f, 0.5f));
		Invoke("removeFlash", flashDuration);
	}

	private void flashEnemy()
	{
		levelController.gc.showFlashEffect();
		giveRandomRotation();
		animator.SetBool("aimPlayer", false);
		animator.SetTrigger("flashed");
	}

	private void removeFlash()
	{
		isFlashed = false;
		animator.SetBool("aimPlayer", true);
		CancelInvoke();
		target = levelController.gc.fpsPlayer;
		if (isStationary)
		{
			GetComponent<EnemyStationary>().recoverFromFlash();
		}
		else if (isStraffer)
		{
			GetComponent<EnemyStrafer>().recoverFromFlash();
		}
		else if (isRunner)
		{
			GetComponent<EnemyRunner>().recoverFromFlash();
		}
		else if (isHider)
		{
			GetComponent<EnemyHider>().recoverFromFlash();
		}
	}

	private void disableColliders()
	{
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = false;
		}
	}

	private void giveRandomRotation()
	{
		Vector3 forward = new Vector3(Random.Range(-100, 100), 0f, Random.Range(-100, 100)) - base.transform.position;
		forward.y = 0f;
		Quaternion rotation = Quaternion.LookRotation(forward);
		base.transform.rotation = rotation;
	}
}
