using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Result : MonoBehaviour {

	public Text resultName;
	public Text resultScore;

	// Use this for initialization
	void Start () {
	
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
