using UnityEngine;
using System.Collections;

public class galaxysize : MonoBehaviour {

	public UILabel percent;
	public UIScrollBar scrollbar;


	public void OnUpdatePercentLabel () {
		int pr = 10+(int)(90 * scrollbar.value);
		percent.text = pr.ToString()+"%";
		gamedata.spaceLimit = 50+(int)(50*scrollbar.value);
	}
}
