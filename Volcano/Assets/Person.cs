using UnityEngine;
using System.Collections;
using DG.Tweening;


public class Person : MonoBehaviour {

	public Condition.Hair hair;
	public Condition.Pant pant;
	public Condition.Skin skin;
    public bool isWalking;
    public GameObject container;
	private SoundManager sounManager;
    private Animator containerAnimator;


	public bool picked = false;
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
		hairGameObject.transform.parent = container.transform;
		hairGameObject.transform.localPosition = new Vector3 (-0.05f, 0.68f, 0);

		GameObject pantGameObject = Instantiate(Resources.Load ("Pants/" + pant.ToString () + "Prefab") ) as GameObject;
		pantGameObject.transform.parent = container.transform;
		pantGameObject.transform.localPosition = new Vector3 (-0.05f, -0.15f, 0);

		GameObject skinGameObject = Instantiate(Resources.Load ("Skins/" + skin.ToString () + "Prefab") ) as GameObject;
		skinGameObject.transform.parent = container.transform;
		skinGameObject.transform.localPosition = new Vector3 (-0.05f, 0, 0);

	}

	void OnMouseDrag(){
		if(GameManager.instance.IsInputBlocked)
		{
			return;
		}
		if( picked )
		{
			return;
		}
		if( startMouseDown < 0 ) {
			return;
		}

		if( Time.time - startMouseDown > 0.3f ) {
			return;
		}
		Vector2 mousePos = new Vector2( Input.mousePosition.x / (float) Screen.width, Input.mousePosition.y / (float) Screen.height );
		Vector2 diff = mousePos - startMousePos;

		if( diff.sqrMagnitude < 0.08f * 0.08f ) {
			return;
		}
		float dotProd = Vector2.Dot( diff.normalized, Vector2.up );
		if( dotProd > 0.6f ) {
			GoToVolcan();
		}
		else if( dotProd < -0.6f ) {
			GoAway();
		}
		startMouseDown = -1f;
	}

	private Vector2 startMousePos = -Vector2.one;
	private float startMouseDown = -1f;
	void OnMouseDown() {
		if(GameManager.instance.IsInputBlocked)
		{
			return;
		}
		if( picked )
		{
			return;
		}
		startMousePos = new Vector2( Input.mousePosition.x / (float) Screen.width, Input.mousePosition.y / (float) Screen.height );
		startMouseDown = Time.time;
	}

	void OnMouseUp() {
		startMouseDown = -1f;
	}

	void GoAway() {
		picked = true;
		GameManager.instance.StartCoroutine( WalkAwayCoroutine( () => {
			GameManager.instance.CheckEndQuestion();
		}) );
		if (QuestionController.instance.IsValid (this)) {
			sounManager.playPeopleGrount();
			GameManager.instance.BadAnswer();
			GameManager.instance.CheckIfLastPerson();    //to block input

		} else {
			sounManager.playHappyPeople();
		}

	}

	void GoToVolcan() {
		picked = true;
        GameManager.instance.people.Remove(this);
		if (QuestionController.instance.IsValid (this)) {
			sounManager.playHappyPeople();
			StartCoroutine( DeathCoroutine(true) );
            GameManager.instance.CheckIfLastPerson();    //to block input
            print ("valido");

		} else {
			sounManager.playPeopleGrount();
			StartCoroutine( DeathCoroutine(false) );
		}

	}

	public IEnumerator WalkAwayCoroutine( System.Action cb = null )
	{
		GameManager.instance.people.Remove(this);
		yield return StartCoroutine( WalkToPositionCoroutine( GameManager.instance.GetClosestOriginPoint( transform.position ), true ) );
		if( cb != null ) {
			cb();
		}
	}

    public IEnumerator WalkToPositionCoroutine( Vector3 destination, bool dieAfter = false )
    {
        isWalking = true;
        Vector3 direction = (destination - transform.position).normalized;

        if(containerAnimator == null)
        {
            containerAnimator = container.GetComponent<Animator>();
        }
        containerAnimator.SetBool("isWalking", true );

        float walkVelocity = Random.Range(8f, 14f);
        while ( transform.position != destination )
        {
			float deltaPos = walkVelocity * Time.deltaTime;
            if (Vector3.Distance(transform.position, destination) > deltaPos)
            {
                transform.position += deltaPos * direction;
            }
            else
            {
                transform.position = destination;
            }
            yield return null;
        }
        isWalking = false;
        containerAnimator.SetBool("isWalking", false);
        if(dieAfter)
        {
            Destroy( this.gameObject );
        }
    }


	IEnumerator DeathCoroutine( bool isOK ){

        yield return StartCoroutine(WalkToPositionCoroutine(GameManager.instance.GetClosestJumpPoint(transform.position)));
        float duration = 0.4f;
        float height = 1.5f;
        Vector3 finalPos = GameManager.instance.deathPoint.position;
        Vector3 middlePoint = finalPos + transform.position;
        middlePoint.y = transform.position.y + height;
        Tweener t1 = transform.DOMoveX(finalPos.x, duration).SetEase(Ease.Linear);
        Sequence seq = DOTween.Sequence();
        TweenCallback changeLayer = () =>
        {
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            foreach( SpriteRenderer sprite in sprites)
            {
                sprite.sortingOrder -= 10;
            }
        };
        seq.Append(transform.DOMoveY(middlePoint.y, duration * 0.5f).SetEase(Ease.OutCubic).OnComplete<Tweener>(changeLayer));
        seq.Append(transform.DOMoveY(finalPos.y, duration * 0.5f).SetEase(Ease.InQuad));
        yield return t1.WaitForCompletion();
        yield return seq.WaitForCompletion();

        GameManager.instance.OnAnswer (isOK);
		Destroy (this.gameObject);
	}

}
