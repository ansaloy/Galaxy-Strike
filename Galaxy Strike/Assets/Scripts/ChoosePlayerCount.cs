﻿using UnityEngine;
using System.Collections;

public class ChoosePlayerCount : MonoBehaviour {

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