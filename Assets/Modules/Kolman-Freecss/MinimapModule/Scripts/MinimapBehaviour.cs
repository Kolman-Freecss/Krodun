using System;
using System.Linq;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class MinimapBehaviour : MonoBehaviour
    {
        public Transform targetToFollow;
        public bool rotateWithTarget = true;
        private ulong clientIdTarget;

        private void Awake()
        {
            GameManager.Instance.OnSceneLoadedChanged += OnGameStarted;
        }
        
        private void OnGameStarted(bool isLoaded, ulong clientId)
        {
            if (isLoaded)
            {
                if (targetToFollow == null)
                {
                    this.clientIdTarget = clientId;
                    GetPlayerToFollow();
                }
            }
        }

        public bool GetPlayerToFollow()
        {
            try
            {
                targetToFollow = GameObject.FindObjectsOfType<KrodunController>().First(p => p.clientId == clientIdTarget).transform.Find("MinimapSymbol");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                targetToFollow = null;
            }

            return targetToFollow != null;
        }
    }
}