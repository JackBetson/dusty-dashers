using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[Header("Game Settings")]
	public VehicleMovement vehicleMovement; // reference to the VehicleMovement script

	[Header("UI References")]
	public GameObject gameOverUI;           // reference to the UI objects that appears when the game is complete
	bool isGameOver;                        // flag to determine if the game is over
	bool levelStart;                        // flag to determine if level has started


	void Awake()
	{
		// If there is no GameManager, set this as the GameManager
		if (instance == null)
			instance = this;

		// ...destroy this and keep the original GameManager
		else if (instance != this)
			Destroy(gameObject);
	}

	void OnEnable()
	{
		// Coroutine to add buffer time before Player can move.
		StartCoroutine(Init());
	}

	IEnumerator Init()
	{
		// Increase this value to add more time, maybe for a countdown or intro scene.
		yield return new WaitForSeconds(.1f);
		levelStart = true;
	}

	void Update()
	{
		// If game is active...
		if (IsActiveGame())
		{
			// ...potentially do stuff here
			// load ui stuff

		}
	}


	public void GameOver()
	{
		// If game is already over exit this method 
		if (isGameOver)
			return;

		// If the game is over...
		// if (some end condition)
		{
			// set game over
			isGameOver = true;

			// show the Game Over UI
			gameOverUI.SetActive(true);
		}
	}

	public bool IsActiveGame()
	{
		return levelStart && !isGameOver;
	}

	public void Restart()
	{
		// Restart scene
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
