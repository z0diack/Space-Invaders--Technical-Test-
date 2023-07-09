using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// --------------------------------------------------------------------------------------------------------
// 
// Base class for Space Invaders game.
//
// Please derive from this class to implement your game.
// 
// --------------------------------------------------------------------------------------------------------

public class BaseSpaceInvaders : MonoSingleton<BaseSpaceInvaders>, ISpaceInvaders
{

	public GameObject 		resultsScreen;

	protected override void Awake()
	{
		base.Awake();
	}

	void Start() 
	{
	
	}

	void Update() 
	{
	
	}

	public void HandleHit( GameObject object1 , GameObject object2 )
	{

	}

	public void GameOver( int score )
	{
		resultsScreen.SetActive( true );
	}
}
