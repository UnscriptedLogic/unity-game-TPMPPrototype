using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFactoryValidation
{
    bool IsProjectCompleted { get; }
    bool IsSpeedingUpFactoryOverTime { get; }

    event EventHandler OnTestFactoryClicked;
    event EventHandler OnProjectCompleted;
    event EventHandler OnProjectEvaluationCompleted;
}