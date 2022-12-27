using System;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using Kolman_Freecss.QuestSystem;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class CursorManager : MonoBehaviour
    {
        [Header("Cursor Settings")] 
        public Texture2D defaultCursor;
        public Texture2D questNotStartedCursor;
        public Texture2D questStartedCursor;
        public Texture2D questCompletedCursor;
        
        [HideInInspector]
        public static CursorManager Instance { get; private set; }

        // Canvas references
        private GameObject _questStartedCanvas;
        private GameObject _questNotStartedCanvas;
        private GameObject _questCompletedCanvas;

        private Camera _currentCamera;
        private GameObject _previousObject;
        private KrodunController _krodunController;

        private void Start()
        {
            Instance = this;
            DontDestroyOnLoad(this);
            ResetCursor();
            SubscribeToDelegatesAndUpdateValues();
        }

        private void SubscribeToDelegatesAndUpdateValues()
        {
            GameManager.Instance.OnSceneLoadedChanged += OnGameStarted;
        }

        public void OnGameStarted(bool isLoaded, ulong clientId)
        {
            if (isLoaded)
            {
                // Get the Object KrodunController that have the clientId
                _krodunController = FindObjectsOfType<KrodunController>().FirstOrDefault(k => k.OwnerClientId == clientId);
                _questNotStartedCanvas = GameObject.Find("QuestNotStartedCanvas");
                _questStartedCanvas = GameObject.Find("QuestStartedCanvas");
                _questCompletedCanvas = GameObject.Find("QuestCompletedCanvas");
                _questStartedCanvas.SetActive(false);
                _questNotStartedCanvas.SetActive(false);
                _questCompletedCanvas.SetActive(false);
            }
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
                ActiveQuestionMarkByStatus(qg.CurrentQuest.Status, target);
            }
            else
            {
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            }
        }

        private void ActiveQuestionMarkByStatus(QuestStatus qs, GameObject target)
        {
            switch (qs)
            {
                case QuestStatus.NotStarted:
                    Cursor.SetCursor(questNotStartedCursor, Vector2.zero, CursorMode.Auto);
                    OnClickWhenHover(target, _questNotStartedCanvas);
                    break;
                case QuestStatus.Started:
                    Cursor.SetCursor(questStartedCursor, Vector2.zero, CursorMode.Auto);
                    OnClickWhenHover(target, _questStartedCanvas);
                    break;
                case QuestStatus.Completed:
                    Cursor.SetCursor(questCompletedCursor, Vector2.zero, CursorMode.Auto);
                    OnClickWhenHover(target, _questCompletedCanvas);
                    break;
                default:
                    Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                    break;
            }
        }

        private void OnClickWhenHover(GameObject target, GameObject canvas)
        {
            if (_krodunController.Input.leftClick && isInsideAreaDistance(target))
            {
                target.GetComponent<QuestGiver>().FaceTarget();
                canvas.SetActive(true);
            }
        }

        /**
     * @return bool true if the gameobject is inside the parameter gameobject area
     */
        bool isInsideAreaDistance(GameObject go)
        {
            return Vector3.Distance(go.transform.position, _krodunController.transform.position) < 5;
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
}