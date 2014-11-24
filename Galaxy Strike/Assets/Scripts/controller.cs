using UnityEngine;
using System.Collections;

public class controller : MonoBehaviour {
	const float minPosX = -75f;
	const float maxPosX = 75f;
	const float minPosY = -75f;
	const float maxPosY = 75f;
	Vector3 startpoint, endpoint;
	bool panning = false;
	float dist;

	const int cameraSizeMin = -100;
	const int cameraSizeMax = 100;
	int cameraInertion;
	Vector3 cameraPosition;

	void Update () {
		Ray ray1 = Camera.main.ScreenPointToRay (Input.mousePosition);
		Ray ray2 = Camera.main.ScreenPointToRay (Input.mousePosition);	
		if (Input.touchCount == 1 || Input.mousePresent) {
			if (Input.GetMouseButtonDown (1)) {
				startpoint = ray1.GetPoint (10);
				startpoint.z = 0;
				panning = true;
			}
			if(panning){
				endpoint = ray2.GetPoint (10); 
				endpoint.z = 0;
				dist = Mathf.Clamp01(Vector3.Distance(endpoint, startpoint));
				
				if(dist >= 0.1){
					Vector3 pos = transform.position + (startpoint - endpoint);
					pos.x = Mathf.Clamp(pos.x, minPosX, maxPosX);
					pos.y = Mathf.Clamp(pos.y, minPosY, maxPosY);
					transform.position = pos;
				}
			}
			if(Input.GetMouseButtonUp(1))panning = false;
		}
		cameraPosition = transform.position;
		if (Input.GetAxis("Mouse ScrollWheel") > 0) cameraInertion = cameraInertion + 3; // forward
		if (Input.GetAxis("Mouse ScrollWheel") < 0) cameraInertion = cameraInertion - 3; // back
		cameraInertion = Mathf.Clamp (cameraInertion, -30, 30);
		if (cameraInertion > 0){
			cameraPosition.z++; cameraInertion--;
		}
		if (cameraInertion < 0){
			cameraPosition.z--; cameraInertion++;
		}

		cameraPosition.z = Mathf.Clamp(cameraPosition.z, cameraSizeMin, cameraSizeMax );
		transform.position = cameraPosition;
	}
}