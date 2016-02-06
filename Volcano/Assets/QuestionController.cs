using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;


public abstract class Condition{

	public bool yes;
	public enum Pant {pant1, pant2,pant3 };
	public enum Hair {hair1, hair2, hair3};
	public enum Skin {body1, body2, body3};

	public abstract bool isValid (Person persona);

	protected abstract GameObject CreateIcon ();


	private GameObject getTrueOrFalse(){

		String option = (this.yes)? "yesPrefab" : "notPrefab";
		GameObject optionGameObject = GameObject.Instantiate(Resources.Load (option)) as GameObject;
		return optionGameObject;

	}

	public  GameObject CreateDisplay(){

		GameObject icon = CreateIcon ();
		GameObject optionGameObject = getTrueOrFalse ();
		optionGameObject.transform.parent = icon.transform;

		return icon;

	}

	public abstract void RemoveInvalidElements(List<Hair> hairs, List<Pant> pants, List<Skin> skins );
	public abstract void AddInvalidElements(List<Hair> hairs, List<Pant> pants, List<Skin> skins );
}

public class PantCondition : Condition{

	public Pant pant;

	public PantCondition(bool yes, Pant pant){

		this.yes = yes;
		this.pant = pant;

	}

	public override bool isValid(Person persona){

		//Si es que tiene que usar ese pantalon
		if (this.yes) {
			//Si el pantalon es el mismo de la pregunta
			if (persona.pant == this.pant) {
				return true;
			}
			return false;

		} else { //Si no tiene que usar ese pantalon

			//Si el pantalon es igual al que no tiene que usar pierde
			if (persona.pant == this.pant) {
				return false;
			}	
			return true;

		}

	}

	protected override GameObject CreateIcon(){

		return GameObject.Instantiate (Resources.Load ("Pants/" + pant.ToString () + "Prefab")) as GameObject;

	}

	public override void RemoveInvalidElements(List<Hair> hairs, List<Pant> pants, List<Skin> skins ){
		for( int i = pants.Count - 1; i >= 0; i-- ){
			Pant p = pants [i];
			if (yes && pant != p ) {
				pants.Remove (p);
			} else if ( !yes && pant == p ) {
				pants.Remove (p);
			}
		}
	}
	public override void AddInvalidElements(List<Hair> hairs, List<Pant> pants, List<Skin> skins ){
		for( int i = QuestionController.instance.allPants.Count - 1; i >= 0; i-- ){
			Pant p = QuestionController.instance.allPants [i];
			if (yes && pant != p ) {
				pants.Add (p);
			} else if ( !yes && pant == p ) {
				pants.Add (p);
			}
		}
	}

}
public class HairCondition : Condition{

	public Hair hair;

	public HairCondition(bool yes, Hair hair){

		this.yes = yes;
		this.hair = hair;

	}

	public override bool isValid(Person persona){

		//Si es que tiene que usar ese pantalon
		if (this.yes) {
			//Si el pantalon es el mismo de la pregunta
			if (persona.hair == this.hair) {
				return true;
			}
			return false;

		} else { //Si no tiene que usar ese pantalon

			//Si el pantalon es igual al que no tiene que usar pierde
			if (persona.hair == this.hair) {
				return false;
			}	
			return true;

		}
			
	}

	protected override GameObject CreateIcon(){
		return GameObject.Instantiate(Resources.Load ("Hairs/" + hair.ToString () + "Prefab") ) as GameObject;
	}

	public override void RemoveInvalidElements(List<Hair> hairs, List<Pant> pants, List<Skin> skins ){
		for( int i = hairs.Count - 1; i >= 0; i-- ){
			Hair h = hairs [i];
			if (yes && hair != h ) {
				hairs.Remove (h);
			} else if ( !yes && hair == h ) {
				hairs.Remove (h);
			}
		}
	}

	public override void AddInvalidElements(List<Hair> hairs, List<Pant> pants, List<Skin> skins ){
		for( int i = QuestionController.instance.allHairs.Count - 1; i >= 0; i-- ){
			Hair h = QuestionController.instance.allHairs [i];
			if (yes && hair != h ) {
				hairs.Add (h);
			} else if ( !yes && hair == h ) {
				hairs.Add (h);
			}
		}
	}
}
public class SkinCondition : Condition{

	public Skin skin;

	public SkinCondition(bool yes, Skin skin){

		this.yes = yes;
		this.skin = skin;

	}

	public override bool isValid(Person persona){

		//Si es que tiene que usar ese pantalon
		if (this.yes) {
			//Si el pantalon es el mismo de la pregunta
			if (persona.skin == this.skin) {
				return true;
			}
			return false;

		} else { //Si no tiene que usar ese pantalon

			//Si el pantalon es igual al que no tiene que usar pierde
			if (persona.skin == this.skin) {
				return false;
			}	
			return true;

		}
			
	}

	protected override GameObject CreateIcon(){
		GameObject icon = GameObject.Instantiate (Resources.Load ("Skins/" + skin.ToString () + "Prefab")) as GameObject;
		icon.transform.localScale = new Vector3 (0.55f, 0.55f, 0.55f);
		return icon;
	}

	public override void RemoveInvalidElements(List<Hair> hairs, List<Pant> pants, List<Skin> skins ){
		for( int i = skins.Count - 1; i >= 0; i-- ){
			Skin s = skins [i];
			if (yes && skin != s ) {
				skins.Remove (s);
			} else if ( !yes && skin == s ) {
				skins.Remove (s);
			}
		}
	}

	public override void AddInvalidElements(List<Hair> hairs, List<Pant> pants, List<Skin> skins ){
		for( int i = QuestionController.instance.allSkins.Count - 1; i >= 0; i-- ){
			Skin s = QuestionController.instance.allSkins [i];
			if (yes && skin != s ) {
				skins.Add (s);
			} else if ( !yes && skin == s ) {
				skins.Add (s);
			}
		}
	}
}

public class QuestionController : MonoBehaviour {

	public GameObject beingText;
	public static QuestionController instance;
	public List<Condition> conditions =  new List<Condition>();
	private List <GameObject> icons = new List<GameObject>();

	public List<Condition.Hair> allHairs = new List<Condition.Hair>();
	public List<Condition.Pant> allPants = new List<Condition.Pant>();
	public List<Condition.Skin> allSkins = new List<Condition.Skin>();

	public List<Condition.Hair> validHairs = new List<Condition.Hair>();
	public List<Condition.Pant> validPants = new List<Condition.Pant>();
	public List<Condition.Skin> validSkins = new List<Condition.Skin>();

	public List<Condition.Hair> invalidHairs = new List<Condition.Hair>();
	public List<Condition.Pant> invalidPants = new List<Condition.Pant>();
	public List<Condition.Skin> invalidSkins = new List<Condition.Skin>();


	// Use this for initialization
	void Start () {
		instance = this;

		Array hairsArray = Enum.GetValues (typeof(Condition.Hair));
		foreach (Condition.Hair h in hairsArray) {
			allHairs.Add (h);
		}

		Array pantsArray = Enum.GetValues (typeof(Condition.Pant));
		foreach (Condition.Pant p in pantsArray) {
			allPants.Add (p);
		}

		Array skinsArray = Enum.GetValues (typeof(Condition.Skin));
		foreach (Condition.Skin s in skinsArray) {
			allSkins.Add (s);
		}
			
		beingText.SetActive(true);
	}

	delegate Condition CreateConditionFunc(bool isPositive);

	public void defineConditions(int numCondition, int numNegations ){

		conditions.Clear();



		List<bool> isPositiveList = new List<bool> ();
		for(int i = 0; i < numCondition; i++ ) {
			isPositiveList.Add( i<= numNegations ? false : true );
		}

		List<CreateConditionFunc> createConditions = new List<CreateConditionFunc> ();
		createConditions.Add (getQuestionHair);
		createConditions.Add (getQuestionPant);
		createConditions.Add (getQuestionSkin);

		numCondition = numCondition > 3 ? 3 : numCondition;

		for (int i = 0; i < numCondition; i++) {
			int index = UnityEngine.Random.Range(0, createConditions.Count);
			int positiveIndex = UnityEngine.Random.Range(0, isPositiveList.Count);
			conditions.Add(createConditions[index]( !isPositiveList[positiveIndex] ) );
			createConditions.RemoveAt (index);
			isPositiveList.RemoveAt( positiveIndex );
		}

		validHairs = new List<Condition.Hair> ( allHairs.ToArray() );
		validPants = new List<Condition.Pant>( allPants.ToArray() );
		validSkins = new List<Condition.Skin>( allSkins.ToArray() );

		invalidHairs = new List<Condition.Hair>();
		invalidPants = new List<Condition.Pant>();
		invalidSkins = new List<Condition.Skin>();

		foreach (Condition condition in conditions) {
			condition.RemoveInvalidElements( validHairs, validPants, validSkins );
			condition.AddInvalidElements( invalidHairs, invalidPants, invalidSkins );
		}

		killIcons ();
		ShowConditions ();
	}

	public void killIcons(){

		foreach (GameObject icon in icons) {

			if (icon != null) {
				Destroy (icon.gameObject);
			}
		}
		icons.Clear ();

	}

	void ShowConditions(){

		beingText.SetActive(false);
		if (conditions.Count == 1) {
			for (int i = 0; i < conditions.Count; i++) {
				Condition cond = conditions [i];
				GameObject icon = cond.CreateDisplay ();
				icons.Add (icon);
				icon.transform.parent = this.transform;
				icon.transform.localPosition = new Vector3 (0f, 0, 0);
			}
			
		} else if (conditions.Count == 2) {
			for (int i = 0; i < conditions.Count; i++) {
				Condition cond = conditions [i];
				GameObject icon = cond.CreateDisplay ();
				icons.Add (icon);
				icon.transform.parent = this.transform;
				icon.transform.localPosition = (i- 0.5f) * 2f * (new Vector3 (1f, 0, 0));
			}

		} else if (conditions.Count == 3) {
			for (int i = 0; i < conditions.Count; i++) {
				Condition cond = conditions [i];
				GameObject icon = cond.CreateDisplay ();
				icons.Add (icon);
				icon.transform.parent = this.transform;
				icon.transform.localPosition = (i- 1f) * 1f * (new Vector3 (1f, 0, 0));
			}

		} else {
			for (int i = 0; i < conditions.Count; i++) {
				Condition cond = conditions [i];
				GameObject icon = cond.CreateDisplay ();
				icons.Add (icon);
				icon.transform.parent = this.transform;
				icon.transform.localPosition = (i - (conditions.Count - 1) / 2) * 3f * (new Vector3 (1f, 0, 0));
			}
		}
	}


	Condition getQuestionPant( bool isPositive ){

		int n = Enum.GetNames(typeof(Condition.Pant)).Length;
		Condition.Pant pant = (Condition.Pant)UnityEngine.Random.Range(0, n);

		Condition condition = new PantCondition (isPositive, pant);
		return condition;		

	}

	Condition getQuestionHair( bool isPositive ){

		int n = Enum.GetNames(typeof(Condition.Hair)).Length;
		Condition.Hair hair = (Condition.Hair)UnityEngine.Random.Range(0, n);

		Condition condition = new HairCondition (isPositive, hair);
		return condition;		

	}
	Condition getQuestionSkin( bool isPositive ){

		int n = Enum.GetNames(typeof(Condition.Skin)).Length;
		Condition.Skin pant = (Condition.Skin)UnityEngine.Random.Range(0, n);

		Condition condition = new SkinCondition (isPositive, pant);
		return condition;		

	}


	public bool IsValid ( Person person ) {
		foreach (Condition cond in conditions) {
			if (!cond.isValid (person)) {
				return false;
			}
		}
		return true;
	}

}