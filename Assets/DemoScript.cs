using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour {
    public InventoryManager inventoryManager;
    public ItemOld[] itemsToPickup;

    public void PickUpItem(int id) {
        bool isAdded = inventoryManager.AddItem(itemsToPickup[id]);

        if (isAdded) {
            Debug.Log("ADDED!");
        } else {
            Debug.Log("Inventory is full");
        }
    }

    public void GetSelectedItem() {
        ItemOld receivedItem = inventoryManager.GetSelectedItem();
        if (receivedItem != null) {
            Debug.Log("Received item" + receivedItem);
        } else {
            Debug.Log("no item");
        }
    }

    public void UseSelectedItem() {
        ItemOld receivedItem = inventoryManager.UseSelectedItem();
        if (receivedItem != null) {
            Debug.Log("Item Used" + receivedItem);
        } else {
            Debug.Log("no item");
        }
    }
}
