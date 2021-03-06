﻿using System;
using System.Collections.Generic;

public class FunctionalComparer<T> : IComparer<T>
{
    private readonly Func<T, T, int> comparer;
    public FunctionalComparer(Func<T, T, int> comparer)
    {
        this.comparer = comparer;
    }
    public static IComparer<T> Create(Func<T, T, int> comparer)
    {
        return new FunctionalComparer<T>(comparer);
    }
    public int Compare(T x, T y)
    {
        return comparer(x, y);
    }
}
