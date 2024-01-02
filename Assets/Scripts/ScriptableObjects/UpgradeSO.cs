using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "ScriptableObjects/Create New Upgrade")]
public class UpgradeSO : ScriptableObject
{
    [System.Serializable]
    public class Reward
    {
        [SerializeField] private Sprite rewardIcon;
        [SerializeField] private string rewardLabel;

        public Sprite RewardIcon => rewardIcon;
        public string RewardLabel => rewardLabel;
    }

    [SerializeField] private string upgradeID;

    [Header("Main Icon")]
    [SerializeField] private Sprite upgradeIcon;
    [SerializeField] private string upgradeLabel;

    [Space(16)]
    [SerializeField] private List<Reward> rewards;

    [Space(12)]
    [SerializeField][TextArea(3, 12)] private string description;

    [Space(12)]
    [SerializeField] private int cost = 100;

    public string UpgradeID => upgradeID;

    public Sprite UpgradeIcon => upgradeIcon;
    public string UpgradeLabel => upgradeLabel;

    public List<Reward> Rewards => rewards;

    public string Description => description;

    public int Cost => cost;
}
