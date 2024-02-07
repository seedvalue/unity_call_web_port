using UnityEngine;

public class PolicyUI : MonoBehaviour
{
	public enum Accounts
	{
		None = 0,
		SunstarGames = 1,
		GalassiaStudios = 2,
		PocketKingStudios = 3,
		TAG = 4,
		FunCraft = 5,
		AwesomeAddictiveGames = 6,
		GameBook = 7,
		BestFreeGames = 8,
		Blockot = 9
	}

	public Accounts accountName;

	public void PrivacyPolicyBtn()
	{
		if (accountName == Accounts.SunstarGames)
		{
			Application.OpenURL("http://www.sunstargames.org/privacypolicy.html");
		}
		else if (accountName == Accounts.GalassiaStudios)
		{
			Application.OpenURL("http://www.galassiastudios.com/privacy-policy");
		}
		else if (accountName == Accounts.PocketKingStudios)
		{
			Application.OpenURL("http://www.sunstargames.org/pkprivacy.html");
		}
		else if (accountName == Accounts.TAG || accountName == Accounts.FunCraft)
		{
			Application.OpenURL("http://www.tagincorp.com/privacypolicy.html");
		}
		else if (accountName == Accounts.AwesomeAddictiveGames)
		{
			Application.OpenURL("http://www.awesomeaddictivegames.com/privacypolicy.html");
		}
		else if (accountName == Accounts.GameBook || accountName == Accounts.BestFreeGames)
		{
			Application.OpenURL("http://www.gaminatorix.com/privacypolicy.html");
		}
		else if (accountName == Accounts.Blockot)
		{
			Application.OpenURL("http://blockot.com/blockotprivacy");
		}
	}
}
