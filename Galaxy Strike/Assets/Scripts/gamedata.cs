using UnityEngine;
using System.Collections;
using System.IO;

public class gamedata : MonoBehaviour {

	void Start (){
		DontDestroyOnLoad (gameObject);
	}

	public static string[] raceName;
	public static int[,,] shipsCost;
	
	public static int turn = 0;
	public static int player = 0; // хто ходить в даний момент 0 - humen, 1-4 - computers AI

	public static int playersCount; // килькисть гравців в згенерованому світі

	public static Color[] playersColor = new Color[] {Color.red,Color.blue,Color.green,Color.yellow,Color.cyan}; //Кольори гравців

	public static int[] playersRace = new int[5]; // индекс гравця та индек його раси 0,,4
	public static int[,] playerResources = new int[5,5]; // корзіна ресурсів 5 позицій
	public static int[,] playerShips = new int[5,4];     // корзіна короблів гравця 4 позиції


	public static float spaceLimit = 100; // константа 50% - 100%    Розмір Галактики для наповнення планетами
	public static int planetsMax = 150; // максимальна кількість планет в ресурсі
	public static Sprite [] planetsSprite; // Масив зображень з ресурсів всіх Планет
	public static string[] planetsName; // Назва планет
	public static int[] planetsSize; // розмір планети
	public static int[] planetsType; // backgrounds menu
	public static string[] planetsDescription; // Текстовий Опис история планети

	public static int planetsLimit; // Розрахунок кількості планет в новоствореній карті
	public static int [] planetsID; // Индекс № планет в Resourse папке
	public static int [,] planetsConnection; // связи планеты. в [,0] - количество линков в 1,2,3,4,5 номера планет скем есть связь.
	public static Vector3 [] planetsPosition; // координати планет в просторі

	public static int[,] planetsResource; // килькисть ресурсив на планеті внедрах
	public static int[,] planetsMining; // килькисть ресурсив добуваєміх за добу

	public static int[] planetsPPP; // Производственние Потужности Planets

	public static int[]  planetsOwner;   // кому належіть планета
	public static int[,] planetsShipsBuilding; // кількість балів накопичених на будівництво корабля. увага значення -1 корабель не відмічений галочкою в діалозі для будівництва.
	public static int[,] planetsShipsFlot; // кількість наявних короблів на пранетах

	public static int[,] moveShipsFlot; // пересилаемі між планетами кораблі атака або перемищення свого флоту на яку планету і скільки прилетіло


	// завантажує базу даних в ресусні масиви + спрайти
	public static void LoadData() {
		planetsSprite = Resources.LoadAll <Sprite> ("");
		int size = planetsSprite.Length;

		planetsName = new string[size]; // Назва планет
		planetsSize = new int[size]; // розмір планети
		planetsType = new int[size] ; // backgrounds menu
		planetsDescription = new string[size]; // Текстовий Опис история планети

		StreamReader reader = new StreamReader ("Assets/Data/planets.txt");
		string [] readedData;
		int i = 0;
		while (!reader.EndOfStream) {
			readedData = reader.ReadLine ().Split(':');
			planetsName[i] = readedData[1];
			planetsSize[i] = int.Parse (readedData[2]);
			planetsType[i] = int.Parse (readedData[3]);
			planetsDescription[i] = readedData[4];
			i++;
		}
		reader.Close ();

		raceName = new string[5];
		reader = new StreamReader ("Assets/Data/racenames.txt");
		for (i = 0; i < 5; i++) raceName[i] = reader.ReadLine();
		reader.Close ();

		shipsCost = new int[5,4,5];
		reader = new StreamReader ("Assets/Data/shipscost.txt");
		for (int raceNumber = 0; raceNumber < 5; raceNumber++)for(int shipNumber = 0; shipNumber < 4; shipNumber++){
			readedData = reader.ReadLine ().Split(':');
			for (int mineralNumber = 0; mineralNumber < 5; mineralNumber++) shipsCost[raceNumber, shipNumber, mineralNumber] = int.Parse (readedData[mineralNumber]);
		}
		reader.Close ();
	}
	
	
	public static void Generate () {
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
		
		gamedata.planetsOwner = new int[gamedata.planetsLimit];
		for (i = 0; i < gamedata.planetsLimit; i++) gamedata.planetsOwner [i] = -1; //вносим признак планети -1 - вільна і нікому не належить. самостійна.
		i = 0;
		while (i < gamedata.playersCount){
			int pos = Random.Range(0, gamedata.planetsLimit);
			if (gamedata.planetsOwner[pos] == -1){
				gamedata.planetsOwner[pos] = i;
				i++;
			}
		}
		gamedata.planetsShipsBuilding = new int[gamedata.planetsLimit, 4]; // скільки накопичилось балів в будівництві кораблів на планетах
		gamedata.planetsShipsFlot = new int[gamedata.planetsLimit, 4]; // які кораблі на яких планетах
		gamedata.moveShipsFlot = new int[gamedata.planetsLimit,4];// пересилаемі між планетами кораблі атака або перемищення свого флоту на яку планету і скільки прилетіло
	}

	// зберігає поточній стан грі в файл
	public static void Save(string fileName) {
		StreamWriter writer = new StreamWriter ("Assets/Save/"+fileName+".txt");
		writer.WriteLine ("turn");
		writer.WriteLine (turn); // номер хода
		writer.WriteLine ("playersCount"); // килькисть гравців в згенерованому світі
		writer.WriteLine (playersCount);
		writer.WriteLine ("playersRace"); // якої раси гравці
		string str = "";
		for (int i = 0; i < playersCount; i++) str += playersRace[i].ToString() + ":";
		str = str.Remove (str.Length - 1);
		writer.WriteLine (str);
		writer.WriteLine ("playerResources"); 
		for (int i = 0; i < playersCount; i++){ // корзіна ресурсив игрока 0,,4
			str = playerResources[i,0].ToString()+":"+playerResources[i,1].ToString()+":"+playerResources[i,2].ToString()+":"+playerResources[i,3].ToString()+":"+playerResources[i,4].ToString();
			writer.WriteLine (str);
		}
		writer.WriteLine ("playerShips"); 
		for (int i = 0; i < playersCount; i++){// корзіна короблів гравця 0..3
			str = playerShips[i,0].ToString()+":"+playerShips[i,1].ToString()+":"+playerShips[i,2].ToString()+":"+playerShips[i,3].ToString();
			writer.WriteLine (str);
		}
		writer.WriteLine ("planetsLimit");
		writer.WriteLine (planetsLimit); // количество планет в генерірованом мире
		writer.WriteLine ("planetsID");
		str = "";
		for (int i = 0; i < planetsLimit; i++) str += planetsID[i].ToString() + ":"; // Індекси планет в генерованому світі з загальної ресурсної бази планет 
		str = str.Remove (str.Length - 1);
		writer.WriteLine (str);
		writer.WriteLine ("planetsConnection");
		for (int i = 0; i < planetsLimit; i++){ // Міжпланетні Звязки 
			str = planetsConnection[i,0].ToString()+":"+planetsConnection[i,1].ToString()+":"+planetsConnection[i,2].ToString()+":"+planetsConnection[i,3].ToString()+":"+planetsConnection[i,4].ToString()+":"+planetsConnection[i,5].ToString();
			writer.WriteLine (str);
		}
		writer.WriteLine ("planetsPosition");
		for (int i = 0; i < planetsLimit; i++){ // Координати планет в 3Д просторі
			str = planetsPosition[i].x.ToString()+":"+planetsPosition[i].y.ToString()+":"+planetsPosition[i].z.ToString();
			writer.WriteLine (str);
		}
		writer.WriteLine ("planetsResource");
		for (int i = 0; i < planetsLimit; i++){ // Ресурсив на планеті внедрах
			str = planetsResource[i,0].ToString()+":"+planetsResource[i,1].ToString()+":"+planetsResource[i,2].ToString()+":"+planetsResource[i,3].ToString()+":"+planetsResource[i,4].ToString();
			writer.WriteLine (str);
		}
		writer.WriteLine ("planetsMining");
		for (int i = 0; i < planetsLimit; i++){ // Видобуток ресурсив на планеті в турн
			str = planetsMining[i,0].ToString()+":"+planetsMining[i,1].ToString()+":"+planetsMining[i,2].ToString()+":"+planetsMining[i,3].ToString()+":"+planetsMining[i,4].ToString();
			writer.WriteLine (str);
		}
		writer.WriteLine ("planetsPPP");
		str = "";
		for (int i = 0; i < planetsLimit; i++) str += planetsPPP[i].ToString() + ":"; // Производственние Потужности Planets
		str = str.Remove (str.Length - 1);
		writer.WriteLine (str);
		writer.WriteLine ("plantesOwner");
		str = "";
		for (int i = 0; i < planetsLimit; i++) str += planetsOwner[i].ToString() + ":"; // Хто власнік планети, яка раса
		str = str.Remove (str.Length - 1);
		writer.WriteLine (str);
		writer.WriteLine ("planetsShipsBuilding");
		for (int i = 0; i < planetsLimit; i++){ // кількість балів накопіченіх на будивніцтво коробля
			str = planetsShipsBuilding[i,0].ToString()+":"+planetsShipsBuilding[i,1].ToString()+":"+planetsShipsBuilding[i,2].ToString()+":"+planetsShipsBuilding[i,3].ToString();
			writer.WriteLine (str);
		}
		writer.WriteLine ("planetsShipsFlot");
		for (int i = 0; i < planetsLimit; i++){ // кількість наявних короблів на пранетах
			str = planetsShipsFlot[i,0].ToString()+":"+planetsShipsFlot[i,1].ToString()+":"+planetsShipsFlot[i,2].ToString()+":"+planetsShipsFlot[i,3].ToString();
			writer.WriteLine (str);
		}
		writer.WriteLine ("moveShipsFlot");
		for (int i = 0; i < planetsLimit; i++){ // пересилаемі між планетами кораблі атака або перемищення свого флоту на яку планету і скільки прилетіло
			str = moveShipsFlot[i,0].ToString()+":"+moveShipsFlot[i,1].ToString()+":"+moveShipsFlot[i,2].ToString()+":"+moveShipsFlot[i,3].ToString();
			writer.WriteLine (str);
		}
		writer.Close ();
	}

	// завантажує стан гри з файла
	public static void Load(string fileName) {
		string str;
		string [] strSplit;
		StreamReader reader = new StreamReader ("Assets/Save/"+fileName+".txt");

		reader.ReadLine(); // номер хода
		str = reader.ReadLine();
		turn = int.Parse (str);

		reader.ReadLine();  // килькисть гравців в згенерованому світі
		str = reader.ReadLine();
		playersCount = int.Parse (str);

		reader.ReadLine(); // якої раси гравці
		strSplit = reader.ReadLine().Split(':');
		for (int i = 0; i < playersCount; i++) playersRace [i] = int.Parse(strSplit[i]);
	
		reader.ReadLine();  // корзіна ресурсив игрока 0,,4
		for (int i = 0; i < playersCount; i++) {
			strSplit = reader.ReadLine().Split(':');
			playerResources[i,0] = int.Parse(strSplit[0]);
			playerResources[i,1] = int.Parse(strSplit[1]);
			playerResources[i,2] = int.Parse(strSplit[2]);
			playerResources[i,3] = int.Parse(strSplit[3]);
			playerResources[i,4] = int.Parse(strSplit[4]);
		
		}

		reader.ReadLine(); // корзіна короблів гравця 0..3
		for (int i = 0; i < playersCount; i++){
			strSplit = reader.ReadLine().Split(':');
			playerShips[i,0] = int.Parse(strSplit[0]);
			playerShips[i,1] = int.Parse(strSplit[1]);
			playerShips[i,2] = int.Parse(strSplit[2]);
			playerShips[i,3] = int.Parse(strSplit[3]);
		}


		//print (playerShips[0,3]);
		//print (playerShips[1,3]);

		reader.Close ();
	}

	void Awake() {
		LoadData ();
	}
}		   