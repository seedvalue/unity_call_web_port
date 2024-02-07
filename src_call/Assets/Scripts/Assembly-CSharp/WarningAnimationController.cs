using UnityEngine;

public class WarningAnimationController : MonoBehaviour
{
	private void OnEnable()
	{
		InvokeRepeating("animate", 0.8f, 0.8f);
	}

	private void animate()
	{
		base.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		LeanTween.scale(base.gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
	}

	private void OnDisable()
	{
		CancelInvoke();
	}
}
