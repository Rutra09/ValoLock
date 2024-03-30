using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Emgu.CV;
using Emgu.CV.Structure;
using ValoLock.Configuration;
using ValoLock.ImageRecognition;

namespace ValoLock.Instalocking
{
    public class InstaLocking
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        
        public int AgentXLocation, AgentYLocation, ButtonXLocation, ButtonYLocation;
        public bool IsLocking = false;
        public LockMethod InstaLockMethod;


        public InstaLocking()
        {
            
        }
        
        public void Lock()
        {
            SetCursorPos(AgentXLocation, AgentYLocation);
            Thread.Sleep(100);
            mouse_event(MOUSEEVENTF_LEFTDOWN, AgentXLocation, AgentYLocation, 0, 0);
            Thread.Sleep(100);
            mouse_event(MOUSEEVENTF_LEFTUP, AgentXLocation, AgentYLocation, 0, 0);
            Thread.Sleep(100);
            SetCursorPos(ButtonXLocation, ButtonYLocation);
            Thread.Sleep(100);
            mouse_event(MOUSEEVENTF_LEFTDOWN, ButtonXLocation, ButtonYLocation, 0, 0);
            Thread.Sleep(100);
            mouse_event(MOUSEEVENTF_LEFTUP, ButtonXLocation, ButtonYLocation, 0, 0);
            Thread.Sleep(100);
        }
        
        public void StartLocking()
        {
            if (InstaLockMethod == LockMethod.SpamClick)
            {
                IsLocking = true;
                while (IsLocking)
                {
                    Lock();
                }
            }
            else if (InstaLockMethod == LockMethod.ScreenRecognition)
            {
                IsLocking = true;
                while (IsLocking)
                {
                    //get lockImage.png from files in System.Drawing Image format
                    Image<Bgr, byte> templateImage = new Image<Bgr, byte>("./imgs/valolock.png");
                    var cords = ImageRecognition.ImageRecognition.GetCoordinates(ScreenCapture.CaptureScreen(), templateImage);
                    bool isImageFound = ImageRecognition.ImageRecognition.IsImagePresentedOnScreenShoot(ScreenCapture.CaptureScreen(), templateImage);
                    Console.WriteLine(isImageFound);
                    Console.WriteLine($"X: {cords?.X}, Y: {cords?.Y}");
                    if (cords != null)
                    {
                        SetCursorPos(AgentXLocation, AgentYLocation);
                        Thread.Sleep(100);
                        mouse_event(MOUSEEVENTF_LEFTDOWN, AgentXLocation, AgentYLocation, 0, 0);
                        Thread.Sleep(100);
                        mouse_event(MOUSEEVENTF_LEFTUP, AgentXLocation, AgentYLocation, 0, 0);
                        Thread.Sleep(100);
                        SetCursorPos(cords.X, cords.Y);
                        Thread.Sleep(100);
                        mouse_event(MOUSEEVENTF_LEFTDOWN, cords.X, cords.Y, 0, 0);
                        Thread.Sleep(100);
                        mouse_event(MOUSEEVENTF_LEFTUP, cords.X, cords.Y, 0, 0);
                        Thread.Sleep(100);
                        // IsLocking = false;
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("Locking failed, please try again");
                        });
                    }
                    
                }
            }
        }
        public void StopLocking()
        {
            IsLocking = false;
        }
        
    }
}