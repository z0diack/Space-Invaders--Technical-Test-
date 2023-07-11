using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Result : MonoBehaviour {


    public Text resultName;
    public Text resultScore;



    // Use this for initialization
    void Start () 
    {      
        int score = PlayerPrefs.GetInt("highscore");
        SetScore("Highscore today", score);
    }


    public void SetScore( string name, int score )
	{
		resultName.text = name;
		resultScore.text = score.ToString();
	}
    

    // Update is called once per frame
    void Update()
    {
        //Updates score with highscore of the player
        int score = PlayerPrefs.GetInt("highscore");
        SetScore("Highscore today", score);
        if (Input.GetKeyDown(KeyCode.R))
            BaseSpaceInvaders.Instance.StartNewGame();
    }
}
