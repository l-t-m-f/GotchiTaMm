using System;

namespace GotchiTaMm;

/// <summary>
/// Clock for the game. Also responsible for aging the GotchiPet
/// based on time elapsed while the game was shutdown.
/// </summary>
internal class Clock
    {
        internal bool clockRunning = false;

        // The total duration of the game. This time is reset when the
        // GotchiPet dies
        internal TimeSpan totalGameTime;
        internal TimeSpan ellapsedSessionTime; // time between session start and current DateTime;

        // The Date and Time at the start of the session
        // Based on a Now DateTime, but the player can configure a local offset
        // which will be applied to the DateTimes for the clock
        internal DateTime sessionStartDateTime;

        internal TimeSpan tsOffset;

        // The Date and Time at the end of the session (Application shutdown)
        // Saved to MEM so that it can be restored and compared with the Now DateTime
        internal DateTime sessionCurrentDateTime;

        internal DateTime sessionEndDateTime;

        // There are two scenarios:

        // A) No SaveState -> Player is asked for a local time. This local time is
        // compared to the machine Now DateTime to calculate the HH:MM offset. The
        // offset is recorded, the sessionStartTime is initialized, as well as the total time,
        // and the clockRunning bool is set true.

        // B) SaveState -> The sessionStartTime and totalTime are restored from the SaveState
        // as well as the timeOffset, so that the proper time can be restored.

        /// <summary>
        /// The clock is constructed at the beginning of the game if 
        /// there is a SaveState. Otherwise, it is initialized when the
        /// player inputs the starting time.
        /// </summary>
        /// <param name="h"></param>
        /// <param name="m"></param>
        /// <param name="s"></param>
        internal Clock(TimeSpan offset)
            {
                this.sessionStartDateTime = (DateTime.Now).Add(offset);
                this.sessionCurrentDateTime = this.sessionStartDateTime;

                //gameTime = new TimeSpan(h, m, s);
                //elapsedTime = new TimeSpan(0, 0, 0);
            }

        internal Clock(TimeSpan restoredOffset, TimeSpan restoredTotalGameTime)
            {
                this.sessionStartDateTime = (DateTime.Now).Add(restoredOffset);
                this.sessionCurrentDateTime = this.sessionStartDateTime;
                this.totalGameTime = restoredTotalGameTime;
            }

        internal void GameSecond()
            {
                this.sessionCurrentDateTime.Add(TimeSpan.FromSeconds(1));
            }
        internal string GetGameTime()
            {
                return $"{this.sessionCurrentDateTime.Hour:D2}:" +
                       $"{this.sessionCurrentDateTime.Minute:D2}:" +
                       $"{this.sessionCurrentDateTime.Second:D2}";
            }
    }