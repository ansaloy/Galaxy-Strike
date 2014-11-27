using UnityEngine;
using System.Collections;

 public class create : MonoBehaviour {
	public GameObject prefabPlanets; // Префаб пустий для темпової відмальовки спрайтів Планет
	GameObject tmp; // Техничний вказивник на обєкт

	void Start () {
		Generate (); // Обраховуємо кількисть планет та розмищуємо їх в грі
		gamedata.Save ("start");
		DrowPlanets ();
	}

	void Generate (){
		float voneplanet = 4 / 3 * Mathf.PI * Mathf.Pow (100, 3) / gamedata.planetsMax; // обєм на одну планету
		gamedata.planetsLimit = (int) (4 / 3 * Mathf.PI * Mathf.Pow (gamedata.spaceLimit, 3) / voneplanet);
		// Заполнение сгенерированого игрового мира планетами из ресурса 
		bool [] planetsBit = new bool[gamedata.planetsMax]; // бит попадания планеты из ресурса в сгенерированый мир игровой сцены
		gamedata.planetsID = new int[gamedata.planetsLimit];
		int i = 0;
		while (i < gamedata.planetsLimit) {
			int pos = Random.Range(0, gamedata.planetsMax);
			if (!planetsBit[pos]){
				gamedata.planetsID[i] = pos;
				planetsBit[pos] = true;
				i++;
			}
		}
		// Заполнение линейних связей между планетами (предыдущий, следующий)
		gamedata.planetsConnection = new int[gamedata.planetsLimit, 6]; 
		for (i = 0; i < gamedata.planetsLimit; i++) gamedata.planetsConnection[i, 0] = 2; // записуемо в лічильник звязків 2
		for (i = 1; i < gamedata.planetsLimit - 1; i++){
			gamedata.planetsConnection[i, 1] = i - 1;
			gamedata.planetsConnection[i, 2] = i + 1;
		}
		gamedata.planetsConnection[0, 1] = gamedata.planetsLimit - 1;
		gamedata.planetsConnection[0, 2] = 1;
		gamedata.planetsConnection[gamedata.planetsLimit - 1, 1] = gamedata.planetsLimit - 2;
		gamedata.planetsConnection[gamedata.planetsLimit - 1, 2] = 0;
		// Заполнение дополнительних связей между планетами
		int connection = (int) Random.Range(gamedata.planetsLimit,gamedata.planetsLimit * 3) / 2; // кількість лінків, що слід утворити
		for(i = 0; i < connection; i++){
			bool accept = true;
			int pos1 = 0; int pos2 = 0; // номера планет, для поєднання лінком
			while (accept){
				pos1 = Random.Range(0, gamedata.planetsLimit);
				pos2 = Random.Range(0, gamedata.planetsLimit);
				if (gamedata.planetsConnection[pos1, 0] < 5 && gamedata.planetsConnection[pos2, 0] < 5) accept = false; // вийти якщо лінк допустимий
				if (pos1 == pos2) accept = true; // продовжити якщо лінк сам на себе
				for (int j = 1; j <= gamedata.planetsConnection[pos1, 0]; j++) if (gamedata.planetsConnection[pos1,j] == pos2) accept = true; // продовжити лінк на цей номер уже існує з цієї планети
				for (int j = 1; j <= gamedata.planetsConnection[pos2, 0]; j++) if (gamedata.planetsConnection[pos2,j] == pos1) accept = true; // продовжити лінк на цей номер уже існує з цієї планети
			}
			gamedata.planetsConnection[pos1, 0]++;
			gamedata.planetsConnection[pos2, 0]++;
			gamedata.planetsConnection[pos1, gamedata.planetsConnection[pos1, 0]] = pos2;
			gamedata.planetsConnection[pos2, gamedata.planetsConnection[pos2, 0]] = pos1;
		}

		gamedata.planetsPosition = new Vector3[gamedata.planetsLimit];
		for (i = 0; i < gamedata.planetsLimit; i++) {
			bool outSphere = true;
			while (outSphere){
				gamedata.planetsPosition[i].x = Random.Range (-1*(float)gamedata.spaceLimit,(float)gamedata.spaceLimit);
				gamedata.planetsPosition[i].y = Random.Range (-1*(float)gamedata.spaceLimit,(float)gamedata.spaceLimit);
				gamedata.planetsPosition[i].z = Random.Range (-1*(float)gamedata.spaceLimit,(float)gamedata.spaceLimit);
				if (Vector3.Distance(gamedata.planetsPosition[i], Vector3.zero) < (float)gamedata.spaceLimit) outSphere = false;
			}
		}

		gamedata.planetsResource = new int[gamedata.planetsLimit,5];
		int radius;
		int totalMinerals;
		for (i = 0; i < gamedata.planetsLimit; i++) {
			radius = gamedata.planetsSize[(gamedata.planetsID[i])];
			totalMinerals = radius*Random.Range (5,15);
			//print (""+i+": "+radius+" => "+totalMinerals);
			gamedata.planetsResource[i,4] = (int) (totalMinerals*Random.Range (0.02f,0.2f)); // 5%-20%
			totalMinerals -= gamedata.planetsResource[i,4];
			gamedata.planetsResource[i,3] = (int) (totalMinerals*Random.Range (0.1f,0.25f)); // 5%-25% от остатка
			totalMinerals -= gamedata.planetsResource[i,3];
			gamedata.planetsResource[i,2] = (int) (totalMinerals*Random.Range (0.15f,0.33f)); // 5%-33% от остатка
			totalMinerals -= gamedata.planetsResource[i,2];
			gamedata.planetsResource[i,1] = (int) (totalMinerals*Random.Range (0.2f,0.5f)); // 5%-50% от остатка
			totalMinerals -= gamedata.planetsResource[i,1];
			gamedata.planetsResource[i,0] = totalMinerals; // 100% от остатка
			//print (""+i+": "+gamedata.planetsResource[i,0]+" "+gamedata.planetsResource[i,1]+" "+gamedata.planetsResource[i,2]+" "+gamedata.planetsResource[i,3]+" "+gamedata.planetsResource[i,4]+" ");
		}

		gamedata.planetsMining = new int[gamedata.planetsLimit,5];
		int totalMining;
		gamedata.planetsPPP = new int[gamedata.planetsLimit];
		int ppp;
		for (i = 0; i < gamedata.planetsLimit; i++) {
			radius = gamedata.planetsSize[(gamedata.planetsID[i])];
			totalMining = Random.Range (radius/20,radius/5); // 100 -25 ходов
			ppp = Random.Range (totalMining,totalMining*3)-totalMining;
			gamedata.planetsPPP[i] = ppp;
			//print (""+i+": "+radius+" => "+totalMining);
			gamedata.planetsMining[i,4] = (int) Mathf.Max (1f,(totalMining*Random.Range (0.05f,0.1f)));
			totalMining = (int) Mathf.Max (0,totalMining-gamedata.planetsMining[i,4]);
			gamedata.planetsMining[i,3] = (int) Mathf.Max (1f,(totalMining*Random.Range (0.1f,0.2f)));
			totalMining = (int) Mathf.Max (0,totalMining-gamedata.planetsMining[i,3]);
			gamedata.planetsMining[i,2] = (int) Mathf.Max (1f,(totalMining*Random.Range (0.2f,0.33f)));
			totalMining = (int) Mathf.Max (0,totalMining-gamedata.planetsMining[i,2]);
			gamedata.planetsMining[i,1] = (int) Mathf.Max (1f,(totalMining*Random.Range (0.33f,0.5f)));
			totalMining = (int) Mathf.Max (0,totalMining-gamedata.planetsMining[i,1]);
			gamedata.planetsMining[i,0] = (int) Mathf.Max (1f,totalMining);
			//print (""+i+": "+gamedata.planetsResource[i,0]+"/"+gamedata.planetsMining[i,0]+" "+gamedata.planetsResource[i,1]+"/"+gamedata.planetsMining[i,1]+" "+gamedata.planetsResource[i,2]+"/"+gamedata.planetsMining[i,2]+" "+gamedata.planetsResource[i,3]+"/"+gamedata.planetsMining[i,3]+" "+gamedata.planetsResource[i,4]+"/"+gamedata.planetsMining[i,4]+" ");
		}

		gamedata.plantesOwner = new int[gamedata.planetsLimit];
		for (i = 0; i < gamedata.planetsLimit; i++) gamedata.plantesOwner [i] = 8; //вносим признак планети 8 - вільна і нікому не належить. самостійна.
		i = 0;
		while (i < gamedata.playersCount){
			int pos = Random.Range(0, gamedata.planetsLimit);
			if (gamedata.plantesOwner[pos] == 8){
				gamedata.plantesOwner[pos] = i;
				i++;
			}
		}
		gamedata.planetsShipsBuilding = new int[gamedata.planetsLimit, 4]; // скільки накопичилось балів в будівництві кораблів на планетах
		gamedata.planetsShipsFlot = new int[gamedata.planetsLimit, 4]; // які кораблі на яких планетах
		gamedata.moveShipsFlot = new int[gamedata.planetsLimit,4];// пересилаемі між планетами кораблі атака або перемищення свого флоту на яку планету і скільки прилетіло
	}

	void DrowPlanets(){
		for (int i = 0; i < gamedata.planetsLimit; i++){
			tmp = (GameObject)Instantiate (prefabPlanets);
			tmp.GetComponent<SpriteRenderer> ().sprite = gamedata.planetsSprite[gamedata.planetsID[i]];
			tmp.transform.position = gamedata.planetsPosition[i];
			if (i >= 0 && i <= 9) tmp.name = "00"+i.ToString();
			if (i > 9 && i <= 99) tmp.name = "0"+i.ToString();
			if (i >= 100) tmp.name = i.ToString();
		}
	}
}