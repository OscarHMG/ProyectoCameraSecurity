using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;
using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Input;
namespace GadgeteerApp1
{
    public partial class Program
    {
        Bitmap mBitmap; //shared image buffer between camera and display
        bool mFirst = true;
        byte mLastR = 0;
        byte mLastG = 0;
        byte mLastB = 0;
        DateTime mLastWarn = DateTime.MinValue;
        ArrayList posiciones = new ArrayList();
        ArrayList actualColores = new ArrayList();
        ArrayList lastColores = new ArrayList();
        void ProgramStarted()
        {
            mBitmap = new Bitmap(camera.CurrentPictureResolution.Width, camera.CurrentPictureResolution.Height); //initialize buffer to camera view size
            camera.BitmapStreamed += camera_BitmapStreamed;
            button.ButtonPressed += button_ButtonPressed;
            button.TurnLedOff(); //mark button as "off"
            Debug.Print("Program Started");
        }

        void button_ButtonPressed(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
        {
            if (button.IsLedOn) //check if button is “on"
            {
                camera.StopStreaming(); //stop streaming
                button.TurnLedOff(); //mark button as "off"
            }
            else
            {
                if (camera.CameraReady)
                {
                    camera.StartStreaming(mBitmap); //start streaming
                    fillColores();
                    button.TurnLedOn(); //mark button as "on"
                }
            }
        }



        void camera_BitmapStreamed(GTM.GHIElectronics.Camera sender, Bitmap bitmap)
        {
            //320*240
            displayT35.SimpleGraphics.DisplayImage(bitmap, 0, 0);
            FillColor(bitmap);
            detectionMove();
        }



        private int detectMotion(Bitmap bitmap, int pX, int pY, Color newColor)
        {
            int resultado = 0;
            newColor = bitmap.GetPixel(pX, pY);
            var r = ColorUtility.GetRValue(newColor);
            var g = ColorUtility.GetGValue(newColor);
            var b = ColorUtility.GetBValue(newColor);

            if (System.Math.Abs(r - mLastR) + System.Math.Abs(g - mLastG) + System.Math.Abs(b - mLastB) >= 100)
            {
                Debug.Print("Motion!!");
                resultado = System.Math.Abs(r - mLastR) + System.Math.Abs(g - mLastG) + System.Math.Abs(b - mLastB);

            }

            mLastR = r;
            mLastB = b;
            mLastG = g;

            return resultado;
        }


        void FillColor(Bitmap bitmap)
        {
            int i, j;
            int posX = 0, posY = 0;

            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    var pixel = bitmap.GetPixel(posX, posY);
                    Color color = ColorUtility.ColorFromRGB(ColorUtility.GetRValue(pixel),
                                  ColorUtility.GetGValue(pixel), ColorUtility.GetBValue(pixel));
                    posiciones.Add(pixel);
                    actualColores.Add(color);

                    posX += 159;
                }
                posY = +119;
                posX = 0;
            }

        }


        private int detectionMove()
        {
            int i;
            Color color, lastColor;
            for (i = 0; i < 9; i++)
            {
                var posicion = posiciones[i];
                color = (Color)actualColores[i];
                lastColor = (Color)actualColores[i];
                var r = ColorUtility.GetRValue(color);
                var g = ColorUtility.GetGValue(color);
                var b = ColorUtility.GetBValue(color);
                var mLastR = ColorUtility.GetRValue(lastColor);
                var mLastG = ColorUtility.GetGValue(lastColor);
                var mLastB = ColorUtility.GetBValue(lastColor);
                if (mFirst)
                {
                    mFirst = false;
                }
                else
                {
                    if (System.Math.Abs(r - mLastR) + System.Math.Abs(g - mLastG) + System.Math.Abs(b - mLastB) >= 100)
                    {
                        Debug.Print("Motion!!");
                        //Manda mensaje
                        return 1; //Intrusop
                    }
                }//Ahora actualizo los colores pasados con los actuales para cada punto
                Color newColor = ColorUtility.ColorFromRGB(r, g, b);
                lastColores[i] = newColor;
            }
            return 0;
        }

        void fillColores()
        {
            int i =0;
            for (i = 0; i < 9; i++ )
            {
                lastColores.Add(ColorUtility.ColorFromRGB(0, 0, 0));
            }
        }
    }
}