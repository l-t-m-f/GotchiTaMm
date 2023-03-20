using System.ComponentModel;

namespace GotchiTaMm
{

    internal enum LifeStageType
    {
        Egg = 0,
        Baby,
        Child,
        Teenager,
        Adult,
        SpecialAdult,
        Senior,
        Death,
    }

    internal enum GotchiFormType
    {
        [Description("Baby")]
        Babytchi = 0,
        [Description("Child")]
        Marutchi,
        [Description("Teenager")]
        Kuchitamatchi,
        Tamatchi,
        [Description("Adult")]
        Tarakotchi,
        Nyorotchi,
        Kuchipatchi,
        Maskutchi,
        Ginjirotchi,
        Mametchi,
        [Description("SpecialAdult")]
        Bill,
    }

    internal class GotchiPet
    {

        internal bool IsSleepy = false;
        internal bool NeedsAttention = false;
        internal bool IsMisbehaving = false;
        internal int WakingHour = 0;
        internal int SleepingHour = 0;
        internal byte[] IllnessGauje = new byte[] { 0, 0 };
        internal bool Poo = false;
        internal byte Age = 0;
        internal byte WeightInGrams = 1;
        internal byte Discipline = 0;
        internal byte[] Hunger = new byte[] { 0, 0, 0, 0 };
        internal byte[] Happiness = new byte[] { 0, 0, 0, 0 };
        internal int EvolutionScore = 0;
        internal LifeStageType LifeStage = LifeStageType.Egg;

        public void Animate()
        {

        }
    }
}
