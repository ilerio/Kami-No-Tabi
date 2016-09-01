using UnityEngine;
using System.Collections;

public class RedFlash : MonoBehaviour 
{
	public GameObject redImage;

	void Awake()
	{
		redImage.SetActive (false);
	}

	public void Flash()
	{
		redImage.SetActive (true);
		Invoke("TurnOff", 0.2f);
	}

	void TurnOff()
	{
		redImage.SetActive (false);
	}

	public void TurnOn()
	{
		redImage.SetActive (true);
	}
}
