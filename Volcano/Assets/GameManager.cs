using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ChallengeType{
	TIME=1,
	ACURACY=2
}

public class GameManager : MonoBehaviour {

	private SoundManager soundManager;
	private UIManager uiManager;
	private SaveDataManager saveDataManager;
	private VolcanoEyeManager volcanoEyeManager;
	private ParticleManager particleManager;

	public static GameManager instance;

    public GameObject gameContainer;
	public GameObject personPrefab;
	public SpriteRenderer lavaSprite;
    public SpriteRenderer skySprite;
    public ParticleSystem volcanParticle;
	public GameObject personContainer;
	public List<Person> people  =  new List<Person>();
	private int remainingGoodAnswers;
	private int lives = 1;
	private int currentLives;
	public GameObject spawnPointsParent;
    List<Transform> spawnPoints = new List<Transform>();
    public GameObject jumpPointsParent;
    List<Transform> jumpPoints = new List<Transform>();
    public Transform deathPoint;

    public GameObject personOriginPointsParent;
    List<Transform> originPoints = new List<Transform>();
    float lavaHeight = 1.52f;

	public GameObject beginText;
	public GameObject endText;

	private float maxTime=30;
    private float currentTime;
    private int level;
	private Vector3 savedLavaPosition;
    private Color savedSkyColor;
    public Color finalSkyColor;
    private float visualPercentage;

    public bool gameFinished;
    public bool isInTransition = true;

    //numero de personas que aparecen 
	//numero de condiciones
	//Numero de personajes validos
	//Numero de preguntas negativas


	int[] minFollowers={2,3,4,4,4,4,5,2,4,6,4,6,8,8,8,10,8,10,10,10,11,12,10,10,10,10,8,8,10,12,12,14,14,12,12,12,14,16,16,18};
	int[] maxFollowers = { 2,4,5,5,6,6,10,2,6,8,8,10,10,12,14,14,12,12,14,16,16,16,12,14,16,18,18,20,20,20,20,20,20,18,16,14,18,18,20,20};
	int[] minCondition={1,1,2,2,1,2,3,1,1,2,2,2,2,2,1,2,2,2,3,3,2,2,1,1,1,1,1,1,2,2,2,3,3,1,1,2,2,2,1,3};
	int[] maxCondition = { 1,1,2,2,2,3,3,1,1,2,2,2,3,3,3,3,3,3,3,3,3,3,2,2,3,2,3,3,3,3,3,3,3,2,2,2,3,3,3,3};
	float[] minValidPercentage={50,40,30,20,20,35,20,50,50,40,35,20,15,15,15,30,20,30,35,30,30,30,30,30,25,25,20,15,10,10,10,10,20,25,20,20,20,10,10,10};
	float[] maxValidPercentage = { 60,50,40,40,60,60,40,50,50,60,60,60,60,55,50,45,40,50,60,55,50,45,45,40,35,35,35,35,40,40,35,35,35,35,30,25,25,35,40,25};
	float[] minNegation={0f,0f,0f,0f,0f,0f,0f,33.3f,33.3f,0f,0f,0f,33.3f,0f,33.3f,0f,0f,33.3f,66.7f,66.7f,33.3f,0f,33.3f,33.3f,0f,33.3f,33.3f,0f,33.3f,33.3f,0f,33.3f,0f,33.3f,0f,33.3f,0f,66.7f,0f,0f};
	float[] maxNegation = { 0f,0f,0f,0f,0f,0f,0f,33.3f,33.3f,33.3f,33.3f,66.7f,66.7f,66.7f,66.7f,33.3f,66.7f,100f,100f,100f,66.7f,100f,66.7f,66.7f,66.7f,66.7f,66.7f,66.7f,66.7f,66.7f,66.7f,66.7f,66.7f,66.7f,66.7f,66.7f,66.7f,100f,100f,100f};

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

	private ChallengeType challengeType;

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

	void Awake(){
		
		uiManager=UIManager._instance;

		challengeType=(ChallengeType)Enum.Parse(typeof(ChallengeType),PlayerPrefs.GetString("challengeType"));
		print("Challenge type is : " + challengeType);

		if(challengeType==ChallengeType.ACURACY){
			uiManager.hideTime();
		}

	}

	IEnumerator Start(){
        currentTime = maxTime;
        
		soundManager =SoundManager._instance;
		volcanoEyeManager=VolcanoEyeManager._instance;
		particleManager=ParticleManager._instance;
		saveDataManager=SaveDataManager._instance;

		savedLavaPosition=lavaSprite.transform.localPosition;
        savedSkyColor = skySprite.color;

		endText.SetActive(false);

		yield return new WaitForSeconds (0.5f);

		while( true ){
			if (Input.GetMouseButtonDown (0)) {
				if(challengeType==ChallengeType.TIME){
					StartCoroutine(UpdateTimeCoroutine());
				}
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
        for (int i = 0; i < jumpPointsParent.transform.childCount; i++)
        {
            jumpPoints.Add(jumpPointsParent.transform.GetChild(i));
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
		float min = (level < minNegation.Length ? minNegation [level] : minNegation [minNegation.Length - 1])/100f;
		float max = (level < maxNegation.Length ? maxNegation [level] : maxNegation [maxNegation.Length - 1])/100f;
		float retVal = UnityEngine.Random.Range(min,max);
		retVal = Mathf.Clamp01( retVal );
		print ("negation percentage for level "+level+ ": " + retVal);
		return retVal;
	}
	
    void loadGame()
    {
        StartCoroutine(loadGameCoroutine());

    }

	IEnumerator UpdateTimeCoroutine(){
        int uiTime = -1;
		while(!gameFinished){
			if(currentTime>0){
				currentTime-=Time.deltaTime;
			}
            else
            {
                currentLives--;
                checkLooseState();
            }

            UpdateLavaAndSky();
            //volcanParticle.Emit((int)(time));

            if ( Mathf.CeilToInt( currentTime ) != uiTime )
            {
                uiTime = Mathf.CeilToInt(currentTime);
                uiManager.changeTime(uiTime);
            }

			yield return null;
		}
			
	}

    void UpdateLavaAndSky()
    {
        float maxDiff = Time.deltaTime * 0.5f;
        float percentage = ((maxTime - currentTime) / maxTime);
        float diffPercentage = percentage - visualPercentage;
        visualPercentage += Mathf.Clamp(diffPercentage, -maxDiff, maxDiff);
        visualPercentage = Mathf.Clamp01(visualPercentage);
        lavaSprite.transform.localPosition = new Vector3(0, savedLavaPosition.y + lavaHeight * visualPercentage);
        skySprite.color = Color.Lerp(savedSkyColor, finalSkyColor, visualPercentage);
    }

    IEnumerator loadGameCoroutine(){

        print("====== loadGame =======");
        isInTransition = true;
        int numConditions = GetConditions ();
		int numNegations =Mathf.RoundToInt(numConditions*GetNegationPercentage());
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

		instanciaPerson.transform.parent = personContainer.gameObject.transform;;

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

		int r = UnityEngine.Random.Range (0, availableInvalidSets.Count);
		SetInvalid selectedSet = availableInvalidSets [r];

		return apereceMono( selectedSet.selectedHairs, selectedSet.selectedPants, selectedSet.selectedSkins, pos );
	}

    public void CheckIfLastPerson()
    {
        remainingGoodAnswers--;
        if (remainingGoodAnswers <= 0)
        {
            isInTransition = true;
            for(int i = people.Count - 1; i >= 0; i-- )
            {
                Person p = people[i];
                if (!p.picked)
                {
                    StartCoroutine(p.WalkToPositionCoroutine(GetClosestOriginPoint( p.transform.position ), true) );
                    people.Remove(p);
                }
            }
        }
    }


    public void OnAnswer( bool isGoodAnswer ){
		if (isGoodAnswer) {
			soundManager.playVolcanoHappy();
			volcanoEyeManager.sethappyEyeFlagTrue();
			if (remainingGoodAnswers <= 0 ) {
				destroyGame();
				level++;
				if(challengeType==ChallengeType.TIME){
					currentTime+=3;
				}
				uiManager.setLevelText(level+1);
				saveDataManager.setBestLevel(level,challengeType);
				loadGame();
			}
            else
            {
                currentTime += 2f;
            }
		} else {
			if(challengeType==ChallengeType.TIME){
				if(currentTime-5>=0){
					currentTime-=5;
				}else{
					currentTime=0;
				}
			}
			checkLooseState();
		}
		particleManager.playBurnPersonParticles();
	}

	private void checkLooseState(){
		
		soundManager.playVolcanoAngry();
		volcanoEyeManager.setAngryEyeFlagTrue();

		if (currentLives <= 0 || challengeType==ChallengeType.ACURACY) {
            StartCoroutine(ShakeCoroutine(2f));
            particleManager.playLooseLavaParticle();
			volcanoEyeManager.setAngryEyeFlagTrueLoose();
			gameFinished = true;
			QuestionController.instance.killIcons ();
			endText.SetActive(true);
			StartCoroutine ( EndCoroutine());
		}
        else
        {
            StartCoroutine(ShakeCoroutine(0.2f));
        }
	}

    private IEnumerator ShakeCoroutine( float duration )
    {
        Vector3 originalPos = gameContainer.transform.localPosition;
        float sign = 1f;
        float startTime = Time.time;
        while(startTime + duration > Time.time ) {
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

    public Vector3 GetClosestJumpPoint(Vector3 pos)
    {
        Vector3 retVal = jumpPoints[0].position;
        foreach (Transform o in jumpPoints)
        {
            if (Vector3.Distance(pos, o.position) < Vector3.Distance(pos, retVal))
            {
                retVal = o.position;
            }
        }
        return retVal;
    }

}
