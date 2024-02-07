using UnityEngine;

public class MoveTrigger : MonoBehaviour
{
	[Tooltip("The NPC to move when the Player enters this trigger.")]
	public AI npcToMove;

	[Tooltip("The position to move the NPC to.")]
	public Transform movePosition;

	[Tooltip("True if NPC should run to the move position, false if they should walk.")]
	public bool runToGoal;

	private bool moved;

	private bool rotated;

	[Tooltip("The next MoveTrigger.cs component to activate after this one.")]
	public MoveTrigger nextMoveTrigger;

	[Tooltip("True if this trigger should be active at scene start, instead of waiting to be activated by other MoveTrigger.cs components when NPC reaches goal.")]
	public bool isStartingTrigger;

	[Tooltip("Sound effects to play when NPC starts traveling to move position (following vocals).")]
	public AudioClip[] followSnds;

	[Tooltip("Volume of follow sound effects.")]
	public float followVol = 0.7f;

	private void Start()
	{
		if (!isStartingTrigger)
		{
			base.gameObject.SetActive(false);
		}
		if ((bool)npcToMove)
		{
			npcToMove.leadPlayer = true;
			npcToMove.followOnUse = false;
		}
	}

	private void OnTriggerStay(Collider col)
	{
		if ((bool)npcToMove && npcToMove.enabled && col.gameObject.tag == "Player" && !moved && !npcToMove.followed)
		{
			if (!runToGoal)
			{
				npcToMove.GoToPosition(movePosition.position, false);
			}
			else
			{
				npcToMove.GoToPosition(movePosition.position, true);
			}
			if (followSnds.Length > 0)
			{
				npcToMove.vocalFx.volume = followVol;
				npcToMove.vocalFx.pitch = Random.Range(0.94f, 1f);
				npcToMove.vocalFx.spatialBlend = 1f;
				npcToMove.vocalFx.clip = followSnds[Random.Range(0, followSnds.Length)];
				npcToMove.vocalFx.PlayOneShot(npcToMove.vocalFx.clip);
			}
			if ((bool)nextMoveTrigger)
			{
				nextMoveTrigger.gameObject.SetActive(true);
				nextMoveTrigger.moved = false;
				nextMoveTrigger.rotated = false;
			}
			npcToMove.followed = true;
			moved = true;
		}
	}

	private void Update()
	{
		if (moved && !rotated && Vector3.Distance(movePosition.position, npcToMove.myTransform.position) < npcToMove.pickNextDestDist)
		{
			npcToMove.cancelRotate = false;
			npcToMove.StartCoroutine(npcToMove.RotateTowards(npcToMove.playerTransform.position, 10f, 2f, false));
			npcToMove.followed = false;
			base.gameObject.SetActive(false);
			rotated = true;
		}
	}
}
