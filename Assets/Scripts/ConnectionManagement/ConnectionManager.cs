using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Kolman_Freecss.Krodun.ConnectionManagement
{
    public class ConnectionManager : MonoBehaviour
    {
        
        public void StartHost(string playerName, string ipAddress, int port)
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipAddress, (ushort) port);
            NetworkManager.Singleton.StartHost();
        }
        
        public void StartClient(string playerName, string ipAddress, int port)
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipAddress, (ushort) port);
            NetworkManager.Singleton.StartClient();
        }
        
    }
}