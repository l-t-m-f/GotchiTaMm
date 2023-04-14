using System;

namespace GotchiTaMm;

/// <summary>
/// Clock for the game. Also responsible for aging the GotchiPet
/// based on time elapsed while the game was shutdown.
/// </summary>
internal class Clock
    {
        internal bool Clock_Running = false;

        // The total duration of the game. This time is reset when the
        // GotchiPet dies
        internal TimeSpan Total_Game_Time;
        internal TimeSpan Ellapsed_Session_Time; // time between session start and current DateTime;

        // The Date and Time at the start of the session
        // Based on a Now DateTime, but the player can configure a local offset
        // which will be applied to the DateTimes for the clock
        private readonly DateTime _session_start_date_time;

        internal TimeSpan Ts_Offset;

        // The Date and Time at the end of the session (Application shutdown)
        // Saved to MEM so that it can be restored and compared with the Now DateTime
        private readonly DateTime _session_current_date_time;

        internal DateTime Session_End_DateTime;

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
        /// <param name="offset"></param>
        internal Clock(TimeSpan offset)
            {
                DateTime session_start_date_time;
                this._session_start_date_time = (DateTime.Now).Add(offset);
                this._session_current_date_time = this._session_start_date_time;
                this.Ts_Offset = default;
                this.Session_End_DateTime = default;
                //gameTime = new TimeSpan(h, m, s);
                //elapsedTime = new TimeSpan(0, 0, 0);
            }

        internal Clock(TimeSpan restored_offset, TimeSpan restored_total_game_time)
            {
                DateTime session_start_date_time;
                this._session_start_date_time = (DateTime.Now).Add(restored_offset);
                this._session_current_date_time = this._session_start_date_time;
                this.Total_Game_Time = restored_total_game_time;
                this.Ts_Offset = default;
                this.Session_End_DateTime = default;
            }

        internal void GameSecond()
            {
                this._session_current_date_time.Add(TimeSpan.FromSeconds(1));
            }
        internal string Get_Game_Time()
            {
                return $"{this._session_current_date_time.Hour:D2}:" +
                       $"{this._session_current_date_time.Minute:D2}:" +
                       $"{this._session_current_date_time.Second:D2}";
            }
    }