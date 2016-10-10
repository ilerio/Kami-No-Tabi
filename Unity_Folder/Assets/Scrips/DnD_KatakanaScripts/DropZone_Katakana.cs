using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DropZone_Katakana : MonoBehaviour, IDropHandler, IPointerEnterHandler
{
	public GameObject objectInThisSpot;

	private bool occupied = false;
	private GamecontrollerDnDKatakana GCDnD;
	private Color colorWhite;
	
	void Awake()
	{
		GCDnD = GamecontrollerDnDKatakana.FindObjectOfType (typeof (GamecontrollerDnDKatakana)) as GamecontrollerDnDKatakana;
		
		colorWhite = Color.white;
		colorWhite.a = 0.393f;
	}
	
	public void OnDrop(PointerEventData eventData)
	{
		//Debug.Log (eventData.pointerDrag.name + " OnDrop to " + gameObject.name);
		
		Dragable dropedObject = eventData.pointerDrag.GetComponent<Dragable>();
		if (dropedObject != null && !occupied)
		{
			// removing the object from the previous DropZone && unregistering it
			if (dropedObject.GetDropZoneParentK() != null)
			{
				DropZone_Katakana dod = dropedObject.GetDropZoneParentK(); // Droped Object Dropzone
				int dodsn = dod.GetSlotNumber(); // Droped Object Dropzone Slot Number
				dod.objectInThisSpot = null;
				GCDnD.unsetInputKatakana(dodsn);
			}

			dropedObject.parentToReturnTo = this.transform;
			objectInThisSpot = eventData.pointerDrag;
			
			int slotNum = this.GetSlotNumber();
			GCDnD.setInputHirgana(slotNum,eventData.pointerDrag.name);
			
			this.transform.GetComponent<Image>().color = colorWhite;
		}
		else if(dropedObject != null && occupied)
		{
			int slotNum = -1;
			
			// Object In this spot handler for moving to new spot
			Transform parentTransform = dropedObject.parentToReturnTo;
			DropZone_Katakana parentDropZone = parentTransform.GetComponent<DropZone_Katakana>();
			if(parentDropZone != null)
				parentDropZone.objectInThisSpot = objectInThisSpot;
			
			objectInThisSpot.GetComponent<Dragable>().parentToReturnTo = dropedObject.parentToReturnTo;
			objectInThisSpot.transform.SetParent(dropedObject.parentToReturnTo);
			objectInThisSpot.transform.SetSiblingIndex(dropedObject.GetPlaceholderSiblingIndex());
			if (dropedObject.GetDropZoneParentK() != null)
			{
				slotNum = dropedObject.GetDropZoneParentK().GetSlotNumber();
				GCDnD.setInputHirgana(slotNum,objectInThisSpot.name);
			}
			dropedObject.parentToReturnTo.transform.GetComponent<Image>().color = colorWhite;
			
			// Object coming into this spot handler
			dropedObject.parentToReturnTo = this.transform;
			eventData.pointerDrag.transform.SetParent(this.transform);
			objectInThisSpot = eventData.pointerDrag;
			slotNum = this.GetSlotNumber();
			GCDnD.setInputHirgana(slotNum,eventData.pointerDrag.name);
			this.transform.GetComponent<Image>().color = colorWhite;
		}
		
		//Debug.Log("Occupied = " + occupied);
	}
	
	public void OnPointerEnter (PointerEventData eventData)
	{
		if (this.transform.childCount > 0)
			occupied = true;
		else
			occupied = false;
		
		//Debug.Log(gameObject.name + " occupied: " + occupied);
	}
	
	public int GetSlotNumber()
	{
		string[] temp = this.name.Split('_'); // The numerical location of the slot
		int num = int.Parse(temp[1]);
		
		return num;
	}
}
