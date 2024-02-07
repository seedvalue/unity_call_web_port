using UnityEngine;

public class ButtonsEventHandler : MonoBehaviour
{
	public GameObject[] AllGames;

	public GameObject[] SelectedButtons;

	private int check;

	private void OpenNewGames(int i)
	{
		if (check != i)
		{
			DisableAll();
			AllGames[0].SetActive(true);
			SelectedButtons[0].SetActive(true);
			check = i;
		}
	}

	private void OpenAllGames(int i)
	{
		if (check != i)
		{
			DisableAll();
			AllGames[1].SetActive(true);
			SelectedButtons[1].SetActive(true);
			check = i;
		}
	}

	private void OpenSimulationGames(int i)
	{
		if (check != i)
		{
			DisableAll();
			AllGames[2].SetActive(true);
			SelectedButtons[2].SetActive(true);
			check = i;
		}
	}

	private void OpenActionGames(int i)
	{
		if (check != i)
		{
			DisableAll();
			AllGames[3].SetActive(true);
			SelectedButtons[3].SetActive(true);
			check = i;
		}
	}

	public void OpenThisPanel(int i)
	{
		switch (i)
		{
		case 0:
			OpenNewGames(i);
			break;
		case 1:
			OpenAllGames(i);
			break;
		case 2:
			OpenSimulationGames(i);
			break;
		case 3:
			OpenActionGames(i);
			break;
		}
	}

	public void DownLoadClicked()
	{
		int selectedIndex = AllGames[check].GetComponent<ObjectCreator>().GetSelectedIndex();
		AllGames[check].GetComponent<ObjectCreator>().objectOfLoadAssest.OnFeatureClick(check, selectedIndex);
	}

	public void BackPressed()
	{
		base.gameObject.SetActive(false);
	}

	private void DisableAll()
	{
		GameObject[] allGames = AllGames;
		foreach (GameObject gameObject in allGames)
		{
			gameObject.SetActive(false);
		}
		GameObject[] selectedButtons = SelectedButtons;
		foreach (GameObject gameObject2 in selectedButtons)
		{
			gameObject2.SetActive(false);
		}
	}
}
