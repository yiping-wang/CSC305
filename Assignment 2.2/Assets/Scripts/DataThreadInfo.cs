using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DataThreadInfo<T>
{
    public readonly System.Action<T> callback;
    public readonly T parameter;

    public DataThreadInfo(System.Action<T> callback, T parameter)
    {
        this.callback = callback;
        this.parameter = parameter;
    }
}
