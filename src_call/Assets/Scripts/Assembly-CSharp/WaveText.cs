using UnityEngine;
using UnityEngine.UI;

public class WaveText : MonoBehaviour
{
	[HideInInspector]
	public int waveGui;

	[HideInInspector]
	public int waveGui2;

	private int oldWave = -512;

	private int oldWave2 = -512;

	[Tooltip("Color of GUIText.")]
	public Color textColor;

	private Text uiTextComponent;

	private void OnEnable()
	{
		uiTextComponent = GetComponent<Text>();
		oldWave = -512;
		oldWave2 = -512;
	}

	private void Update()
	{
		if (waveGui != oldWave || waveGui2 != oldWave2)
		{
			uiTextComponent.text = "Wave " + waveGui + " - Remaining : " + waveGui2;
			uiTextComponent.color = textColor;
			oldWave = waveGui;
			oldWave2 = waveGui2;
		}
	}
}
