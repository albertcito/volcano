using UnityEngine;
using System.Collections;

public class SaveDataManager : MonoBehaviour {

	public static SaveDataManager _instance;
	private int bestLevel;

	// Use this for initialization
	void Awake () {
		_instance=this;
		if(PlayerPrefs.GetInt("bestLevel")==null){
			bestLevel=1;
			PlayerPrefs.SetInt("bestLevel",bestLevel);
		}else{
			bestLevel=PlayerPrefs.GetInt("bestLevel");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void saveSoundPreset(float soundLevel){
		PlayerPrefs.SetFloat("soundLevel",soundLevel);
	}

	public void setBestLevel(int level){
		if(level>bestLevel){
			bestLevel=level;
			PlayerPrefs.SetInt("bestLevel",bestLevel);
		}
	}
}
