public struct Note
{
    public Time time;
    public int slice;

    public Note(Time time, int slice)
    {
        this.time = time;
        this.slice = slice;
    }
}
