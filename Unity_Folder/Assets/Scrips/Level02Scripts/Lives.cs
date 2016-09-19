using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Lives : MonoBehaviour 
{
	public GameObject heart1;
	public GameObject heart2;
	public GameObject heart3;
	public GameObject heart4;

	private int lives;

	void Start()
	{
		lives = 4;
	}

	public void AddLife()
	{
		if (lives < 4) 
		{
			switch (lives)
			{
				case 1:
					heart2.SetActive (true);
					break;
				case 2:
					heart3.SetActive (true);
					break;
				case 3:
					heart4.SetActive (true);
					break;
				default:
					Debug.Log("Error Lives");
					break;
			}
			lives++;
		}
	}

	public void DecLife()
	{
		if (lives > 0) 
		{
			switch (lives)
			{
			case 1:
				heart1.SetActive (false);
				break;
			case 2:
				heart2.SetActive (false);
				break;
			case 3:
				heart3.SetActive (false);
				break;
			case 4:
				heart4.SetActive (false);
				break;
			default:
				Debug.Log("Error Lives");
				break;
			}
			lives--;
		}
	}

	public int getNumLives()
	{
		return lives;
	}
}
