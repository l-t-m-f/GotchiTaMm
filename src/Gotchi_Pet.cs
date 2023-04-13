using System.ComponentModel;
using System;

namespace GotchiTaMm;

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
        internal byte Weight = 1;
        internal const byte WEIGHT_MAX = 100;

        internal byte Hunger = 4;
        internal const byte HUNGER_MAX = 4;
        internal byte ConsecutiveSnacks = 0;
        internal const float CONSECUTIVE_SNACK_REDUCE_TICK_TIMING = 3000.0f;
        internal const byte CONSECUTIVE_SNACKS_MAX = 15;
        internal const byte CONSECUTIVE_SNACKS_SICK_THRESHOLD = 4;
        internal const byte CONSECUTIVE_SNACKS_NEARDEATH_THRESHOLD = 11;
        internal bool IsSick = false;
        internal bool IsNearDeath = false;

        // Happiness mechanics
        internal byte Happiness = 4;
        const byte HAPPY_MAX = 4;



        internal int EvolutionScore = 0;
        internal LifeStageType LifeStage = LifeStageType.Egg;

        internal void Animate()
            {

            }

        internal enum MealType
            {
                MEAL = 0,
                SNACK = 1,
            }

        /*
         * Feeds the GotchiPet a meal or snack.
         */
        internal void Feed(MealType meal)
            {
                switch (meal)
                    {

                        case MealType.MEAL:
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

                        case MealType.SNACK:
                            {
                                if (this.Happiness < HAPPY_MAX)
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
                                        this.LifeStage = LifeStageType.Death;
                                    }
                                break;
                            }

                        default:
                            break;

                    }
            }

        internal void PlayWith()
            {

            }

        internal void GiveMeds()
            {

            }

        internal void Clean()
            {

            }

        internal void GetStatus()
            {

            }

        internal void Discipline()
            {

            }

    }