using System;
using Unity.Collections;
using Unity.Netcode;

namespace Model
{
    public struct Player : INetworkSerializable, System.IEquatable<Player>
    {
        /*public Player(ulong clientId)
        {
            this.Id = clientId;
            this.IsReady = false;
        }

        public Player(ulong clientId, string playerName)
        {
            this.Id = clientId;
            this.Name = playerName != null && playerName.Length > 0 ? playerName : "Player_" + clientId;
            this.IsReady = false;
        }*/

        public ulong Id;
        public FixedString128Bytes Name;
        public bool IsConnected;
        public bool IsReady;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out Id);
                reader.ReadValueSafe(out Name);
                reader.ReadValueSafe(out IsConnected);
                reader.ReadValueSafe(out IsReady);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(Id);
                writer.WriteValueSafe(Name);
                writer.WriteValueSafe(IsConnected);
                writer.WriteValueSafe(IsReady);
            }
        }

        public bool Equals(Player other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is Player other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, IsConnected, IsReady);
        }
    }
}