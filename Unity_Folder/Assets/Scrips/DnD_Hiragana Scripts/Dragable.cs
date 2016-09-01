using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Dragable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler 
{
	public Transform parentToReturnTo = null;

	private GameObject placeholder = null;

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

		parentToReturnTo = this.transform.parent;
		this.transform.SetParent(this.transform.parent.parent);

		GetComponent<CanvasGroup>().blocksRaycasts = false;
	}

	public void OnDrag(PointerEventData eventData)
	{
		//Debug.Log ("OnDrag");

		this.transform.position = eventData.position;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		//Debug.Log ("OnEndDrag");
		this.transform.SetParent(parentToReturnTo);
		this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
		GetComponent<CanvasGroup>().blocksRaycasts = true;

		Destroy (placeholder);
	}

	public int GetPlaceholderSiblingIndex()
	{
		if (placeholder != null)
			return placeholder.transform.GetSiblingIndex();
		else
			return 0;
	}

	public DropZone GetDropZoneParent()
	{
		return parentToReturnTo.transform.GetComponent<DropZone>();
	}
	
	public void OnPointerDown (PointerEventData eventData)
	{
		if (eventData != null && this.transform.parent.name.CompareTo("Holder") == 0)
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