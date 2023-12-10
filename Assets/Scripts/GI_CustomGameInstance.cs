using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;
using static UnscriptedEngine.ULevelObject;

public class GI_CustomGameInstance : UGameInstance
{
    public Bindable<int> packets = new Bindable<int>(100);

    public Bindable<float> conveyorBeltSpeed = new Bindable<float>(0.5f);
}
