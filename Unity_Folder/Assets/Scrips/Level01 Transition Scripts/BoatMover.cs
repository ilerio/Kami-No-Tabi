using UnityEngine;
using System.Collections;

public class BoatMover : MonoBehaviour 
{
	public float speed;

	void Update () 
	{
		this.transform.Translate(Vector3.right * Time.deltaTime * speed);
	}
}
