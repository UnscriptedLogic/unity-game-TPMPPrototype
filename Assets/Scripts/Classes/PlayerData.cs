using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnscriptedEngine.UObject;

[System.Serializable]
public class PlayerData
{
    public class GameValues
    {
        public int credits = 400;
        public float beltSpeed = 1f;
        public float tickSpeed = 0.4f;
    }

    private List<Project> projects;

    public bool useMouseCursorToMove = true;

    public Bindable<int> credits = new Bindable<int>(0);
    public Bindable<float> conveyorBeltSpeed = new Bindable<float>(0.5f);
    public Bindable<float> tickSpeed = new Bindable<float>(0.4f);

    public List<Project> Projects => projects;

    public PlayerData(GameValues values, List<Project> projects)
    {
        credits.Value = values.credits;
        conveyorBeltSpeed.Value = values.beltSpeed;
        tickSpeed.Value = values.tickSpeed;

        this.projects = new List<Project>(projects);
    }
}