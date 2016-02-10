using UnityEngine;
using System.Collections;

public class SaveDataManager : MonoBehaviour {

	public static SaveDataManager _instance;
	private int bestLevel;

	// Use this for initialization
	void Awake () {
		_instance=this;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void saveSoundPreset(float soundLevel){
		PlayerPrefs.SetFloat("soundLevel",soundLevel);
	}

	public void setBestLevel(int level,ChallengeType challengeType){
		if(challengeType==ChallengeType.TIME){
			if(PlayerPrefs.GetInt("bestLevelTime")==null){
				bestLevel=PlayerPrefs.GetInt("bestLevelTime");
			}else{
				bestLevel=1;
			}
		}

		if(challengeType==ChallengeType.ACURACY){
			if(PlayerPrefs.GetInt("bestLevelAcuracy")==null){
				bestLevel=PlayerPrefs.GetInt("bestLevelAcuracy");
			}else{
				bestLevel=1;
			}
		}
			
		if(level>bestLevel){
			bestLevel=level;

			if(challengeType==ChallengeType.TIME){
				PlayerPrefs.SetInt("bestLevelTime",bestLevel);
			}

			if(challengeType==ChallengeType.ACURACY){
				PlayerPrefs.SetInt("bestLevelAcuracy",bestLevel);
			}
		}
	}
}
