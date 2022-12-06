using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class IPUIMediator : MonoBehaviour
    {
        public void OnExitMenu()
        {
            gameObject.SetActive(false);
        }
    }
}