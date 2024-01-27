using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour {
    public InventoryManager inventoryManager;
    public Item[] itemsToPickup;

    public void PickUpItem(int id) {
        bool isAdded = inventoryManager.AddItem(itemsToPickup[id]);

        if (isAdded) {
            Debug.Log("ADDED!");
        } else {
            Debug.Log("Inventory is full");
        }
    }
}
