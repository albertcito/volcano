using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour {

	private AudioSource[] audio;

	public static SoundManager _instance;
	public static UIManager uiManager;

	// Use this for initialization
	void Awake () {
		uiManager=UIManager._instance;
		audio = this.gameObject.GetComponents<AudioSource>();
		playMainTheme();
		_instance=this;
		AudioListener.volume = 0.3F;;
	}

	public void volumeOnOff(){
		if(AudioListener.volume>0){
			AudioListener.volume = 0.0F;
		}else{
			AudioListener.volume = 0.3F;
		}
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
