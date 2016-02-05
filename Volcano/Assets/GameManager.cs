using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {

	private SoundManager soundManager;
	private UIManager uiManager;
	private SaveDataManager saveDataManager;
	private VolcanoEyeManager volcanoEyeManager;

	public static GameManager instance;

    public GameObject gameContainer;
	public GameObject personPrefab;
	public GameObject volcan;
	public List<Person> people  =  new List<Person>();
	private int remainingGoodAnswers;
	public int lives = 3;
	private int currentLives;
	public GameObject spawnPointsParent;
    List<Transform> spawnPoints = new List<Transform>();

    public GameObject personOriginPointsParent;
    List<Transform> originPoints = new List<Transform>();
    float lavaHeight = 1f;

	public GameObject beginText;
	public GameObject endText;

	private int level;
	public bool gameFinished;
    public bool isInTransition = true;

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


	//La idea de esto es dar un porcentage de monos con caracteristicas similares a la receta esto define 
	//como un numeor entre los dos valores minimos y maximos
	//En caso de una categoria positiva 
	//En caso de una cagegoria negativa suar el procentage para poner la categoria especifica en los aldeanos 
	//En caso de 2 categorias
		//2 positivas incluir al alsar aldeanos con una de las dos categorias segun el procentage
		//1 positivas 1 negativa incluir al alsar aldeanos con la categoria positiva y tambien 
			//la categoria negativa puede ser las dos o solo una
	//En caso 3 categorias seguir mas menos la misma logica que cuando hay dos categorias agregando la tercera
	//La idea general de esto es confundir mas al jugador entregando respuestas casi correctas para las recetas
	float[] minUnvalidMisleadingPercentage= { 20, 25, 20, 20, 20, 25, 40, 30, 25, 45, 50, 55, 60, 60, 50, 60, 50, 60, 60};
	float[] maxUnvalidMisleadingPercentage= { 30, 35, 40, 50, 50, 50, 60, 40, 40, 50, 60, 60, 60, 65, 65, 65, 70, 70, 70 };

    public bool IsInputBlocked {
        get {
            if (gameFinished)
            {
                return true;
            }
            if (isInTransition)
            {
                return true;
            }
            return false;
        } }

	IEnumerator Start(){
		soundManager=SoundManager._instance;
		uiManager=UIManager._instance;
		saveDataManager=SaveDataManager.instance_;
		volcanoEyeManager=VolcanoEyeManager._instance;

		endText.SetActive(false);

		yield return new WaitForSeconds (0.5f);

		while( true ){
			if (Input.GetMouseButtonDown (0)) {
				break;
			}
			yield return 0;
		}

		instance = this;
		for( int i = 0; i < spawnPointsParent.transform.childCount; i++){
			spawnPoints.Add( spawnPointsParent.transform.GetChild(i));
		}
        for (int i = 0; i < personOriginPointsParent.transform.childCount; i++)
        {
            originPoints.Add(personOriginPointsParent.transform.GetChild(i));
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
	
    void loadGame()
    {
        StartCoroutine(loadGameCoroutine());
    }


    IEnumerator loadGameCoroutine(){

        print("====== loadGame =======");
        isInTransition = true;
        int numConditions = GetConditions ();
		int numNegations = Mathf.RoundToInt ( numConditions * GetNegationPercentage() );
		print ("num negations for level " + level + ": "  + numNegations );
		QuestionController.instance.defineConditions ( numConditions, numNegations );

		int total = GetFollowers();
		int valid = Mathf.RoundToInt(GetValidPercentage() * total );
		print ( "total valid for level " + level + ": " + valid );

		yield return StartCoroutine( AparecerMonoCoroutine(valid,total-valid) );

        isInTransition = false;

    }

	IEnumerator AparecerMonoCoroutine(int valido, int invalido){

		List<Transform> remainingPos = new List<Transform> (spawnPoints);

		remainingGoodAnswers = 0;
        for (int i = 0; i <valido; i++){
			int r = UnityEngine.Random.Range (0,remainingPos.Count);
			Vector3 personPos = remainingPos[r].position;
            Vector3 personOriginPos = GetClosestOriginPoint(personPos );
            Person newPerson = monoValido(personOriginPos );
            StartCoroutine( newPerson.WalkToPositionCoroutine( personPos ) );
			remainingPos.RemoveAt (r);

			people.Add (newPerson);
			if (QuestionController.instance.IsValid (newPerson)) {
				remainingGoodAnswers++;
			}
		}

		for(int i = 0; i <invalido; i++){
			int r = UnityEngine.Random.Range (0,remainingPos.Count);
            Vector3 personPos = remainingPos[r].position;
            Vector3 personOriginPos = GetClosestOriginPoint(personPos);
            Person newPerson = monoInvalido (personOriginPos );
            StartCoroutine(newPerson.WalkToPositionCoroutine(personPos));
            remainingPos.RemoveAt (r);

			people.Add (newPerson);
			if (QuestionController.instance.IsValid (newPerson)) {
				remainingGoodAnswers++;
				print ("ERROR ... check");
			}

		}

        while ( true )
        {
            bool peopleIsWalking = false;
            foreach( Person p in people )
            {
                if (p.isWalking )
                {
                    peopleIsWalking = true;
                    break;
                }
            }
            if( !peopleIsWalking)
            {
                break;
            }
            yield return null;
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

        instanciaPerson.transform.parent = gameContainer.transform;

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

		return apereceMono( selectedSet.selectedHairs, selectedSet.selectedPants, selectedSet.selectedSkins, pos );
	}

    public void CheckIfLastPerson()
    {
        remainingGoodAnswers--;
        if (remainingGoodAnswers <= 0)
        {
            isInTransition = true;
            foreach( Person p in people )
            {
                if (!p.picked)
                {
                    StartCoroutine(p.WalkToPositionCoroutine(GetClosestOriginPoint(p.transform.position)));
                }
            }
        }
    }


    public void OnAnswer( bool isGoodAnswer ){
		if (isGoodAnswer) {
			volcanoEyeManager.sethappyEyeFlagTrue();
			if (remainingGoodAnswers <= 0) {
				destroyGame();
				level++;
				uiManager.setLevelText(level+1);
				saveDataManager.setBestLevel(level);
				loadGame();
			}
		} else {
            StartCoroutine( ShakeCoroutine() );
			soundManager.playVolcanoAngry();
			volcanoEyeManager.setAngryEyeFlagTrue();
			currentLives--;
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

    private IEnumerator ShakeCoroutine()
    {
        Vector3 originalPos = gameContainer.transform.localPosition;
        float sign = 1f;
        for (int i = 0; i < 5; i++) {
            gameContainer.transform.localPosition = originalPos + sign * 0.15f * Vector3.right;
            sign *= -1f;
            yield return 0;
            yield return 0;
        }
        gameContainer.transform.localPosition = originalPos;
    }

	private void destroyGame(){

		foreach (Person person in people) {

			if (person != null) {
				Destroy (person.gameObject);
			}
		}
		people.Clear ();

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

    Vector3 GetClosestOriginPoint( Vector3 pos )
    {
        Vector3 retVal = originPoints[0].position;
        foreach ( Transform o in originPoints )
        {
            if( Vector3.Distance( pos, o.position ) < Vector3.Distance(pos, retVal) ) {
                retVal = o.position;
            }
        }
        return retVal;
    }

}
