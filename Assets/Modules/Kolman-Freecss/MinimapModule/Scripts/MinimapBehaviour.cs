using System;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class MinimapBehaviour : MonoBehaviour
    {
        public Transform targetToFollow;
        public bool rotateWithTarget = true;

        private void Awake()
        {
            GameManager.Instance.OnSceneLoadedChanged += OnGameStarted;
        }
        
        private void OnGameStarted(bool isLoaded, ulong clientId)
        {
            Debug.Log("Minimap OnGameStarted");
            if (isLoaded)
            {
                if (targetToFollow == null)
                {
                    targetToFollow = GameObject.FindGameObjectsWithTag("Player")[0].transform.Find("MinimapSymbol");
                }
            }
        }
    }
}