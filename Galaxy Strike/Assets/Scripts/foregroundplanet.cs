using UnityEngine;
using System.Collections;

public class foregroundplanet : MonoBehaviour {
	void Start () {
		SphereCollider obj = gameObject.AddComponent<SphereCollider> ();
		obj.isTrigger = true;
	}
}