using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Dragable_L03 : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler 
{
	public GameObject item;
	public Transform originalParent = null;
	
	private GameObject placeholder = null;

	void Update()
	{
		if (this.transform.parent.tag.Equals("Slot") && (this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts == true))
		{	
			this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
		}
	}	

	public void OnBeginDrag(PointerEventData eventData)
	{
		//Debug.Log ("OnBeginDrag");
		
		placeholder = new GameObject();
		placeholder.name = "placeholder";
		placeholder.transform.SetParent(this.transform.parent);
		
		// size stuff
		LayoutElement le = placeholder.AddComponent<LayoutElement>();
		le.preferredWidth = this.GetComponent<ILayoutElement>().preferredWidth;
		le.preferredHeight = this.GetComponent<ILayoutElement>().preferredHeight;
		
		placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());

		this.transform.SetParent(this.transform.parent.parent);
		
		this.GetComponent<CanvasGroup>().blocksRaycasts = false;
	}
	
	public void OnDrag(PointerEventData eventData)
	{
		//Debug.Log ("OnDrag");
		
		this.transform.position = eventData.position;
	}
	
	public void OnEndDrag(PointerEventData eventData)
	{
		//Debug.Log ("OnEndDrag");
		this.transform.SetParent(originalParent);
		this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
		this.GetComponent<CanvasGroup>().blocksRaycasts = true;

		Destroy (placeholder);
	}
	
	public int GetPlaceholderSiblingIndex()
	{
		if (placeholder != null)
			return placeholder.transform.GetSiblingIndex();
		else
			return 0;
	}
	
	public void OnPointerDown (PointerEventData eventData)
	{
		originalParent = this.transform.parent;

		if (eventData != null && this.transform.parent.tag.CompareTo("Holder") == 0)
		{
			Vector3 scale;
			scale.x = 2f;
			scale.y = 2f;
			scale.z = 1f;
			this.transform.localScale = scale;
		}
	}
	
	public void OnPointerUp (PointerEventData eventData)
	{
		if (eventData != null)
		{
			Vector3 scale;
			scale.x = 1f;
			scale.y = 1f;
			scale.z = 1f;
			this.transform.localScale = scale;
		}
	}
}