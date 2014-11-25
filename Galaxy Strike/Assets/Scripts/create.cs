using UnityEngine;
using System.Collections;

 public class create : MonoBehaviour {

	public GameObject prefabPlanets; // Префаб пустий для темпової відмальовки спрайтів Планет
	GameObject tmp; // Техничний вказивник на обєкт

	int planetsMax = 150; // максимальна кількість планет в ресурсі

	public static float spaceLimit = 100; // константа 50% - 100%    Розмір Галактики для наповнення планетами
	
	void Start () {
		PlanetsFit (); // Обраховуємо кількисть планет та розмищуємо їх в грі
		DrowPlanets ();
	}
	

	void PlanetsFit (){
		float voneplanet = 4 / 3 * Mathf.PI * Mathf.Pow (100, 3) / planetsMax; // обєм на одну планету
		gamedata.planetsLimit = (int) (4 / 3 * Mathf.PI * Mathf.Pow (spaceLimit, 3) / voneplanet);

		// planetsLimit = 7;
		// Заполнение сгенерированого игрового мира планетами из ресурса 
		bool [] planetsBit = new bool[planetsMax]; // бит попадания планеты из ресурса в сгенерированый мир игровой сцены
		gamedata.planetsID = new int[gamedata.planetsLimit];
		int i = 0;
		while (i < gamedata.planetsLimit) {
			int pos = Random.Range(0, planetsMax);
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
				gamedata.planetsPosition[i].x = Random.Range (-1*(float)spaceLimit,(float)spaceLimit);
				gamedata.planetsPosition[i].y = Random.Range (-1*(float)spaceLimit,(float)spaceLimit);
				gamedata.planetsPosition[i].z = Random.Range (-1*(float)spaceLimit,(float)spaceLimit);
				if (Vector3.Distance(gamedata.planetsPosition[i], Vector3.zero) < (float)spaceLimit) outSphere = false;
			}
		}
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