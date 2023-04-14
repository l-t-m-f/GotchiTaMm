namespace GotchiTaMm;

[Serializable]
public class Save_State
    {
        public DateTime Last_Time { get; set; }

        internal Gotchi_Pet Pet { get; set; }

        internal Game_State Game_State { get; set; }

        public Save_State()
            {
                this.Pet = new Gotchi_Pet();
                this.Game_State = new Game_State_Start();
            }
    }