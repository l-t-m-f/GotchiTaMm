using System;
using System.ComponentModel;
using System.Reflection;

using static SDL2.SDL;

namespace GotchiTaMm;

public static class Util
    {
        public static string Get_Description<T>(this T e) where T : Enum
            {
                var description_attribute = e.GetType()
                    .GetMember(e.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>();
                return description_attribute?.Description ?? e.ToString();
            }

        public static void Draw_Ellipsoid(IntPtr renderer, SDL_Rect circle)
            {
                double pih = Math.PI / 2;
                const int prec = 300; // precision value; value of 1 will draw a diamond, 27 makes pretty smooth circles.
                double theta = 0; // angle that will be increased each loop

                int x = (int)(circle.w * Math.Cos(theta));//start point
                int y = (int)(circle.h * Math.Sin(theta));//start point
                int x1 = x;
                int y1 = y;

                double step = pih / prec; // amount to add to theta each time (degrees)
                for (theta = step ; theta <= pih ; theta += step)//step through only a 90 arc (1 quadrant)
                    {
                        //get new point location
                        x1 = (int)(circle.w * Math.Cos(theta) + 0.5); //new point (+.5 is a quick rounding method)
                        y1 = (int)(circle.h * Math.Sin(theta) + 0.5); //new point (+.5 is a quick rounding method)

                        //draw line from previous point to new point, ONLY if point incremented
                        if ((x != x1) || (y != y1))//only draw if coordinate changed
                            {
                                SDL_RenderDrawLine(renderer, circle.x + x, circle.y - y, circle.x + x1, circle.y - y1);//quadrant TR
                                SDL_RenderDrawLine(renderer, circle.x - x, circle.y - y, circle.x - x1, circle.y - y1);//quadrant TL
                                SDL_RenderDrawLine(renderer, circle.x - x, circle.y + y, circle.x - x1, circle.y + y1);//quadrant BL
                                SDL_RenderDrawLine(renderer, circle.x + x, circle.y + y, circle.x + x1, circle.y + y1);//quadrant BR
                            }
                        //save previous points
                        x = x1;//save new previous point
                        y = y1;//save new previous point
                    }
                //arc did not finish because of rounding, so finish the arc 
                if (x != 0)
                    {
                        x = 0;
                        SDL_RenderDrawLine(renderer, circle.x + x, circle.y - y, circle.x + x1, circle.y - y1);//quadrant TR
                        SDL_RenderDrawLine(renderer, circle.x - x, circle.y - y, circle.x - x1, circle.y - y1);//quadrant TL
                        SDL_RenderDrawLine(renderer, circle.x - x, circle.y + y, circle.x - x1, circle.y + y1);//quadrant BL
                        SDL_RenderDrawLine(renderer, circle.x + x, circle.y + y, circle.x + x1, circle.y + y1);//quadrant BR
                    }
            }
        public static void FillEllipsoid(IntPtr renderer, SDL_Rect circle)
            {
                double pih = Math.PI / 2;
                const int prec = 300; // precision value; value of 1 will draw a diamond, 27 makes pretty smooth circles.
                double theta = 0; // angle that will be increased each loop

                int x = (int)(circle.w * Math.Cos(theta));//start point
                int y = (int)(circle.h * Math.Sin(theta));//start point
                int x1 = x;
                int y1 = y;

                SDL_RenderDrawLine(renderer, circle.x - x1, circle.y - y1, circle.x + x1, circle.y - y1);
                SDL_RenderDrawLine(renderer, circle.x - x1, circle.y + y1, circle.x + x1, circle.y + y1);

                double step = pih / prec; // amount to add to theta each time (degrees)
                for (; theta <= pih + step ; theta += step)//step through only a 90 arc (1 quadrant)
                    {
                        //get new point location
                        x1 = (int)(circle.w * Math.Cos(theta) + 0.5); //new point (+.5 is a quick rounding method)
                        y1 = (int)(circle.h * Math.Sin(theta) + 0.5); //new point (+.5 is a quick rounding method)

                        //draw line from previous point to new point, ONLY if point incremented
                        if ((x != x1) || (y != y1))//only draw if coordinate changed
                            {
                                SDL_RenderDrawLine(renderer, circle.x - x1, circle.y - y1, circle.x + x1, circle.y - y1);
                                SDL_RenderDrawLine(renderer, circle.x - x1, circle.y + y1, circle.x + x1, circle.y + y1);
                            }
                        //save previous points
                        x = x1;//save new previous point
                        y = y1;//save new previous point
                    }
                //arc did not finish because of rounding, so finish the arc
                if (x != 0)
                    {
                        x = 0;
                        SDL_RenderDrawLine(renderer, circle.x - x1, circle.y - y1, circle.x + x1, circle.y - y1);
                        SDL_RenderDrawLine(renderer, circle.x - x1, circle.y + y1, circle.x + x1, circle.y + y1);
                    }
            }


        internal static void Blit(IntPtr renderer, IntPtr texture, int x, int y)
            {
                SDL_Rect destination;
                destination.x = x;
                destination.y = y;
                SDL_QueryTexture(texture, out uint format, out int access, out destination.w, out destination.h);
                SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref destination);
            }

        internal static void BlitRect(IntPtr renderer, IntPtr texture, SDL_Rect rect)
            {
                SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref rect);
            }
    }
