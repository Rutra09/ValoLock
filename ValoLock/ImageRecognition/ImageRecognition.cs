
// using System.Drawing;

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Point = System.Drawing.Point;


namespace ValoLock.ImageRecognition
{
    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coordinate(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
    
    
    
    internal static class ImageRecognition
    {
        
        private static Mat BitmapToMat(Image image)
        {
            using (Bitmap bitmap = new Bitmap(image))
            {
                return bitmap.ToMat();
            }
        }

        
        internal static Coordinate? GetCoordinates(Image screenshot,   Image<Bgr, byte> template)
        {
            using (Mat imgScreenshot = BitmapToMat(screenshot))
            using (Mat imgTemplate = template.Mat)
            using (Mat result = new Mat())
            {
                if (imgScreenshot.Depth != DepthType.Cv8U && imgScreenshot.Depth != DepthType.Cv32F)
                {
                    imgScreenshot.ConvertTo(imgScreenshot, DepthType.Cv8U);
                }

                if (imgTemplate.Depth != DepthType.Cv8U && imgTemplate.Depth != DepthType.Cv32F)
                {
                    imgTemplate.ConvertTo(imgTemplate, DepthType.Cv8U);
                }

                if (imgScreenshot.NumberOfChannels != imgTemplate.NumberOfChannels)
                {
                    CvInvoke.CvtColor(imgScreenshot, imgScreenshot, ColorConversion.Bgr2Gray);
                    CvInvoke.CvtColor(imgTemplate, imgTemplate, ColorConversion.Bgr2Gray);
                }

                CvInvoke.MatchTemplate(imgScreenshot, imgTemplate, result, TemplateMatchingType.CcoeffNormed);
                // Rest of the code...
                CvInvoke.Normalize(result, result, 0, 1, NormType.MinMax, DepthType.Cv32F);
                double minVal = 0, maxVal = 0;
                Point minLoc = new Point(), maxLoc = new Point();
                CvInvoke.MinMaxLoc(result, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

                if (maxVal >= 0.9)
                {
                    return new Coordinate(maxLoc.X + imgTemplate.Width / 2, maxLoc.Y + imgTemplate.Height / 2);
                }
                else
                {
                    return null;
                }
            }
        }

        internal static bool IsImagePresentedOnScreenShoot(Image screenshot, Image<Bgr, byte> template)
        {
            using (Mat imgScreenshot = BitmapToMat(screenshot))
            using (Mat imgTemplate = template.Mat)
            using (Mat result = new Mat())
            {
                if (imgScreenshot.Depth != DepthType.Cv8U && imgScreenshot.Depth != DepthType.Cv32F)
                {
                    imgScreenshot.ConvertTo(imgScreenshot, DepthType.Cv8U);
                }

                if (imgTemplate.Depth != DepthType.Cv8U && imgTemplate.Depth != DepthType.Cv32F)
                {
                    imgTemplate.ConvertTo(imgTemplate, DepthType.Cv8U);
                }

                if (imgScreenshot.NumberOfChannels != imgTemplate.NumberOfChannels)
                {
                    CvInvoke.CvtColor(imgScreenshot, imgScreenshot, ColorConversion.Bgr2Gray);
                    CvInvoke.CvtColor(imgTemplate, imgTemplate, ColorConversion.Bgr2Gray);
                }

                CvInvoke.MatchTemplate(imgScreenshot, imgTemplate, result, TemplateMatchingType.CcoeffNormed);
                // Rest of the code...
                CvInvoke.Normalize(result, result, 0, 1, NormType.MinMax, DepthType.Cv32F);
                double minVal = 0, maxVal = 0;
                Point minLoc = new Point(), maxLoc = new Point();
                CvInvoke.MinMaxLoc(result, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

                if (maxVal >= 0.9)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


    }
    public class ScreenCapture
    {
        public static Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }

        public static Image CaptureWindow(IntPtr handle)
        {
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            User32.Rect windowRect = new User32.Rect();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
            GDI32.SelectObject(hdcDest, hOld);
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            Image img = Image.FromHbitmap(hBitmap);
            GDI32.DeleteObject(hBitmap);
            return img;
        }
    }
    public static class User32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImportAttribute("User32.dll")]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern long SetCursorPos(int x, int y);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetCursorPos(out POINT lpPoint);
    }
    public static class GDI32
    {
        public const int SRCCOPY = 0x00CC0020;
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hObjectSource,
            int nXSrc, int nYSrc, int dwRop);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
            int nHeight);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
    }
}
