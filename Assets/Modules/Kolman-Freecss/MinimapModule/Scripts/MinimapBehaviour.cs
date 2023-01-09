using System;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class MinimapBehaviour : MonoBehaviour
    {
        public Transform targetToFollow;
        public bool rotateWithTarget = true;

        private void Start()
        {
            if (targetToFollow == null)
            {
                targetToFollow = GameObject.FindGameObjectsWithTag("Player")[0].transform.Find("MinimapSymbol");
            }
        }

        void Update()
        {
            if (targetToFollow == null)
            {
                targetToFollow = GameObject.FindGameObjectsWithTag("Player")[0].transform.Find("MinimapSymbol");
            }
        }
    }
}