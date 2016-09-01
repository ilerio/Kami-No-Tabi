using UnityEngine;
using System.Collections;

public class L02_InGameTransition : MonoBehaviour
{
	void transition(SpriteRenderer sr)
	{
		while (sr.color.a > 0)
		{
//			sr.color.a -= Time.deltaTime * sr.color.a;
		}
	}
}
