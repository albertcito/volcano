using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager _instance;

	public GameObject levelText;
	public GameObject timeText;

	// Use this for initialization
	void Awake () {

		_instance=this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setLevelText(int level){
		levelText.GetComponent<Text>().text="Level "+level;
	}

	public void changeTime(int time){
		timeText.GetComponent<Text>().text=""+time;
	}

	public static UIManager getInstance(){
		return _instance;
	}
}
