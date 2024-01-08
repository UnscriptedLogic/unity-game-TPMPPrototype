using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDeployer
{
    bool HasReachedRequiredRate { get; }
}
