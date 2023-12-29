using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Requirements
{
    public static List<Requirement> list = new List<Requirement>()
    {
        new DeployersMeetRateRequirement(),
    };
}

[System.Serializable]
public class Requirement
{
    [SerializeField] protected string name;
    [SerializeField] protected string gameDescription;
    [SerializeField] protected string realWorldDescription;

    protected bool isConditionMet;

    public virtual bool Condition(GM_LevelManager levelManager) => false;

    public string Name => name;
    public string GameDescription => gameDescription;
    public string RealWorldDescription => realWorldDescription;
    public bool IsConditionMet => isConditionMet;
}

public class DeployersMeetRateRequirement : Requirement
{
    public DeployersMeetRateRequirement()
    {
        name = "Rate Requirement";
        gameDescription = "All deployers must be recieving the required page at their requried rate to pass this requirement";
        realWorldDescription = "Every visit to a web page uses resources, be it loading user data or performing calculations. " +
            "As web developers, it is part of our duty to consider the advantages and limitations of each technology with this factor in mind.";
    }

    public override bool Condition(GM_LevelManager levelManager)
    {
        for (int i = 0; i < levelManager.Deployers.Count; i++)
        {
            if (!levelManager.Deployers[i].HasReachedRequiredRate)
            {
                return false;
            }
        }

        return true;
    }
}