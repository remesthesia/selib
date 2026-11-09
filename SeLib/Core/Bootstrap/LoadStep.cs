using System;
using System.Collections;

namespace SeLib.Core.Bootstrap;

internal readonly struct LoadStep(float weight, Func<IEnumerator> coroutine)
{
    public readonly float Weight = weight;
    public readonly Func<IEnumerator> Coroutine = coroutine;
}