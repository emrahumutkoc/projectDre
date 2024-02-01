using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    [SerializeField] private GameObject mainInventoryGroup;
    [SerializeField] public GameObject toolBarInventoryGroup;
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;


    public event EventHandler<OnSelectedItemChangedEventArgs> OnSelectedItemChanged;
    public class OnSelectedItemChangedEventArgs : EventArgs {
        public ItemOld selectedItem;
        public int selectedIndex;
        public InventoryItem inventoryItem;
    }

    public int selectedSlotIndex = -1;
    private bool isInventoryActive = false;


    private void Start() {
        mainInventoryGroup.SetActive(isInventoryActive);
        ChangeSelectedSlot(0);
    }

    private void Update() {
        HandleInventorySelection();
        HandleMainInventory();
    }

    private void HandleMainInventory() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            isInventoryActive = !isInventoryActive;
            mainInventoryGroup.SetActive(isInventoryActive);
            if (isInventoryActive) {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    private void HandleInventorySelection() {
        if (Input.inputString != null) {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < (toolBarInventoryGroup.transform.childCount + 1)) {
                ChangeSelectedSlot(number - 1);
            }
        }
    }

    public void ChangeSelectedSlot(int newValue) {
        if (selectedSlotIndex >= 0) {
            inventorySlots[selectedSlotIndex].Deselect();
        }

        InventorySlot slot = inventorySlots[newValue];
        slot.Select();
        selectedSlotIndex = newValue;


        InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();
        if (inventoryItem != null) {
            ItemOld item = inventoryItem.item;
            OnSelectedItemChanged?.Invoke(this, new OnSelectedItemChangedEventArgs { selectedItem = item, selectedIndex = selectedSlotIndex, inventoryItem = inventoryItem });
        } else {
            OnSelectedItemChanged?.Invoke(this, new OnSelectedItemChangedEventArgs { selectedItem = null, selectedIndex = selectedSlotIndex, inventoryItem = null });
        }
    }

    private InventorySlot FindAvailableSlot(ItemOld item, ObjectGrabbable objectGrabbable) {
        // Check item available anywhere and stackable

        // Check selected available
        InventorySlot selectedSlot = inventorySlots[selectedSlotIndex];
        InventoryItem selectedItemInSlot = selectedSlot.GetComponentInChildren<InventoryItem>();
        if (selectedItemInSlot == null) {
            return selectedSlot;
        }

        // Find any available slot
        for (int i = 0; i < inventorySlots.Length; i++) {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null) {
                return slot;
            }
        }

        return null;
    }

    public InventoryItem AddTheItem(ObjectGrabbable objectGrabbable) {
        ItemOld item = objectGrabbable.scriptableItem;


        InventoryItem addedItem = null;
        InventorySlot addedInventorySlot = null;
        // Add over to available item slot
        for (int i = 0; i < inventorySlots.Length; i++) {
            InventorySlot invenSlot = inventorySlots[i];
            InventoryItem itemInSlot = invenSlot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && item.stackable && itemInSlot.count < item.maximumHold) {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                itemInSlot.grabbableObject = objectGrabbable;
                addedItem = itemInSlot;
                addedInventorySlot = invenSlot;
            }
        }
        // Find a available slot
        if (addedItem == null && addedInventorySlot == null) {
            InventorySlot slot = FindAvailableSlot(item, objectGrabbable);
            if (slot != null) {
                InventoryItem newInventoryItem = SpawnNewItem(item, slot, objectGrabbable);
                addedItem = newInventoryItem;
                addedInventorySlot = slot;
            }
        }

        if (addedItem != null && addedInventorySlot != null) {
            int newIndex = GetIndexSlot(addedInventorySlot);
            if (newIndex == selectedSlotIndex) {
                OnSelectedItemChanged?.Invoke(this, new OnSelectedItemChangedEventArgs { selectedIndex = selectedSlotIndex, inventoryItem = addedItem });
            }
        }

        return addedItem;
    }

    // will be removed
    public bool AddItem(ItemOld item) {

        //Item item = objectGrabbable.scriptableItem;
        // Current selected slot
        InventorySlot selectedSlot = inventorySlots[selectedSlotIndex];
        InventoryItem selectedItemInSlot = selectedSlot.GetComponentInChildren<InventoryItem>();
        // First available item
        InventoryItem firstAvailableItem = FindFirstAvailableSlotForExistItem(item);
        if (firstAvailableItem != null) { // collect to first available item
            firstAvailableItem.count++;
            firstAvailableItem.RefreshCount();
            return true;
        } else if (selectedItemInSlot == null) { // take selected slot
            SpawnNewItem(item, selectedSlot);
            return true;
        } else { // find any empty slot
            for (int i = 0; i < inventorySlots.Length; i++) {
                InventorySlot slot = inventorySlots[i];
                InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                if (itemInSlot == null) {
                    SpawnNewItem(item, slot);
                    return true;
                }
            }
        }

        /*// Check if any slot has the same item with count lower than max
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
        }*/

        return false;
    }

    private int GetIndexSlot(InventorySlot slot) {
        for (int i = 0; i < inventorySlots.Length; i++) {
            InventorySlot inventorySlot = inventorySlots[i];
            if (inventorySlot != null && inventorySlot == slot) {
                return i;
            }
        }

        return -1;
    }

    public InventoryItem SpawnNewItem(ItemOld item, InventorySlot slot, ObjectGrabbable objectGrabbable) {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem newInvetoryItem = newItemGo.GetComponent<InventoryItem>();
        newInvetoryItem.InitializeItem(item);
        newInvetoryItem.grabbableObject = objectGrabbable;

        return newInvetoryItem;
    }


    // will be removed
    public void SpawnNewItem(ItemOld item, InventorySlot slot) {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem newInvetoryItem = newItemGo.GetComponent<InventoryItem>();
        newInvetoryItem.InitializeItem(item);
    }

    public ItemOld GetSelectedItem() {
        InventorySlot slot = inventorySlots[selectedSlotIndex];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        if (itemInSlot != null) {
            ItemOld item = itemInSlot.item;
            return item;
        }

        return null;
    }

    public ItemOld UseSelectedItem() {
        InventorySlot slot = inventorySlots[selectedSlotIndex];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null) {
            itemInSlot.count--;
            if (itemInSlot.count <= 0) {
                Destroy(itemInSlot.gameObject);
            } else {
                itemInSlot.RefreshCount();
            }
            return itemInSlot.item;
        }

        return null;
    }

    public bool UseItem(ItemOld item) {
        InventoryItem itemInSlot = FindSameItemInInventory(item);
        if (itemInSlot != null) {
            itemInSlot.count--;
            if (itemInSlot.count <= 0) {
                Destroy(itemInSlot.gameObject);
            } else {
                itemInSlot.RefreshCount();
            }
            return true;
        }

        return false;
    }

    // not in usage
    public void DropSelectedItem() {
        // Current selected slot
        InventorySlot selectedSlot = inventorySlots[selectedSlotIndex];
        InventoryItem selectedItemInSlot = selectedSlot.GetComponentInChildren<InventoryItem>();
        Destroy(selectedItemInSlot.gameObject);
    }

    private InventoryItem FindSameItemInInventory(ItemOld item) {
        for (int i = 0; i < inventorySlots.Length; i++) {
            InventoryItem itemInSlot = inventorySlots[i].GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item) {
                return itemInSlot;
            }
        }
        return null;
    }

    private InventoryItem FindFirstAvailableSlotForExistItem(ItemOld item) {
        // if stackable find first slot
        for (int i = 0; i < inventorySlots.Length; i++) {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && item.stackable && itemInSlot.count < item.maximumHold) {
                return itemInSlot;
            }
        }
        return null;
    }
}
