using UnityEngine;
using System.Collections;

public class planet : MonoBehaviour {
	Vector3 centerOfPS; // центр оборота
	Vector3 axisMoove; // Вектор направления движения планеты
	float speedRotation; // скорость оборота по траектории

	bool direction; // направление вращения
	float speedTurn; // скорость вращения
	float turn = 0; // кут поворота 0 до 360

	void Start () {
		centerOfPS = transform.position + new Vector3 (Random.Range (-30f, 30f), Random.Range (-30f, 30f), Random.Range (-30f, 30f));
		axisMoove = new Vector3 (Random.Range (-1f, 1f), Random.Range (-1, 1f), Random.Range (-1f, 1f));
		speedRotation = Random.Range (0.1f, 3f);
		if (Random.Range(0,2) == 0) direction = true;
		speedTurn = Random.Range (1f, 3f);
	}

	void Update () {
		transform.RotateAround (centerOfPS, axisMoove, speedRotation * Time.deltaTime);
		if (direction){
			turn = turn + Time.deltaTime * speedTurn;
			if (turn > 360) turn = 0;
		}
		else {
			turn = turn - Time.deltaTime * speedTurn;
			if (turn < 0) turn = 360;
		} 
		transform.rotation = Quaternion.Euler (0, 0, turn);
	}
}