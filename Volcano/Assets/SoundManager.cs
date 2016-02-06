using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour {

	private AudioSource[] audio;

	public static SoundManager _instance;

	// Use this for initialization
	void Awake () {
		audio = this.gameObject.GetComponents<AudioSource>();
		playMainTheme();
		_instance=this;


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
