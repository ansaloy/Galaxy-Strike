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
		Application.LoadLevel (1);
	}

	public void LoadGame () {
		HideMainMenu ();
		loadGameMenu.SetActive(true);
	}

	public void Quit() {
		Application.Quit ();
	}
}
