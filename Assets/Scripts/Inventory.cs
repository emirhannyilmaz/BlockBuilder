using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
    World world;

    public InventorySlot[] inventorySlots;
    public Toolbar toolbar;

    private void Start() {
        world = GameObject.Find("World").GetComponent<World>();

        foreach(InventorySlot slot in inventorySlots) {
            slot.icon.sprite = world.blockTypes[slot.itemID].icon;
            slot.icon.enabled = true;
        }
    }

    public void ChangeItem(int index) {
        toolbar.itemSlots[toolbar.currentItemSlot].itemID = inventorySlots[index].itemID;
        toolbar.Redraw();
    }
}

[System.Serializable]
public class InventorySlot {
    public byte itemID;
    public Image icon;
}
