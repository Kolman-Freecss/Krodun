using System;
using Kolman_Freecss.Krodun.ConnectionManagement;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class IPUIMediator : MonoBehaviour
    {
        
        public const string DefaultIp = "127.0.0.1";
        public string Ip => DefaultIp;
        
        public const int DefaultPort = 9998;
        public int Port => DefaultPort;
        public string playerName = "Player";
        
        IPJoinUI _joinUI;
        IPHostUI _hostUI;
        ConnectionManager _connectionManagement;

        private void Awake()
        {
            _joinUI = FindObjectOfType<IPJoinUI>();
            _hostUI = FindObjectOfType<IPHostUI>();
            _connectionManagement = FindObjectOfType<ConnectionManager>();
        }

        private void Start()
        {
            // Show create IP as default
            ToggleCreateIPUI();
        }

        public void ToggleJoinIPUI()
        {
            _joinUI.Show();
            _hostUI.Hide();
        }
        
        public void ToggleCreateIPUI()
        {
            _hostUI.Show();
            _joinUI.Hide();
        }
        
        public void JoinWithIp(string ip, string port)
        {
            Int32.TryParse(port, out var portInt);
            if (portInt <= 0)
            {
                portInt = DefaultPort;
            }
            
            ip = string.IsNullOrEmpty(ip) ? DefaultIp : ip;
            
            
        }

        public void HostIpRequest(string ip, string port)
        {
            Int32.TryParse(port, out var portInt);
            if (portInt <= 0)
            {
                portInt = DefaultPort;
            }

            ip = string.IsNullOrEmpty(ip) ? DefaultIp : ip;
            
            _connectionManagement.StartHost(playerName, ip, portInt);
            
        }

        public void OnExitMenu()
        {
            gameObject.SetActive(false);
        }
    }
}