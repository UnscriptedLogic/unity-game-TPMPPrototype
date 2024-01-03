using System.Collections.Generic;
using UnityEngine;
using static UnscriptedEngine.UObject;

public class UpgradeFactory : MonoBehaviour
{
    private PlayerData playerData;

    public UpgradeFactory(Bindable<PlayerData> playerData)
    {
        playerData.OnValueChanged += OnPlayerDataChanged;
    }

    private void OnPlayerDataChanged(PlayerData playerData)
    {
        this.playerData = playerData;
    }

    public void ObtainBootstrap()
    {
        playerData.upgradesObtained.Value = new List<string>(playerData.upgradesObtained.Value)
        {
            "bootstrap"
        };
    }

    public void DoubleBeltSpeed()
    {
        playerData.conveyorBeltSpeed.Value *= 2;
    }
}
