using System;
using UnityEngine;

namespace Kolman_Freecss.QuestSystem
{
    [System.Serializable]
    public class Objective
    {
        [SerializeField] private EventQuestType _eventQuestType;
        public EventQuestType EventQuestType => _eventQuestType;
        
        [SerializeField] private int _requiredAmount;
        public int RequiredAmount
        {
            get => _requiredAmount;
        }
        
        [SerializeField] private AmountType _amountType;
        public AmountType AmountType => _amountType;
        
        public bool isCompleted;
        
        private int _currentAmount;
        
        public int CurrentAmount
        {
            get { return _currentAmount; }
        }
        
        public Objective(Objective objective)
        {
            isCompleted = objective.isCompleted;
            _currentAmount = 0;
            _requiredAmount = objective._requiredAmount;
        }
        
        /**
         * returns true if the objective is completed
         */
        public bool UpdateAmount()
        {
            if (isCompleted)
                return true;
            _currentAmount ++;
            if (_currentAmount >= _requiredAmount)
            {
                isCompleted = true;
            }
            return isCompleted;
        }
        
        /*public void ResetAmount()
        {
            _currentAmount = 0;
        }*/
    }
}