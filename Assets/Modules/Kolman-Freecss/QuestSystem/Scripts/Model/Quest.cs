using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kolman_Freecss.QuestSystem
{
    public class Quest
    {
        private int _id;
        public int ID
        {
            get { return _id; }
        }

        public string title;
        public string description;
        public string objectiveText;
        public List<Objective> objectives = new List<Objective>();
        public int storyStep;
        public Reward reward;

        public QuestStatus Status { get; set; }

        public int StoryId;

        public Quest()
        {
            Status = QuestStatus.Inactive;
        }
        
        public Quest(QuestSO questSo) : this()
        {
            title = questSo.TitleValue;
            description = questSo.DescriptionValue;
            objectiveText = questSo.ObjectivesValue;
            questSo.Objectives.ForEach(x => objectives.Add(new Objective(x)));
            storyStep = questSo.StoryStep;
            reward = questSo.RewardValue;
            StoryId = questSo.storyId;
        }
        
        public Quest(QuestSO questSo, int questId) : this(questSo)
        {
            _id = questId;
        }
        
        public void ActiveQuest()
        {
            Status = QuestStatus.NotStarted;
        }
        
        public void StartQuest()
        {
            Status = QuestStatus.Started;
        }
        
        public Quest CompleteQuest()
        {
            Status = QuestStatus.Completed;
            return this;
        }
        
        public bool UpdateQuestObjectiveAmount(EventQuestType eventQuestType, AmountType amountType)
        {
            objectives.ForEach(x =>
            {
                if (x.EventQuestType == eventQuestType && x.AmountType == amountType)
                {
                    if (x.UpdateAmount())
                    {
                        if (AllObjectivesCompleted())
                        {
                            CompleteQuest();
                        }
                    };
                }
            });
            return Status == QuestStatus.Completed;
        }

        private bool AllObjectivesCompleted()
        {
            return objectives.TrueForAll(x => x.isCompleted);
        }

        public Quest UpdateStatus()
        {
            return UpdateStatus(this);
        }
    
        public Quest UpdateStatus(Quest quest)
        {
            if (Status.Equals(QuestStatus.NotStarted))
            {
                //if (CheckStartCondition())
                //{
                    Status = QuestStatus.Started;
                //}
            }
            else if (Status.Equals(QuestStatus.Inactive))
            {
                //if (CheckActiveCondition())
                //{
                    Status = QuestStatus.NotStarted;
                //}
            }
            else if (Status == QuestStatus.Started)
            {
                if (CheckEndCondition(quest))
                {
                    CompleteQuest();
                }
            }
            return this;
        }
        
        /*private bool CheckStartCondition()
        {
            return _questSo.StartCondition.CheckCondition();
        }*/
        
        private bool CheckEndCondition(Quest quest)
        {
            bool endCondition = true;
            quest.objectives.ForEach(o =>
            {
                if (!o.isCompleted)
                {
                    endCondition = false;
                }
            });
            return endCondition;
        }
        
        public bool IsNotStarted()
        {
            return Status == QuestStatus.NotStarted;
        }
        
        public bool IsStarted()
        {
            return Status == QuestStatus.Started;
        }
        
        public bool IsCompleted()
        {
            return QuestStatus.Completed.Equals(Status);
        }
        
    }

}