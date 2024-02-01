using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour {
    [Header("UI")]
    public GameObject inventory;
    private List<Slot> allInventorySlots = new List<Slot>();
    public List<Slot> inventorySlots = new List<Slot>();
    public List<Slot> hotbarSlots = new List<Slot>();
    public Image crosshair;
    public TMP_Text itemHoverText;

    [Header("Raycast")]
    public float raycastDistance = 5f;
    public LayerMask itemLayer;
    public Transform dropLocation; // The location items will be dropped from.

    [Header("Drag and Drop")]
    public Image dragIconImage;
    private Item currentDraggedItem;
    private int currentDragSlotIndex = -1;

    [Header("Equippable Items")]
    public List<GameObject> equippableItems = new List<GameObject>();
    public Transform selectedItemImage;
    private int curHotbarIndex = -1;

    public void Start() {
        toggleInventory(false);

        allInventorySlots.AddRange(hotbarSlots);
        allInventorySlots.AddRange(inventorySlots);

        foreach (Slot uiSlot in allInventorySlots) {
            uiSlot.initialiseSlot();
        }
    }

    public void Update() {
        itemRaycast(Input.GetKeyDown(KeyCode.E));

        if (Input.GetKeyDown(KeyCode.Tab))
            toggleInventory(!inventory.activeInHierarchy);

        if (inventory.activeInHierarchy && Input.GetMouseButtonDown(0)) {
            dragInventoryIcon();
        } else if (currentDragSlotIndex != -1 && Input.GetMouseButtonUp(0) || currentDragSlotIndex != -1 && !inventory.activeInHierarchy) // If we are hovered over a slot and release, if we are dragging an item and close the inventory
          {
            dropInventoryIcon();
        }

        if (Input.GetKeyDown(KeyCode.G)) // The button we need to press to drop items from the inventory
            dropItem();

        for (int i = 1; i < hotbarSlots.Count + 1; i++) {
            if (Input.GetKeyDown(i.ToString())) {
                enableHotbarItem(i - 1);
                curHotbarIndex = i - 1;
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            attempToUseItem();
        }

        dragIconImage.transform.position = Input.mousePosition;
    }

    private void itemRaycast(bool hasClicked = false) {
        itemHoverText.text = "";
        Ray ray = Camera.main.ScreenPointToRay(crosshair.transform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, itemLayer)) {
            if (hit.collider != null) {
                if (hasClicked) // Pick up
                {
                    Item newItem = hit.collider.GetComponent<Item>();
                    if (newItem) {
                        addItemToInventory(newItem);
                    }
                } else // Get the name
                  {
                    Item newItem = hit.collider.GetComponent<Item>();

                    if (newItem) {
                        itemHoverText.text = newItem.name;
                    }
                }
            }
        }
    }

    private void addItemToInventory(Item itemToAdd) {
        int leftoverQuantity = itemToAdd.currentQuantity;
        Slot openSlot = null;
        for (int i = 0; i < allInventorySlots.Count; i++) {
            Item heldItem = allInventorySlots[i].getItem();

            if (heldItem != null && itemToAdd.name == heldItem.name) {
                int freeSpaceInSlot = heldItem.maxQuantity - heldItem.currentQuantity;

                if (freeSpaceInSlot >= leftoverQuantity) {
                    heldItem.currentQuantity += leftoverQuantity;
                    Destroy(itemToAdd.gameObject);
                    allInventorySlots[i].updateData();
                    return;
                } else // Add as much as we can to the current slot
                  {
                    heldItem.currentQuantity = heldItem.maxQuantity;
                    leftoverQuantity -= freeSpaceInSlot;
                }
            } else if (heldItem == null) {
                if (!openSlot)
                    openSlot = allInventorySlots[i];
            }

            allInventorySlots[i].updateData();
        }

        if (leftoverQuantity > 0 && openSlot) {
            openSlot.setItem(itemToAdd);
            itemToAdd.currentQuantity = leftoverQuantity;
            itemToAdd.gameObject.SetActive(false);
        } else {
            itemToAdd.currentQuantity = leftoverQuantity;
        }
    }

    private void toggleInventory(bool enable) {
        inventory.SetActive(enable);

        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enable;

        if (!enable) {
            foreach (Slot curSlot in allInventorySlots) {
                curSlot.hovered = false;
            }
        }

        // Disable the rotation of the camera.
        // Camera.main.GetComponent<FirstPersonLook>().sensitivity = enable ? 0 : 2;
    }

    private void dropItem() {
        for (int i = 0; i < allInventorySlots.Count; i++) {
            Slot curSlot = allInventorySlots[i];
            if (curSlot.hovered && curSlot.hasItem()) {
                // drop all items
                /*curSlot.getItem().gameObject.SetActive(true);
                curSlot.getItem().transform.position = dropLocation.position;
                curSlot.setItem(null);
                break;*/
                Item droppedItem = Instantiate(curSlot.getItem().gameObject, dropLocation.position, Quaternion.identity).GetComponent<Item>();
                droppedItem.gameObject.SetActive(true);
                droppedItem.currentQuantity = 1;
                curSlot.getItem().currentQuantity -= 1;

                curSlot.updateData();
                if (curSlot.getItem().currentQuantity == 0) {
                    curSlot.setItem(null);
                }

                break;
            }
        }
    }

    private void dragInventoryIcon() {
        for (int i = 0; i < allInventorySlots.Count; i++) {
            Slot curSlot = allInventorySlots[i];
            if (curSlot.hovered && curSlot.hasItem()) {
                currentDragSlotIndex = i; // Update the current drag slot index variable.

                currentDraggedItem = curSlot.getItem(); // Get the item from the current slot
                dragIconImage.sprite = currentDraggedItem.icon;
                dragIconImage.color = new Color(1, 1, 1, 1); // Make the follow mouse icon opaque (visible).

                curSlot.setItem(null); // Remove the item from the slot we just picked up the item from.
            }
        }
    }

    private void dropInventoryIcon() {
        // Reset our drag item variables
        dragIconImage.sprite = null;
        dragIconImage.color = new Color(1, 1, 1, 0); // Make invisible.

        for (int i = 0; i < allInventorySlots.Count; i++) {
            Slot curSlot = allInventorySlots[i];
            if (curSlot.hovered) {
                if (curSlot.hasItem()) // Swap the items.
                {
                    Item itemToSwap = curSlot.getItem();

                    if (itemToSwap.name == currentDraggedItem.name & itemToSwap.currentQuantity != itemToSwap.maxQuantity) {
                        int leftoverQuantity = currentDraggedItem.currentQuantity;
                        int freSpaceInSlot = itemToSwap.maxQuantity - itemToSwap.currentQuantity;

                        if (freSpaceInSlot >= leftoverQuantity) {
                            itemToSwap.currentQuantity += leftoverQuantity;

                            Destroy(currentDraggedItem);
                            allInventorySlots[currentDragSlotIndex].setItem(null);
                            allInventorySlots[i].updateData();
                            return;
                        } else {
                            itemToSwap.currentQuantity = itemToSwap.maxQuantity;
                            allInventorySlots[i].updateData();
                            currentDraggedItem.currentQuantity = currentDraggedItem.currentQuantity -= freSpaceInSlot;
                            break;
                        }
                    } else {
                        curSlot.setItem(currentDraggedItem);

                        allInventorySlots[currentDragSlotIndex].setItem(itemToSwap);

                        resetDragVariables();
                        return;
                    }
                } else // Place the item with no swap.
                  {
                    curSlot.setItem(currentDraggedItem);
                    resetDragVariables();
                    return;
                }
            }
        }

        // If we get to this point we dropped the item in an invalid location (or closed the inventory).
        allInventorySlots[currentDragSlotIndex].setItem(currentDraggedItem);
        resetDragVariables();
    }

    private void resetDragVariables() {
        currentDraggedItem = null;
        currentDragSlotIndex = -1;
    }

    private void enableHotbarItem(int hotbarIndex) {
        foreach (GameObject a in equippableItems) {
            a.SetActive(false);
        }

        Slot hotbarSlot = hotbarSlots[hotbarIndex];
        selectedItemImage.transform.position = hotbarSlots[hotbarIndex].transform.position;

        if (hotbarSlot.hasItem()) {
            if (hotbarSlot.getItem().equippableItemIndex != -1) {
                equippableItems[hotbarSlot.getItem().equippableItemIndex].SetActive(true);
            }
        }
    }

    private void attempToUseItem() {
        if (curHotbarIndex == -1)
            return;
        Item curItem = hotbarSlots[curHotbarIndex].getItem();
        if (curItem) {
            curItem.UseItem();
            if (curItem.currentQuantity != 0) {
                hotbarSlots[curHotbarIndex].updateData();
            } else {
                hotbarSlots[curHotbarIndex].setItem(null);
            }

            enableHotbarItem(curHotbarIndex);
        }
    }
}
