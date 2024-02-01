using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour {
    public ItemOld scriptableItem;
    private Rigidbody rb;
    private Transform objectGrabPointTransform;
    private Transform initalParent;
    [HideInInspector] public bool isGrabbing = false;
    private int amount = 0;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        initalParent = transform.parent;

        if (transform.parent != null && transform.parent == objectGrabPointTransform) {
            SetPropOnHand();
        }
    }

    public void Grab(Transform objectGrabPointTransform) {
        this.objectGrabPointTransform = objectGrabPointTransform;
        SetPropOnHand();
        transform.SetParent(objectGrabPointTransform);
        transform.SetLocalPositionAndRotation(Vector3.zero, transform.rotation);
        if (scriptableItem.maximumHold > (amount + 1)) amount++;

/*        if (transform.parent != objectGrabPointTransform) {

        } else if (transform.parent == objectGrabPointTransform && !gameObject.activeSelf) {
            gameObject.SetActive(true);
        }*/
    }

    public void BringItToPlayer(Transform objectGrabPointTransform) {
        Grab(objectGrabPointTransform);
        transform.gameObject.SetActive(false);
    }

    public void Drop() {
        transform.SetParent(initalParent);
        if (amount - 1 < 0) amount--;
        SetPropOnGround();
    }

    private void SetPropOnHand() {
        isGrabbing = true;
        rb.isKinematic = true;

        if (TryGetComponent<Collider>(out Collider collider)) {
            collider.enabled = false;
        }
    }

    private void SetPropOnGround() {
        isGrabbing = false;
        rb.isKinematic = false;

        if (TryGetComponent<Collider>(out Collider collider)) {
            collider.enabled = true;
        }
    }

/*    private void OnSelectedItemChanged(object sender, InventoryManager.OnSelectedItemChangedEventArgs e) {
        Debug.Log("CHANGEDDD" + e.selectedItem);

    }*/
}
