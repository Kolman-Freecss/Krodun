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

        private void Update()
        {
            if (QuestManager.Instance.CurrentStory != null && QuestManager.Instance.CurrentStory.CurrentQuest != null)
            {
                DisplayQuestInfo(QuestManager.Instance.CurrentStory.CurrentQuest);
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
            QuestManager.Instance.AcceptQuest();
            CloseQuest();
        }
        
        public void OnCompleteQuest()
        {
            QuestManager.Instance.CompleteQuest();
            CloseQuest();
        }
        
        /*public void OnDeclineQuest()
        {
            QuestManager.Instance.DeclineQuest();
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