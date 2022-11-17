using System.Collections.Generic;
using UnityEngine;

// Database for all the items in the game
[CreateAssetMenu(fileName = "ItemList", menuName = "New Item List", order = 1)]
public class InventoryItemList : ScriptableObject
{
    public List<InventoryItem> itemList;

    public InventoryItem GetItem(string itemName)
    {
        return itemList.Find(x => x.itemName == itemName);
    }

    public InventoryItem GetItem(int itemIndex)
    {
        return itemList[itemIndex];
    }
}

