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
	public static int player = 0; // 0 - humen, 1-4 - computers AI

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

	public static int[]  plantesOwner;   // кому належіть планета
	public static int[,] planetsShipsBuilding; // кількість балів накопіченіх на будивніцтво коробля 0,,3
	public static int[,] planetsShipsFlot; // кількість наявних короблів на пранетах

	public static int playersCount = 2; // килькисть гравців в згенерованому світі
	public static int[] playersRace; // индекс гравця та индек його раси 0,,4

	public static float spaceLimit = 100; // константа 50% - 100%    Розмір Галактики для наповнення планетами
	public static int[,] playerResources; // корзіна ресурсив игрока 0,,4
	public static int[,] playerShips;     // корзіна короблів гравця 0..3


	public static int[,] moveShipsFlot; // пересилаемі між планетами кораблі атака або перемищення свого флоту


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
			//print (planetsDescription[i]);
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


	// зберігає поточній стан грі в файл
	public static void Save(string fileName) {
		StreamWriter writer = new StreamWriter ("Assets/Save/"+fileName+".txt");
		writer.WriteLine ("turn");
		writer.WriteLine (turn); // номер хода
		writer.WriteLine ("player");
		writer.WriteLine (player); // 0 - humen, 1-4 - computers AI
		writer.WriteLine ("planetsLimit");
		writer.WriteLine (planetsLimit); // количество планет в генерірованом мире
		writer.WriteLine ("planetsID");
		string str = "";
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
		for (int i = 0; i < planetsLimit; i++) str += plantesOwner[i].ToString() + ":"; // Хто власнік планети, яка раса
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

		writer.Close ();
	}

	// завантажує стан гри з файла
	public static void Load(string fileName) {
	}

	void Awake() {
		LoadData ();
	}
}		   

/*


public static int playersCount = 2; // килькисть гравців в згенерованому світі
public static int[] playersRace; // индекс гравця та индек його раси 0,,4

public static float spaceLimit = 100; // константа 50% - 100%    Розмір Галактики для наповнення планетами
public static int[,] playerResources; // корзіна ресурсив игрока 0,,4
public static int[,] playerShips;     // корзіна короблів гравця 0..3


public static int[,] moveShipsFlot; // пересилаемі між планетами кораблі атака або перемищення свого флоту
*/