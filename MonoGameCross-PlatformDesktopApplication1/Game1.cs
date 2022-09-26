using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.Json;
using AutoMapper;
using AutoMapper.Internal;
using DiplomacyEngine.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Svg;
using Svg.Transforms;
using Color = Microsoft.Xna.Framework.Color;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using Point = System.Drawing.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net;
using System.Diagnostics;

namespace MonoGameCross_PlatformDesktopApplication1
{
    //Old working dir
    // /home/whomagoo/RiderProjects/MonoGameCross-PlatformDesktopApplication1/MonoGameCross-PlatformDesktopApplication1/bin/Debug/netcoreapp3.1
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D svg;
        private SvgDocument _svgDoc;

        private bool draw = true;
        private Microsoft.Xna.Framework.Point pos;
        private SpriteFont font;

        private List<Texture2D> items;

        private Bitmap mouseReg;
        private List<LocationView> _countiresIndex;

        private Dictionary<string, LocationView> _countriesMap;

        private int prevIndex = -1;
        private SvgPaintServer prevColor;

        private List<LocationView> locations = new();

        private ExpiringText et = new ExpiringText();

        private Dropdown dropDown;

        DiplomacyGame dg;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            // _graphics.PreferMultiSampling = true;
            // _graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
            Window.AllowUserResizing = true;
            _graphics.ApplyChanges();

            items = new List<Texture2D>();

            using (var fs = new FileStream("/home/whomagoo/RiderProjects/MonoGameCross-PlatformDesktopApplication1/MonoGameCross-PlatformDesktopApplication1/Content/EuropeGameBoard.json", FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            {
                dg = new DiplomacyGame(sr);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = this.Content.Load<SpriteFont>("Fonts/sf");

            _svgDoc = SvgDocument.Open("/home/whomagoo/RiderProjects/MonoGameCross-PlatformDesktopApplication1/MonoGameCross-PlatformDesktopApplication1/Content/map2.svg");
            // _svgDoc = SvgDocument.Open("/home/whomagoo/RiderProjects/MonoGameCross-PlatformDesktopApplication1/MonoGameCross-PlatformDesktopApplication1/Content/map.svg");

            // _svgDoc = SvgDocument.Open("/home/whomagoo/RiderProjects/MonoGameCross-PlatformDesktopApplication1/MonoGameCross-PlatformDesktopApplication1/image.svg");

            foreach (var v in _svgDoc.Children)
            {
                Console.WriteLine(v.ID);
            }




            int width = 4, height = 4;

            Color[] backgroundTexture = new Color[width * height];

            Array.Fill(backgroundTexture, Color.Gray);

            var solidTexture = new Texture2D(this._graphics.GraphicsDevice, width, height);

            solidTexture.SetData(backgroundTexture);

            dropDown = new Dropdown(new string[] { "Attack", "Support", "Defend", "Move" }, font, solidTexture);

            //
            // Norway.Fill = new SvgColourServer(System.Drawing.Color.Green);
            //
            // var borderz = _svgDoc.GetElementById("defs2");
            // var borders = _svgDoc.GetElementById("layer3");
            //
            // foreach (var border in borders.Children)
            // {
            //     border.StrokeWidth = new SvgUnit(SvgUnitType.Millimeter, .5f);
            //     border.Stroke = new SvgColourServer(System.Drawing.Color.White);
            // }

            // _svgDoc.GetElementById("star").Fill = SvgPaintServer.None;



            // float scaleConstant = (float)(1 / 0.26458333);
            float scaleConstant = 8F;

            int i = 0;
            _countiresIndex = new List<LocationView>();
            _countriesMap = new Dictionary<string, LocationView>();

            float imageWidth = _svgDoc.ViewBox.Width;
            float imageHeight = _svgDoc.ViewBox.Height;

            float layerWidth = _svgDoc.Bounds.Width;
            float layerHeight = _svgDoc.Bounds.Height;

            Bitmap hitboxes = new Bitmap((int)(imageWidth * scaleConstant), (int)(imageHeight * scaleConstant));

            var countires = _svgDoc.GetElementById("layer4").DeepCopy();


            Stopwatch timerSvgCast = new Stopwatch();
            Stopwatch timerSaveBmp = new Stopwatch();
            Stopwatch timerSvgRender = new Stopwatch();
            Stopwatch timerSvgRenderHitbox = new Stopwatch();
            Stopwatch timerCombineHitboxes = new Stopwatch();
            Stopwatch timerOverall = Stopwatch.StartNew();

            var curLayer = countires as SvgVisualElement;

            SortedList<long, string> sl = new SortedList<long, string>();

            if (curLayer != null)
            {
                Console.WriteLine(curLayer.Bounds);

                foreach (var c in countires.Children)
                {
                    timerSvgCast.Start();
                    var frag = c as SvgVisualElement;
                    timerSvgCast.Stop();
                    if (frag != null)
                    {

                        frag.Fill = new SvgColourServer(System.Drawing.Color.White);
                        frag.Stroke = new SvgColourServer(System.Drawing.Color.LightGray);

                        var oldBounds = frag.Bounds;

                        var transform = new SvgScale(scaleConstant, scaleConstant);

                        if (frag.Transforms == null)
                        {
                            frag.Transforms = new SvgTransformCollection(){
                                transform
                            };
                        }
                        else
                        {
                            var transformsExisting = frag.Transforms;
                            transformsExisting.Add(transform);
                            // frag.Transforms = new SvgTransformCollection() {new SvgScale(scaleConstant * 0.26458333F, scaleConstant* 0.26458333F)};
                        }

                        float scaleX = 1;
                        float scaleY = 1;

                        foreach (var transf in frag.Transforms)
                        {

                            SvgScale scale2 = transf as SvgScale;
                            if (null != scale2)
                            {
                                scaleX *= scale2.X;
                                scaleY *= scale2.Y;
                            }
                        }

                        System.Console.WriteLine("%sw% " + frag.StrokeWidth);

                        frag.StrokeWidth = frag.StrokeWidth * .4F;

                        // frag.FlushStyles();
                        // frag.StrokeWidth = oldSW;

                        frag.Transforms.Add(new SvgTranslate(-frag.Bounds.X, -frag.Bounds.Y));

                        var NewBounds = frag.Bounds;

                        if(oldBounds != NewBounds){
                            System.Console.WriteLine("Updated Bounds for " + frag.ID);
                        }

                        // bounds = newDoc.Bounds;

                        // Console.WriteLine(newDoc.ViewBox);
                        // frag.InvalidateChildPaths();
                        // frag.Children.Add(new SvgRectangle
                        // {
                        //  X= 0,
                        //  Y=0,
                        //  Width = 30,
                        //  Height = 30,
                        //  StrokeWidth = new SvgUnit(SvgUnitType.Pixel, 8),
                        //  Stroke = new SvgColourServer(System.Drawing.Color.Black)
                        // });



                        // frag.SpaceHandling = XmlSpaceHandling.Preserve;

                        var bounds = frag.Bounds;
                        var pixelWidth = (int)(scaleX * bounds.Width);// + bounds.X) * scaleFactor;
                        var pixelHeight = (int)(scaleY * bounds.Height);// + bounds.Y) * scaleFactor;
                        var pixelX = (int)(scaleX * bounds.X);
                        var pixelY = (int)(scaleY * bounds.Y);

                        timerSaveBmp.Start();

                        Bitmap bitmap = new Bitmap(pixelWidth * 4, pixelHeight * 4);
                        timerSaveBmp.Stop();

                        timerSvgRender.Restart();
                        var mrenderer = SvgRenderer.FromImage(bitmap);

                        // SizeF s = new SizeF(bounds.Width, bounds.Height);
                        // if (frag.Transforms != null){
                        // mrenderer.ScaleTransform(1, 1);
                        // }
                            //   frag.Transforms?.Add(new SvgTranslate(-bounds.X, -bounds.Y));
                        // mrenderer.SetBoundable(frag);
                        mrenderer.SmoothingMode = SmoothingMode.HighQuality;
                        // newDoc.InvalidateChildPaths();
                        frag.RenderElement(mrenderer);

                        timerSvgRender.Stop();

                        // sl.Add(timerSvgRender.ElapsedTicks, frag.ID);

                        //Give black border around image

                        // for (int x = 0; x < bitmap.Width; x++)
                        // {
                        //     bitmap.SetPixel(x, 0, System.Drawing.Color.Black);
                        //     bitmap.SetPixel(x, bitmap.Height - 1, System.Drawing.Color.Black);
                        // }
                        //
                        // for (int y = 1; y < bitmap.Height; y++)
                        // {
                        //     bitmap.SetPixel(0, y, System.Drawing.Color.Black);
                        //     bitmap.SetPixel(bitmap.Width - 1, y, System.Drawing.Color.Black);
                        // }

                        using (MemoryStream ms = new MemoryStream(bitmap.Width * bitmap.Height))
                        {
                            timerSvgRender.Start();
                            bitmap.Save(ms, ImageFormat.Png);

                            timerSvgRender.Stop();

                            LocationView l = new LocationView(dg)
                            {
                                DisplayRegion = new RectangleF(pixelX, pixelY, bitmap.Width, bitmap.Height),
                                // DisplayRegion = new RectangleF(0, 0, bounds.X * scaleFactor + pixelWidth,bounds.Y * scaleFactor + pixelHeight),
                                Graphic = Texture2D.FromStream(GraphicsDevice, ms),
                                // Graphic = svgToTexture(newDoc, scaleFactor),
                                Id = c.ID
                            };
                            locations.Add(l);
                            Console.WriteLine($"{i}: {c.ID}: {bounds}: ");//{newDoc.GetDimensions()}");

                            // timerSvgRenderHitbox.Start();

                            // Bitmap lhitbox = new Bitmap(pixelWidth, pixelHeight);
                            // mrenderer = SvgRenderer.FromImage(lhitbox);
                            // // mrenderer.ScaleTransform(scaleConstant, scaleConstant);
                            // // mrenderer.SmoothingMode = SmoothingMode.None;

                            // mrenderer.SetBoundable(frag);
                            // mrenderer.ScaleTransform(scaleConstant, scaleConstant);

                            // frag.Stroke = SvgPaintServer.None;
                            // int guid = Math.Abs(Guid.NewGuid().GetHashCode());
                            var color = IntToColor(i);
                            // frag.Color = new SvgColourServer(color);

                            // frag.RenderElement(mrenderer);

                            // timerSvgRenderHitbox.Stop();

                            var noneColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
                            // for (int x = (int)frag.Bounds.X; x < hitboxes.Width && x < lhitbox.Width; x++)

                            ConcurrentBag<int> pixelsSetCounts = new ConcurrentBag<int>();

                            timerCombineHitboxes.Start();

                            Parallel.For(0, Math.Min(hitboxes.Width - pixelX, bitmap.Width), x =>
                            {
                                int pixelsSet = 0;

                                for (int y = 0; y + pixelY < hitboxes.Height && y < bitmap.Height; y++)
                                {
                                    if (bitmap.GetPixel(x, y) != noneColor)
                                    {
                                        if (hitboxes.GetPixel(x+pixelX, y+pixelY) != noneColor)
                                        {
                                            Console.WriteLine("Possible Error!");
                                        }
                                        hitboxes.SetPixel(x+pixelX, y+pixelY, color);
                                        pixelsSet++;
                                    }
                                }

                                pixelsSetCounts.Add(pixelsSet);


                            });

                            int pixelsSet = pixelsSetCounts.Sum();

                            timerCombineHitboxes.Stop();

                            if (pixelsSet == 0)
                            {
                                Console.WriteLine($"i={i}, ID={c.ID}, no pixels set");
                            }
                            else
                            {
                                ;
                            }

                            Console.WriteLine(pixelsSet);
                            _countiresIndex.Add(l);
                            _countriesMap.Add(l.Id, l);

                            System.Console.WriteLine();
                        }


                        // c.Fill = new SvgColourServer(System.Drawing.Color.FromArgb(0xFF, (i / 255 / 255) % 255, (i / 255) % 255,i % 255));
                        // _countiresIndex.Add();

                        i += 1;
                    }
                }
            }

            Console.WriteLine("svgCast " + timerSvgCast.ElapsedMilliseconds);
            Console.WriteLine("svgRender" + timerSvgRender.ElapsedMilliseconds);
            Console.WriteLine("bmp" + timerSaveBmp.ElapsedMilliseconds);
            Console.WriteLine("svgRenderHitboxes" + timerSvgRenderHitbox.ElapsedMilliseconds);
            Console.WriteLine("combineBitmap" + timerCombineHitboxes.ElapsedMilliseconds);
            Console.WriteLine("Overall" + timerOverall.ElapsedMilliseconds);

            foreach (var id in sl)
            {
                System.Console.WriteLine($"{id.Key * 1000.0 / Stopwatch.Frequency:F2}, {id.Value}");
            }

            hitboxes.Save("/home/whomagoo/RiderProjects/MonoGameCross-PlatformDesktopApplication1/MonoGameCross-PlatformDesktopApplication1/Content/map_hitbox.png", ImageFormat.Png);
            mouseReg = hitboxes;

            using (MemoryStream mss = new MemoryStream())
            {
                mouseReg.Save(mss, ImageFormat.Bmp);
                items.Add(Texture2D.FromStream(_graphics.GraphicsDevice, mss));
            }

            // _svgDoc.Transforms.Add(new SvgScale(scaleConstant));



            // mouseReg = new Bitmap(1920, 1080);
            // var renderer = SvgRenderer.FromImage(mouseReg);
            // renderer.ScaleTransform(5f, 5f);
            // renderer.SmoothingMode = SmoothingMode.None;
            // countires.RenderElement(renderer);

            // using (MemoryStream ms = new MemoryStream())
            // {
            // mouseReg.Save(
            // "/home/whomagoo/RiderProjects/MonoGameCross-PlatformDesktopApplication1/MonoGameCross-PlatformDesktopApplication1/Content/map.png");
            // mouseReg.Save(ms, ImageFormat.Png);
            // svg = Texture2D.FromStream(GraphicsDevice, ms);
            // items.Add(svg);
            // }


            // Bitmap bmpMap = _svgDoc.Draw(1920,1080);
            //Bitmap bmp = countires.Draw(1920,1080);

            // items.Add(svgToTexture(_svgDoc, scaleConstant));




            // Bitmap countryLabelsBMP = new Bitmap((int)(imageWidth * scaleConstant), (int)(imageHeight * scaleConstant));
            // var textRenderer = SvgRenderer.FromImage(countryLabelsBMP);

            // textRenderer.ScaleTransform(scaleConstant, scaleConstant);
            // // textRenderer.SetBoundable(text);
            // textRenderer.SmoothingMode = SmoothingMode.HighQuality;
            // text.InvalidateChildPaths();
            // text.RenderElement(textRenderer);

            // MemoryStream textStream = new MemoryStream();

            // countryLabelsBMP.Save(textStream, ImageFormat.Bmp);

            // // svgToTexture(text, scaleConstant);

            // Texture2D text2d = Texture2D.FromStream(_graphics.GraphicsDevice, textStream);

            var countryBorders = _svgDoc.GetElementById("layer2");
            items.Add(RenderLayer(countryBorders, imageWidth, imageHeight, scaleConstant));

            var locationBorders = _svgDoc.GetElementById("layer3");
            items.Add(RenderLayer(locationBorders, imageWidth, imageHeight, scaleConstant));


            var text = _svgDoc.GetElementById("layer5");
            Texture2D text2d = RenderLayer(text, imageWidth, imageHeight, scaleConstant);

            items.Add(text2d);

            // using (MemoryStream ms = new MemoryStream())
            // {
            // bmpMap.Save(ms, ImageFormat.Bmp);
            // svg = Texture2D.FromStream(GraphicsDevice, ms);
            // items.Add(svg);
            // }


            // TODO: use this.Content to load your game content here
        }

        private Texture2D RenderLayer(SvgElement text, float imageWidth, float imageHeight, float scaleConstant)
        {
            Bitmap countryLabelsBMP = new Bitmap((int)(imageWidth * scaleConstant), (int)(imageHeight * scaleConstant));
            var textRenderer = SvgRenderer.FromImage(countryLabelsBMP);

            textRenderer.ScaleTransform(scaleConstant, scaleConstant);
            // textRenderer.SetBoundable(text);
            textRenderer.SmoothingMode = SmoothingMode.HighQuality;
            text.RenderElement(textRenderer);

            using (MemoryStream textStream = new MemoryStream())
            {
                countryLabelsBMP.Save(textStream, ImageFormat.Bmp);

                // svgToTexture(text, scaleConstant);

                Texture2D text2d = Texture2D.FromStream(_graphics.GraphicsDevice, textStream);

                return text2d;
            }
        }

        private int ColorToInt(System.Drawing.Color c)
        {
            return c.R * 255 * 255 + c.G * 255 + c.B - 1;
        }

        private System.Drawing.Color IntToColor(int i)
        {
            i += 1;
            return System.Drawing.Color.FromArgb(255, (i / 255 / 255) % 255, (i / 255) % 255, i % 255);
        }
        private Texture2D svgToTexture(SvgDocument doc, float scale = 1)
        {
            var bmp = doc.Draw((int)(doc.ViewBox.Width * scale), (int)(doc.ViewBox.Height * scale));

            if (doc.ID == "svg5")
            {
                bmp.Save("/home/whomagoo/RiderProjects/MonoGameCross-PlatformDesktopApplication1/MonoGameCross-PlatformDesktopApplication1/Content/map_svgDoc.png");
            }

            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                return Texture2D.FromStream(GraphicsDevice, ms);
            }
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                draw = false;
            }
            else
            {
                draw = true;
            }

            dropDown.Update(gameTime);

            var ms = Mouse.GetState();

            if (IsActive)
            {

                pos = ms.Position;

                try
                {
                    double xRatio = 1.0 * mouseReg.Width / GraphicsDevice.Viewport.Width;
                    double yRatio = 1.0 * mouseReg.Height / GraphicsDevice.Viewport.Height;
                    // var mousing = mouseReg.GetPixel((int) (pos.X * xRatio), (int) (pos.Y * yRatio));

                    GraphicsUnit vvv = GraphicsUnit.Pixel;

                    if (mouseReg.GetBounds(ref vvv).Contains(pos.X, pos.Y))
                    {
                        var mousing = mouseReg.GetPixel(pos.X, pos.Y);
                        // int index = mousing.R * 255 * 255 + mousing.G * 255 + mousing.B;
                        int index = ColorToInt(mousing);

                        if (index != prevIndex)
                        {
                            bool updated = false;
                            int newIndex = index;
                            if (newIndex != prevIndex)
                            {
                                // _svgDoc.GetElementById(_countiresIndex[prevIndex]).Fill = prevColor;
                                if (prevIndex != -1)
                                {
                                    _countiresIndex[prevIndex].IsMoused = false;
                                }

                                if (newIndex != -1)
                                {
                                    _countiresIndex[newIndex].IsMoused = true;
                                }
                                Console.WriteLine($"{prevIndex}->{newIndex}");
                                prevIndex = newIndex;
                            }
                            // if (updated)
                            // {
                            //     var bmp = _svgDoc.Draw(1920, 1080);
                            //     using (MemoryStream mss = new MemoryStream())
                            //     {
                            //         bmp.Save(mss, ImageFormat.Bmp);
                            //         svg = Texture2D.FromStream(GraphicsDevice, mss);
                            //         items[1] = svg;
                            //     }
                            // }
                        }

                        if (ms.LeftButton == ButtonState.Pressed)
                        {

                            if (0 < index && index < _countiresIndex.Count)
                            {
                                LocationView selectedView = _countiresIndex[index];
                                System.Console.WriteLine("Showing text");
                                et.SetExpiringMilliseconds(3000);
                                et.SetText(selectedView.Id, gameTime);

                                var selectedLocation = dg._dg.GetLocation(selectedView.Id);

                                foreach (var location in selectedLocation.Neighbors)
                                {
                                    if (_countriesMap.ContainsKey(location.Name))
                                    {
                                        _countriesMap[location.Name].HighlightMS = 1000;
                                    }
                                    else
                                    {
                                        System.Console.WriteLine($"{selectedView.Id} had a made up neighbor {location.Name}");
                                    }
                                }

                                dropDown.Select(ms.Position);
                            }

                        }
                    }
                }
                catch (Exception _)
                {

                }
            }
            et.Update(gameTime);

            // if (ms.LeftButton == ButtonState.Pressed)
            // {
            //     var nor = _svgDoc.GetElementById<SvgPath>("Norway");
            // }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Clear(Color.SandyBrown);

            _spriteBatch.Begin();
            // TODO: Add your drawing code here

            int i = 0;

            foreach (var l in locations)
            {
                l.Draw(_spriteBatch);
            }

            if (!draw) // gameTime.TotalGameTime == gameTime.ElapsedGameTime)
            {
                _spriteBatch.Draw(items[0], new Vector2(0, 0), Color.White);
                // _spriteBatch.Draw(items[0], new Rectangle(0,0,GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height), Color.White);
            }

            // _spriteBatch.Draw(items[1], new Rectangle(0,0,GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height), Color.White);
            // _spriteBatch.Draw(items[1], new Vector2(0, 0), Color.White);

            for (int j = 1; j < items.Count; j++)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad0 + j) || true)
                {
                    _spriteBatch.Draw(items[j], new Vector2(0, 0), Color.White);
                }
            }

            foreach (var item in items)
            {

            }

            _spriteBatch.DrawString(font, pos.ToString() + "i: " + prevIndex, new Vector2(0f, 0f), Color.Black);
            _spriteBatch.DrawString(font, et, new Vector2(40f, 40f), Color.Black);

            dropDown.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}