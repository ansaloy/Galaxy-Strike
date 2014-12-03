using UnityEngine;
using System.Collections;

public class gameplay : MonoBehaviour {
	public static bool LockScreen { get { return gamedata.player != 0; } }
	int [] PlanetsStatusAI = new int[150];
	void Start () {
	}
	void Update () {	
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

	// функция заповнення масивів/ Відобування ресурсів з планети; розбудова кораблів/ в
	void NewProduction() {
		// Planets GRAB resources
		int [] mineral = new int[5];
		for (int i = 0; i < gamedata.planetsLimit; i++) if (gamedata.planetsOwner[i] == gamedata.player) {
			for (int j = 0; j < 5; j++){
				mineral[j] = ((gamedata.planetsResource[i,j] >= gamedata.planetsMining[i,j]) ? gamedata.planetsMining[i,j] : gamedata.planetsResource[i,j]);
				gamedata.planetsResource[i,j] -= mineral[j];
				gamedata.playerResources[gamedata.player,j] += mineral[j];
			} 
		}

		// Planets Build Ships
		int deltaPPP; 

		for (int i = 0; i < gamedata.planetsLimit; i++) if (gamedata.planetsOwner[i] == gamedata.player) {
			deltaPPP = gamedata.planetsPPP[i]; 
			// додаткова прибавка потужностей на будівніцтво кораблів в в разі простою видобутку
			deltaPPP += (gamedata.planetsResource[i,0] == 0) ? gamedata.planetsMining[i,0] : 0;
			deltaPPP += (gamedata.planetsResource[i,1] == 0) ? gamedata.planetsMining[i,1] : 0;
			deltaPPP += (gamedata.planetsResource[i,2] == 0) ? gamedata.planetsMining[i,2] : 0;
			deltaPPP += (gamedata.planetsResource[i,3] == 0) ? gamedata.planetsMining[i,3] : 0;
			deltaPPP += (gamedata.planetsResource[i,4] == 0) ? gamedata.planetsMining[i,4] : 0;
			if (gamedata.planetsShipsFlag[i,0] || gamedata.planetsShipsFlag[i,1] || gamedata.planetsShipsFlag[i,2] || gamedata.planetsShipsFlag[i,3]){ // стоїть флажок будівніцтва хоч на одному з кораблів
				while (deltaPPP > 0 ){
					if (gamedata.planetsShipsFlag[i,0] && deltaPPP > 0) {gamedata.planetsShipsBuilding[i,0]++; deltaPPP--;}
					if (gamedata.planetsShipsFlag[i,1] && deltaPPP > 0) {gamedata.planetsShipsBuilding[i,1]++; deltaPPP--;}
					if (gamedata.planetsShipsFlag[i,2] && deltaPPP > 0) {gamedata.planetsShipsBuilding[i,2]++; deltaPPP--;}
					if (gamedata.planetsShipsFlag[i,3] && deltaPPP > 0) {gamedata.planetsShipsBuilding[i,3]++; deltaPPP--;}
				}
			}
			int buildedShips = 0;
			int race = gamedata.playersRace[gamedata.player]; //  якої раси гравець
			if (gamedata.planetsShipsFlag[i,0]) buildedShips = Mathf.Abs(gamedata.planetsShipsBuilding[i,0] / 10); else buildedShips = 0; // кількисть кораблів 0-го ТИПУ для побудови з накопіченіх ППП
			while (buildedShips > 0){ //  Будуємо кораблі 0-го типу
				if (gamedata.playerResources[gamedata.player,0] >= gamedata.shipsCost[race,0,0] && gamedata.playerResources[gamedata.player,1] >= gamedata.shipsCost[race,0,1] && gamedata.playerResources[gamedata.player,2] >= gamedata.shipsCost[race,0,2] && gamedata.playerResources[gamedata.player,3] >= gamedata.shipsCost[race,0,3] && gamedata.playerResources[gamedata.player,4] >= gamedata.shipsCost[race,0,4]){
					gamedata.planetsShipsFlot[i,0]++;// помістити корабель на планету
					gamedata.playerShips[gamedata.player,0]++;// додати корабель в кошик
					for(int j=0; j < 5; j++) gamedata.playerResources[gamedata.player,j] -= gamedata.shipsCost[race,0,j];// оплата ресурсами за побудований корабель
				}
			}
			if (gamedata.planetsShipsFlag[i,1]) buildedShips = Mathf.Abs(gamedata.planetsShipsBuilding[i,1] / 20); else buildedShips = 0; // кількисть кораблів 1-го ТИПУ для побудови з накопіченіх ППП
			while (buildedShips > 0){ //  Будуємо кораблі 1-го типу
				if (gamedata.playerResources[gamedata.player,0] >= gamedata.shipsCost[race,1,0] && gamedata.playerResources[gamedata.player,1] >= gamedata.shipsCost[race,1,1] && gamedata.playerResources[gamedata.player,2] >= gamedata.shipsCost[race,1,2] && gamedata.playerResources[gamedata.player,3] >= gamedata.shipsCost[race,1,3] && gamedata.playerResources[gamedata.player,4] >= gamedata.shipsCost[race,1,4]){
					gamedata.planetsShipsFlot[i,1]++;// помістити корабель на планету
					gamedata.playerShips[gamedata.player,1]++;// додати корабель в кошик
					for(int j=0; j < 5; j++) gamedata.playerResources[gamedata.player,j] -= gamedata.shipsCost[race,1,j];// оплата ресурсами за побудований корабель
				}
			}
			if (gamedata.planetsShipsFlag[i,2]) buildedShips = Mathf.Abs(gamedata.planetsShipsBuilding[i,2] / 30); else buildedShips = 0; // кількисть кораблів 2-го ТИПУ для побудови з накопіченіх ППП
			while (buildedShips > 0){ //  Будуємо кораблі 2-го типу
				if (gamedata.playerResources[gamedata.player,0] >= gamedata.shipsCost[race,2,0] && gamedata.playerResources[gamedata.player,1] >= gamedata.shipsCost[race,2,1] && gamedata.playerResources[gamedata.player,2] >= gamedata.shipsCost[race,2,2] && gamedata.playerResources[gamedata.player,3] >= gamedata.shipsCost[race,2,3] && gamedata.playerResources[gamedata.player,4] >= gamedata.shipsCost[race,2,4]){
					gamedata.planetsShipsFlot[i,2]++;// помістити корабель на планету
					gamedata.playerShips[gamedata.player,2]++;// додати корабель в кошик
					for(int j=0; j < 5; j++) gamedata.playerResources[gamedata.player,j] -= gamedata.shipsCost[race,2,j];// оплата ресурсами за побудований корабель
				}
			}
			if (gamedata.planetsShipsFlag[i,3]) buildedShips = Mathf.Abs(gamedata.planetsShipsBuilding[i,3] / 50); else buildedShips = 0; // кількисть кораблів 2-го ТИПУ для побудови з накопіченіх ППП
			while (buildedShips > 0){ //  Будуємо кораблі 3-го типу
				if (gamedata.playerResources[gamedata.player,0] >= gamedata.shipsCost[race,3,0] && gamedata.playerResources[gamedata.player,1] >= gamedata.shipsCost[race,3,1] && gamedata.playerResources[gamedata.player,2] >= gamedata.shipsCost[race,3,2] && gamedata.playerResources[gamedata.player,3] >= gamedata.shipsCost[race,3,3] && gamedata.playerResources[gamedata.player,4] >= gamedata.shipsCost[race,3,4]){
					gamedata.planetsShipsFlot[i,3]++;// помістити корабель на планету
					gamedata.playerShips[gamedata.player,3]++;// додати корабель в кошик
					for(int j=0; j < 5; j++) gamedata.playerResources[gamedata.player,j] -= gamedata.shipsCost[race,3,j];// оплата ресурсами за побудований корабель
				}
			}
		}
	}

	// логика Computers AI
	void ComputerAI(){
		DetectPlanetsStatusAI ();

	}
	// Встановлення статусу планет: 0 - невизначений/ігнорувати; 1 - захоплювати планету; 2 - своя прикордонна планета межує з противником; 3 - своя межує з сірими та своїми 
	void DetectPlanetsStatusAI(){
		for (int i = 0; i < gamedata.planetsLimit; i++){

			if (gamedata.planetsOwner[i] == gamedata.player) {
				if (ConnectedMyAndFreeAI(i)) PlanetsStatusAI[i] = 3; // 3 - своя межує з сірими та своїми
				else PlanetsStatusAI[i] = 2; // 2 - своя прикордонна планета межує з противником;
			}
			else{
				if (ConnectedWhithMyAI(i)) PlanetsStatusAI[i] = 1; // 1 - захоплювати планету cіру або противника
				else PlanetsStatusAI[i] = 0; // 0 - невизначений/ігнорувати чужа планета і з моїми не поеднана
			}

		} 
	}
	// Перебираю лінкі чи всі сусідні планети мої і пусті 
	bool ConnectedMyAndFreeAI(int plnNumb){
		bool result = true;
		for (int i = 1; i <= gamedata.planetsConnection[plnNumb,0]; i++) if (gamedata.planetsOwner[gamedata.planetsConnection[plnNumb,i]] > 0) result = false; // -1 планета нічия, 0 - своя
		return result;
	}
	// перебіраю лінки чи є хоч одна моя планети в сусідах
	bool ConnectedWhithMyAI(int plnNumb){
		bool result = false;
		for (int i = 1; i <= gamedata.planetsConnection[plnNumb,0]; i++) if (gamedata.planetsOwner[gamedata.planetsConnection[plnNumb,i]] == 0) result = true; // 0 - своя
		return result;
	}
	
}