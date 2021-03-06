﻿using UnityEngine;
using System.Collections;

public class chooseplayercount : MonoBehaviour {

	UIButton[] buttons;
	private UIButton selected;

	void SelectButton(UIButton select) {
		if (selected != select) {
			if (selected != null) {
				selected.defaultColor = select.defaultColor;
			}
			select.defaultColor = Color.blue;
			selected = select;
			gamedata.playersCount = int.Parse (selected.name);
			gamedata.playersRace = new int[gamedata.playersCount];
			for (int i=0; i < gamedata.playersCount; i++) gamedata.playersRace[i] = Random.Range(0,6); // !!! тимчасове заповнення різними расами гравців
		}
	}

	public void OnButtonClick() {
		SelectButton (UIButton.current);
	}

	// Use this for initialization
	void Start () {
		buttons = gameObject.GetComponentsInChildren<UIButton> ();
		SelectButton (buttons[1]);
	}
}