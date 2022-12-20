using Unity.Netcode;

namespace Kolman_Freecss.QuestSystem
{
    public struct QuestState : INetworkSerializable
    {
        public int QuestId;
        public bool isFinished;
        public bool IsCompleted;
        public int CurrrentAmount;
        public QuestStatus Status;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref QuestId);
            serializer.SerializeValue(ref isFinished);
            serializer.SerializeValue(ref IsCompleted);
            serializer.SerializeValue(ref CurrrentAmount);
            serializer.SerializeValue(ref Status);
        }
            
        public void setState(QuestState state)
        {
            QuestId = state.QuestId;
            isFinished = state.isFinished;
            IsCompleted = state.IsCompleted;
            CurrrentAmount = state.CurrrentAmount;
            Status = state.Status;
        }
            
        public static QuestState DefaultValue()
        {
            return new QuestState
                {};
        }
    }
}