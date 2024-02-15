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

    public void DoubleBeltSpeed()
    {
        playerData.conveyorBeltSpeed.Value *= 2;
    }
}
