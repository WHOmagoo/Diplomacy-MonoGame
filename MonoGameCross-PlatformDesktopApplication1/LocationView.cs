using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

using System;
using DiplomacyEngine.Model;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace MonoGameCross_PlatformDesktopApplication1;

public class LocationView
{
    private static int indext = 0;
    public RectangleF DisplayRegion;
    public Texture2D Graphic;

    private string _id;
    public string Id {
        get { return _id; }
        set { _id = value.Replace('_', ' ');}
    }
    public int index = indext++;

    private List<Color> cs;

    private DiplomacyGame dg;


    public LocationView(DiplomacyGame dg)
    {
        cs = new List<Color>();
        // cs.Add(Color.Red);
        // cs.Add(Color.Orange);
        // cs.Add(Color.Yellow);
        // cs.Add(Color.GreenYellow);
        // cs.Add(Color.DarkGreen);
        // cs.Add(Color.Cyan);
        // cs.Add(Color.DarkBlue);
        // cs.Add(Color.DarkMagenta);
        // cs.Add(Color.HotPink);
        // cs.Add(Color.SaddleBrown);
        // cs.Add(Color.DimGray);
        // cs.Add(Color.White);
        cs.Add(new Color(255, 0, 0, 255));
        cs.Add(new Color(255, 128, 0, 255));
        cs.Add(new Color(255, 255, 0, 255));
        cs.Add(new Color(0, 255, 0, 255));
        cs.Add(new Color(0, 255, 255, 255));
        cs.Add(new Color(0, 0, 255, 255));
        cs.Add(new Color(128, 0, 255, 255));
        cs.Add(new Color(255, 0, 255, 255));
        cs.Add(new Color(255, 192, 192 , 255));
        cs.Add(new Color(192, 255, 192 , 255));
        cs.Add(new Color(192, 192, 255 , 255));
        cs.Add(new Color(255, 255, 255 , 255));

        this.dg = dg;

    }
    public bool IsMoused { get; set; }

    private long _highlightMS = 0;

    public long HighlightMS 
    {
        get {

            if(_highlightMS <= 0){
                return _highlightMS;
            }

            return Math.Max(0, _highlightMS - highlightingExpiration.ElapsedMilliseconds);
        }
        set { 
            _highlightMS = value;

            if(_highlightMS > 0){
                highlightingExpiration = Stopwatch.StartNew();
            }
        } 
    }

    public bool IsHighlighting(){
        long remaining = HighlightMS;

        return remaining != 0;
    }
    private Stopwatch highlightingExpiration;

    public void Draw(SpriteBatch spriteBatch)
    {
        // int color = Math.Abs(Id.GetHashCode());
        
        // Color c = cs[color % cs.Count];

        var rect = new Rectangle(0, 0, (int) DisplayRegion.Width,
            (int) DisplayRegion.Height);

        spriteBatch.Draw(Graphic, new Vector2(DisplayRegion.X, DisplayRegion.Y), rect, GetColor());

        if(IsMoused){
            spriteBatch.Draw(Graphic, new Vector2(DisplayRegion.X, DisplayRegion.Y), rect, new Color(Color.Black, .3F));
        }
    }

    private Color GetColor(){


        if(IsHighlighting()){
            return new Color(Color.Gray, .3F);
        }

        // if(IsMoused){
        //     return new Color(Color.Red, .3f);
        // }

        try {
            var loc = dg._dg.GetLocation(Id);

            if(loc.Type == dg._dg.GetLocationType("Offshore")){
                return Color.LightSeaGreen;
            } else if (loc.Value > 0){
                return Color.SaddleBrown;
            } else {
                return Color.Orange;
            }

        } catch(KeyNotFoundException _){
            return Color.Black;
        }

        // int color = new Random(Id.GetHashCode()).Next(0, 255 * 255 * 255);

        // Color c = new Color((byte) (color), (byte) (color / 255), (byte) (color / 255 / 255));

        // return c;
    }
}