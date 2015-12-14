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
using System.Web.Mail;

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
        static Bitmap currentBitmap, previousBitmap;
        static Boolean block = false;
        private GT.Timer timer;
        void ProgramStarted()
        {
            mBitmap = new Bitmap(camera.CurrentPictureResolution.Width, camera.CurrentPictureResolution.Height); //initialize buffer to camera view size
            camera.BitmapStreamed += camera_BitmapStreamed;
            button.ButtonPressed += button_ButtonPressed;
            button.TurnLedOff(); //mark button as "off"
            timer = new GT.Timer(2000);
            timer.Tick += new GT.Timer.TickEventHandler(timer_Tick);
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
                    //fillColores();
                    button.TurnLedOn(); //mark button as "on"
                }
            }
        }



        void camera_BitmapStreamed(GTM.GHIElectronics.Camera sender, Bitmap bitmap)
        {
            //320*240

            displayT35.SimpleGraphics.DisplayImage(bitmap, 0, 0);
            detectionMove(bitmap);
        }


        ArrayList FillColor(Bitmap bitmap)
        {
            int i, j;
            ArrayList temp = new ArrayList();
            int posX = 0, posY = 0;

            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    var pixel = bitmap.GetPixel(posX, posY);
                    Color color = ColorUtility.ColorFromRGB(ColorUtility.GetRValue(pixel),
                                  ColorUtility.GetGValue(pixel), ColorUtility.GetBValue(pixel));
                    temp.Add(color);
                    posX += 159;
                }
                posY = +119;
                posX = 0;
            }
            return temp;
        }

        void detectionMove(Bitmap bitmap)
        {
            int cont = 0;
            if (currentBitmap != null)
            {
                previousBitmap = currentBitmap;
                lastColores = FillColor(previousBitmap);
            }

            if (previousBitmap != null)
            {

                for (int x = 0; x < 9; x++)
                {
                    Color oldColour = (Color)lastColores[x];
                    Color newColour = (Color)actualColores[x];
                    int deltaRed = System.Math.Abs(ColorUtility.GetRValue(oldColour) - ColorUtility.GetRValue(newColour));
                    int deltaGreen = System.Math.Abs(ColorUtility.GetGValue(oldColour) - ColorUtility.GetGValue(newColour));
                    int deltaBlue = System.Math.Abs(ColorUtility.GetBValue(oldColour) - ColorUtility.GetBValue(newColour));
                    int deltaTotal = deltaRed + deltaGreen + deltaBlue;
                    if (deltaTotal > 50)
                    {
                        cont++;
                    }
                }
                if (cont >= 1)
                {
                    block = true;
                    timer.Start();
                }
                else
                {
                    Debug.Print("Nothing");
                }
            }
            currentBitmap = bitmap;
            actualColores = FillColor(currentBitmap);
        }


        private void timer_Tick(GT.Timer timer)
        {
           if(block){
               sendNotification("smoncayo@espol.edu.ec","omoncayo@espol.edu.ec","Aviso, AALARMA!","Intruso detectado");
               block = false;
           }
        }
        public static void sendNotification(string MessageFrom, string MessageTo, string MessageSubject, string MessageBody)
    {
      MailMessage message = new MailMessage();
      message.From        = MessageFrom;
      message.To          = MessageTo;
      message.Subject     = MessageSubject;
      message.BodyFormat  = MailFormat.Text;
      message.Body        = MessageBody;
 
      try
      {
        Debug.Print("Mensaje enviandose");
        SmtpMail.Send(message);
      }
      catch( Exception exHttp )
      {
          Debug.Print("ERROR enviando notificacion");
    }
  }
    }
}