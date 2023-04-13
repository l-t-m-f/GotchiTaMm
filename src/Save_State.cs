using System;

namespace GotchiTaMm;

[Serializable]
public class Save_State
    {
        public DateTime LastTime { get; set; }

        internal Gotchi_Pet Pet { get; set; }

        internal Game_State GameState { get; set; }

        public Save_State()
            {
                this.Pet = new Gotchi_Pet();
                this.GameState = new GameStartState();
            }
    }