using System;
using System.Collections;
using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;
//Librerias de Gadgeteer
using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;


namespace CameraSecurity
{
    public partial class Program
    {
        public Bitmap actualPicture;
        public ArrayList pictures;
        private static int contador = 0;
        
        float nivelDeDeteccion;

        void ProgramStarted()
        {
            //Incializa las variables de deteccion
            //deteccion = new MotionDetector(new TwoFramesDifferenceDetector(), new MotionBorderHighlighting());
            nivelDeDeteccion = 0;
            //Lista de todos los dispositivos de entrada de video
            
            camera.BitmapStreamed += camera_BitmapStreamed;
            button.ButtonPressed += button_ButtonPressed;
            camera.PictureCaptured += camera_PictureCaptured;
            //pictures = new ArrayList();
            Debug.Print("Program Started");
        }

        void camera_PictureCaptured(Camera sender, GT.Picture picture)
        {
            camera.TakePicture();
        }

        void button_ButtonPressed(Button sender, Button.ButtonState state)
        {
            
            camera.StartStreaming();
        }

        void camera_BitmapStreamed(Camera sender, Bitmap bitmap)
        {
            displayT35.SimpleGraphics.DisplayImage(bitmap, 0, 0);
            if (bitmap != null) {
                pictures.Add(bitmap);
                contador++;
               Debug.Print("Cont: "+pictures.Capacity);
                //compareBitmaps(pictures);
            }
  
        }

        /*private void compareBitmaps(ArrayList pictures)
        {
            int i = 0;
           
            if (pictures.Count >= 2) {
                for (i = 0; i < pictures.Count-1; i++)
                {
                    if (Equals((Bitmap)pictures[i], (Bitmap)pictures[i + 1]))
                    {
                        Debug.Print("SAME PIC");
                    }
                }
            }
        }*/

        private static bool Equals(Bitmap bmp1, Bitmap bmp2)
        {
            int cont = 0;
            for (int x = 0; x < bmp1.Width; ++x)
            {
                for (int y = 0; y < bmp1.Height; ++y)
                {
                    if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                    {
                        cont++;
                    }
                }
            }
            if (cont >= 50) {
                return false;
            }
            return true;
        }

        void camera_CameraConnected(Camera sender)
        {
            Debug.Print("CAMERA IS ON");
        }

    }
}
