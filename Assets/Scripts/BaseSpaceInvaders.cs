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



	//Player variables
	[Tooltip("Movement speed for the player")]
	public float moveSpeed = 15f;
	public float hInput;
	private GameObject player;
	

	//Boundaries
	private readonly float minX = -63f;
	private readonly float maxX = 63f;

	//Sheild Variables
	public GameObject shieldPrefab;
	private readonly float hStart = -46f;
	private readonly float spacingH = 30f;

	//Enemy Variables
	public GameObject enemyPrefab;
	public float enemyMoveSpeed = 5f;
	private readonly int enemyNumRows = 5;
	private readonly int enemyNumColumns = 10;
	private readonly float enemySpacingH = 12;
	private readonly float enemySpacingV = 12f;
	private Vector2 startingPosition = new Vector2(-55, 170);
	private readonly float enemyVerticalMovement = -5f;
	private float direction = 1f;


	//Shooting
	public GameObject bulletPrefab;
	private readonly float bulletSpeed = 100f;
	private bool canShoot = true;
	private readonly float bulletDelay = 1f;

	//Game variables
	public GameObject 		resultsScreen;
	public int score;
	public Text scoreUI;

	protected override void Awake()
	{
		base.Awake();
		player = GameObject.Find("Player");

		score = 0;

		//Iterating the sheild spawning
		for(int i = 0; i<4; i++)
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

	void Start() 
	{
	
	}

	void Update() 
	{
		//Getting input from the user.
		hInput = Input.GetAxisRaw("Horizontal");
		// Calculate the new position
		Vector2 newPosition = player.transform.position + new Vector3(hInput * moveSpeed * Time.deltaTime, 0f,0f);

		// Clamp the position within the valid range
		newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

		// Update the player's position
		player.transform.position = newPosition;

		//Need to add delay
        if (Input.GetKeyDown(KeyCode.Space) && canShoot)
        {
			SpawnBullet(bulletDelay);
        }


		//Enemy movement
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject e in enemies)
        {
			// Calculate the horizontal movement
			float horizontalMovement = direction * enemyMoveSpeed * Time.deltaTime;


			e.transform.Translate(horizontalMovement, 0f, 0f);
			Vector3 clampedPosition = e.transform.position;
			clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
			e.transform.position = clampedPosition;

			if (e.transform.position.x == minX || e.transform.position.x == maxX)
            {
				foreach(GameObject e1 in enemies)
                {
					//Moving z moves the y??
					e1.transform.transform.Translate(0f, 0f, enemyVerticalMovement);
				}
				direction = -direction;
				enemyMoveSpeed += 1f;
			}
		}
		scoreUI.text = score.ToString();

		if (enemies.Length == 0)
		{
			GameOver(score);
		}
	}

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

	private void SpawnBullet(float delay)
    {
		StartCoroutine(SpawnBulletWithDelay(delay));
    }

	public void HandleHit( GameObject object1 , GameObject object2 )
	{
		if(object1.CompareTag("Enemy") && object2.CompareTag("Bullet"))
        {
			Destroy(object1);
			Destroy(object2);
			score += 10;
        }

		if(object1.CompareTag("Shield") && object2.CompareTag("Bullet"))
        {
			Destroy(object2);
        }

		if((object1.name == "Player") && (object2.CompareTag("Enemy")))
		{
			GameObject[] enenmies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach(GameObject e in enenmies)
            {
				Destroy(e);
            }
			GameOver(score);
        }

		if((object1.name == "End Trigger" && object2.CompareTag("Enemy")))
        {
			GameObject[] enenmies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject e in enenmies)
			{
				Destroy(e);
			}
			GameOver(score);
		}
	}

	public void GameOver( int score )
	{
		resultsScreen.SetActive( true );
	}
}
