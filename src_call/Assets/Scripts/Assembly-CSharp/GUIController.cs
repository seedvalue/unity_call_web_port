using UnityEngine;

public class GUIController : MonoBehaviour
{
	private FormationSwitches formationHandler = new FormationSwitches();

	private void Start()
	{
		formationHandler.Init();
	}

	private void OnGUI()
	{
		formationHandler.OnGUI();
	}
}
