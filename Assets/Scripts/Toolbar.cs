using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour {
    World world;
    public Player player;

    public RectTransform highlight;
    public ItemSlot[] itemSlots;

    private void Start() {
        world = GameObject.Find("World").GetComponent<World>();

        foreach(ItemSlot slot in itemSlots) {
            slot.icon.sprite = world.blockTypes[slot.itemID].icon;
            slot.icon.enabled = true;
        }

        player.selectedBlockIndex = itemSlots[0].itemID;
    }

    public void Select(int index) {
        highlight.position = itemSlots[index].icon.transform.position;
        player.selectedBlockIndex = itemSlots[index].itemID;
    }
}

[System.Serializable]
public class ItemSlot {
    public byte itemID;
    public Image icon;
}
