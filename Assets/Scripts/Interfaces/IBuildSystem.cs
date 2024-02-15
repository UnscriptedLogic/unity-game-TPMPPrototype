using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuildSystem
{
    TickSystem.Ticker NodeTickSystem { get; }
    float GlobalBeltSpeed { get; }
}