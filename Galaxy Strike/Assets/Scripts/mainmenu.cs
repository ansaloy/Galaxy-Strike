using UnityEngine;
using System.Collections;

public class mainmenu : MonoBehaviour {

	public GameObject mainMenu, newGameMenu, loadGameMenu;

	public void HideMainMenu () {
		mainMenu.SetActive (false);
	}

	public void ShowMainMenu() {
		newGameMenu.SetActive(false);
		loadGameMenu.SetActive(false);
		mainMenu.SetActive(true);
	}

	public void NewGame() {
		HideMainMenu ();
		newGameMenu.SetActive (true);
	}

	public void StartGame() {
		gamedata.Generate ();
		Application.LoadLevel ("Galaxy Strike");
	}

	public void LoadGameMenu () {
		HideMainMenu ();
		loadGameMenu.SetActive(true);
	}

	public void LoadGame() {
		if (fileview.selected != null) {
			gamedata.Load (fileview.selected);
			Application.LoadLevel("Galaxy Strike");
		}
	}

	public void Quit() {
		Application.Quit ();
	}
}
