﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {

	private SoundManager soundManager;
	private VolcanoEyeManager volcanoEyeManager;

	public static GameManager instance;

	public GameObject personPrefab;
	public GameObject volcan;
	public List<Person> persons  =  new List<Person>();
	private int remainingGoodAnswers;
	public int lives = 3;
	private int currentLives;
	public GameObject spawnPointsParent;
	List<Transform> spawnPoints = new List<Transform>();
	float lavaHeight = 1f;

	public GameObject endText;

	private int level;
	public bool gameFinished;
	public bool gameStarted;

	//numero de personas que aparecen 
	int[] minFollowers={3,4,5,5,6,8,12,5,5,6,11,10,5,5,8,8,10,12,10};
	int[] maxFollowers = { 5, 8, 10, 9, 11, 10, 15, 6, 6, 14, 18, 16, 7, 7, 12, 12, 20, 20, 20 };
	//numero de condiciones
	int[] minCondition={1,1,1,2,2,1,2,1,1,1,1,2,3,3,2,2,2,2,3};						
	int[] maxCondition = { 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3 };						
	//Numero de personajes validos
	float[] minValidPercentage={30,25,20,20,20,25,30,20,20,20,20,20,30,30,30,20,20,20,10};
	float[] maxValidPercentage = { 60, 50, 40, 30, 30, 30, 40, 30, 30, 40, 30, 25, 40, 40, 40, 30, 30, 30, 20 };
	//Numero de preguntas negativas
	int[] minNegation={0,0,0,0,0,0,0,1,1,0,0,1,0,1,0,1,0,1,1};
	int[] maxNegation = { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2 };

	IEnumerator Start(){
		soundManager=SoundManager._instance;
		volcanoEyeManager=VolcanoEyeManager._instance;
		endText.SetActive(false);

		yield return new WaitForSeconds (0.5f);

		while( true ){
			if (Input.GetMouseButtonDown (0)) {
				print("preiosno");
				break;
			}
			yield return 0;
		}

		instance = this;
		for( int i = 0; i < spawnPointsParent.transform.childCount; i++){
			spawnPoints.Add( spawnPointsParent.transform.GetChild(i));
		}
		currentLives = lives;
		personPrefab = Resources.Load ("PersonPrefab") as GameObject;
		loadGame();

	}

	int GetFollowers(){
		int min = level < minFollowers.Length ? minFollowers [level] : minFollowers [minFollowers.Length - 1];
		int max = level < maxFollowers.Length ? maxFollowers [level] : maxFollowers [maxFollowers.Length - 1];
		int retVal = UnityEngine.Random.Range(min,max);
		retVal = Mathf.Clamp (retVal, 0, spawnPoints.Count);
		print ("followers for level "+level+ ": " + retVal);
		return retVal;
	}

	float GetValidPercentage(){
		float min = (level < minValidPercentage.Length ? minValidPercentage [level] : minValidPercentage [minValidPercentage.Length - 1])/100f;
		float max = (level < maxValidPercentage.Length ? maxValidPercentage [level] : maxValidPercentage [maxValidPercentage.Length - 1])/100f;
		float retVal = UnityEngine.Random.Range(min,max);
		retVal = Mathf.Clamp01( retVal );
		print ("valid percentage for level "+level+ ": " + retVal);
		return retVal;
	}

	int GetConditions(){
		int min = level < minCondition.Length ? minCondition [level] : minCondition [minCondition.Length - 1];
		int max = level < maxCondition.Length ? maxCondition [level] : maxCondition [maxCondition.Length - 1];
		int retVal = UnityEngine.Random.Range(min,max);
		retVal = Mathf.Clamp( retVal, 1, 3 );
		print ("conditions for level "+level+ ": " + retVal);
		return retVal;
	}

	float GetNegationPercentage(){
		float min = (level < minNegation.Length ? minNegation [level] : minNegation [minNegation.Length - 1])/3f;
		float max = (level < maxNegation.Length ? maxNegation [level] : maxNegation [maxNegation.Length - 1])/3f;
		float retVal = UnityEngine.Random.Range(min,max);
		retVal = Mathf.Clamp01( retVal );
		print ("negation percentage for level "+level+ ": " + retVal);
		return retVal;
	}
		
	void loadGame(){

		int numConditions = GetConditions ();
		int numNegations = Mathf.RoundToInt ( numConditions * GetNegationPercentage() );
		print ("num negations for level " + level + ": "  + numNegations );
		QuestionController.instance.defineConditions ( numConditions, numNegations );

		int total = GetFollowers();
		int valid = Mathf.RoundToInt(GetValidPercentage() * total );
		print ( "total valid for level " + level + ": " + valid );

		AparecerMonoCoroutine(valid,total-valid);

	}

	void AparecerMonoCoroutine(int valido, int invalido){

		List<Transform> remainingPos = new List<Transform> (spawnPoints);

		remainingGoodAnswers = 0;
		for(int i = 0; i <valido; i++){
			int r = UnityEngine.Random.Range (0,remainingPos.Count-1);
			Person newPerson = monoValido(remainingPos[r].position);
			remainingPos.RemoveAt (r);

			persons.Add (newPerson);
			if (QuestionController.instance.IsValid (newPerson)) {
				remainingGoodAnswers++;
			}
		}

		for(int i = 0; i <invalido; i++){
			int r = UnityEngine.Random.Range (0,remainingPos.Count-1);
			Person newPerson = monoInvalido (remainingPos [r].position);
			remainingPos.RemoveAt (r);

			persons.Add (newPerson);
			if (QuestionController.instance.IsValid (newPerson)) {
				remainingGoodAnswers++;
				print ("ERROR ... check");
			}

		}
		if (remainingGoodAnswers != valido) {
			print ("ERROR 2... check");
		}
	}
	/*
	 * Va imprimieno monos en la pantalla
	 */
	Person apereceMono( List<Condition.Hair> hairs, List<Condition.Pant>  pants, List<Condition.Skin> skins, Vector3 pos) {

		GameObject instanciaPerson = Instantiate<GameObject>(personPrefab);
		Person person = instanciaPerson.GetComponent<Person> ();

		int nHair = hairs.Count;
		int nPant = pants.Count;
		int nSkin = skins.Count;

		int sHair = UnityEngine.Random.Range (0, nHair);
		int sPant = UnityEngine.Random.Range (0, nPant);
		int sSkin = UnityEngine.Random.Range (0, nSkin);

		person.Configure(hairs[sHair], pants[sPant], skins[sSkin] );

		instanciaPerson.transform.position = pos;
		return person;

	}

	Person monoValido(Vector3 pos) {
		return apereceMono( QuestionController.instance.validHairs, QuestionController.instance.validPants, QuestionController.instance.validSkins, pos );
	}

	public class SetInvalid {
		public List<Condition.Hair> selectedHairs;
		public List<Condition.Pant> selectedPants;
		public List<Condition.Skin> selectedSkins;
		public SetInvalid( List<Condition.Hair> h, List<Condition.Pant> p, List<Condition.Skin> s) {
			selectedHairs = h;
			selectedPants = p;
			selectedSkins = s;
		}
	}

	Person monoInvalido(Vector3 pos){

		List<SetInvalid> availableInvalidSets = new List<SetInvalid> ();


		if( QuestionController.instance.invalidHairs.Count > 0 ){
			SetInvalid setInvalid = new SetInvalid (QuestionController.instance.invalidHairs,
				                        QuestionController.instance.allPants, QuestionController.instance.allSkins);
			availableInvalidSets.Add (setInvalid);
		}
		if( QuestionController.instance.invalidPants.Count > 0 ){
			SetInvalid setInvalid = new SetInvalid (QuestionController.instance.allHairs,
				QuestionController.instance.invalidPants, QuestionController.instance.allSkins);
			availableInvalidSets.Add (setInvalid);
		}

		if( QuestionController.instance.invalidSkins.Count > 0 ){
			SetInvalid setInvalid = new SetInvalid (QuestionController.instance.allHairs,
				QuestionController.instance.allPants, QuestionController.instance.invalidSkins);
			availableInvalidSets.Add (setInvalid);
		}

		if(availableInvalidSets.Count == 0 ){
			print ("Error Could not find sets");
			SetInvalid setInvalid = new SetInvalid (QuestionController.instance.allHairs,
				QuestionController.instance.allPants, QuestionController.instance.allSkins);
			availableInvalidSets.Add (setInvalid);
		}

		int r = UnityEngine.Random.Range (0, availableInvalidSets.Count - 1);
		SetInvalid selectedSet = availableInvalidSets [r];

		if (selectedSet.selectedHairs == QuestionController.instance.invalidHairs) {
			print ("invalid hairs");
		}
		if (selectedSet.selectedPants == QuestionController.instance.invalidPants) {
			print ("invalid pants");
		}
		if (selectedSet.selectedSkins == QuestionController.instance.invalidSkins) {
			print ("invalid skins");
		}

		return apereceMono( selectedSet.selectedHairs, selectedSet.selectedPants, selectedSet.selectedSkins, pos );
	}


	public void OnAnswer( bool isGoodAnswer ){
		if (isGoodAnswer) {
			volcanoEyeManager.sethappyEyeFlagTrue();
			remainingGoodAnswers--;
			if (remainingGoodAnswers <= 0) {
				destroyGame();
				level++;
				loadGame();
			}
		} else {
			soundManager.playVolcanoAngry();
			volcanoEyeManager.setAngryEyeFlagTrue();
			currentLives--;
			GameObject volcan = GameObject.FindGameObjectWithTag ("volcan");
			Vector3 v = volcan.transform.position;

			volcan.transform.localPosition += new Vector3 (0, lavaHeight / ((float)lives));


			//subir lava
			if (currentLives <= 0) {
				gameFinished = true;
				QuestionController.instance.killIcons ();
				endText.SetActive(true);
				StartCoroutine ( EndCoroutine());
			}
		}
	}


	private void destroyGame(){

		foreach (Person person in persons) {

			if (person != null) {
				Destroy (person.gameObject);
			}
		}
		persons.Clear ();

	}

	IEnumerator EndCoroutine(){
		yield return new WaitForSeconds (0.5f);
		while(true) {
			if( Input.GetMouseButtonDown(0) ){
				UnityEngine.SceneManagement.SceneManager.LoadScene (0);
				yield break;
			}
			yield return 0;
		}
	}

}
