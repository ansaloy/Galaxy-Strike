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
	public static int[] planetsSize; // розмір планети0
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
	public static bool[,] planetsShipsFlag; // флаг признак галочка будівництва даного корабля на планеті
	public static int[,] planetsShipsBuilding; // кількість балів накопичених на будівництво корабля. 
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
		float voneplanet = 4 / 3 * Mathf.PI * Mathf.Pow (100, 3) / planetsMax; // обєм на одну планету
		planetsLimit = (int) (4 / 3 * Mathf.PI * Mathf.Pow (spaceLimit, 3) / voneplanet);
		// Заполнение сгенерированого игрового мира планетами из ресурса 
		bool [] planetsBit = new bool[planetsMax]; // бит попадания планеты из ресурса в сгенерированый мир игровой сцены
		planetsID = new int[planetsLimit];
		int i = 0;
		while (i < planetsLimit) {
			int pos = Random.Range(0, planetsMax);
			if (!planetsBit[pos]){
				planetsID[i] = pos;
				planetsBit[pos] = true;
				i++;
			}
		}
		// Заполнение линейних связей между планетами (предыдущий, следующий)
		planetsConnection = new int[planetsLimit, 6]; 
		for (i = 0; i < planetsLimit; i++) planetsConnection[i, 0] = 2; // записуемо в лічильник звязків 2
		for (i = 1; i < planetsLimit - 1; i++){
			planetsConnection[i, 1] = i - 1;
			planetsConnection[i, 2] = i + 1;
		}
		planetsConnection[0, 1] = planetsLimit - 1;
		planetsConnection[0, 2] = 1;
		planetsConnection[planetsLimit - 1, 1] = planetsLimit - 2;
		planetsConnection[planetsLimit - 1, 2] = 0;
		// Заполнение дополнительних связей между планетами
		int connection = (int) Random.Range(planetsLimit,planetsLimit * 3) / 2; // кількість лінків, що слід утворити
		for(i = 0; i < connection; i++){
			bool accept = true;
			int pos1 = 0; int pos2 = 0; // номера планет, для поєднання лінком
			while (accept){
				pos1 = Random.Range(0, planetsLimit);
				pos2 = Random.Range(0, planetsLimit);
				if (planetsConnection[pos1, 0] < 5 && planetsConnection[pos2, 0] < 5) accept = false; // вийти якщо лінк допустимий
				if (pos1 == pos2) accept = true; // продовжити якщо лінк сам на себе
				for (int j = 1; j <= planetsConnection[pos1, 0]; j++) if (planetsConnection[pos1,j] == pos2) accept = true; // продовжити лінк на цей номер уже існує з цієї планети
				for (int j = 1; j <= planetsConnection[pos2, 0]; j++) if (planetsConnection[pos2,j] == pos1) accept = true; // продовжити лінк на цей номер уже існує з цієї планети
			}
			planetsConnection[pos1, 0]++;
			planetsConnection[pos2, 0]++;
			planetsConnection[pos1, planetsConnection[pos1, 0]] = pos2;
			planetsConnection[pos2, planetsConnection[pos2, 0]] = pos1;
		}
		
		planetsPosition = new Vector3[planetsLimit];
		for (i = 0; i < planetsLimit; i++) {
			bool outSphere = true;
			while (outSphere){
				planetsPosition[i].x = Random.Range (-1*(float)spaceLimit,(float)spaceLimit);
				planetsPosition[i].y = Random.Range (-1*(float)spaceLimit,(float)spaceLimit);
				planetsPosition[i].z = Random.Range (-1*(float)spaceLimit,(float)spaceLimit);
				if (Vector3.Distance(planetsPosition[i], Vector3.zero) < (float)spaceLimit) outSphere = false;
			}
		}
		
		planetsResource = new int[planetsLimit,5];
		int radius;
		int totalMinerals;
		for (i = 0; i < planetsLimit; i++) {
			radius = planetsSize[(planetsID[i])];
			totalMinerals = radius*Random.Range (5,15);
			//print (""+i+": "+radius+" => "+totalMinerals);
			planetsResource[i,4] = (int) (totalMinerals*Random.Range (0.02f,0.2f)); // 5%-20%
			totalMinerals -= planetsResource[i,4];
			planetsResource[i,3] = (int) (totalMinerals*Random.Range (0.1f,0.25f)); // 5%-25% от остатка
			totalMinerals -= planetsResource[i,3];
			planetsResource[i,2] = (int) (totalMinerals*Random.Range (0.15f,0.33f)); // 5%-33% от остатка
			totalMinerals -= planetsResource[i,2];
			planetsResource[i,1] = (int) (totalMinerals*Random.Range (0.2f,0.5f)); // 5%-50% от остатка
			totalMinerals -= planetsResource[i,1];
			planetsResource[i,0] = totalMinerals; // 100% от остатка
			//print (""+i+": "+planetsResource[i,0]+" "+planetsResource[i,1]+" "+planetsResource[i,2]+" "+planetsResource[i,3]+" "+planetsResource[i,4]+" ");
		}
		
		planetsMining = new int[planetsLimit,5];
		int totalMining;
		planetsPPP = new int[planetsLimit];
		int ppp;
		for (i = 0; i < planetsLimit; i++) {
			radius = planetsSize[(planetsID[i])];
			totalMining = Random.Range (radius/20,radius/5); // 100 -25 ходов
			ppp = Random.Range (4,radius/10);
			planetsPPP[i] = ppp;
			//print (""+i+": "+radius+" => "+totalMining);
			planetsMining[i,4] = (int) Mathf.Max (1f,(totalMining*Random.Range (0.05f,0.1f)));
			totalMining = (int) Mathf.Max (0,totalMining-planetsMining[i,4]);
			planetsMining[i,3] = (int) Mathf.Max (1f,(totalMining*Random.Range (0.1f,0.2f)));
			totalMining = (int) Mathf.Max (0,totalMining-planetsMining[i,3]);
			planetsMining[i,2] = (int) Mathf.Max (1f,(totalMining*Random.Range (0.2f,0.33f)));
			totalMining = (int) Mathf.Max (0,totalMining-planetsMining[i,2]);
			planetsMining[i,1] = (int) Mathf.Max (1f,(totalMining*Random.Range (0.33f,0.5f)));
			totalMining = (int) Mathf.Max (0,totalMining-planetsMining[i,1]);
			planetsMining[i,0] = (int) Mathf.Max (1f,totalMining);
			//print (""+i+": "+planetsResource[i,0]+"/"+planetsMining[i,0]+" "+planetsResource[i,1]+"/"+planetsMining[i,1]+" "+planetsResource[i,2]+"/"+planetsMining[i,2]+" "+planetsResource[i,3]+"/"+planetsMining[i,3]+" "+planetsResource[i,4]+"/"+planetsMining[i,4]+" ");
		}
		
		planetsOwner = new int[planetsLimit];
		for (i = 0; i < planetsLimit; i++) planetsOwner [i] = -1; //вносим признак планети -1 - вільна і нікому не належить. самостійна.
		i = 0;
		while (i < playersCount){
			int pos = Random.Range(0, planetsLimit);
			if (planetsOwner[pos] == -1){
				planetsOwner[pos] = i;
				i++;
			}
		}

		planetsShipsFlag = new bool[planetsLimit, 4];
		for (i = 0; i < planetsLimit; i++){
			planetsShipsFlag[i,0] = true;
			planetsShipsFlag[i,1] = true;
			planetsShipsFlag[i,2] = true;
			planetsShipsFlag[i,3] = true;
		}

		planetsShipsBuilding = new int[planetsLimit, 4]; // скільки накопичилось балів в будівництві кораблів на планетах
		planetsShipsFlot = new int[planetsLimit, 4]; // які кораблі на яких планетах
		for (i = 0; i < planetsLimit; i++){
			radius = planetsSize[(planetsID[i])];
			planetsShipsFlot[i,0] = (int) Random.Range(0.5f, radius / 50);
			planetsShipsFlot[i,1] = (int) Random.Range(0.25f, radius / 100);
			planetsShipsFlot[i,2] = (int) Random.Range(0.15f, radius / 150);
			planetsShipsFlot[i,3] = (int) Random.Range(0.1f, radius / 250);
		}
		moveShipsFlot = new int[planetsLimit,4];// пересилаемі між планетами кораблі атака або перемищення свого флоту на яку планету і скільки прилетіло
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
		for (int i = 0; i < planetsLimit; i++) str += planetsOwner[i].ToString() + ":"; // Хто власник планети, який гравець
		str = str.Remove (str.Length - 1);
		writer.WriteLine (str);
		writer.WriteLine ("planetsShipsFlag");  // Флаг будівництва корабля
		for (int i = 0; i < planetsLimit; i++) {
			if (planetsShipsFlag[i,0]) str = "1:"; else str = "0:";
			if (planetsShipsFlag[i,1]) str += "1:"; else str += "0:";
			if (planetsShipsFlag[i,2]) str += "1:"; else str += "0:";
			if (planetsShipsFlag[i,3]) str += "1"; else str += "0";
			writer.WriteLine (str);
		}
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

		reader.ReadLine(); // количество планет в генерірованом мире
		str = reader.ReadLine();
		planetsLimit = int.Parse (str);

		planetsID = new int[planetsLimit];
		reader.ReadLine(); // Індекси планет в генерованому світі з загальної ресурсної бази планет 
		strSplit = reader.ReadLine().Split(':');
		for (int i = 0; i < planetsLimit; i++) planetsID[i] = int.Parse(strSplit[i]);

		planetsConnection = new int[planetsLimit, 6]; 
		reader.ReadLine(); // Міжпланетні Звязки 
		for (int i = 0; i < planetsLimit; i++){
			strSplit = reader.ReadLine().Split(':');
			planetsConnection[i,0] = int.Parse(strSplit[0]);
			planetsConnection[i,1] = int.Parse(strSplit[1]);
			planetsConnection[i,2] = int.Parse(strSplit[2]);
			planetsConnection[i,3] = int.Parse(strSplit[3]);
			planetsConnection[i,4] = int.Parse(strSplit[4]);
			planetsConnection[i,5] = int.Parse(strSplit[5]);
		}

		planetsPosition = new Vector3[planetsLimit];
		reader.ReadLine(); // Координати планет в 3Д просторі
		for (int i = 0; i < planetsLimit; i++){
			strSplit = reader.ReadLine().Split(':');
			planetsPosition[i].x = float.Parse(strSplit[0]);
			planetsPosition[i].y = float.Parse(strSplit[1]);
			planetsPosition[i].z = float.Parse(strSplit[2]);
		}

		planetsResource = new int[planetsLimit,5];
		reader.ReadLine();  // Ресурсив на планеті внедрах
		for (int i = 0; i < planetsLimit; i++) {
			strSplit = reader.ReadLine().Split(':');
			planetsResource[i,0] = int.Parse(strSplit[0]);
			planetsResource[i,1] = int.Parse(strSplit[1]);
			planetsResource[i,2] = int.Parse(strSplit[2]);
			planetsResource[i,3] = int.Parse(strSplit[3]);
			planetsResource[i,4] = int.Parse(strSplit[4]);
		}

		planetsMining = new int[planetsLimit,5];
		reader.ReadLine();  // Видобуток ресурсив на планеті в турн
		for (int i = 0; i < planetsLimit; i++) {
			strSplit = reader.ReadLine().Split(':');
			planetsMining[i,0] = int.Parse(strSplit[0]);
			planetsMining[i,1] = int.Parse(strSplit[1]);
			planetsMining[i,2] = int.Parse(strSplit[2]);
			planetsMining[i,3] = int.Parse(strSplit[3]);
			planetsMining[i,4] = int.Parse(strSplit[4]);
		}

		planetsPPP = new int[planetsLimit];
		reader.ReadLine(); // Производственние Потужности Planets
		strSplit = reader.ReadLine().Split(':');
		for (int i = 0; i < planetsLimit; i++) planetsPPP[i] = int.Parse(strSplit[i]);

		planetsOwner = new int[planetsLimit];
		reader.ReadLine(); // Хто власник планети, який гравець
		strSplit = reader.ReadLine().Split(':');
		for (int i = 0; i < planetsLimit; i++) planetsOwner[i] = int.Parse(strSplit[i]);

		planetsShipsFlag = new bool[planetsLimit, 4];
		reader.ReadLine();// Флаг будівництва корабля
		for (int i = 0; i < planetsLimit; i++) {
			strSplit = reader.ReadLine().Split(':');
			if (strSplit[0] == "1") planetsShipsFlag[i,0] = true;
			if (strSplit[1] == "1") planetsShipsFlag[i,1] = true;
			if (strSplit[2] == "1") planetsShipsFlag[i,2] = true;
			if (strSplit[3] == "1") planetsShipsFlag[i,3] = true;
		}

		planetsShipsBuilding = new int[planetsLimit, 4];
		reader.ReadLine(); // кількість балів накопіченіх на будивніцтво коробля
		for (int i = 0; i < planetsLimit; i++) {
			strSplit = reader.ReadLine().Split(':');
			planetsShipsBuilding[i,0] = int.Parse(strSplit[0]);
			planetsShipsBuilding[i,1] = int.Parse(strSplit[1]);
			planetsShipsBuilding[i,2] = int.Parse(strSplit[2]);
			planetsShipsBuilding[i,3] = int.Parse(strSplit[3]);
		}

		planetsShipsFlot = new int[planetsLimit, 4];
		reader.ReadLine(); // кількість наявних короблів на пранетах
		for (int i = 0; i < planetsLimit; i++) {
			strSplit = reader.ReadLine().Split(':');
			planetsShipsFlot[i,0] = int.Parse(strSplit[0]);
			planetsShipsFlot[i,1] = int.Parse(strSplit[1]);
			planetsShipsFlot[i,2] = int.Parse(strSplit[2]);
			planetsShipsFlot[i,3] = int.Parse(strSplit[3]);
		}

		moveShipsFlot = new int[planetsLimit, 4];
		reader.ReadLine(); ;// пересилаемі між планетами кораблі атака або перемищення свого флоту на яку планету і скільки прилетіло
		for (int i = 0; i < planetsLimit; i++) {
			strSplit = reader.ReadLine().Split(':');
			moveShipsFlot[i,0] = int.Parse(strSplit[0]);
			moveShipsFlot[i,1] = int.Parse(strSplit[1]);
			moveShipsFlot[i,2] = int.Parse(strSplit[2]);
			moveShipsFlot[i,3] = int.Parse(strSplit[3]);
		}

		reader.Close ();
	}

	void Awake() {
		LoadData ();
	}
}		   