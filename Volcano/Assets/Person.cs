using UnityEngine;
using System.Collections;


public class Person : MonoBehaviour {

	public Condition.Hair hair;
	public Condition.Pant pant;
	public Condition.Skin skin;
	public GameObject question; 

	private SoundManager sounManager;

	bool pinchado = false;
	float x,y;
	// Use this for initialization
	void Start () {
		sounManager=SoundManager._instance;
		x = GameObject.FindGameObjectWithTag ("volcan").transform.position.x-2.3f;
		y = GameObject.FindGameObjectWithTag ("volcan").transform.position.y + 6.2f;

	}
	/*
	 * Viste a la persona 
	 */
	public void Configure( Condition.Hair hair, Condition.Pant pant, Condition.Skin skin ){

		this.hair = hair;
		this.pant = pant;
		this.skin = skin;


		GameObject hairGameObject = Instantiate(Resources.Load ("Hairs/" + hair.ToString () + "Prefab") ) as GameObject;
		hairGameObject.transform.parent = this.transform;
		hairGameObject.transform.localPosition = new Vector3 (-0.05f, 0.68f, 0);

		GameObject pantGameObject = Instantiate(Resources.Load ("Pants/" + pant.ToString () + "Prefab") ) as GameObject;
		pantGameObject.transform.parent = this.transform;
		pantGameObject.transform.localPosition = new Vector3 (-0.05f, -0.15f, 0);

		GameObject skinGameObject = Instantiate(Resources.Load ("Skins/" + skin.ToString () + "Prefab") ) as GameObject;
		skinGameObject.transform.parent = this.transform;
		skinGameObject.transform.localPosition = new Vector3 (-0.05f, 0, 0);

	}
	
	// Update is called once per frame
	void Update () {

	}
	void OnMouseDown() {

		if(GameManager.instance.IsInputBlocked)
        {
			return;
		}

		pinchado = true;
		if (QuestionController.instance.IsValid (this)) {
			sounManager.playHappyPeople();
			StartCoroutine( moverSprite(true) );
            GameManager.instance.CheckIfLastPerson();    //to block input
            print ("valido");

		} else {
			sounManager.playPeopleGrount();
			StartCoroutine( moverSprite(false) );
		}

	}


	IEnumerator moverSprite( bool isOK ){
		
		this.transform.position = new Vector3 (x, y,-1);


		float h = this.transform.localScale.y*0.2f;


		for(int i = 0; i <10; i++){
			yield return new WaitForSeconds (0.1f);
			y = y - h;
			this.transform.position = new Vector3 (x, y,-2);

		}
	
		GameManager.instance.OnAnswer (isOK);
		Destroy (this.gameObject);
	}

}
