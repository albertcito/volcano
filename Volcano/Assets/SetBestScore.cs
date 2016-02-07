using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetBestScore : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		if(PlayerPrefs.GetInt("bestLevel")==null){
			this.GetComponent<Text>().text=""+1;
		}else{
			this.GetComponent<Text>().text="Best level  "+(PlayerPrefs.GetInt("bestLevel")+1);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
