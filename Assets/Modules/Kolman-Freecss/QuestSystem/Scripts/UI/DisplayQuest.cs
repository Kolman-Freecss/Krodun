using TMPro;
using UnityEngine;

namespace Kolman_Freecss.QuestSystem
{
    public class DisplayQuest : MonoBehaviour
    {
        [Header("Quest")]
        [SerializeField]
        TextMeshProUGUI questName;
        
        [SerializeField]
        TextMeshProUGUI questDescription;
        
        [SerializeField]
        TextMeshProUGUI objectiveDescription;
        
        [SerializeField]
        TextMeshProUGUI objectiveProgress;
        
        [SerializeField]
        TextMeshProUGUI questReward;
        
        QuestManager _questManager;

        private void Awake()
        {
            _questManager = FindObjectOfType<QuestManager>();
        }

        private void Update()
        {
            if (_questManager.currentStory != null && _questManager.currentStory.CurrentQuest != null)
            {
                DisplayQuestInfo(_questManager.currentStory.CurrentQuest);
            }
        }

        public void DisplayQuestInfo(Quest quest)
        {
            questName.text = quest.title;
            questDescription.text = quest.description;
            objectiveDescription.text = quest.objectiveText;
            objectiveProgress.text = "";
            quest.objectives.ForEach(o => objectiveProgress.text += o.CurrentAmount + "/" + o.RequiredAmount);
            questReward.text = quest.reward.ToString();
        }
        
        public void OnAcceptQuest()
        {
            _questManager.AcceptQuest();
            CloseQuest();
        }
        
        public void OnCompleteQuest()
        {
            _questManager.CompleteQuest();
            CloseQuest();
        }
        
        /*public void OnDeclineQuest()
        {
            _questManager.DeclineQuest();
        }*/
        
        public void OnExit()
        {
            CloseQuest();
        }
        
        public void CloseQuest()
        {
            gameObject.SetActive(false);
        }
        
    }
}