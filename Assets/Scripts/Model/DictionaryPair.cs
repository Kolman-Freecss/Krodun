using System;
using Unity.Netcode;

namespace Model
{
    [Serializable]
    public class DictionaryPair
    {
        public int key;
        public NetworkObject value;
    }
}