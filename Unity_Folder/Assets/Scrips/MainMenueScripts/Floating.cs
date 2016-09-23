using UnityEngine;
using System.Collections;

public class Floating : MonoBehaviour 
{
	public float bobSpeed;

	private bool floatUp;

	void Start () 
	{
		if (Random.value > 0.5f)
			floatUp = true;
		else
			floatUp = false;

		if(floatUp)
			Invoke("FloatingUp", 0.1f);
		else
			Invoke("FloatingDown", 0.1f);
	}

	void Update () 
	{
		if(floatUp)
			Invoke("FloatingUp", 1.2f);
		else
			Invoke("FloatingDown", 1.2f);
	}

	void FloatingUp()
	{
		Vector3 pos = this.transform.position; 
		float OffsetY = (bobSpeed*Time.deltaTime);
		pos.y += OffsetY;
		this.transform.position = pos;
		
		floatUp = false;
	}
	
	void FloatingDown()
	{
		Vector3 pos = this.transform.position; 
		float OffsetY = (bobSpeed*Time.deltaTime);
		pos.y -= OffsetY;
		this.transform.position = pos;
		
		floatUp = true;
	}
}
