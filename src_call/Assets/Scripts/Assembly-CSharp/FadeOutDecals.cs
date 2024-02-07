using UnityEngine;

public class FadeOutDecals : MonoBehaviour
{
	[HideInInspector]
	public float startTime;

	[Tooltip("How long should this mark stay in the scene before fading.")]
	public int markDuration = 10;

	private Renderer RendererComponent;

	[HideInInspector]
	public Color tempColor;

	[HideInInspector]
	public GameObject parentObj;

	[HideInInspector]
	public Transform parentObjTransform;

	[HideInInspector]
	public Transform myTransform;

	private void Awake()
	{
		myTransform = base.transform;
		RendererComponent = GetComponent<Renderer>();
		parentObj = myTransform.parent.gameObject;
		parentObjTransform = parentObj.transform;
	}

	public void InitializeDecal()
	{
		startTime = Time.time + (float)markDuration;
		tempColor = RendererComponent.material.color;
		tempColor.a = 1f;
		RendererComponent.material.color = tempColor;
	}

	private void Update()
	{
		if (startTime < Time.time)
		{
			tempColor.a -= Time.deltaTime;
			RendererComponent.material.color = tempColor;
			tempColor.a = Mathf.Clamp(tempColor.a, 0f, 255f);
		}
		if (startTime < Time.time - (float)markDuration)
		{
			parentObjTransform.parent = AzuObjectPool.instance.transform;
			parentObj.SetActive(false);
		}
	}
}
