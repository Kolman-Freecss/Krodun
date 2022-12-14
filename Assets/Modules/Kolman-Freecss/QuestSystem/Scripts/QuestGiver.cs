using System;
using System.Collections.Generic;
using Kolman_Freecss.Krodun;
using Unity.Netcode;
using UnityEngine;


namespace Kolman_Freecss.QuestSystem
{
    public class QuestGiver : NetworkBehaviour
    {
        #region Inspector Variables
        [Header("Quest Info")] public List<QuestSO> QuestsSO = new List<QuestSO>();
        public Quest CurrentQuest;
        public List<GameObject> QuestMarkers;
        #endregion

        #region Auxiliar Variables

        private List<Quest> Quests = new List<Quest>();
        
        // Auxiliar variables
        private GameObject _notStarted;
        private GameObject _inProgress;
        private GameObject _completed;
        
        [HideInInspector]
        public NetworkVariable<bool> NotStarted = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone,
            writePerm: NetworkVariableWritePermission.Owner);
        [HideInInspector]
        public NetworkVariable<bool> InProgress = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner);
        [HideInInspector]
        public NetworkVariable<bool> Completed = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner);

        [HideInInspector] public NetworkVariable<QuestState> QuestStateSync = new NetworkVariable<QuestState>(QuestState.DefaultValue(), NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner);

        KrodunController _player;

        #endregion
        

        private void Awake()
        {
            _player = FindObjectOfType<KrodunController>();
            QuestsSO.ForEach(x => Quests.Add(new Quest(x, x.StoryStep)));
            _notStarted = QuestMarkers.Find(g => g.name == "ExclamationNotStarted");
            _inProgress = QuestMarkers.Find(g => g.name == "ExclamationStarted");
            _completed = QuestMarkers.Find(g => g.name == "QuestCompletedMark");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                NotStarted.Value = true;
                InProgress.Value = false;
                Completed.Value = false;
            }
            
            CurrentQuest = Quests[0];
            SubscribeToDelegatesAndUpdateValues();
        }

        private void SubscribeToDelegatesAndUpdateValues()
        {
            QuestStateSync.OnValueChanged += UpdateQuestClientRpc;
            NotStarted.OnValueChanged += (previousValue, newValue) =>
            {
                _notStarted.SetActive(newValue);
            };
            InProgress.OnValueChanged += (previousValue, newValue) =>
            {
                _inProgress.SetActive(newValue);
            };
            Completed.OnValueChanged += (previousValue, newValue) =>
            {
                _completed.SetActive(newValue);
            };
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void UpdateQuestServerRpc(QuestState state, ServerRpcParams serverRpcParams = default)
        {
            var clientId = serverRpcParams.Receive.SenderClientId;
            Debug.Log($"QuestGiver: UpdateQuestServerRpc: {clientId}");
            QuestStateSyncValue.setState(state);
        }
        
        [ClientRpc]
        public void UpdateQuestClientRpc(QuestState previousState, QuestState newState)
        {
            Debug.Log($"QuestGiver: UpdateQuestClientRpc: {newState}");
            CurrentQuest.objectives[0].isCompleted = newState.IsCompleted;
            CurrentQuest.objectives[0].CurrentAmount = newState.CurrrentAmount;
            CurrentQuest.Status = newState.Status;
            RefreshQuestMarkServerRpc();
        }

        public Quest UpdateQuestStatus(Quest quest)
        {
            CurrentQuest.UpdateStatus(quest);
            var state = new QuestState
            {
                IsCompleted = CurrentQuest.Status == QuestStatus.Completed,
                Status = CurrentQuest.Status,
            };
            UpdateQuestServerRpc(state);
            RefreshQuestMarkServerRpc();
            return CurrentQuest;
        }

        /**
         * Refresh the quest by quest id parameter
         */
        public void RefreshQuest(int questId)
        {
            Debug.Log(nameof(IsLocalPlayer) + IsLocalPlayer + ", " + nameof(IsServer) + IsServer + ", " + nameof(IsClient) + IsClient + ", " + nameof(IsHost) + IsHost
                      + ", " + nameof(IsOwner) + IsOwner + nameof(NetworkManager.Singleton.LocalClientId) + NetworkManager.Singleton.LocalClientId);
            Quest qs = Quests.Find(x => x.ID == questId).UpdateStatus();
            if (qs.Status == QuestStatus.NotStarted)
            {
                CurrentQuest = qs;
            }
            var state = new QuestState {IsCompleted = qs.objectives[0].isCompleted, CurrrentAmount = qs.objectives[0].CurrentAmount, Status = qs.Status};
            UpdateQuestServerRpc(state);
            RefreshQuestMarkServerRpc();
        }

        /**
         * Displays one mark on the quest giver by the current quest status
         */
        [ServerRpc(RequireOwnership = false)]
        private void RefreshQuestMarkServerRpc(ServerRpcParams serverRpcParams = default)
        {
            Debug.Log($"QuestGiver: RefreshQuestMarkServerRpc: {serverRpcParams.Receive.SenderClientId}");
            if (CurrentQuest != null)
            {
                HideQuestMarks();
                switch (CurrentQuest.Status)
                {
                    case QuestStatus.NotStarted:
                        NotStarted.Value = true;
                        break;
                    case QuestStatus.Started:
                        InProgress.Value = true;
                        break;
                    case QuestStatus.Completed:
                        Completed.Value = true;
                        break;
                }
            }
            else
            {
                HideQuestMarks();
            }
        }

        private void HideQuestMarks()
        {
            NotStarted.Value = false;
            InProgress.Value = false;
            Completed.Value = false;
        }
        
        /**
         * Face the quest giver to the player
         */
        public void FaceTarget()
        {
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = lookRotation;
        }
        
        public void FinishQuest()
        {
            CurrentQuest = null;
            var state = new QuestState {isFinished = true};
            UpdateQuestServerRpc(state);
            RefreshQuestMarkServerRpc();
        }

        public Quest CompleteQuest()
        {
            RefreshQuestMarkServerRpc();
            var state = new QuestState {IsCompleted = true, CurrrentAmount = 0, Status = QuestStatus.Completed};
            UpdateQuestServerRpc(state);
            return CurrentQuest.CompleteQuest();
        }
        
        public bool HasQuest(Quest quest)
        {
            return Quests.Contains(quest);
        }

        public bool HasQuest(int questId)
        {
            return Quests.Exists(x => x.ID == questId);
        }

        #region ################## GETTERS && SETTERS ################## 

        /*public bool NotStarted.Value { get => NotStarted.Value; set => NotStarted.Value = value; }
        
        public bool InProgress.Value { get => InProgress.Value; set => InProgress.Value = value; }
        
        public bool Completed.Value { get => Completed.Value; set => Completed.Value = value; }*/
        
        public QuestState QuestStateSyncValue { get => QuestStateSync.Value; set => QuestStateSync.Value = value; }

        #endregion


        #region Struct NetworkVariable to Sync Quest State

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

        #endregion
    }
}