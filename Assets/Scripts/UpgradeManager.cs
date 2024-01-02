using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class UpgradeManager : MonoBehaviour
{
    public class Milestone
    {
        public List<MilestoneReward> rewards;
    }

    public class MilestoneReward
    {
        public Action ObtainUpgrade;
    }

    private List<Milestone> milestoneList;
    private GI_CustomGameInstance gameInstance;

    public List<Milestone> MilestoneList => milestoneList;

    private void Start()
    {
        gameInstance = UGameModeBase.instance.GameInstance.CastTo<GI_CustomGameInstance>();

        milestoneList = new List<Milestone>()
        {
            new Milestone()
            {
                rewards = new List<MilestoneReward>() 
                {
                    new MilestoneReward()
                    {
                        ObtainUpgrade = () => 
                        {
                            gameInstance.playerData.Value.upgradesObtained.Value = new List<string>(gameInstance.playerData.Value.upgradesObtained.Value)
                            {
                                "designmk1"
                            };
                        }
                    },
                    new MilestoneReward()
                    {
                        ObtainUpgrade = () =>
                        {
                            gameInstance.playerData.Value.upgradesObtained.Value = new List<string>(gameInstance.playerData.Value.upgradesObtained.Value)
                            {
                                "logicmk1"
                            };
                        }
                    },
                    new MilestoneReward()
                    {
                        ObtainUpgrade = () =>
                        {
                            gameInstance.playerData.Value.upgradesObtained.Value = new List<string>(gameInstance.playerData.Value.upgradesObtained.Value)
                            {
                                "utilitymk1"
                            };

                            gameInstance.playerData.Value.conveyorBeltSpeed.Value *= 2f;
                        }
                    },
                }
            }
        };
    }

    public void Upgrade(int milestoneIndex, int rewardIndex)
    {
        milestoneList[milestoneIndex].rewards[rewardIndex].ObtainUpgrade();
    }
}
