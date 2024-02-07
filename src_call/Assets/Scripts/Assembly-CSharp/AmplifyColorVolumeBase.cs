using UnityEngine;

[AddComponentMenu("")]
public class AmplifyColorVolumeBase : MonoBehaviour
{
	public Texture2D LutTexture;

	public float EnterBlendTime = 1f;

	public bool ShowInSceneView = true;

	private void OnDrawGizmos()
	{
		if (ShowInSceneView)
		{
			BoxCollider component = GetComponent<BoxCollider>();
			if (component != null)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawIcon(base.transform.position, "lut-volume.png", true);
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.DrawWireCube(component.center, component.size);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		BoxCollider component = GetComponent<BoxCollider>();
		if (component != null)
		{
			Color green = Color.green;
			green.a = 0.2f;
			Gizmos.color = green;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawCube(component.center, component.size);
		}
	}
}
