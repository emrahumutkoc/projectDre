using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvontoryManager : MonoBehaviour {
    [SerializeField] GameObject mainInvontoryGroup;


    private bool isInventoryActive = false;

    private void Start() {
        mainInvontoryGroup.SetActive(isInventoryActive);
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
