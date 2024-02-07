using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
	public Text timerText;

	public int totalTime;

	private float timeLeft;

	private int tempTime;

	private int previousTime;

	public GameController gc;

	public string addtionalText;

	public bool detonateBomb;

	private void OnEnable()
	{
		timerText.text = totalTime + " sec";
		timeLeft = totalTime;
		tempTime = totalTime;
		previousTime = tempTime;
	}

	private void Update()
	{
		if (tempTime > 0)
		{
			timeLeft -= Time.deltaTime;
			tempTime = (int)timeLeft;
			if (tempTime != previousTime)
			{
				previousTime = tempTime;
				timerText.text = tempTime + " sec " + addtionalText;
			}
		}
		else if (detonateBomb)
		{
			gc.detotanteBomb();
		}
		else
		{
			gc.hostagesBurnt();
		}
	}
}
