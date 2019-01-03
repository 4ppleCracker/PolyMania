# PolyMania
Rhythm game where you click a polygon

Been working on this game since 5th august, this is the second rewrite

First version originally written in MonoGame is [here](https://github.com/nobbele/RhythmGame)<br />
First rewrite of the game made in Unity is [here](https://github.com/nobbele/RhythmGameUnity)

For new people making rhythm games here are some tips on how to design it
 1. Do not store notes as beat number and then calculate something via the bpm, thats just overcomplicating it,<br />
    instead store them as milliseconds.
    
 2. Do not do timing via some kind of timer object, its very inaccurate and will become very offsynced if something lags<br />
    instead keep track of the current song position and see if it has been passed ("note time" - "time to show note").
    
    For example, note time is 2400 milliseconds and notes should show up 500 milliseconds before it should be pressed.<br />
    First calculate the time you should show the note with ("note time" - "time to show note"),<br />
    which in this case is (2400 - 500) which is 1900, using the song position we can check if its more than 1900.<br />
    For example if the song position is 1600, 1600 is not later than 1900 so we should not spawn the note however,<br />
    if the song position is 2000, its later than 1900 so we should spawn the note.

Good links<br />
[Here's a quick and dirty guide I just wrote: How To Make A Rhythm Game](https://www.reddit.com/r/gamedev/comments/2fxvk4/heres_a_quick_and_dirty_guide_i_just_wrote_how_to/)
  
