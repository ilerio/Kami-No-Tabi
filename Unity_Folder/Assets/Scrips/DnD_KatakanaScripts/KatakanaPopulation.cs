using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class KatakanaPopulation : MonoBehaviour 
{
	public Sprite[] katakana; 
	public GameObject katakanaItem;
	public Transform holder;
	
	private Stack<int> katakanaNumStack = new Stack<int>();
	
	void Awake()
	{
		int[] check = new int[46];
		int randomNumber;
		int loopCount = 0;
		
		while(katakanaNumStack.Count < 46) // should be 46
		{
			randomNumber = Random.Range(0, katakana.Length);
			bool flag = true;
			
			if(check[randomNumber] != 0)
				flag = false;
			
			if(flag == true)
			{
				katakanaNumStack.Push(randomNumber);
				//Debug.Log (randomNumber + " selected.");
				check[randomNumber] = -1;
			}
			/*else
				Debug.Log ("DENIED!");*/
			
			loopCount ++;
		}
		
		Debug.Log ("pre-selection of integers done! " + loopCount + " loop throughs.");
		
		while (katakanaNumStack.Count > 0)
		{
			int current = katakanaNumStack.Pop();
			//Debug.Log (katakanaNumStack.Count + ": " + current);
			GameObject newKatakana = Instantiate(katakanaItem);
			newKatakana.transform.SetParent(holder);
			newKatakana.GetComponent<Image>().sprite = katakana[current];
			newKatakana.name = katakana[current].name;
			
			Vector3 scale;
			scale.x = 1f;
			scale.y = 1f;
			scale.z = 1f;
			newKatakana.transform.localScale = scale;
		}
	}
}
