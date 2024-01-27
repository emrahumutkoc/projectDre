using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    [SerializeField] GameObject mainInvontoryGroup;

    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;

    private bool isInventoryActive = false;

    private void Start() {
        mainInvontoryGroup.SetActive(isInventoryActive);
    }

    public bool AddItem(Item item) {

        // Check if any slot has the same item with count lower than max
        for (int i = 0; i < inventorySlots.Length; i++) {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && item.stackable && itemInSlot.count < item.maximumHold) {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                // SpawnNewItem(item, slot);
                return true;
            }
        }


        // find any empty slot
        for (int i = 0; i < inventorySlots.Length; i++) {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null) {
                SpawnNewItem(item, slot);
                return true;
            }
        }

        return false;
    }
    public void SpawnNewItem(Item item, InventorySlot slot) {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem invetoryItem = newItemGo.GetComponent<InventoryItem>();
        invetoryItem.InitializeItem(item);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            isInventoryActive = !isInventoryActive;
            mainInvontoryGroup.SetActive(isInventoryActive);
            if (isInventoryActive) {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
