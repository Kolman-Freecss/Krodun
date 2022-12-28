using Ragnarok;
using Unity.Netcode;
using UnityEngine;

/**********************************************************
 * 
 * In Game Item:
 * Place this on all items that you want to be able to pickup and add to an inventory
 * 
 * Put the name of the item and make sure that the spelling matches that item's name on your inventory list
 * 
 *********************************************************/


public class SceneItem : NetworkBehaviour
{
    [SerializeField]
    private string itemName;  //CASE SENSITIVE - write in the name of the item that matches the name in the InventoryList so we find the right item.
    private InventoryItemList database;
    private bool hasRun;
    private GameObject inventoryFullText;

    // private CollectibleItemSet collectibleItemSet;
    // private UniqueID uniqueID;
    private void Awake()
    {
        inventoryFullText = GameObject.Find("InventoryFull");       //find the game object and make a local reference to it
        //  database = FindObjectOfType<InventoryItemList>();
    }

    void Start()
    {
        //find our InventoryItemList so we can pick our item from the list
    //    database = FindObjectOfType<InventoryItemList>();
     //   Debug.Log(database);


        //    uniqueID = GetComponent<UniqueID>();

        //     collectibleItemSet = FindObjectOfType<CollectibleItemSet>();
        //if (collectibleItemSet.CollectedItems.Contains(uniqueID.ID))
        //{
        //    Destroy(this.gameObject);
        //    return;
        //}

    }

    private void OnTriggerEnter(Collider other)
    {
        /*if (!NetworkManager.Singleton.IsServer) return;*/
        if (other.CompareTag("Player"))
        {
            //        collectibleItemSet.CollectedItems.Add(uniqueID.ID);
            if (!hasRun)
            {
                //add item to the Inventory we want, the players
                //if (other.GetComponent<Inventory>().characterItems.Count < other.GetComponent<Inventory>().inventoryDisplay.numberOfSlots)
                //{
                    other.GetComponent<Inventory>().AddItem(itemName);
                    DestroyItemServerRpc();
                //}
                //else if (other.GetComponent<Inventory>().characterItems.Count == other.GetComponent<Inventory>().inventoryDisplay.numberOfSlots)

                //{
                //    inventoryFullText.SetActive(true);
                //}

                hasRun = true;

                //other.GetComponent<Inventory>().AddItem(itemName);
                

            }

        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void DestroyItemServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        Destroy(gameObject);
        if (gameObject.GetComponent<NetworkObject>())
        {
            gameObject.GetComponent<NetworkObject>().Despawn();
        } else
        {
            gameObject.GetComponentsInChildren<NetworkObject>()[0].Despawn();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            if (networkObject.IsSpawned)
                networkObject.Despawn();
        } else if (gameObject.GetComponentsInChildren<NetworkObject>().Length > 0)
        {
            networkObject = gameObject.GetComponentsInChildren<NetworkObject>()[0];
            if (networkObject.IsSpawned)
                networkObject.Despawn();
        }
    }
}
