﻿//Apache2, 2014-present, WinterDev

using System;
using System.Text;
using LayoutFarm.Composers;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class HtmlBox : AbstractRectUI, IEventPortal
    {
        WaitingContentKind waitingContentKind;
        string waitingHtmlString;
        HtmlDocument waitingHtmlDoc;
        enum WaitingContentKind : byte
        {
            NoWaitingContent,
            HtmlFragmentString,
            HtmlString,
            HtmlDocument
        }

        HtmlHost htmlhost;
        MyHtmlContainer myHtmlCont;
        //presentation
        HtmlRenderBox htmlRenderBox;
        HtmlInputEventAdapter inputEventAdapter;
        public HtmlBox(HtmlHost htmlHost, int width, int height)
            : base(width, height)
        {
            this.htmlhost = htmlHost;
        }
        internal HtmlHost HtmlHost
        {
            get { return this.htmlhost; }
        }

        protected override void OnContentLayout()
        {
            this.PerformContentLayout();
        }
        public override void PerformContentLayout()
        {
            this.RaiseLayoutFinished();
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.htmlRenderBox; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.htmlRenderBox != null; }
        }

        HtmlInputEventAdapter GetInputEventAdapter()
        {
            if (inputEventAdapter == null)
            {
                inputEventAdapter = this.htmlhost.GetNewInputEventAdapter();
                inputEventAdapter.Bind(myHtmlCont);
            }
            return inputEventAdapter;
        }

        void IEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {
            e.CurrentContextElement = this;
            GetInputEventAdapter().MouseUp(e, htmlRenderBox.CssBox);
        }
        void IEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
            this.Focus();
            e.CurrentContextElement = this;
            GetInputEventAdapter().MouseDown(e, htmlRenderBox.CssBox);
        }
        void IEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {
            e.CurrentContextElement = this;
            GetInputEventAdapter().MouseMove(e, htmlRenderBox.CssBox);
        }
        void IEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {
            e.CurrentContextElement = this;
        }
        void IEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            e.CurrentContextElement = this;
            GetInputEventAdapter().KeyDown(e, htmlRenderBox.CssBox);
        }
        void IEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            e.CurrentContextElement = this;
            GetInputEventAdapter().KeyPress(e, htmlRenderBox.CssBox);
        }
        void IEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            e.CurrentContextElement = this;
            GetInputEventAdapter().KeyUp(e, htmlRenderBox.CssBox);
        }
        bool IEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            e.CurrentContextElement = this;
            var result = GetInputEventAdapter().ProcessDialogKey(e, htmlRenderBox.CssBox);
            return result;
        }
        void IEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {
            e.CurrentContextElement = this;
        }
        void IEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {
            e.CurrentContextElement = this;
        }
        protected override void OnKeyUp(UIKeyEventArgs e)
        {
            if (e.Ctrl)
            {
                switch (e.KeyCode)
                {
                    case UIKeys.C:
                        {
                            //ctrl+ c => copy to clipboard
                            StringBuilder stbuilder = new StringBuilder();
                            this.myHtmlCont.CopySelection(stbuilder);
                            LayoutFarm.UI.Clipboard.SetText(stbuilder.ToString());
                        }
                        break;
                }
            }

            e.CurrentContextElement = this;
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (htmlRenderBox == null)
            {
                var newFrRenderBox = new HtmlRenderBox(rootgfx, this.Width, this.Height);
                newFrRenderBox.SetController(this);
                newFrRenderBox.HasSpecificSize = true;
                newFrRenderBox.SetLocation(this.Left, this.Top);
                //set to this field if ready
                this.htmlRenderBox = newFrRenderBox;
            }
            switch (this.waitingContentKind)
            {
                default:
                case WaitingContentKind.NoWaitingContent:
                    break;
                case WaitingContentKind.HtmlDocument:
                    {
                        LoadHtmlDom(this.waitingHtmlDoc);
                    }
                    break;
                case WaitingContentKind.HtmlFragmentString:
                    {
                        LoadHtmlFragmentString(this.waitingHtmlString);
                    }
                    break;
                case WaitingContentKind.HtmlString:
                    {
                        LoadHtmlString(this.waitingHtmlString);
                    }
                    break;
            }

            return htmlRenderBox;
        }
        //-----------------------------------------------------------------------------------------------------
        void ClearWaitingContent()
        {
            this.waitingHtmlDoc = null;
            this.waitingHtmlString = null;
            waitingContentKind = WaitingContentKind.NoWaitingContent;
        }
        public void LoadHtmlDom(HtmlDocument htmldoc)
        {
            if (htmlRenderBox == null)
            {
                this.waitingContentKind = WaitingContentKind.HtmlDocument;
                this.waitingHtmlDoc = htmldoc;
            }
            else
            {
                //just parse content and load 
                this.myHtmlCont = HtmlContainerHelper.CreateHtmlContainer(this.htmlhost, htmldoc, htmlRenderBox);
                SetHtmlContainerEventHandlers();
                ClearWaitingContent();
                RaiseLayoutFinished();
            }
        }
        public void LoadHtmlString(string htmlString)
        {
            if (htmlRenderBox == null)
            {
                this.waitingContentKind = WaitingContentKind.HtmlString;
                this.waitingHtmlString = htmlString;
            }
            else
            {
                //just parse content and load 
                this.myHtmlCont = HtmlContainerHelper.CreateHtmlContainerFromFullHtml(this.htmlhost, htmlString, htmlRenderBox);
                SetHtmlContainerEventHandlers();
                ClearWaitingContent();
            }
        }
        public void LoadHtmlFragmentString(string fragmentHtmlString)
        {
            if (htmlRenderBox == null)
            {
                this.waitingContentKind = WaitingContentKind.HtmlFragmentString;
                this.waitingHtmlString = fragmentHtmlString;
            }
            else
            {
                //just parse content and load 
                this.myHtmlCont = HtmlContainerHelper.CreateHtmlContainerFromFragmentHtml(this.htmlhost, fragmentHtmlString, htmlRenderBox);
                SetHtmlContainerEventHandlers();
                ClearWaitingContent();
            }
        }


        void SetHtmlContainerEventHandlers()
        {
            myHtmlCont.AttachEssentialHandlers(
                //1.
                (s, e) => this.InvalidateGraphics(),
                //2.
                (s, e) =>
                {
                    //---------------------------
                    if (htmlRenderBox == null) return;
                    //--------------------------- 
                    htmlhost.GetRenderTreeBuilder().RefreshCssTree(myHtmlCont.RootElement);
                    var lay = this.htmlhost.GetSharedHtmlLayoutVisitor(myHtmlCont);
                    myHtmlCont.PerformLayout(lay);
                    this.htmlhost.ReleaseHtmlLayoutVisitor(lay);
                },
                //3.
                (s, e) => this.InvalidateGraphics(),
                //4
                (s, e) => { this.RaiseLayoutFinished(); });
        }
        public MyHtmlContainer HtmlContainer
        {
            get { return this.myHtmlCont; }
        }
        public override void SetViewport(int x, int y, object reqBy)
        {
            base.SetViewport(x, y, reqBy);
            if (htmlRenderBox != null)
            {
                htmlRenderBox.SetViewport(x, y);
            }
        }
        public override int InnerWidth
        {
            get
            {
                return this.htmlRenderBox.HtmlWidth;
            }
        }
        public override int InnerHeight
        {
            get
            {
                return this.htmlRenderBox.HtmlHeight;
            }
        }
         
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "htmlbox");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}





