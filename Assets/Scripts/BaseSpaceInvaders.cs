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
	// ----------------
	// Player variables
	// ----------------
	[Tooltip("Movement speed for the player")]
	[SerializeField] float moveSpeed = 15f;
	
	//Horizontal input from the player
	private float hInput;
	
	//Player Gameobject
	private GameObject player;

	//Starting Position of the player
	private Vector3 playerStartPosition = Vector3.zero;
	
	//-----------
	// Boundaries
	// Map boundaries for enemies and player
	// ----------
	private readonly float minX = -63f;
	private readonly float maxX = 63f;


	// ----------------
	// Sheild Variables
	// ----------------
	[Tooltip("Shield prefab")]
	[SerializeField] GameObject shieldPrefab;
	private readonly float hStart = -46f;
	private readonly float spacingH = 30f;


	// ---------------
	// Enemy Variables
	// ----------------
	[Tooltip("Enemy prefab")]
	[SerializeField] GameObject enemyPrefab;

	//Enemy move speed
	private float enemyMoveSpeed = 5f;

	//Enemy vertical movement (How much it moves down once the boundaries are touched)
	private readonly float enemyVerticalMovement = -5f;

	//Current direction of the enemy
	private float direction = 1f;

	//Enemy spawning variables
	private readonly int enemyNumRows = 5;
	private readonly int enemyNumColumns = 10;
	private readonly float enemySpacingH = 12;
	private readonly float enemySpacingV = 12f;
	private Vector2 startingPosition = new Vector2(-55, 170);

	//Empty array of all the enemies spawned in
	private GameObject[] enemies;
	[Tooltip("Enemy bullet prefab")]
	[SerializeField] GameObject enemyBulletPrefab;
	private readonly float enemyShootDelay = 0.8f;
	private float timer = 0f;


	// ---------
	// Shooting
	// ---------
	//Player bullet prefab
	[SerializeField] GameObject bulletPrefab;
	private readonly float bulletSpeed = 100f;
	private bool canShoot = true;
	private readonly float bulletDelay = 1f;


	// --------------
	// UI variables
	// --------------
	[Tooltip("Results screen parent")]
	[SerializeField] GameObject resultsScreen;
	[SerializeField] GameObject results;
	[Tooltip("Current score of the player")]
	public int score;
	[Tooltip("Live updating score text")]
	[SerializeField] Text scoreUI;
	[Tooltip("Highscore text")]
	[SerializeField] Text highScoreUI;

	// ------
	// Audio
	// ------
	//Very basic audio, makes it feel better with audio.
	[SerializeField] private AudioSource shootAudio;
	[SerializeField] private AudioSource explosion;
	[SerializeField] private AudioSource enemyKilled;

	protected override void Awake()
	{
		base.Awake();
		StartNewGame();
		//Sets highscore to 0 on first play.
		PlayerPrefs.SetInt("highscore", 0);
	}

	void Start() 
	{
	
	}

	void Update() 
	{
        if (!resultsScreen.activeSelf) {
            //Getting input from the user.
            hInput = Input.GetAxisRaw("Horizontal");
            // Calculate the new position
            Vector2 newPosition = player.transform.position + new Vector3(hInput * moveSpeed * Time.deltaTime, 0f, 0f);

            // Clamp the position within the valid range
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

            // Update the player's position
            player.transform.position = newPosition;

			//Checks if player can shoot and if space is pressed.
            if (Input.GetKeyDown(KeyCode.Space) && canShoot)
            {
                SpawnBullet(bulletDelay);
            }
        }

		//Enemy movement
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject e in enemies)
        {
			// Calculate the horizontal movement
			float horizontalMovement = direction * enemyMoveSpeed * Time.deltaTime;

			//Moves them across 
			e.transform.Translate(horizontalMovement, 0f, 0f);

			//Locks them into boundaries
			Vector3 clampedPosition = e.transform.position;
			clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
			e.transform.position = clampedPosition;

			//If the boundary is touched it will move them down and flip the direction.
			if (e.transform.position.x == minX || e.transform.position.x == maxX)
            {
				foreach(GameObject e1 in enemies)
                {
					//Moving z moves the y due to the prefab
					e1.transform.transform.Translate(0f, 0f, enemyVerticalMovement);
				}
				direction = -direction;
				enemyMoveSpeed += 1f;
			}
		}

		//Updating score UI element
		scoreUI.text = score.ToString();


		//Timer for when random enemy can shoot.
		if (true)
		{
			timer += Time.deltaTime;

			if (timer >= enemyShootDelay)
			{
				ShootFromRandomEnemy();
				timer = 0f;
			}
		}

		//If all enemies are dead the game will end.
		if (enemies.Length == 0)
		{
			GameOver(score);
		}
	}

	/// <summary>
	/// Spawns a bullet on the player with a delay.
	/// </summary>
	/// <param name="delay">Delay between shooting.</param>
	/// <returns></returns>
	private IEnumerator SpawnBulletWithDelay(float delay)
	{
		canShoot = false;
		// Calculate the spawn position in front of the player
		Vector3 spawnPosition = player.transform.position + new Vector3(0f, 7.5f, 0f);

		// Instantiate the bullet object
		GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

		// Set the rotation of the bullet to match the prefab's rotation
		bullet.transform.rotation = bulletPrefab.transform.rotation;

		// Apply upward velocity to the bullet
		Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
		bulletRigidbody.velocity = Vector2.up * bulletSpeed;

		yield return new WaitForSeconds(delay);
		canShoot = true;
	}

	/// <summary>
	/// Finds all the enemies and selects a random one to shoot from.
	/// </summary>
	private void ShootFromRandomEnemy()
	{

		enemies = GameObject.FindGameObjectsWithTag("Enemy");
		if (enemies.Length == 0)
		{
			Debug.LogWarning("No enemies in the array.");
			return;
		}

		int randomIndex = Random.Range(0, enemies.Length);
		GameObject randomEnemy = enemies[randomIndex];

		Vector3 spawnPosition = randomEnemy.transform.position;

		GameObject bullet = Instantiate(enemyBulletPrefab, spawnPosition, Quaternion.identity);
		bullet.transform.rotation = enemyBulletPrefab.transform.rotation;

		Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
		bulletRigidbody.velocity = Vector2.down * bulletSpeed;
	}

	/// <summary>
	/// Encapsulates the spawn bullet with delay, and adds audio to it
	/// </summary>
	/// <param name="delay">Delay between spawning bullets.</param>
	private void SpawnBullet(float delay)
    {
		shootAudio.Play();
		StartCoroutine(SpawnBulletWithDelay(delay));
    }

	/// <summary>
	/// Handles all the collisions within the game
	/// </summary>
	/// <param name="object1">First object in the collision</param>
	/// <param name="object2">Second object in the collision</param>
	public void HandleHit( GameObject object1 , GameObject object2 )
	{
		//Enemy and bullet
		if(object1.CompareTag("Enemy") && object2.CompareTag("Bullet"))
        {
			Destroy(object1);
			Destroy(object2);
			score += 10;
			enemyKilled.Play();
        }

		//Shield and player bullet
		if(object1.CompareTag("Shield") && object2.CompareTag("Bullet"))
        {
			Destroy(object2);
        }

		//Shield and enemy bullet
		if (object1.CompareTag("Shield") && object2.CompareTag("Enemy Bullet"))
		{
			Destroy(object1);
			Destroy(object2);
		}

		//Player and enemy
		if ((object1.name == "Player") && (object2.CompareTag("Enemy")))
		{
			GameObject[] enenmies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach(GameObject e in enenmies)
            {
				Destroy(e);
            }
			explosion.Play();
			GameOver(score);
        }

		//Enemy and the end trigger
		if((object1.name == "End Trigger" && object2.CompareTag("Enemy")))
        {
			GameObject[] enenmies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject e in enenmies)
			{
				Destroy(e);
			}
			explosion.Play();
			GameOver(score);
		}

		//Player and an enemy bullet
		if(object1.name == "Player" && object2.CompareTag("Enemy Bullet"))
		{
			GameObject[] enenmies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject e in enenmies)
			{
				Destroy(e);
			}
			explosion.Play();
			GameOver(score);
		}

	}

	/// <summary>
	/// Handles when the game is over and sets the highscore if needed, shows results screen after.
	/// </summary>
	/// <param name="score">Score the player received.</param>
	public void GameOver( int score )
	{
		if(score > PlayerPrefs.GetInt("highscore"))
		{
			PlayerPrefs.SetInt("highscore", score);
        }

		resultsScreen.SetActive( true );
	}

	/// <summary>
	/// Function to start a new game, moves the player to the starting location as well as spawns in enemies & shields.
	/// </summary>
	public void StartNewGame()
    {
		player = GameObject.Find("Player");
		player.transform.position = playerStartPosition;
		score = 0;
		highScoreUI.text = PlayerPrefs.GetInt("highscore").ToString();

		resultsScreen.SetActive(false);

		//Iterating the sheild spawning
		for (int i = 0; i < 4; i++)
		{
			float hPos = hStart + (i * spacingH);
			GameObject newSheild = Instantiate(shieldPrefab);
			newSheild.transform.position = new Vector2(hPos, 20);
		}


		//Enemy Spawning
		Vector2 currentPos = startingPosition;

		for (int row = 0; row < enemyNumRows; row++)
		{
			for (int col = 0; col < enemyNumColumns; col++)
			{
				Instantiate(enemyPrefab);
				enemyPrefab.transform.position = currentPos;
				currentPos.x += enemySpacingH;
			}

			currentPos.x = startingPosition.x;
			currentPos.y -= enemySpacingV;
		}
	}
}
