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
        
        private void Awake()
        {
            _joinUI = FindObjectOfType<IPJoinUI>();
            _hostUI = FindObjectOfType<IPHostUI>();
        }

        private void Start()
        {
            // Show create IP as default
            ToggleCreateIP();
        }

        public void ToggleJoinIPUI()
        {
            AudioSource.PlayClipAtPoint(SoundManager.Instance.ButtonClickSound, Camera.main.transform.position);
            _joinUI.Show();
            _hostUI.Hide();
        }

        private void ToggleCreateIP()
        {
            _hostUI.Show();
            _joinUI.Hide();
        }
        
        public void ToggleCreateIPUI()
        {
            AudioSource.PlayClipAtPoint(SoundManager.Instance.ButtonClickSound, Camera.main.transform.position);
            ToggleCreateIP();
        }
        
        public void JoinWithIp(string ip, string port)
        {
            Int32.TryParse(port, out var portInt);
            if (portInt <= 0)
            {
                portInt = DefaultPort;
            }
            
            ip = string.IsNullOrEmpty(ip) ? DefaultIp : ip;
            
            ConnectionManager.Instance.StartClient(playerName, ip, portInt);
            
        }

        public void HostIpRequest(string ip, string port)
        {
            Int32.TryParse(port, out var portInt);
            if (portInt <= 0)
            {
                portInt = DefaultPort;
            }

            ip = string.IsNullOrEmpty(ip) ? DefaultIp : ip;
            
            ConnectionManager.Instance.StartHost(playerName, ip, portInt);
            
        }

        public void OnExitMenu()
        {
            AudioSource.PlayClipAtPoint(SoundManager.Instance.ButtonClickSound, Camera.main.transform.position);
            gameObject.SetActive(false);
        }
    }
}