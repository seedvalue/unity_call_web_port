using UnityEngine;
using UnityEngine.UI;

public class WarmupText : MonoBehaviour
{
	[HideInInspector]
	public float warmupGui;

	private float oldWarmup = -512f;

	[Tooltip("Color of GUIText.")]
	public Color textColor;

	[HideInInspector]
	public bool waveBegins;

	[HideInInspector]
	public bool waveComplete;

	private Text uiTextComponent;

	private void OnEnable()
	{
		uiTextComponent = GetComponent<Text>();
		oldWarmup = -512f;
	}

	private void Update()
	{
		if (warmupGui == oldWarmup)
		{
			return;
		}
		if (!waveComplete)
		{
			if (!waveBegins)
			{
				uiTextComponent.text = "Warmup Time : " + Mathf.Round(warmupGui);
			}
			else
			{
				uiTextComponent.text = "INCOMING WAVE";
			}
		}
		else
		{
			uiTextComponent.text = "WAVE COMPLETE";
		}
		uiTextComponent.color = textColor;
		oldWarmup = warmupGui;
	}
}
