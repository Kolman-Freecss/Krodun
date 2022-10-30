using System;
using System.Collections;
using System.Collections.Generic;
using Kolman_Freecss.QuestSystem;
using StarterAssets;
using UnityEngine;
using HighlightObject = StarterAssets.Utils.HighlightObject;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor Settings")] public Texture2D defaultCursor;
    public Texture2D questNotStartedCursor;
    public Texture2D questStartedCursor;
    public Texture2D questCompletedCursor;

    [Header("Canvas Settings")] public GameObject questStartedCanvas;
    public GameObject questNotStartedCanvas;
    public GameObject questCompletedCanvas;

    Camera _currentCamera;
    GameObject _previousObject;
    StarterAssetsInputs _inputs;

    private void Awake()
    {
        questStartedCanvas.SetActive(false);
        questNotStartedCanvas.SetActive(false);
        questCompletedCanvas.SetActive(false);
        _inputs = GetComponent<StarterAssetsInputs>();
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
            SetCursor(info);
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

    private void SetCursor(RaycastHit info)
    {
        GameObject target = info.collider.gameObject;
        QuestGiver qg = target.GetComponent<QuestGiver>();
        if (qg && qg.CurrentQuest != null)
        {
            if (qg.CurrentQuest.IsStarted())
            {
                Cursor.SetCursor(questStartedCursor, Vector2.zero, CursorMode.Auto);
                if (_inputs.click && isInsideAreaDistance(target))
                {
                    questStartedCanvas.SetActive(true);
                }
            }
            else if (qg.CurrentQuest.IsCompleted())
            {
                Cursor.SetCursor(questCompletedCursor, Vector2.zero, CursorMode.Auto);
                if (_inputs.click && isInsideAreaDistance(target))
                {
                    questCompletedCanvas.SetActive(true);
                }
            }
            else if (qg.CurrentQuest.IsNotStarted())
            {
                Cursor.SetCursor(questNotStartedCursor, Vector2.zero, CursorMode.Auto);
                if (_inputs.click && isInsideAreaDistance(target))
                {
                    questNotStartedCanvas.SetActive(true);
                }
            }
            else
            {
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            }
        }
        else
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        }
    }
    
    /**
     * @return bool true if the gameobject is inside the parameter gameobject area
     */
    bool isInsideAreaDistance(GameObject go)
    {
        return Vector3.Distance(go.transform.position, transform.position) < 5;
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