using System;
using System.Drawing;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using Svg;

namespace MonoGameCross_PlatformDesktopApplication1
{
    public class SvgHandler
    {
        public static Texture2D BitmapToTexture2D(
            GraphicsDevice GraphicsDevice,
            System.Drawing.Bitmap image)
        {
            // Buffer size is size of color array multiplied by 4 because    
            // each pixel has four color bytes   
            int bufferSize = image.Height * image.Width * 4;
            // Create new memory stream and save image to stream so    
            // we don't have to save and read file   
            System.IO.MemoryStream memoryStream =
                new System.IO.MemoryStream(bufferSize);
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            // Creates a texture from IO.Stream - our memory stream   
            Texture2D texture = Texture2D.FromStream(
                GraphicsDevice, memoryStream);
            return texture;
        }
        
        // private Bitmap SVGToBitmap(Stream svgStream)
        // {
        //
        //
        //     // GdiGraphicsRenderer renderer = new GdiGraphicsRenderer();
        //     // RasterWindow rw = new RasterWindow(480, 360, renderer);
        //     // renderer.Window = rw;
        //     // SvgDocument doc = new SvgDocument(rw);
        //     // try
        //     // {
        //     //     doc.Load(svgStream);
        //     //     ISvgRect view = doc.RootElement.Viewport;
        //     //     XmlNodeList lst = doc.GetElementsByTagName("text");
        //     //     foreach (XmlNode node in lst)
        //     //     {
        //     //         if (node is ISvgTextElement)
        //     //         {
        //     //             ISvgTextElement itext = (ISvgTextElement)node;
        //     //             itext.SetAttribute("y", "0");
        //     //             itext.SetAttribute("x", "0");
        //     //         }
        //     //     }
        //     //     double height = Math.Max(view.Height, 1d);
        //     //     double width = Math.Max(view.Width, 1d);
        //     //     rw.Resize((int)width, (int)height);
        //     //     renderer.Render(doc);
        //     // }
        //     // catch (Exception)
        //     // {
        //     //     return new Bitmap(1,1);
        //     // }
        //     // return renderer.RasterImage;
        // }
    }
}