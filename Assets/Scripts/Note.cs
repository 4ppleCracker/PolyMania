public struct Note
{
    //Data
    public Time time;
    public int slice;
    public bool clicked;
    public bool generated;
    public bool missed;
    public int accuracy;

    //Accessors
    public Time TimeToClick => time - Conductor.Instance.Position;

    public override string ToString()
    {
        return $"Time = {time}, Offset Time = {time} slice = {slice}, clicked = {clicked}, generated = {generated}, missed = {missed}, accuracy = {accuracy}";
    }

    public Note(Time time, int slice)
    {
        this.time = time;
        this.slice = slice;
        clicked = false;
        generated = false;
        missed = false;
        accuracy = 0;
    }
}
