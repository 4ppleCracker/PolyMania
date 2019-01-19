using System.Collections.Generic;
using UnityEngine;

public abstract class ReplayEvent
{
    public Time time;
    public abstract void Execute();
}
public class ReplayMousePosition : ReplayEvent
{
    public Vector2 position;
    public override void Execute()
    {
        throw new System.NotImplementedException();
    }
}
public class ReplayKeyClick : ReplayEvent
{
    public override void Execute()
    {
        throw new System.NotImplementedException();
    }
}
public class Replay {
    public List<ReplayEvent> events;
}
