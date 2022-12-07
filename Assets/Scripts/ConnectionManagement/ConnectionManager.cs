using System;
using Unity.Netcode;
using UnityEngine;

namespace Kolman_Freecss.Krodun.ConnectionManagement
{
    public class ConnectionManager : MonoBehaviour
    {
        
        public void StartHost(string playerName, string ipAddress, int port)
        {
            NetworkManager.Singleton.ip = ipAddress;
            NetworkManager.singleton.networkPort = port;
            NetworkManager.singleton.StartHost();

            try
            {
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }
}