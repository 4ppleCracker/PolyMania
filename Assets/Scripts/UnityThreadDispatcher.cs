using System;
using System.Collections.Generic;

public class UnityThreadDispatcher : SingletonBehaviour<UnityThreadDispatcher>
{
    public static Queue<Action> actions;
    public static void Run(Action action)
    {
        actions.Enqueue(action);
    }

    public void Update()
    {
        while(actions.Count > 0)
        {
            actions.Dequeue()();
        }
    }
}