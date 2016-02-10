using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetBestScore : MonoBehaviour {

	public GameObject title;
	private bool flipFlag;

	// Use this for initialization
	void Awake () {
		StartCoroutine (textChange());
	}

	IEnumerator textChange(){
		while(true){

			if(flipFlag){
				changeTextAcuracy();
				flipFlag=false;
			}else{
				changeTextTime();
				flipFlag=true;
			}

			yield return new WaitForSeconds (4f);
		}
	}


	private void changeTextAcuracy(){
		if(PlayerPrefs.GetInt("bestLevelAcuracy")==null){
			this.GetComponent<Text>().text=""+1;
		}else{
			title.GetComponent<Text>().text="Eternal Ritual";
			this.GetComponent<Text>().text="Best level  "+(PlayerPrefs.GetInt("bestLevelAcuracy")+1);
		}
	}

	private void changeTextTime(){
		if(PlayerPrefs.GetInt("bestLevelTime")==null){
			this.GetComponent<Text>().text=""+1;
		}else{
			title.GetComponent<Text>().text="Time Ritual";
			this.GetComponent<Text>().text="Best level  "+(PlayerPrefs.GetInt("bestLevelTime")+1);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
