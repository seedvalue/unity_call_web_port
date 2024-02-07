using UnityEngine;

public class TrendingButtunClick : MonoBehaviour
{
	[HideInInspector]
	public TrendingGamesRotation trending;

	public void Clicke()
	{
		trending.Click();
	}
}
