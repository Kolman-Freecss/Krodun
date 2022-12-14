using Kolman_Freecss.Krodun;
using Ragnarok;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image spriteImage; //our image inside the slot
    public InventoryItem item; //our item inside the slot
    public InventorySlot selectedItem; //reference for our game object "SelectedItem" - so we can change it -
    private TextMeshProUGUI itemNameText; //ref to our slot text
    public bool dropScreen;
    public GameObject dropSpawner;

    [HideInInspector] public bool vendor = false;
    [HideInInspector] public bool treasureChest = false;
    [HideInInspector] public bool inventory = false;
    [HideInInspector] public Inventory player;
    [HideInInspector] public Inventory tChest;

    private void Awake()
    {
        GameManager.Instance.OnSceneLoadedChanged += OnGameStarted;
    }
    
    private void OnGameStarted(bool isLoaded)
    {
        Debug.Log("Inventory OnGameStarted");
        if (isLoaded)
        {
            GameObject gObject = GameObject.Find("TreasureChest");
            if (gObject)
                tChest = gObject.GetComponent<Inventory>();
        
            gObject = GameObject.Find("Player");
            if (gObject)
                player = gObject.GetComponent<Inventory>();
            else
            {
                gObject = GameObject.FindWithTag("Player");
                if (gObject)
                    player = gObject.GetComponent<Inventory>();
            }

            selectedItem =
                GameObject.Find("SelectedItem")
                    .GetComponent<
                        InventorySlot>(); //find the game object named selectedItem and make a local reference to it
            dropSpawner = GameObject.Find("DropSpawner");
            spriteImage = GetComponent<Image>(); //setup a reference for our local image component 
            Setup(null); //Lets setup the slot to be empty (null) by running the setup
            itemNameText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    /**
     * Run this when an inventory slot needs to be updated
     */
    public void Setup(InventoryItem item)
    {
        this.item = item; //this slot will now hold the constructor(InventoryItem item) as its new item

        if (this.item != null) //Lets update the slot with our new item's info:
        {
            spriteImage.color =
                Color.white; //get the image set up, white will make sure the item looks like its image(i.e. if it were black you couldnt see the image)
            spriteImage.sprite = this.item.itemIcon; //get the image of our new item and set it in the slot

            if (itemNameText != null) //this will be null if we are the selectedItem 
            {
                itemNameText.text = item.itemName; //put the item's name in the slot
            }
        }
        else //Setup is being ran for the first time on this inventory slot or something pushed Setup(null) through here - which we would want to do when we empty a slot out    
        {
            spriteImage.color =
                Color.clear; //This inventory Slot is empty, lets make it clear(alpha set to zero) so we dont see anything.
            if (itemNameText != null)
            {
                itemNameText.text = null;
            }
        }
    }

    /**
     * This inventory slot has been clicked on
     */
    public void OnPointerClick(PointerEventData eventData)
    {
        //!!WARNING!!Do not have selectedItem highlighted in the Hierarchy window or it will not update!!WARNING!!
        if (eventData.button == PointerEventData.InputButton.Right) // button was right clicked
        {
            if (vendor && this.item != null) //-----------------VENDOR SLOT ------------------------//
            {
                player.GiveItem(this.item.itemName); //add item to player's inventory   
                Debug.Log("gave to player");
                if (this.item.isUnique)
                {
                    Setup(null); //if it is, then remove it from vendor's inventory
                    InventoryEvents.OnScrollInfoDeactivated(); //remove the mouse over info
                }
            }

            if (tChest && tChest.GetComponent<Inventory>().inventoryDisplay.isActiveAndEnabled) //Is the display active?
            {
                if (this.item != null) // is there an item in the slot?
                {
                    if (inventory) //im a player inventory slot:
                    {
                        tChest.GiveItem(this.item.itemName); //add item to player's treasure chest                  
                        Debug.Log("gave to chest");
                        Setup(null); //remove from inventory
                        InventoryEvents.OnScrollInfoDeactivated(); //remove the mouse over info
                    }

                    if (treasureChest) //I am a treasure chest slot:
                    {
                        player.GiveItem(this.item.itemName); //add item to player's inventory
                        Debug.Log("gave to player");
                        Setup(null); //remove from treasure chest
                        InventoryEvents.OnScrollInfoDeactivated(); //remove the mouse over info
                    }
                }
            }

            Debug.Log("Right Mouse Button Clicked on: " + name);
            //currently right clicking turns it off... could be a good place to trade item to another inventory display
            //    InventoryEvents.OnClickDeactivated();
        }

        if (eventData.button == PointerEventData.InputButton.Left) // User left clicked on this slot
        {
            if (dropScreen)
            {
                if (selectedItem.item != null)
                {
                    Vector3 pos = dropSpawner.transform.position;
                    Quaternion rot = dropSpawner.transform.rotation;
                    GameObject item = Instantiate(selectedItem.item.itemObject.gameObject, pos, rot);
                    if (item.GetComponent<NetworkObject>())
                    {
                        item.GetComponent<NetworkObject>().Spawn();
                    } else
                    {
                        item.GetComponentsInChildren<NetworkObject>()[0].Spawn();
                    }
                    //currently does not remove from inventory...
                    player.GetComponent<Inventory>().RemoveItem(selectedItem.item.itemName);
                    selectedItem.Setup(null);
                    Debug.Log("we've  thrown out the item");
                }

                return;
            }
            else
            {
                if (vendor)
                {
                    //-------check player can afford item here----------------------
                    return; //for now we just dont allow the action1 to do anything
                }

                if (this.item != null) //---This slot has an item loaded into it already ---
                {
                    if (selectedItem.item !=
                        null) //put item in here and take out item (swap) if there is one in the slot already
                    {
                        InventoryItem clone = new InventoryItem(selectedItem.item);
                        selectedItem.Setup(this.item);
                        Setup(clone);
                    }
                    else //take the item and leave the slot empty
                    {
                        selectedItem.Setup(this.item);
                        Setup(null);
                    }
                }
                else if (selectedItem.item != null) //we have a selected item and we clicked into an empty slot
                {
                    Setup(selectedItem.item); //put the selected items info into this slot
                    selectedItem.Setup(null); //remove the selected item from our selection
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!dropScreen && item != null && item.itemDescription != null)
        {
            InventoryEvents.OnScrollInfoActivated(item.itemDescription);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!dropScreen)
        {
            InventoryEvents.OnScrollInfoDeactivated();
        }
    }
}