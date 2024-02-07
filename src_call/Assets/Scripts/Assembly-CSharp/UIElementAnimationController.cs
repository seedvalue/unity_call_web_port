using UnityEngine;

public class UIElementAnimationController : MonoBehaviour
{
	public Vector3 startPos;

	public Vector3 moveToPos;

	public float speed = 0.1f;

	public float startWait;

	public LeanTweenType type = LeanTweenType.easeOutBack;

	private void OnEnable()
	{
		GetComponent<RectTransform>().anchoredPosition = startPos;
		Invoke("animateElement", startWait);
	}

	private void animateElement()
	{
		LeanTween.move(GetComponent<RectTransform>(), moveToPos, speed).setEase(type);
	}
}
