using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour 
{
	public Text timer;
	public Text incDec;
	public static float startingTime = 20f;
	public GameObject incDecGO;

	private float countingTime;
	private int incAmmount;
	private int decAmmount;

	// Use this for initialization
	void Start () 
	{
		timer.text = "0";
		countingTime = startingTime;
		incDecGO.SetActive (false);
		incAmmount = 5;
		decAmmount = -3;
	}
	
	// Update is called once per frame
	void Update () 
	{
		countingTime -= Time.deltaTime;

		if(countingTime > 0)
			timer.text = countingTime.ToString("0");
		else
			timer.text = "0";
	}

	public void AddTime()
	{
		// adds time on correct answer
		// handels incdec shower

		incDec.color = Color.green;
		IncDec (incAmmount);

		countingTime += incAmmount;
	}

	public void DecTime()
	{
		// Decs time on wrong answer but only in higher levels.
		// handels incdec shower

		incDec.color = Color.red;
		IncDec(decAmmount);

		countingTime += decAmmount;
	}

	void IncDec(int IncDecAmmount)
	{
		if (IncDecAmmount > 0)
			incDec.text = "+" + IncDecAmmount;
		else
			incDec.text = "" + IncDecAmmount;

		incDecGO.SetActive (true);
		Invoke("IncDecSetInactive",0.5f);
	}
	
	void IncDecSetInactive()
	{
		incDecGO.SetActive (false);
	}

	public int getCountingTime()
	{
		return Mathf.FloorToInt(countingTime);
	}

	/*/ used for progression
	public void SetIncAmmount(int Ammount)
	{

	}

	public void SetDecAmmount(int Ammount)
	{

	}*/

	/*public void ResetTime()
	{
		countingTime = startingTime;
	}*/
}
