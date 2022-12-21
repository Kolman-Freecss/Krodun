using System.Collections.Generic;
using Kolman_Freecss.Krodun;
using Unity.Netcode;
using UnityEngine;

namespace Kolman_Freecss.QuestSystem
{
    public class QuestGiver : NetworkBehaviour
    {
        #region ######## Inspector Variables ########
        [Header("Quest Info")] public List<QuestSO> QuestsSO = new List<QuestSO>();
        public Quest CurrentQuest;
        public List<GameObject> QuestMarkers;
        #endregion

        #region ######## Network Variables ########
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

        #endregion
        
        #region ######## Auxiliar Variables ########

        private List<Quest> Quests = new List<Quest>();
        
        // Auxiliar variables
        private GameObject _notStarted;
        private GameObject _inProgress;
        private GameObject _completed;
        
        KrodunController _player;
        
        #endregion
        

        private void Awake()
        {
            QuestsSO.ForEach(x => Quests.Add(new Quest(x, x.StoryStep)));
            _notStarted = QuestMarkers.Find(g => g.name == "ExclamationNotStarted");
            _inProgress = QuestMarkers.Find(g => g.name == "ExclamationStarted");
            _completed = QuestMarkers.Find(g => g.name == "QuestCompletedMark");
            SubscribeToDelegatesAndUpdateValues();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                NotStarted.Value = true;
                InProgress.Value = false;
                Completed.Value = false;
                QuestManager.Instance.OnCollectItemEvent += OnItemCollectedServerRpc;
            }
            
            CurrentQuest = Quests[0];
            SubscribeToDelegatesAndUpdateValues();
        }
        
        private void SubscribeToDelegatesAndUpdateValues()
        {
            QuestStateSync.OnValueChanged += UpdateQuestState;
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
            GameManager.Instance.OnSceneLoadedChanged += OnGameStarted;
        }
        
        public void OnGameStarted(bool isLoaded)
        {
            if (isLoaded)
            {
                _player = FindObjectOfType<KrodunController>();
            }
        }
        
        public void UpdateQuestState(QuestState previousState, QuestState newState)
        {
            if (newState.isFinished)
            {
                CurrentQuest = null;
            }
            else
            {
                CurrentQuest.objectives[0].isCompleted = newState.IsCompleted;
                CurrentQuest.objectives[0].CurrentAmount = newState.CurrrentAmount;
                CurrentQuest.Status = newState.Status;
            }
            RefreshQuestMarkServerRpc();
        }

        public Quest UpdateQuestStatus(Quest quest)
        {
            CurrentQuest.UpdateStatus(quest);
            SyncQuestStatus(CurrentQuest);
            return CurrentQuest;
        }

        /**
         * Refresh the quest by quest id parameter
         */
        public void RefreshQuest(int questId)
        {
            Quest qs = Quests.Find(x => x.ID == questId).UpdateStatus();
            if (qs.Status == QuestStatus.NotStarted)
            {
                CurrentQuest = qs;
            }
            SyncQuestStatus(qs);
        }
        
        private void SyncQuestStatus(Quest quest)
        {
            var state = new QuestState
            {
                IsCompleted = quest.objectives[0].isCompleted, 
                CurrrentAmount = quest.objectives[0].CurrentAmount, 
                Status = quest.Status
            };
            UpdateQuestServerRpc(state);
            RefreshQuestMarkServerRpc();
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
        
        #region ######## ServerCalls ########
        
        [ServerRpc(RequireOwnership = false)]
        public void UpdateQuestServerRpc(QuestState state, ServerRpcParams serverRpcParams = default)
        {
            var clientId = serverRpcParams.Receive.SenderClientId;
            QuestStateSyncValue = state;
        }
        
        [ServerRpc]
        private void OnItemCollectedServerRpc(EventQuestType eventQuestType, AmountType amountType, int questId)
        {
            if (CurrentQuest.ID != questId) return;
            if (CurrentQuest.UpdateQuestObjectiveAmount(eventQuestType, amountType))
            {
                // We update the quest status in quest giver
                UpdateQuestStatus(CurrentQuest);
            }
            else
            {
                SyncQuestStatus(CurrentQuest);
            }
        }

        /**
         * Displays one mark on the quest giver by the current quest status
         */
        [ServerRpc(RequireOwnership = false)]
        private void RefreshQuestMarkServerRpc(ServerRpcParams serverRpcParams = default)
        {
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
        
        #endregion

        #region ################## GETTERS && SETTERS ################## 

        /*public bool NotStarted.Value { get => NotStarted.Value; set => NotStarted.Value = value; }
        
        public bool InProgress.Value { get => InProgress.Value; set => InProgress.Value = value; }
        
        public bool Completed.Value { get => Completed.Value; set => Completed.Value = value; }*/
        
        public QuestState QuestStateSyncValue { get => QuestStateSync.Value; set => QuestStateSync.Value = value; }

        #endregion
    }
}