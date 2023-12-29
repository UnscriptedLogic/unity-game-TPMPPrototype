using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    private List<Project> projects;

    public List<Project> Projects => projects;

    public PlayerData(List<Project> projects)
    {
        this.projects = new List<Project>(projects);
    }
}
