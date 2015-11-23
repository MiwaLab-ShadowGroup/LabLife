using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Robot
{
    class LightAction 
    {
        CalculatePosition CP;
        Light light;

        public LightAction()
        {
            this.CP = new CalculatePosition(0);
        }
        public void Action(List<Human> list_human, ref GameObject robot)
        {
            this.light = robot.transform.GetChild(0).GetComponent<Light>();
            this.ChageLightColor(list_human);
        }

        void ChageLightColor(List<Human> list_human)
        {
            float h = (float) (this.CP.Pearson(list_human) / (this.CP.CenterPosition(list_human)).magnitude);
            this.light.color = this.HSVtoRGB(h, 0.5f, 1);
        }

        Color HSVtoRGB(float h, float s, float v)
        {
            Color colorRGB = new Color(v, v, v);
            if (s > 0)
            {
                int i = (int) (h * 6.0f);
                float f = h - (float)i;

                switch (i)
                {
                    case 0:
                        colorRGB.g *= 1 - s * (1 - f);
                        colorRGB.b *= 1 - s;
                        break;
                    case 1:
                        colorRGB.r *= 1 - s * f;
                        colorRGB.b *= 1 - s;
                        break;
                    case 2:
                        colorRGB.r *= 1 - s;
                        colorRGB.b *= 1 - s * (1 - f);
                        break;
                    case 3:
                        colorRGB.r *= 1 - s;
                        colorRGB.g *= 1 - s * f;
                        break;
                    case 4:
                        colorRGB.r *= 1 - s * (1 - f);
                        colorRGB.g *= 1 - s;
                        break;
                    case 5:
                        colorRGB.g *= 1 - s;
                        colorRGB.b *= 1 - s * f;
                        break;
                }
            }

            return colorRGB;
        }
    }
}
