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

	public void playGame(){
		UnityEngine.SceneManagement.SceneManager.LoadScene (1);
	}
}
