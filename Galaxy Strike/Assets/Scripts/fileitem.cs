using UnityEngine;
using System.Collections;

public class fileitem : MonoBehaviour {

	static GameObject selected;
	Color selectedColor = new Vector4(235f/255,235f/255,235f/255,1);

	void Deselect() {
		if (null != selected) {
			selected.transform.Find ("FileItemBG").GetComponent<UISprite> ().spriteName = "filei - background";
			selected.transform.Find ("FileNameLabel").GetComponent<UILabel> ().color = Color.white;
			selected = null;
		}
	}

	void Select() {
		Deselect ();
		selected = gameObject;
		selected.transform.Find ("FileItemBG").GetComponent<UISprite> ().spriteName = "selected file -background";	
		selected.transform.Find ("FileNameLabel").GetComponent<UILabel> ().color = selectedColor;
		fileview.NotifySelected (selected.transform.Find ("FileNameLabel").GetComponent<UILabel> ().text);
	}

	public void OnClick() {
		if (transform.Find ("FileNameLabel").GetComponent<UILabel> ().text != "") {
			Select ();
		}
	}
}
