using UnityEngine;
using System.Collections;

public class chooserace : MonoBehaviour {

	static string[] spriteNames = new string[] {"nobody","Wookiees","Stormtroopers","Darth Mauls","Yodas","Darth Vaders"};

	int race = -1;

	void ChangeRace() {
		transform.Find ("Face").GetComponent<UISprite> ().spriteName = spriteNames [race+1];
		transform.Find ("Name").GetComponent<UILabel> ().text = ((race == -1)?("Nobody"):(gamedata.raceName [race]));
	}

	public void OnChangeRace() {
		++race;
		if (race == gamedata.raceName.Length) {
			race = -1;
		}
		selected = race;
	}

	public int selected { 
		get {
				return race;
			}
		set {
				race = Mathf.Max (-1,value);
				race = Mathf.Min (race,gamedata.raceName.Length-1);
				ChangeRace();
			}
	}
}