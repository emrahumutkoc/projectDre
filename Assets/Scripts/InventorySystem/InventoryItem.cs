using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [Header("UI")]
    public Image image;
    public Text countText;

    [HideInInspector] public Item item;
    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;


    public void InitializeItem(Item newItem) {
        item = newItem;
        image.sprite = newItem.image;
        RefreshCount();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        Debug.Log("begindrag");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void RefreshCount() {
        countText.text = count.ToString();
        countText.gameObject.SetActive(count > 1);
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
        Debug.Log("onDrag");
    }

    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("end drag");

        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
}
