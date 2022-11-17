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
        
        private QuestStatus _status;
        public QuestStatus Status
        {
            get { return _status; }
        } 
            
        public int StoryId;

        public Quest()
        {
            _status = QuestStatus.Inactive;
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
            _status = QuestStatus.NotStarted;
        }
        
        public void StartQuest()
        {
            _status = QuestStatus.Started;
        }
        
        public Quest CompleteQuest()
        {
            _status = QuestStatus.Completed;
            return this;
        }
        
        public bool UpdateQuestObjectiveAmount(EventQuestType eventQuestType, AmountType amountType)
        {
            objectives.ForEach(x =>
            {
                Debug.Log(x.EventQuestType + " " + eventQuestType + " " + x.AmountType + " " + amountType);
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
            return _status == QuestStatus.Completed;
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
            if (_status.Equals(QuestStatus.NotStarted))
            {
                //if (CheckStartCondition())
                //{
                    _status = QuestStatus.Started;
                //}
            }
            else if (_status.Equals(QuestStatus.Inactive))
            {
                //if (CheckActiveCondition())
                //{
                    _status = QuestStatus.NotStarted;
                //}
            }
            else if (_status == QuestStatus.Started)
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
            return _status == QuestStatus.NotStarted;
        }
        
        public bool IsStarted()
        {
            return _status == QuestStatus.Started;
        }
        
        public bool IsCompleted()
        {
            return QuestStatus.Completed.Equals(_status);
        }
        
    }

}