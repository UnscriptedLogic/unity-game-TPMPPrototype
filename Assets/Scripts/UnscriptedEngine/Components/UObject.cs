using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class UObject : MonoBehaviour
{
    public T CastTo<T>() where T : UObject
    {
        if (this as T)
        {
            return (T)this;
        }

        return default(T);
    }

    public void CastTo<T>(Action<T> OnSuccess, Action OnFailure = null) where T : UObject
    {
        if (this as T)
        {
            OnSuccess(this as T);
        }
        else
        {
            OnFailure?.Invoke();
        }
    }
}
