using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class fileview : MonoBehaviour {
	public GameObject prefab;

	int nextId = 0;
	const int minLines = 12;
	public static string selected = null;
	static readonly string extension = "txt";

	List<GameObject> childs = new List<GameObject>();

	DirectoryInfo dirInfo = new DirectoryInfo ("./Assets");

	static public void NotifySelected(string filename) {
		selected = filename;
	}

	void CreateChild(string label,Vector3 pos,int id,bool selected) {
		GameObject mChild = NGUITools.AddChild (gameObject, prefab);
		mChild.name = "fileItem" + nextId++;
		mChild.transform.localPosition = pos;
		mChild.transform.Find ("FileNameLabel").GetComponent<UILabel> ().text = label;
		if (selected) {
			mChild.GetComponent<fileitem>().OnClick();
		}
		childs.Add (mChild);
	}
	
	void Fill() {
		Vector4 clipArea = gameObject.GetComponent<UIPanel> ().finalClipRegion;
		Vector3 pos = Vector3.zero;
		float step = prefab.GetComponent<UIWidget> ().localSize.y;
		pos.x = clipArea.x;
		pos.y = clipArea.y + clipArea.w / 2f - step/2+2 ;
		int idx = 0;
		string[] names = getFileNames();
		if (selected == null && names.Length != 0) selected = names [0];
		while (idx < names.Length) { 
			CreateChild(names[idx],pos,idx,selected==names[idx]);
			++idx;
			pos.y -= step;
		}
		while (idx < minLines) {
			CreateChild ("", pos, idx++, false);
			pos.y -= step;
		}
	}

	void Start() {
		DirectoryInfo[] dirs = dirInfo.GetDirectories ("Save");
		if (dirs.Length == 0) {
			dirInfo = dirInfo.CreateSubdirectory ("Save");
		} else {
			dirInfo = dirs [0];
		}
		Fill ();
	}
	
	string[] getFileNames() {
		FileInfo[] files = dirInfo.GetFiles ("*."+extension);
		string[] ret = new string[files.Length];
		int idx = 0;
		foreach (FileInfo fl in files) {
			ret [idx] = fl.Name.Replace(fl.Extension,"");
			++idx;
		}
		return ret;
	}
}