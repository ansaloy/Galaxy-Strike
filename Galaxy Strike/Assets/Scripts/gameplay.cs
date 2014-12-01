using UnityEngine;
using System.Collections;

public class gameplay : MonoBehaviour {

	public static bool LockScreen { get { return gamedata.player != 0; } }


	void Start () {
	
	}

	// функция заповнення масивів/ Відобування ресурсів з планети; розбудова кораблів/ в
	void NewProduction() {
		// Planets GRAB resources
		int [] mineral = new int[5];
		for (int i = 0; i < gamedata.planetsLimit; i++) if (gamedata.planetsOwner[i] == gamedata.player) {
			for (int j = 0; j < 5; j++){
				mineral[j] = ((gamedata.planetsResource[i,j] >= gamedata.planetsMining[i,j]) ? gamedata.planetsMining[i,j] : gamedata.planetsResource[i,j]);
				gamedata.planetsResource[i,j] =- mineral[j];
				gamedata.playerResources[gamedata.player,j] =+ mineral[j];
			} 
		}
		// Planets Build Ships





	}

	//функція викликається по завершенню хода ігрока 
	public void NextPlayer() {
			gamedata.player++;
			if (gamedata.player == gamedata.playersCount) {
				gamedata.player = 0;
				gamedata.turn++;
				GameObject.Find("TurnLabel").GetComponent<UILabel>().text = " Turn: "+gamedata.turn.ToString();
			}
			NewProduction ();
		}


	void Update () {
	
	}
}
