﻿// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("4.2 MixHtml and Text")]
    class Demo_MixHtml : DemoBase
    {
        HtmlBoxes.HtmlIslandHost islandHost;
        HtmlBoxes.HtmlIslandHost GetIslandHost(SampleViewport viewport)
        {
            if (islandHost == null)
            {
                islandHost = new HtmlBoxes.HtmlIslandHost(viewport.P);
                 
                //islandHost.RequestResource += myHtmlIsland_RequestResource;
            }
            return islandHost;
        }
        protected override void OnStartDemo(SampleViewport viewport)
        {

            var htmlIslandHost = GetIslandHost(viewport);
            ////==================================================
            //html box
            HtmlBox htmlBox = new HtmlBox(htmlIslandHost, 800, 400);
            viewport.AddContent(htmlBox);
            string html = @"<html><head></head><body><div>OK1</div><div>OK2</div></body></html>";
            htmlBox.LoadHtmlText(html);
            //================================================== 

            //textbox
            var textbox = new LayoutFarm.CustomWidgets.TextBox(400, 100, true);
            textbox.SetLocation(0, 200);
            viewport.AddContent(textbox);
            textbox.Focus();

        }

    }

}