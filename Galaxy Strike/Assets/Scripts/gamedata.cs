using UnityEngine;
using System.Collections;

public class gamedata : MonoBehaviour {

	void Start (){
		DontDestroyOnLoad (gameObject);
	}

	public static int turn;
	public static int player; // 0 - humen, 1-4 - computers AI

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

	public static int playersCount; // килькисть гравців в згенерованому світі
	public static int[] playersNation; // индекс гравця та индек його раси 0,,3

	public static int[,] playerResources; // корзіна ресурсив игрока 0,,4
	public static int[,] playerShips;     // корзіна короблів гравця 0..3


	public static int[,] moveShipsFlot; // пересилаемі між планетами кораблі атака або перемищення свого флоту


	// завантажує базу даних в ресусні масиви + спрайти
	public static void LoadData() {
		planetsSprite = Resources.LoadAll <Sprite> ("");
	}


	// зберігає поточній стан грі в файл
	public static void Save(string fileName) {
		}

	// завантажує стан грі з файла
	public static void Load(string fileName) {
		}

	void Awake() {
		LoadData ();
	}
}
