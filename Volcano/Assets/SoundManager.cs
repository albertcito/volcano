using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour {

	private AudioSource[] audio;

	public static SoundManager _instance;
	public static UIManager uiManager;
	public static SaveDataManager saveDataManager;

	// Use this for initialization
	void Awake () {
		uiManager=UIManager._instance;
		saveDataManager=SaveDataManager._instance;
		audio = this.gameObject.GetComponents<AudioSource>();
		playMainTheme();
		_instance=this;
		if(!PlayerPrefs.HasKey("soundLevel")){
			AudioListener.volume = 1.0F;
		}else{
			AudioListener.volume =PlayerPrefs.GetFloat("soundLevel");
			uiManager.soundONOFF();
		}
	}

	public void volumeOnOff(){
		if(AudioListener.volume>0){
			AudioListener.volume = 0.0F;
		}else{
			AudioListener.volume = 1F;
		}
		saveDataManager.saveSoundPreset(AudioListener.volume);
		uiManager.soundONOFF();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void playMainTheme(){
		audio[0].Play();
		audio[0].loop=true;
	}

	public void playVolcanoAngry(){
		audio[1].Play();
	}

	public void playPeopleGrount(){
		audio[2].Play();
	}

	public void playHappyPeople(){
		audio[3].Play();
	}

	public void playVolcanoHappy(){
		audio[4].Play();
	}

	public static SoundManager getSoundManager(){
		return _instance;
	}

}
