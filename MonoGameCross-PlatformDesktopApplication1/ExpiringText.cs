using System;
using Microsoft.Xna.Framework;

public class ExpiringText {

    private string text = "";
    int milliseconds;  

    TimeSpan gt_start;


    public ExpiringText(){

    }

    public ExpiringText(string text, int milliseconds, GameTime gt){
        this.text = text;
        this.milliseconds = milliseconds;
        this.gt_start = gt.TotalGameTime;
    }


    public void SetText(string text, GameTime gt_new){
        if(gt_new != null){
            gt_start = gt_new.TotalGameTime;
        }

        this.text = text;
    }

    public void SetExpiringMilliseconds(int milliseconds){
        this.milliseconds = milliseconds;
    }

    public void Update(GameTime gt_new){
        if (gt_start == null) return;

        if((gt_new.TotalGameTime- gt_start).TotalMilliseconds > milliseconds){
            text = "";
        }
    }

    public string ToString(){
        return text;
    }

    public static implicit operator string(ExpiringText et){
        return et.ToString();
    }
}