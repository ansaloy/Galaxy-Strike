using UnityEngine;
using System.Collections;

public class foregroundplanet : MonoBehaviour {

	const float delta = 0.2f;
	float saved = 0;

	// Use this for initialization
	void Start () {
		SphereCollider cd = gameObject.AddComponent<SphereCollider> ();
		cd.isTrigger = true;
	}

	void OnMouseDown() {
		if ( (Time.time-saved) < delta) {
			planet.selected = gameObject.transform.parent.gameObject;
		} else {
			saved = Time.time;
		}
	}
	
}
