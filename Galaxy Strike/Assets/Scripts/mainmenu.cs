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
		chooserace[] btns = newGameMenu.GetComponentsInChildren<chooserace> ();
		for (int i = 0; i < gamedata.playersCount; i++) {
			btns[i].selected = gamedata.playersRace[i];
		}
		newGameMenu.SetActive (true);
	}

	public void StartGame() {
		chooserace[] btns = newGameMenu.GetComponentsInChildren<chooserace> ();
		int idx = 0;
		foreach (chooserace cr in btns) {
			if (cr.selected != -1) {
				gamedata.playersRace[idx] = cr.selected;
				++idx;
			}
		}
		gamedata.playersCount = idx;
		while ( idx < gamedata.playersRace.Length) {
			gamedata.playersRace[idx] = -1;
			idx++;
		}
		if ( gamedata.playersCount != 0 ) {
			gamedata.Generate ();
			Application.LoadLevel ("Galaxy Strike");
		}
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
