using UnityEngine;
using System.Collections;

public class visualization : MonoBehaviour {
	public GameObject prefabPlanets; // Префаб пустий для темпової відмальовки спрайтів Планет
	GameObject tmp; // Техничний вказивник на обєкт

	void Start () {
		//gamedata.Save ("start");
		//gamedata.Load ("start");
		DrowPlanets ();
	}

	void DrowPlanets(){
		for (int i = 0; i < gamedata.planetsLimit; i++){
			tmp = (GameObject)Instantiate (prefabPlanets);
			tmp.transform.Find("Foreground").GetComponent<SpriteRenderer> ().sprite = gamedata.planetsSprite[gamedata.planetsID[i]];
			float scale = gamedata.planetsSize[gamedata.planetsID[i]]/128f;
			tmp.transform.Find ("Background").localScale = new Vector3(1.15f*scale,1.15f*scale,0f);
			tmp.transform.position = gamedata.planetsPosition[i];
			if (i >= 0 && i <= 9) tmp.name = "00"+i.ToString();
			if (i > 9 && i <= 99) tmp.name = "0"+i.ToString();
			if (i >= 100) tmp.name = i.ToString();
		}
	}
}