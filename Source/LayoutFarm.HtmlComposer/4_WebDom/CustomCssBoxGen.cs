﻿//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo


using PixelFarm.Drawing;
using LayoutFarm.Css;
using LayoutFarm.HtmlBoxes;
namespace LayoutFarm.Composers
{
    //delegate for create cssbox
    public delegate LayoutFarm.HtmlBoxes.CssBox CreateCssBoxDelegate(
            LayoutFarm.WebDom.IHtmlElement domE,
            LayoutFarm.HtmlBoxes.CssBox parentBox,
            LayoutFarm.Css.BoxSpec spec,
            LayoutFarm.HtmlBoxes.HtmlHost htmlhost);
    //temp !, test only for custom box creation
    static class CustomBoxGenSample1
    {
        internal static LayoutFarm.HtmlBoxes.CssBox CreateCssBox(
            LayoutFarm.WebDom.IHtmlElement domE,
            LayoutFarm.HtmlBoxes.CssBox parentBox,
            LayoutFarm.Css.BoxSpec spec,
            LayoutFarm.HtmlBoxes.HtmlHost htmlhost)
        {
            //create cssbox 
            //test only!           
            var newspec = new BoxSpec();
            BoxSpec.InheritStyles(newspec, spec);
            newspec.BackgroundColor = Color.Blue;
            newspec.Width = new CssLength(50, CssUnitOrNames.Pixels);
            newspec.Height = new CssLength(50, CssUnitOrNames.Pixels);
            newspec.Position = CssPosition.Absolute;
            newspec.Freeze(); //freeze before use
            HtmlElement htmlElement = (HtmlElement)domE;
            var newBox = new CssBox(newspec);
            newBox.SetController(domE);
            htmlElement.SetPrincipalBox(newBox);
            //auto set bc of the element

            parentBox.AppendChild(newBox);
            htmlhost.UpdateChildBoxes(htmlElement, true);
            //----------
            return newBox;
        }
    }
    //------------------------------------------------------------
    public delegate void LazyCssBoxCreator(out RenderElement re, out object controller);
}