using System.ComponentModel;
using System;

namespace GotchiTaMm;

internal enum LifeStageType
    {
        EGG = 0,
        BABY,
        CHILD,
        TEENAGER,
        ADULT,
        SPECIAL_ADULT,
        SENIOR,
        DEATH,
    }

internal enum GotchiFormType
    {
        [Description("Baby")]
        BABYTCHI = 0,
        [Description("Child")]
        MARUTCHI,
        [Description("Teenager")]
        KUCHITAMATCHI,
        TAMATCHI,
        [Description("Adult")]
        TARAKOTCHI,
        NYOROTCHI,
        KUCHIPATCHI,
        MASKUTCHI,
        GINJIROTCHI,
        MAMETCHI,
        [Description("SpecialAdult")]
        BILL,
    }

internal class Gotchi_Pet
    {

        internal bool IsSleepy = false;
        internal bool NeedsAttention = false;
        internal bool IsMisbehaving = false;
        internal int WakingHour = 0;
        internal int SleepingHour = 0;
        internal byte[] IllnessGauje = new byte[] { 0, 0 };
        internal bool Poo = false;
        internal byte Age = 0;
        internal byte DisciplineScore = 0;

        // Hunger mechanics
        private byte Weight = 1;
        private const byte WEIGHT_MAX = 100;

        private byte Hunger = 4;
        private const byte HUNGER_MAX = 4;
        private byte ConsecutiveSnacks = 0;
        internal const float CONSECUTIVE_SNACK_REDUCE_TICK_TIMING = 3000.0f;
        private const byte CONSECUTIVE_SNACKS_MAX = 15;
        private const byte CONSECUTIVE_SNACKS_SICK_THRESHOLD = 4;
        private const byte CONSECUTIVE_SNACKS_NEARDEATH_THRESHOLD = 11;
        private bool IsSick = false;
        private bool IsNearDeath = false;

        // Happiness mechanics
        private byte Happiness = 4;
        const byte _HAPPY_MAX = 4;


        public int EvolutionScore = 0;
        internal LifeStageType LifeStage = LifeStageType.EGG;

        internal void Animate()
            {

            }

        internal enum Meal_Type
            {
                MEAL = 0,
                SNACK = 1,
            }

        /*
         * Feeds the GotchiPet a meal or snack.
         */
        internal void Feed(Meal_Type meal)
            {
                switch (meal)
                    {

                        case Meal_Type.MEAL:
                            {
                                Console.WriteLine($"You fed the GotchiPet a good ol' bowl of rice!");
                                if (this.Hunger < HUNGER_MAX)
                                    {
                                        this.Hunger = HUNGER_MAX;
                                    }
                                if (this.Weight < WEIGHT_MAX)
                                    {
                                        this.Weight++;
                                    }
                                break;
                            }

                        case Meal_Type.SNACK:
                            {
                                if (this.Happiness < _HAPPY_MAX)
                                    {
                                        this.Happiness++;
                                    }
                                if (this.Weight < WEIGHT_MAX)
                                    {
                                        this.Weight += 2;
                                    }

                                this.ConsecutiveSnacks++;
                                if (this.ConsecutiveSnacks >=
                                    CONSECUTIVE_SNACKS_SICK_THRESHOLD
                                    && !this.IsSick)
                                    {
                                        this.IsSick = true;
                                    }
                                if (this.ConsecutiveSnacks >=
                                    CONSECUTIVE_SNACKS_NEARDEATH_THRESHOLD
                                    && !this.IsNearDeath)
                                    {
                                        this.IsNearDeath = true;
                                    }
                                if (this.ConsecutiveSnacks >= CONSECUTIVE_SNACKS_MAX
                                    && this.IsSick && this.IsNearDeath)
                                    {
                                        this.LifeStage = LifeStageType.DEATH;
                                    }
                                break;
                            }

                        default:
                            break;

                    }
            }

        internal void Play_With()
            {

            }

        internal void Give_Meds()
            {

            }

        internal void Clean()
            {

            }

        internal void Get_Status()
            {

            }

        internal void Discipline()
            {

            }

    }