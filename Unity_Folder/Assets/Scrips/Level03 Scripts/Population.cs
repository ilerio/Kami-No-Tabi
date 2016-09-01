using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Population : MonoBehaviour {

	public Transform holderHiragana;
	public Transform holderKatakana;
	public GameObject item;

	private Characters ch;

	void Start ()
	{
		ch = this.transform.GetComponent<Characters>();
		IEnumerator hiragana = ch.getHiraganaEnum();
		IEnumerator katakana = ch.getKatakanaEnum();

		Vector3 scale;
		scale.x = 1f;
		scale.y = 1f;
		scale.z = 1f;

		// Populate Hiragana
		while(hiragana.MoveNext())
		{
			GameObject newHiragana = Instantiate(item);
			newHiragana.transform.SetParent(holderHiragana);
			newHiragana.GetComponent<Text>().text = (string) hiragana.Current;
			newHiragana.name = (string) hiragana.Current;
			newHiragana.transform.localScale = scale;
		}

		// Populate Katakana
		while(katakana.MoveNext())
		{
			GameObject newKatakana = Instantiate(item);
			newKatakana.transform.SetParent(holderKatakana);
			newKatakana.GetComponent<Text>().text = (string) katakana.Current;
			newKatakana.name = (string) katakana.Current;
			newKatakana.transform.localScale = scale;
		}
	}
}