﻿using UnityEngine;
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
					gamedata.playerShips[gamedata.player,0]++;
					for(int j=0; j < 5; j++) gamedata.playerResources[gamedata.player,j] -= gamedata.shipsCost[race,0,j];
				}
			}



			/*
			buildedShips = Mathf.Abs(gamedata.planetsShipsBuilding[i,0] / 20); // кількисть кораблів 1-го ТИПУ для побудови з накопіченіх ППП
			while (buildedShips > 0){ //  Будуємо кораблі 0-го типу
				if (gamedata.playerResources[gamedata.player,0] >= gamedata.shipsCost[race,0,0] && gamedata.playerResources[gamedata.player,1] >= gamedata.shipsCost[race,0,1] && gamedata.playerResources[gamedata.player,2] >= gamedata.shipsCost[race,0,2] && gamedata.playerResources[gamedata.player,3] >= gamedata.shipsCost[race,0,3] && gamedata.playerResources[gamedata.player,4] >= gamedata.shipsCost[race,0,4]){
					gamedata.playerShips[gamedata.player,0]++;
					for(int j=0; j < 5; j++) gamedata.playerResources[gamedata.player,j] -= gamedata.shipsCost[race,0,j];
				}
			}
			*/




		

		}
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
