﻿// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm
{
    public abstract partial class RootGraphic
    {
        public delegate void PaintToOutputDelegate();


        Rectangle flushRect;
        Rectangle accumulateInvalidRect;
        bool hasAccumRect;

        public RootGraphic(int width, int heigth)
        {
            this.Width = width;
            this.Height = heigth;
        }

        public abstract GraphicsPlatform P { get; }

        public IFonts SampleIFonts { get { return this.P.SampleIFonts; } }

        public abstract void CaretStartBlink();
        public abstract void CaretStopBlink();
        public abstract void ClearRenderRequests();

        public abstract void AddToLayoutQueue(RenderElement renderElement);

        public int GraphicUpdateBlockCount
        {
            get;
            set;
        }
        internal int Width
        {
            get;
            set;
        }
        internal int Height
        {
            get;
            set;
        }

        public bool DisableGraphicOutputFlush
        {
            get;
            set;
        }
        public bool LayoutQueueClearing
        {
            get;
            set;
        }

        public bool IsInRenderPhase
        {
            get;
            set;
        }
        public abstract void CloseWinRoot();
        public void BeginGraphicUpdate()
        {
            GraphicUpdateBlockCount++;
            DisableGraphicOutputFlush = true;
        }
        public void EndGraphicUpdate()
        {
            GraphicUpdateBlockCount--;
            if (GraphicUpdateBlockCount <= 0)
            {
                DisableGraphicOutputFlush = false;
                FlushAccumGraphicUpdate();
                GraphicUpdateBlockCount = 0;
            }
        }

        public abstract void ForcePaint();
        protected PaintToOutputDelegate paintToOutputHandler;
        CanvasInvalidateRequestDelegate canvasInvaliddateReqDel;

        public void SetPaintToOutputHandler(PaintToOutputDelegate paintToOutputHandler)
        {
            this.paintToOutputHandler = paintToOutputHandler;
        }

        public abstract GraphicsTimerTask RequestGraphicsIntervalTask(
            object uniqueName,
            TaskIntervalPlan planName,
            int intervalMs,
            EventHandler<GraphicsTimerTaskEventArgs> tickhandler);

        public abstract void RemoveIntervalTask(object uniqueName);


#if DEBUG
        RootGraphic dbugVRoot
        {
            get { return this; }
        }
        bool dbugNeedContentArrangement
        {
            get;
            set;
        }
        bool dbugNeedReCalculateContentSize
        {
            get;
            set;
        }
#endif
        public abstract void PrepareRender();
        public void FlushAccumGraphicUpdate()
        {
            if (hasAccumRect)
            {
                this.FlushGrapchics(accumulateInvalidRect);

                hasAccumRect = false;
            }
            this.GraphicUpdateBlockCount = 0;
        }

        public void SetCanvasInvalidateRequest(CanvasInvalidateRequestDelegate canvasInvaliddateReqDel)
        {
            this.canvasInvaliddateReqDel = canvasInvaliddateReqDel;

        }
        void FlushGrapchics(Rectangle rect)
        {
            this.canvasInvaliddateReqDel(ref rect);
        }

        public void InvalidateGraphicArea(RenderElement fromElement,ref Rectangle elementClientRect)
        {
            if (this.IsInRenderPhase) { return; }             
            //--------------------------------------

            int globalX = 0;
            int globalY = 0;
            bool isBubbleUp = false;

            RenderElement startVisualElement = fromElement;
#if DEBUG

            RootGraphic dbugMyroot = this.dbugVRoot;
            if (dbugMyroot.dbugEnableGraphicInvalidateTrace && dbugMyroot.dbugGraphicInvalidateTracer != null)
            {

                dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(">> :" + elementClientRect.ToString(),
                    startVisualElement);
            }
            int dbug_ncount = 0;

#endif


            do
            {
#if DEBUG

                if (!fromElement.Visible)
                {

                    if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                        dbugMyroot.dbugGraphicInvalidateTracer != null)
                    {

                        string state_str = "EARLY-RET: ";
                        if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                        {
                            state_str = "!!" + state_str;
                        }
                        dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(state_str, fromElement);
                        while (dbug_ncount > 0)
                        {
                            dbugMyroot.dbugGraphicInvalidateTracer.PopElement();
                            dbug_ncount--;
                        }
                    }
                    return;
                }
                else if (fromElement.IsInvalidateGraphicBlocked)
                {
                    if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                            dbugMyroot.dbugGraphicInvalidateTracer != null)
                    {
                        string state_str = "BLOCKED2: ";
                        if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                        {
                            state_str = "!!" + state_str;
                        }
                        dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(state_str, fromElement); while (dbug_ncount > 0)
                        {
                            dbugMyroot.dbugGraphicInvalidateTracer.PopElement();
                            dbug_ncount--;
                        }
                    }
                    return;
                }
#else 
                if (!fromElement.Visible || fromElement.IsInvalidateGraphicBlocked)
                {
                    return;
                }
#endif


#if DEBUG
                if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                    dbugMyroot.dbugGraphicInvalidateTracer != null)
                {
                    dbug_ncount++;
                    dbugMyroot.dbugGraphicInvalidateTracer.PushVisualElement(fromElement);
                    string state_str = ">> ";
                    if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                    {
                        state_str = "!!" + state_str;
                    }
                    dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(state_str, fromElement);
                }
#endif


                globalX += fromElement.BubbleUpX;
                globalY += fromElement.BubbleUpY;


                if (fromElement.MayHasViewport && isBubbleUp)
                {
                    elementClientRect.Offset(globalX, globalY);
                    if (fromElement.HasDoubleScrollableSurface)
                    {
                        //container.VisualScrollableSurface.WindowRootNotifyInvalidArea(elementClientRect);
                    }
                    Rectangle elementRect = fromElement.RectBounds;
                    elementRect.Offset(fromElement.ViewportX, fromElement.ViewportY);
                    elementClientRect.Intersect(elementRect);
                    globalX = -fromElement.ViewportX;
                    globalY = -fromElement.ViewportY;
                }

                if (fromElement.IsTopWindow)
                {

                    break;
                }
                else
                {

#if DEBUG
                    if (fromElement.dbugParentVisualElement == null)
                    {
                        if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                            dbugMyroot.dbugGraphicInvalidateTracer != null)
                        {
                            string state_str = "BLOCKED3: ";
                            if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                            {
                                state_str = "!!" + state_str;
                            }
                            dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(state_str, fromElement); while (dbug_ncount > 0)
                            {
                                dbugMyroot.dbugGraphicInvalidateTracer.PopElement();
                                dbug_ncount--;
                            }
                        }
                    }
#endif

                    fromElement = fromElement.ParentRenderElement;
                    if (fromElement == null)
                    {
                        return;
                    }
                }

                isBubbleUp = true;

            } while (true);

#if DEBUG
            if (dbugMyroot.dbugEnableGraphicInvalidateTrace
             && dbugMyroot.dbugGraphicInvalidateTracer != null)
            {
                while (dbug_ncount > 0)
                {
                    dbugMyroot.dbugGraphicInvalidateTracer.PopElement();
                    dbug_ncount--;
                }
            }
#endif

            //----------------------------------------
            elementClientRect.Offset(globalX, globalY);
            Rectangle rootGlobalArea = elementClientRect;


            if (elementClientRect.Top > this.Height
                || elementClientRect.Left > this.Width
                || elementClientRect.Bottom < 0
                || elementClientRect.Right < 0)
            {
#if DEBUG
                if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                    dbugMyroot.dbugGraphicInvalidateTracer != null)
                {
                    dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("ZERO-EEX");
                    dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("\r\n");
                }
#endif
                return;
            }



            if (!this.DisableGraphicOutputFlush)
            {
                if (hasAccumRect)
                {

                    accumulateInvalidRect = Rectangle.Union(accumulateInvalidRect, rootGlobalArea);
#if DEBUG
                    if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                        dbugMyroot.dbugGraphicInvalidateTracer != null)
                    {
                        string state_str = "SUDDEN_1: ";
                        if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                        {
                            state_str = "!!" + state_str;
                        }
                        dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(state_str + accumulateInvalidRect);
                        dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("\r\n");
                    }
#endif



                    this.FlushGrapchics(rootGlobalArea);
                    this.flushRect = accumulateInvalidRect;


                    hasAccumRect = false;
                }
                else
                {

                    this.FlushGrapchics(rootGlobalArea);


#if DEBUG
                    if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                    dbugMyroot.dbugGraphicInvalidateTracer != null)
                    {

                        string state_str = "SUDDEN_2: ";
                        if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                        {
                            state_str = "!!" + state_str;
                        }
                        dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(state_str +
                               rootGlobalArea.ToString() + " " +
                               startVisualElement.dbug_FullElementDescription());
                        dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("\r\n");
                    }
#endif
                }
            }
            else
            {
                if (!hasAccumRect)
                {
                    accumulateInvalidRect = rootGlobalArea;
                    hasAccumRect = true;
                }
                else
                {
                    accumulateInvalidRect = Rectangle.Union(accumulateInvalidRect, rootGlobalArea);
                }


#if DEBUG
                if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                    dbugMyroot.dbugGraphicInvalidateTracer != null)
                {
                    string state_str = "ACC: ";
                    if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                    {
                        state_str = "!!" + state_str;
                    }
                    dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("ACC: " + accumulateInvalidRect.ToString());
                    dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("\r\n");
                }
#endif
            }
        }

        public abstract TopWindowRenderBox TopWindowRenderBox
        {
            get;
            protected set;
        }

    }
}