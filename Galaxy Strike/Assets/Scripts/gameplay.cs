using UnityEngine;
using System.Collections;

public class gameplay : MonoBehaviour {

	public static bool LockScreen { get { return gamedata.player != 0; } }

	// Use this for initialization
	void Start () {

	}

	void NewProduction() {

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

	// Update is called once per frame
	void Update () {
	
	}
}
