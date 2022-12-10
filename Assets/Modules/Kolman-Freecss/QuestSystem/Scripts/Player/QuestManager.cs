using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kolman_Freecss.QuestSystem
{
    public class QuestManager : MonoBehaviour
    {
        public List<StorySO> storiesSO = new List<StorySO>();
        private List<Story> stories = new List<Story>();
        public Story currentStory;
        private List<QuestGiver> _questGivers = new List<QuestGiver>();

        private void Start()
        {
            _questGivers = FindObjectsOfType<QuestGiver>().ToList();
            storiesSO.ForEach(storySO => { stories.Add(new Story(storySO)); });
            //TODO : Add a way to choose the story
            currentStory = stories[0];
            currentStory.StartStory();
            RefreshQuestGivers();
        }

        /*private void Update()
        {
            // ONLY!! Use it like hack to test the quest system            
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (currentStory.UpdateQuestObjectiveAmount(currentStory.CurrentQuest.objectives[0].EventQuestType,
                        currentStory.CurrentQuest.objectives[0].AmountType))
                {
                    Debug.Log("Complete quest -> " + currentStory.CurrentQuest.storyStep);
                    // First we update the quest status in quest giver
                    UpdateStatusGiverByQuestId(currentStory.CurrentQuest);
                    // Then we complete the quest
                    CompleteStatusGiverByQuestId(currentStory.CurrentQuest.ID);
                }

                Debug.Log("Update objective -> " + currentStory.CurrentQuest.objectives[0].CurrentAmount + " / " +
                          currentStory.CurrentQuest.objectives[0].RequiredAmount);
                Debug.Log("isCompleted -> " + currentStory.CurrentQuest.objectives[0].isCompleted);
                if (currentStory.CurrentQuest.objectives[0].isCompleted)
                {
                    Debug.Log("Complete quest -> " + currentStory.CurrentQuest.storyStep);
                    Debug.Log("Name quest -> " + currentStory.CurrentQuest.title);
                    CompleteQuest();
                }
            }
        }*/

        /*
         * This method is used to update the quest objetive
         * @param eventQuestType : The type of the event
         * @param eventEnum : The enum of the event
         */
        public void EventTriggered(EventQuestType eventQuestType, AmountType amountType)
        {
            if (currentStory.UpdateQuestObjectiveAmount(eventQuestType, amountType))
            {
                Debug.Log("Objetive progress");
                // We update the quest status in quest giver
                UpdateStatusGiverByQuestId(currentStory.CurrentQuest);
            }
        }

        /**
         * Finish the current quest and start the next one
         */
        public void CompleteQuest()
        {
            FinishStatusGiverByQuestId(currentStory.CurrentQuest.ID);
            if (currentStory.CompleteQuest() != null)
            {
                RefreshQuestGivers();
            }
            else
            {
                currentStory.CompleteStory();
                Debug.Log("Story completed");
            }
        }

        /**
         * Update the current story and the status of the current quest giver
         */
        public void AcceptQuest()
        {
            UpdateStatusGiverByQuestId(currentStory.CurrentQuest);
            currentStory.AcceptQuest();
        }

        /**
         * Update the current story and the next quest givers
         */
        public void NextQuest()
        {
            UpdateStatusGiverByQuestId(currentStory.CurrentQuest);
            currentStory.NextQuest();
            RefreshQuestGivers();
        }

        public void FinishStatusGiverByQuestId(int questId)
        {
            GetQuestGiverByQuestId(questId).FinishQuest();
        }

        public Quest CompleteStatusGiverByQuestId(int questId)
        {
            return GetQuestGiverByQuestId(questId).CompleteQuest();
        }

        /**
         * Update the current quest of the quest givers
         */
        public Quest UpdateStatusGiverByQuestId(Quest quest)
        {
            return GetQuestGiverByQuestId(quest.ID).UpdateQuestStatus(quest);
        }

        /**
         * Refresh the quest givers that are on the current steps in the current story 
         */
        void RefreshQuestGivers()
        {
            QuestGiver qg = GetQuestGiverByQuestId(currentStory.CurrentQuest.ID);
            if (qg != null)
            {
                qg.RefreshQuest(currentStory.CurrentQuest.ID);
            }
        }

        QuestGiver GetQuestGiverByQuestId(int questId)
        {
            return _questGivers.Find(q => q.HasQuest(questId));
        }

    }
}