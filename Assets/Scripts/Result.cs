using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Result : MonoBehaviour {

	public Text resultName;
	public Text resultScore;


	// Use this for initialization
	void Awake () {
		Debug.Log("Test");


        SetScore("test", BaseSpaceInvaders.Instance.score);
	}
    private bool IsLetterOrNumber(char c)
    {
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9');
    }

    public void SetScore( string name, int score )
	{
		resultName.text = name;
		resultScore.text = score.ToString();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
