using UnityEngine;
using System.Collections;

public class galaxysize : MonoBehaviour {

	public UILabel percent;
	public UIScrollBar scrollbar;

	// Use this for initialization
	public void OnUpdatePercentLabel () {
		int pr = 10+(int)(90 * scrollbar.value);
		percent.text = pr.ToString()+"%";
		create.spaceLimit = 50+(int)(50*scrollbar.value);
	}
}
