﻿//MIT, 2014-present, WinterDev
using LayoutFarm.UI;
namespace LayoutFarm.Demo
{
    class Demo02_CreateHtmlDomStyle2 : DemoBase
    {
        public Demo02_CreateHtmlDomStyle2()
        {
        }
        protected override void OnStartDemo(HtmlPanel panel)
        {
            var htmldoc = panel.HtmlHost.CreatePresentationHtmlDoc();
            var rootNode = htmldoc.RootNode;
            //1. create body node             
            // and content  

            //style 2, lambda and adhoc attach event
            rootNode.AddChild("body", body =>
            {
                body.AddChild("div", div =>
                {
                    div.AddChild("span", span =>
                    {
                        span.AddTextContent("ABCD");
                        //3. attach event to specific span
                        span.AttachEvent(UIEventName.MouseDown, e =>
                        {
                            //-------------------------------
                            //mousedown on specific span !
                            //-------------------------------
#if DEBUG
                            // System.Diagnostics.Debugger.Break();
                            //Console.WriteLine("span");
#endif                                              

                            //test stop propagation 
                            e.StopPropagation();
                        });
                    });
                    //----------------------
                    div.AttachEvent(UIEventName.MouseDown, e =>
                    {
#if DEBUG
                        //this will not print 
                        //if e has been stop by its child
                        // System.Diagnostics.Debugger.Break();
                        //Console.WriteLine("div");
#endif

                    });
                });
            });
            //2. add to view 
            panel.LoadHtmlDom(htmldoc,
               LayoutFarm.Composers.CssDefaults.DefaultStyleSheet);
        }
    }
}