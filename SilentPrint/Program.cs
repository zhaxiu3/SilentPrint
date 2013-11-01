using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentPrint
{
    public enum PageSetting { 
        Left,
        Right,
        Center
    }
    class Program
    {
        private static PageSetting setting;
        private static string filename;
        private static void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {

            Image i = Image.FromFile(filename);
            Rectangle m = ev.MarginBounds;
            Rectangle target;

            int left = 0;
            int top =0;
            
            double newwidth = 0;
            double newheight = 0;

            double scaledimagewidth = 0;
            double scaledimageheight = 0;


            double pageratio = (double)m.Width / (double)m.Height;
            double imageratio = (double)i.Width / (double)i.Height;


            if (imageratio > pageratio) // image is wider
            {
                newheight = i.Height > m.Height ? (double)i.Height : (double)m.Height;
                newwidth = newheight * pageratio;

                scaledimageheight = newheight;
                scaledimagewidth = scaledimageheight * imageratio;

            }
            else
            {
                newwidth = i.Width > m.Width ? (double)i.Width : (double)m.Width;
                newheight = newwidth / pageratio;

                scaledimagewidth = newwidth;
                scaledimageheight = scaledimagewidth / imageratio;

            }

            Image newimage = new Bitmap((int)newwidth, (int)newheight);
            Graphics p = Graphics.FromImage(newimage);
            Console.WriteLine(scaledimagewidth + "  "+newwidth + "   " + newheight);

            switch (setting) {
                case PageSetting.Left:
                    left = 0;
                    top = 0;
                    break;
                case PageSetting.Right:
                    left = (int)(newwidth - scaledimagewidth);
                    top = (int)(newheight - scaledimageheight);
                    break;
                case PageSetting.Center:
                    left = (int)(newwidth - scaledimagewidth) / 2;
                    top = (int)(newheight - scaledimageheight) / 2;
                    break;
            }

            target = new Rectangle(left, top, (int)scaledimagewidth, (int)scaledimageheight);
            p.DrawImage(i, target);

            ev.Graphics.DrawImage(newimage, ev.MarginBounds );

            Console.WriteLine(ev.MarginBounds);
            Console.WriteLine(target);
        }
        static void Main(string[] args)
        {

            if (args.Length < 1) {
                throw new Exception("no file specified");
            }
            else if (args.Length < 2)
            {
                setting = PageSetting.Left;
            }
            else {
                switch (args[1]) { 
                    case "left":
                        setting = PageSetting.Left;
                        break;
                    case "right":
                        setting = PageSetting.Right;
                        break;
                    case "center":
                        setting = PageSetting.Center;
                        break;
                    default:
                        setting = PageSetting.Left;
                        break;
                }
            }

            filename = args[0];
            try
            {
                try
                {
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += new PrintPageEventHandler
                       (pd_PrintPage);
                    pd.PrintController = new StandardPrintController();
                    Margins margins = new Margins(10, 10, 10, 10);
                    pd.DefaultPageSettings.Margins = margins;
                    pd.Print();
                }
                finally
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
