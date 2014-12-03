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

	RaycastHit hit;
	float timeSaved = 0f;
	int cameraTracker = 0;
	Vector3 lerpStart, lerpEnd;


	void Update () {
		Ray ray1 = Camera.main.ScreenPointToRay (Input.mousePosition);
		Ray ray2 = Camera.main.ScreenPointToRay (Input.mousePosition);	
		if (Input.touchCount == 1 || Input.mousePresent) {
			if (Input.GetMouseButtonDown (0)) {
				startpoint = ray1.GetPoint (100);
				startpoint.z = 0;
				panning = true;
				if (timeSaved == 0f) timeSaved = Time.time;
			}
			if(panning){
				endpoint = ray2.GetPoint (100); 
				endpoint.z = 0;
				dist = Mathf.Clamp01(Vector3.Distance(endpoint, startpoint));
				
				if(dist >= 0.1){
					Vector3 pos = transform.position + (startpoint - endpoint);
					pos.x = Mathf.Clamp(pos.x, minPosX, maxPosX);
					pos.y = Mathf.Clamp(pos.y, minPosY, maxPosY);
					transform.position = pos;
				}
			}
			if(Input.GetMouseButtonUp(0)){
				panning = false;
				if (Time.time - timeSaved < 0.2f){
					if (Physics.Raycast(ray1, out hit)) if (hit.collider != null){
						gamedata.planetSelected = int.Parse(hit.transform.parent.name);
						lerpStart = transform.position;
						lerpEnd = hit.transform.position - new Vector3(0,0,3);
						cameraTracker = 25;
					}
				}
				timeSaved = 0f;
			}
		}

		if (cameraTracker > 0){

			transform.position = Vector3.Lerp(lerpStart, lerpEnd, 1f - 0.04f * cameraTracker );


			cameraTracker--;
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