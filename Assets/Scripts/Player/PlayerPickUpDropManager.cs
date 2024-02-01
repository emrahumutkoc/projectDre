using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDropManager : MonoBehaviour {

    [SerializeField] private Transform playercameraTransform;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private float pickUpDistance = 2f;
    [SerializeField] private InventoryManager inventoryManager;

    private List<ObjectGrabbable> itemsOnTheToolBar = new List<ObjectGrabbable>();

    private ObjectGrabbable objectGrabbable;
    private ObjectGrabbable currentHoldingObject;

    private void Start() {
        inventoryManager.OnSelectedItemChanged += OnSelectedItemChanged;
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            if (objectGrabbable != null && objectGrabbable.isGrabbing) {
                DropItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            if (Physics.Raycast(playercameraTransform.position, playercameraTransform.forward, out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask)) {
                if (raycastHit.transform.TryGetComponent(out objectGrabbable)) {

                    // bool isAdded = inventoryManager.AddItem(objectGrabbable);
                    /*if (isAdded) {
                        objectGrabbable.Grab(objectGrabPointTransform);
                    }*/
                    InventoryItem inventoryItem = inventoryManager.AddTheItem(objectGrabbable);
                    if (inventoryItem != null) {
                        objectGrabbable.BringItToPlayer(objectGrabPointTransform);
                    }
                    Debug.Log(inventoryItem);

                }
            }
            /*if (objectGrabbable == null) {
                // not carrying an object, try to grab
                if (Physics.Raycast(playercameraTransform.position, playercameraTransform.forward, out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask)) {
                    if (raycastHit.transform.TryGetComponent(out objectGrabbable)) {
                        Debug.Log("objectGrabbable.scriptableItem" + objectGrabbable.scriptableItem);
                        bool isAdded = inventoryManager.AddItem(objectGrabbable.scriptableItem);
                        if (isAdded) {
                            objectGrabbable.Grab(objectGrabPointTransform);
                        }
                    }
                }
            } else { // an item is grabbed. need to add
                
            }*/
        }
    }

    private void OnSelectedItemChanged(object sender, InventoryManager.OnSelectedItemChangedEventArgs e) {

        Debug.Log("selected item " + e.selectedItem + "index    " + e.selectedIndex);

        InventoryItem inventoryItem = e.inventoryItem;
        if (inventoryItem?.grabbableObject) {
            inventoryItem.grabbableObject.Grab(objectGrabPointTransform);
            HideAllItemsOnHand();
            inventoryItem.grabbableObject.gameObject.SetActive(true);
            currentHoldingObject = inventoryItem.grabbableObject;
        } else if (inventoryItem == null) {
            HideAllItemsOnHand();
            currentHoldingObject = null;
        }


        int toolBarLength = inventoryManager.toolBarInventoryGroup.transform.childCount;


        // itemsOnTheToolBar[e.selectedIndex] = e.selectedItem.objectTheItem;
        // allGrabbableObjects

        /* if (e.selectedItem != null) {
             Debug.Log(e.selectedItem + "is selected");
             // objectGrabbable.Grab(objectGrabPointTransform);
             if (objectGrabPointTransform.childCount > 0) {
                 for (int i = 0; i < objectGrabPointTransform.childCount; i++) {
                     ObjectGrabbable o = objectGrabPointTransform.GetChild(i).GetComponent<ObjectGrabbable>();
                     if (o != null && o.scriptableItem.prefab == e.selectedItem.prefab) {
                         HideAllItemsOnHand();
                         o.gameObject.SetActive(true);
                         Debug.Log("Ayný item");
                     }
                 }
             }

         } else {
             HideAllItemsOnHand();
         }*/
    }

    private void HideAllItemsOnHand() {
        if (objectGrabPointTransform.childCount > 0) {
            for (int i = 0; i < objectGrabPointTransform.childCount; i++) {
                objectGrabPointTransform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void DropItem() {
        inventoryManager.DropSelectedItem();
        currentHoldingObject.Drop();
        currentHoldingObject = null;
    }
}
