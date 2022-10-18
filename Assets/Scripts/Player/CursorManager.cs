using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets.Utils;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor Settings")]
    public Texture2D defaultCursor;
    public Texture2D questCursor;
    
    Camera _currentCamera;
    GameObject _previousObject;

    private void Awake()
    {
        ResetCursor();
    }

    void Update()
    {
        if (!_currentCamera)
        {
            _currentCamera = Camera.main;
            return;
        }
        MousePosition();
    }

    void MousePosition()
    {
        RaycastHit info;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("NPC")))
        {
            HighlightGameObject(info);
            SetCursor();
        }
        else
        {
            UnHighlightGameObject();
            ResetCursor();
        }
    }

    private void ResetCursor()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    private void SetCursor()
    {
        Cursor.SetCursor(questCursor, Vector2.zero, CursorMode.Auto);
    }

    /**
     * Highlights the game object that the mouse is hovering over
     */
    void HighlightGameObject(RaycastHit info)
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
            HighlightObject infoHighlightObject = info.collider.gameObject.GetComponent<HighlightObject>();
            if (infoHighlightObject)
            {
                infoHighlightObject.Highlight();
            }
        }
    }
    
    void UnHighlightGameObject()
    {
        if (_previousObject)
        {
            _previousObject.GetComponent<HighlightObject>().UnHighlight();
        }
        _previousObject = null;
    }
}
