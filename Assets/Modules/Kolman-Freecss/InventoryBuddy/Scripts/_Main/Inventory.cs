﻿using System.Collections.Generic;
using Kolman_Freecss.Krodun;
using Kolman_Freecss.QuestSystem;
using Unity.Netcode;
using UnityEngine;

namespace Ragnarok //this creates a namespace for all of the Ragnarok scripts so they dont interfere with yours
{
    /*******************************************************
     * 
     * Inventory:
     * 
     * Use this on your player/vendor...whatever game object will own an inventory of their own
     * 
     * Set the database we want to pull item info from(most likely the one that has all of our items listed(the asset created with InventoryBuddy))
     * 
     * 
     * 
     * 
     *******************************************************/

    public class Inventory : NetworkBehaviour
    {
        public List<InventoryItem> characterItems = new List<InventoryItem>();  //create a new list called items                                                                             
        public InventoryItemList database;                                      //pick the list we want to get info from
        [HideInInspector]
        public InventoryDisplay inventoryDisplay;
        private QuestManager questManager;
        private Canvas _inventoryCanvas;

        private void Awake()
        {
            GameManager.Instance.OnSceneLoadedChanged += OnGameStarted;
        }
        
        private void OnGameStarted(bool isLoaded)
        {
            Debug.Log("Inventory OnGameStarted");
            if (inventoryDisplay == null && isLoaded)
            {
                questManager = FindObjectOfType<QuestManager>();
                inventoryDisplay = FindObjectOfType<InventoryDisplay>();
                _inventoryCanvas = FindObjectOfType<ActivateUI>().GetComponent<Canvas>();
                _inventoryCanvas.enabled = false;
            }
        }

        public void GiveItem(string itemName)
        {
            InventoryItem itemToAdd = database.GetItem(itemName);
            characterItems.Add(itemToAdd);
            inventoryDisplay.AddNewItem(itemToAdd);         //add the item to the inventory display
        }

        public void AddItem(string itemName)
        {
            if (!IsOwner) return;
            
            /*if (IsServer)
            {
                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] {NetworkManager.Singleton.LocalClientId}
                    }
                };*/
                AddItemInventory(itemName);
            /*}*/
        }
        
        public void AddItemInventory(string itemName)
        {
            Debug.Log("Item added to inventory of client -> " + NetworkManager.Singleton.LocalClientId);
            InventoryItem itemToAdd = database.GetItem(itemName);   //get reference to our listed item
            characterItems.Add(itemToAdd);                                   //add reference to our local items list
            inventoryDisplay.AddNewItem(itemToAdd);
            questManager.EventTriggered(EventQuestType.Collect, itemToAdd.amountType);
            //     InventoryEvents.OnItemAddedToInventory(itemToAdd);      //call event using our referenced item, the event will tell the display to show it.
            //   Debug.Log("Item addded: " + itemToAdd.itemName);
        }
        
        /*[ClientRpc]
        public void AddItemClientRpc(string itemName, ulong clientId, ClientRpcParams clientRpcParams = default)
        {
            Debug.Log("Item added to inventory of client -> " + clientId);
            InventoryItem itemToAdd = database.GetItem(itemName);   //get reference to our listed item
            characterItems.Add(itemToAdd);                                   //add reference to our local items list
            inventoryDisplay.AddNewItem(itemToAdd);
            questManager.EventTriggered(EventQuestType.Collect, itemToAdd.amountType);
            //     InventoryEvents.OnItemAddedToInventory(itemToAdd);      //call event using our referenced item, the event will tell the display to show it.
            //   Debug.Log("Item addded: " + itemToAdd.itemName);
        }*/

        /*public void AddItems(List<InventoryItem> items)
        {
            foreach (InventoryItem item in items)
            {
                AddItem(item.itemName);
                questManager.EventTriggered(EventQuestType.Collect, item.amountType);
            }
        }*/

        public InventoryItem CheckThisItem(string itemName)
        {
            return characterItems.Find(InventoryItem => InventoryItem.itemName == itemName);
        }

        public void RemoveItem(string itemName)
        {
            InventoryItem item = CheckThisItem(itemName);
            if (item != null)
            {
                characterItems.Remove(item);
                Debug.Log("Item removed: " + item.itemName);
            }
        }
        //void Save()
        //{
        //    SaveLoad.Save<List<InventoryItem>>(Items, "Inventory");
        //}

        //void Load()
        //{
        //    if (SaveLoad.SaveExists("Inventory"))
        //    {
        //        AddItems(SaveLoad.Load<List<InventoryItem>>("Inventory"));
        //    }
        //}
    }
}

