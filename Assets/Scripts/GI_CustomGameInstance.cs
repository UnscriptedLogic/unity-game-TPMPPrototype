using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;
using static UnscriptedEngine.ULevelObject;

public class GI_CustomGameInstance : UGameInstance
{
    public Bindable<int> credits = new Bindable<int>(0);

    public Bindable<float> conveyorBeltSpeed = new Bindable<float>(0.5f);

    public Bindable<float> tickSpeed = new Bindable<float>(0.4f);
}
