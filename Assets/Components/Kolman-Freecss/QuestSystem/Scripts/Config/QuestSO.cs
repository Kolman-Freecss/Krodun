using UnityEngine;
using UnityEngine.Serialization;

namespace Kolman_Freecss.QuestSystem
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest")]
    public class QuestSO : ScriptableObject
    {
        [FormerlySerializedAs("title")] [SerializeField]
        string _title;

        public string TitleValue
        {
            get => _title;
        }

        [FormerlySerializedAs("description")] [TextArea(2, 6)] [SerializeField]
        string _description;

        public string DescriptionValue
        {
            get => _description;
        }

        [FormerlySerializedAs("objectives")] [TextArea(2, 6)] [SerializeField]
        string _objectives;

        public string ObjectivesValue
        {
            get => _objectives;
        }

        // Implement your rewards here. By default it will add Reward model inside the component
        [FormerlySerializedAs("reward")] [SerializeField]
        Reward _reward;

        public Reward RewardValue
        {
            get => _reward;
        }
    }
}