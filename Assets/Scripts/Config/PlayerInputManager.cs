using System;
using Kolman_Freecss.Krodun.ConnectionManagement;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Kolman_Freecss.Krodun
{
    public class PlayerInputManager : UnityEngine.InputSystem.PlayerInputManager
    {
        public static PlayerInputManager Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
}