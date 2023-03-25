namespace GotchiTaMm
{
    internal class Clock
    {
        internal TimeSpan gameTime;
        internal TimeSpan elapsedTime;

        internal Clock(int h, int m, int s)
        {
            gameTime = new TimeSpan(h, m, s);
            elapsedTime = new TimeSpan(0, 0, 0);
        }

        // Method to increment the elapsed time
        internal void IncrementTime(int hours, int minutes, int seconds)
        {
            elapsedTime += new TimeSpan(hours, minutes, seconds);
            gameTime += new TimeSpan(hours, minutes, seconds);
        }

        // Get the elapsed time in hours
        internal int GetElapsedHours()
        {
            return (int)elapsedTime.TotalHours;
        }

        // Get the elapsed time in minutes
        internal int GetElapsedMinutes()
        {
            return (int)elapsedTime.TotalMinutes;
        }

        // Get the elapsed time in seconds
        internal int GetElapsedSeconds()
        {
            return (int)elapsedTime.TotalSeconds;
        }

        internal int GetGameTimeHours()
        {
            return (int)gameTime.Hours;
        }

        // Get the elapsed time in minutes
        internal int GetGameTimeMinutes()
        {
            return (int)gameTime.Minutes;
        }

        // Get the elapsed time in seconds
        internal int GetGameTimeSeconds()
        {
            return (int)gameTime.Seconds;
        }

        // Get the elapsed time as a formatted string
        internal string GetGameTime()
        {
            return $"{gameTime.Hours:D2}:{gameTime.Minutes:D2}:{gameTime.Seconds:D2}";
        }

        // Get the elapsed time as a formatted string
        internal string GetElapsedTime()
        {
            return $"{elapsedTime.Hours:D2}:{elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}";
        }

    }
}
