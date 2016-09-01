using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DropZone_L03 : MonoBehaviour, IDropHandler, IPointerEnterHandler
{
	public GameObject item;
	public GameObject objectInThisSpot;
	
	private bool occupied = false;
	private Color colorWhite;
	
	void Awake()
	{
		colorWhite = Color.white;
		colorWhite.a = 0.393f;
	}
	
	public void OnDrop(PointerEventData eventData)
	{
		//Debug.Log (eventData.pointerDrag.name + " OnDrop to " + gameObject.name);

		Dragable_L03 dropedObjectDragable = eventData.pointerDrag.GetComponent<Dragable_L03>();
		GameObject dropedObject = eventData.pointerDrag;
		if (dropedObjectDragable != null && !occupied)
		{
			GameObject newItem = Instantiate(item);
			newItem.name = dropedObject.name;
			newItem.GetComponent<Text>().text = dropedObject.GetComponent<Text>().text;
			
			newItem.transform.SetParent(this.transform);
			objectInThisSpot = newItem;

			this.transform.GetComponent<Image>().color = colorWhite;
		}
		else if(dropedObjectDragable != null && occupied)
		{
			Destroy(objectInThisSpot);

			GameObject newItem = Instantiate(item);
			newItem.name = dropedObject.name;
			newItem.GetComponent<Text>().text = dropedObject.GetComponent<Text>().text;
			
			newItem.transform.SetParent(this.transform);
			objectInThisSpot = newItem;

			this.transform.GetComponent<Image>().color = colorWhite;
		}
	}
	
	public void OnPointerEnter (PointerEventData eventData)
	{
		if (this.transform.childCount > 0)
			occupied = true;
		else
			occupied = false;
		
		//Debug.Log(gameObject.name + " occupied: " + occupied);
	}
}
