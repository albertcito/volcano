using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager _instance;

	public GameObject levelText;
	public GameObject timeText;
	public GameObject soundText;
	public GameObject menuPanel;
	public GameObject personContainer;

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

	public void soundONOFF(){
		if(AudioListener.volume == 0.0F){
			soundText.GetComponent<Text>().text="Sound OFF";
		}else{
			soundText.GetComponent<Text>().text="Sound ON";
		}
	}

	public void showMenuPanel(){
		if(menuPanel.active){
			menuPanel.SetActive(false);
			Time.timeScale = 1.0f;
			//personContainer.SetActive(true);

		}else{
			menuPanel.SetActive(true);
			Time.timeScale = 0f;
			//personContainer.SetActive(false);
		}
	}



	public void replayGame(){
		Time.timeScale = 1.0f;
		UnityEngine.SceneManagement.SceneManager.LoadScene (1);
	}

	public void goMain(){
		Time.timeScale = 1.0f;
		UnityEngine.SceneManagement.SceneManager.LoadScene (0);
	}

	public void hideTime(){
		timeText.SetActive(false);
	}
}
