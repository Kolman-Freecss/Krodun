using System.Collections;
using System.Collections.Generic;
using StarterAssets.Utils;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    
    public Texture2D cursorTexture; 
    Camera currentCamera;
       
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }
        HighlightGameObject();
    }
    
    void HighlightGameObject()
    {
        //highlight the object
        RaycastHit info;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("NPC")))
        {
            if (info.collider.gameObject.GetComponent<HighlightObject>())
            {
                info.collider.gameObject.GetComponent<HighlightObject>().Highlight();
            }
        }
    }
}
