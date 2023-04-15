using static SDL2.SDL;

namespace GotchiTaMm;

internal class Gotchi_Pet
   {
      private bool _is_sleepy = false;
      private bool _needs_attention = false;
      private bool _is_misbehaving = false;
      private int _waking_hour = 0;
      private int _sleeping_hour = 0;
      internal byte[] IllnessGauje = { 0, 0 };
      private bool _poo = false;
      private byte _age = 0;
      private byte _discipline_score = 0;

      // Hunger mechanics
      private byte _weight = 1;
      private const byte _WEIGHT_MAX = 100;

      private byte _hunger = 4;
      private const byte _HUNGER_MAX = 4;
      private byte _consecutive_snacks;
      internal const float CONSECUTIVE_SNACK_REDUCE_TICK_TIMING = 3000.0f;
      private const byte _CONSECUTIVE_SNACKS_MAX = 15;
      private const byte _CONSECUTIVE_SNACKS_SICK_THRESHOLD = 4;
      private const byte _CONSECUTIVE_SNACKS_NEARDEATH_THRESHOLD = 11;
      private bool _is_sick;
      private bool _is_near_death;

      // Happiness mechanics
      private byte _happiness = 4;
      const byte _HAPPY_MAX = 4;


      private int _evolution_score = 0;
      internal LifeStageType LifeStage = LifeStageType.EGG;

      private Gotchi_Pet_Form_Type
         _pet_form = Gotchi_Pet_Form_Type.BABYTCHI;

      internal SDL_Rect Render_Rect;
      internal SDL_Point Render_Offset;

      private Random _rng;

      internal Gotchi_Pet()
         {
            this.Update_Render_Rect();
            this._rng = new Random();
         }

      internal void Walk()
         {
            this.Render_Offset.x += this._rng.Next(-2, 3);
            this.Render_Offset.y += this._rng.Next(-2, 3);
         }
      
      internal void Animate()
         {
         }

      internal void Update_Render_Rect()
         {
            switch (this._pet_form)
               {
                  case Gotchi_Pet_Form_Type.BABYTCHI:
                     {
                        this.Render_Rect = Subsystem_Imaging
                           .Instance.Sprite_Atlas
                           .Get_Atlas_Image_Rect("Pet_v1");

                        this.Render_Rect.w =
                           (int)(this.Render_Rect.w *
                                 Main_App.SCREEN_RATIO);
                        this.Render_Rect.h =
                           (int)(this.Render_Rect.h *
                                 Main_App.SCREEN_RATIO);
                        this.Render_Rect.x =
                           (int)(Main_App.WINDOW_W * Main_App.SCREEN_RATIO
                                 / 2) + this.Render_Offset.x - this
                                 .Render_Rect.w/2;
                        this.Render_Rect.y =
                           (int)(Main_App.WINDOW_H * Main_App.SCREEN_RATIO
                                 / 2) + this.Render_Offset.y- this
                              .Render_Rect.h/2;
                     }
                     break;
                  case Gotchi_Pet_Form_Type.MARUTCHI:
                     break;
                  case Gotchi_Pet_Form_Type.KUCHITAMATCHI:
                     break;
                  case Gotchi_Pet_Form_Type.TAMATCHI:
                     break;
                  case Gotchi_Pet_Form_Type.TARAKOTCHI:
                     break;
                  case Gotchi_Pet_Form_Type.NYOROTCHI:
                     break;
                  case Gotchi_Pet_Form_Type.KUCHIPATCHI:
                     break;
                  case Gotchi_Pet_Form_Type.MASKUTCHI:
                     break;
                  case Gotchi_Pet_Form_Type.GINJIROTCHI:
                     break;
                  case Gotchi_Pet_Form_Type.MAMETCHI:
                     break;
                  case Gotchi_Pet_Form_Type.BILL:
                     break;
                  default:
                     throw new ArgumentOutOfRangeException();
               }
         }
      
      internal void Draw()
         {
            switch (_pet_form)
               {
                  case Gotchi_Pet_Form_Type.BABYTCHI:
                     SDL_RenderCopy(Main_App.Renderer, Subsystem_Imaging
                           .Instance.Sprite_Atlas
                           .Get_Atlas_Image("Pet_v1"),
                        IntPtr.Zero,
                        ref this.Render_Rect);
                     break;
                  case Gotchi_Pet_Form_Type.MARUTCHI:
                     break;
                  case Gotchi_Pet_Form_Type.KUCHITAMATCHI:
                     break;
                  case Gotchi_Pet_Form_Type.TAMATCHI:
                     break;
                  case Gotchi_Pet_Form_Type.TARAKOTCHI:
                     break;
                  case Gotchi_Pet_Form_Type.NYOROTCHI:
                     break;
                  case Gotchi_Pet_Form_Type.KUCHIPATCHI:
                     break;
                  case Gotchi_Pet_Form_Type.MASKUTCHI:
                     break;
                  case Gotchi_Pet_Form_Type.GINJIROTCHI:
                     break;
                  case Gotchi_Pet_Form_Type.MAMETCHI:
                     break;
                  case Gotchi_Pet_Form_Type.BILL:
                     break;
                  default:
                     throw new ArgumentOutOfRangeException();
               }
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
                        Console.WriteLine(
                           $"You fed the GotchiPet a good ol' bowl of rice!");
                        if (this._hunger < _HUNGER_MAX)
                           {
                              this._hunger = _HUNGER_MAX;
                           }

                        if (this._weight < _WEIGHT_MAX)
                           {
                              this._weight++;
                           }

                        break;
                     }

                  case Meal_Type.SNACK:
                     {
                        if (this._happiness < _HAPPY_MAX)
                           {
                              this._happiness++;
                           }

                        if (this._weight < _WEIGHT_MAX)
                           {
                              this._weight += 2;
                           }

                        this._consecutive_snacks++;
                        if (this._consecutive_snacks >=
                            _CONSECUTIVE_SNACKS_SICK_THRESHOLD
                            && !this._is_sick)
                           {
                              this._is_sick = true;
                           }

                        if (this._consecutive_snacks >=
                            _CONSECUTIVE_SNACKS_NEARDEATH_THRESHOLD
                            && !this._is_near_death)
                           {
                              this._is_near_death = true;
                           }

                        if (this._consecutive_snacks >=
                            _CONSECUTIVE_SNACKS_MAX
                            && this._is_sick && this._is_near_death)
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