using System.Collections.Generic;
using UnityEngine;

public class NPCRegistry : MonoBehaviour
{
	private FPSRigidBodyWalker FPSWalker;

	[Tooltip("List containing references to all NPCs' AI.cs components, for use in other scripts (populated automatically at runtime).")]
	public List<AI> Npcs = new List<AI>();

	private GameObject playerObj;

	private Transform playerTransform;

	private float nearestNpcDist;

	private float NpcDist;

	private float playerDist;

	private float playerDistMod;

	private RaycastHit hit;

	private void Start()
	{
		playerObj = Camera.main.transform.GetComponent<CameraControl>().playerObj;
		FPSWalker = playerObj.GetComponent<FPSRigidBodyWalker>();
		playerTransform = playerObj.transform;
	}

	public void UnregisterNPC(AI NpcAI)
	{
		for (int i = 0; i < Npcs.Count; i++)
		{
			if (Npcs[i] == NpcAI)
			{
				Npcs.RemoveAt(i);
			}
		}
	}

	public void MoveFolowingNpcs(Vector3 position)
	{
		bool flag = false;
		for (int i = 0; i < Npcs.Count; i++)
		{
			if (Npcs[i].leadPlayer || Npcs[i].factionNum != 1 || (!Npcs[i].followPlayer && !Npcs[i].orderedMove))
			{
				continue;
			}
			Npcs[i].GoToPosition(position, true);
			Npcs[i].followPlayer = false;
			if (!flag && ((bool)Npcs[i].moveToFx1 || (bool)Npcs[i].moveToFx2))
			{
				if (Random.value > 0.5f)
				{
					Npcs[i].vocalFx.clip = Npcs[i].moveToFx1;
				}
				else
				{
					Npcs[i].vocalFx.clip = Npcs[i].moveToFx2;
				}
				Npcs[i].vocalFx.pitch = Random.Range(0.94f, 1f);
				Npcs[i].vocalFx.spatialBlend = 0f;
				Npcs[i].vocalFx.PlayOneShot(Npcs[i].vocalFx.clip);
				flag = true;
			}
		}
	}

	public void FindClosestTarget(GameObject NPC, AI NpcAIcomponent, Vector3 myPos, float distance, int myFaction)
	{
		nearestNpcDist = distance;
		AI aI = null;
		playerDist = Vector3.Distance(myPos, playerTransform.position);
		if (!NpcAIcomponent.heardPlayer)
		{
			if (FPSWalker.crouched)
			{
				playerDistMod = distance * NpcAIcomponent.sneakRangeMod;
			}
			else if (FPSWalker.prone)
			{
				playerDistMod = distance * (NpcAIcomponent.sneakRangeMod * 0.75f);
			}
			else
			{
				playerDistMod = distance;
			}
		}
		else
		{
			playerDistMod = distance;
		}
		for (int i = 0; i < Npcs.Count; i++)
		{
			NpcDist = Vector3.Distance(myPos, Npcs[i].myTransform.position);
			if (NpcDist < distance && NpcDist < nearestNpcDist && NpcAIcomponent != Npcs[i] && ((myFaction == 1 && Npcs[i].factionNum == 2) || (myFaction == 1 && Npcs[i].factionNum == 3) || (myFaction == 2 && Npcs[i].factionNum == 1) || (myFaction == 2 && Npcs[i].factionNum == 3) || (myFaction == 3 && Npcs[i].factionNum != 3)))
			{
				nearestNpcDist = NpcDist;
				aI = Npcs[i];
			}
		}
		if ((myFaction != 1 && playerDist < playerDistMod && playerDist < nearestNpcDist) || NpcAIcomponent.huntPlayer)
		{
			NpcAIcomponent.target = playerTransform;
			NpcAIcomponent.targetEyeHeight = FPSWalker.capsule.height * 0.25f;
			NpcAIcomponent.TargetAIComponent = null;
		}
		else if ((bool)aI)
		{
			NpcAIcomponent.TargetAIComponent = aI;
			NpcAIcomponent.targetEyeHeight = aI.eyeHeight;
			NpcAIcomponent.target = aI.myTransform;
			NpcAIcomponent.lastVisibleTargetPosition = aI.myTransform.position + aI.myTransform.up * aI.eyeHeight;
		}
	}
}
