using System.Collections;
using System.Collections.Generic;
using StarterAssets.Utils;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    
    public Texture2D cursorTexture; 
    Camera _currentCamera;
    GameObject _previousObject;

    void Update()
    {
        if (!_currentCamera)
        {
            _currentCamera = Camera.main;
            return;
        }
        HighlightGameObject();
    }
    
    /**
     * Highlights the game object that the mouse is hovering over
     */
    void HighlightGameObject()
    {
        RaycastHit info;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("NPC")))
        {
            if (_previousObject != info.collider.gameObject)
            {
                if (_previousObject)
                {
                    _previousObject.GetComponent<HighlightObject>().UnHighlight();
                }
                _previousObject = info.collider.gameObject;
            }
            else
            {
                if (info.collider.gameObject.GetComponent<HighlightObject>())
                {
                    info.collider.gameObject.GetComponent<HighlightObject>().Highlight();
                }
            }
            //Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            if (_previousObject)
            {
                _previousObject.GetComponent<HighlightObject>().UnHighlight();
            }
            _previousObject = null;
            //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
