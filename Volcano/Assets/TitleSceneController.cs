using UnityEngine;
using System.Collections;

public class TitleSceneController : MonoBehaviour {

	
	// Update is called once per frame
	void Awake () {


		if(!PlayerPrefs.HasKey("soundLevel")){
			AudioListener.volume = 1F;
		}else{
			AudioListener.volume = PlayerPrefs.GetFloat("soundLevel");
		}
	}

	public void playTimeChallenge(){
		PlayerPrefs.SetString("challengeType","TIME");
		UnityEngine.SceneManagement.SceneManager.LoadScene (1);
	}

	public void playAcuracyChallenge(){
		PlayerPrefs.SetString("challengeType","ACURACY");
		UnityEngine.SceneManagement.SceneManager.LoadScene (1);
	}
}
