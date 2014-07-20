﻿//BSD 2014, WinterDev
//ArthurHub

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using System.Drawing;
using HtmlRenderer.Drawing;
using HtmlRenderer.Boxes;
using HtmlRenderer.Diagnostics;
using HtmlRenderer.Css;


namespace HtmlRenderer
{

    /// <summary>
    /// Low level handling of Html Renderer logic.<br/>
    /// Allows html layout and rendering without association to actual control, those allowing to handle html rendering on any graphics object.<br/>
    /// Using this class will require the client to handle all propagations of mouse/keyboard events, layout/paint calls, scrolling offset, 
    /// location/size/rectangle handling and UI refresh requests.<br/>
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>MaxSize and ActualSize:</b><br/>
    /// The max width and height of the rendered html.<br/>
    /// The max width will effect the html layout wrapping lines, resize images and tables where possible.<br/>
    /// The max height does NOT effect layout, but will not render outside it (clip).<br/>
    /// <see cref="ActualSize"/> can exceed the max size by layout restrictions (unwrap-able line, set image size, etc.).<br/>
    /// Set zero for unlimited (width/height separately).<br/>
    /// </para>
    /// <para>
    /// <b>ScrollOffset:</b><br/>
    /// This will adjust the rendered html by the given offset so the content will be "scrolled".<br/>
    /// Element that is rendered at location (50,100) with offset of (0,200) will not be rendered 
    /// at -100, therefore outside the client rectangle.
    /// </para> 

    /// <para>
    /// <b>Refresh event:</b><br/>
    /// Raised when html renderer requires refresh of the control hosting (invalidation and re-layout).<br/>
    /// There is no guarantee that the event will be raised on the main thread, it can be raised on thread-pool thread.
    /// </para>
    /// <para>
    /// <b>RenderError event:</b><br/>
    /// Raised when an error occurred during html rendering.<br/>
    /// </para>
    /// </remarks>
    public abstract class HtmlContainer : IDisposable
    {


        #region Fields and Consts
        /// <summary>
        /// the root css box of the parsed html
        /// </summary>
        private CssBox _rootBox;

        /// <summary>
        /// the text fore color use for selected text
        /// </summary>
        private Color _selectionForeColor;

        /// <summary>
        /// the backcolor to use for selected text
        /// </summary>
        private Color _selectionBackColor;

        /// <summary>
        /// Gets or sets a value indicating if antialiasing should be avoided 
        /// for geometry like backgrounds and borders
        /// </summary>
        private bool _avoidGeometryAntialias;

        /// <summary>
        /// Gets or sets a value indicating if image asynchronous loading should be avoided (default - false).<br/>
        /// </summary>
        private bool _avoidAsyncImagesLoading;

        /// <summary>
        /// Gets or sets a value indicating if image loading only when visible should be avoided (default - false).<br/>
        /// </summary>
        private bool _avoidImagesLateLoading;

        /// <summary>
        /// Use GDI+ text rendering to measure/draw text.
        /// </summary>
        private bool _useGdiPlusTextRendering;

        /// <summary>
        /// the top-left most location of the rendered html
        /// </summary>
        private PointF _location;

        /// <summary>
        /// the max width and height of the rendered html, effects layout, actual size cannot exceed this values.<br/>
        /// Set zero for unlimited.<br/>
        /// </summary>
        private SizeF _maxSize;

        /// <summary>
        /// Gets or sets the scroll offset of the document for scroll controls
        /// </summary>
        private PointF _scrollOffset;

        /// <summary>
        /// The actual size of the rendered html (after layout)
        /// </summary>

        float _actualWidth;
        float _actualHeight;
        #endregion
        /// <summary>
        /// 99999
        /// </summary>
        const int MAX_WIDTH = 99999;
        //-----------------------------------------------------------
        //controll task of this container
        System.Timers.Timer timTask = new System.Timers.Timer();
        List<ImageBinder> requestImageBinderUpdates = new List<ImageBinder>();
        //-----------------------------------------------------------

        public HtmlContainer()
        {
            timTask.Interval = 20;//20 ms task
            timTask.Elapsed += new System.Timers.ElapsedEventHandler(timTask_Elapsed);
            timTask.Enabled = true;
        }

#if DEBUG
        static int dd = 0;
#endif

        void timTask_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (requestImageBinderUpdates.Count > 0)
            {
                requestImageBinderUpdates.Clear();
                this.RequestRefresh(false);
#if DEBUG
                dd++;
                //Console.WriteLine(dd);
#endif
            }
        }
        public void AddRequestImageBinderUpdate(ImageBinder binder)
        {
            this.requestImageBinderUpdates.Add(binder);
        }

        /// <summary>
        /// Gets or sets a value indicating if anti-aliasing should be avoided for geometry like backgrounds and borders (default - false).
        /// </summary>
        public bool AvoidGeometryAntialias
        {
            get { return _avoidGeometryAntialias; }
            set { _avoidGeometryAntialias = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if image asynchronous loading should be avoided (default - false).<br/>
        /// True - images are loaded synchronously during html parsing.<br/>
        /// False - images are loaded asynchronously to html parsing when downloaded from URL or loaded from disk.<br/>
        /// </summary>
        /// <remarks>
        /// Asynchronously image loading allows to unblock html rendering while image is downloaded or loaded from disk using IO 
        /// ports to achieve better performance.<br/>
        /// Asynchronously image loading should be avoided when the full html content must be available during render, like render to image.
        /// </remarks>
        public bool AvoidAsyncImagesLoading
        {
            get { return _avoidAsyncImagesLoading; }
            set { _avoidAsyncImagesLoading = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if image loading only when visible should be avoided (default - false).<br/>
        /// True - images are loaded as soon as the html is parsed.<br/>
        /// False - images that are not visible because of scroll location are not loaded until they are scrolled to.
        /// </summary>
        /// <remarks>
        /// Images late loading improve performance if the page contains image outside the visible scroll area, especially if there is large 
        /// amount of images, as all image loading is delayed (downloading and loading into memory).<br/>
        /// Late image loading may effect the layout and actual size as image without set size will not have actual size until they are loaded
        /// resulting in layout change during user scroll.<br/>
        /// Early image loading may also effect the layout if image without known size above the current scroll location are loaded as they
        /// will push the html elements down.
        /// </remarks>
        public bool AvoidImagesLateLoading
        {
            get { return _avoidImagesLateLoading; }
            set { _avoidImagesLateLoading = value; }
        }

        /// <summary>
        /// Use GDI+ text rendering to measure/draw text.<br/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// GDI+ text rendering is less smooth than GDI text rendering but it natively supports alpha channel
        /// thus allows creating transparent images.
        /// </para>
        /// <para>
        /// While using GDI+ text rendering you can control the text rendering using <see cref="Graphics.TextRenderingHint"/>, note that
        /// using <see cref="TextRenderingHint.ClearTypeGridFit"/> doesn't work well with transparent background.
        /// </para>
        /// </remarks>
        public bool UseGdiPlusTextRendering
        {
            get { return _useGdiPlusTextRendering; }
            set
            {
                if (_useGdiPlusTextRendering != value)
                {
                    _useGdiPlusTextRendering = value;
                    RequestRefresh(true);
                }
            }
        }


        /// <summary>
        /// The scroll offset of the html.<br/>
        /// This will adjust the rendered html by the given offset so the content will be "scrolled".<br/>
        /// </summary>
        /// <example>
        /// Element that is rendered at location (50,100) with offset of (0,200) will not be rendered as it
        /// will be at -100 therefore outside the client rectangle.
        /// </example>
        public PointF ScrollOffset
        {
            get { return _scrollOffset; }
            set { _scrollOffset = value; }
        }

        /// <summary>
        /// The top-left most location of the rendered html.<br/>
        /// This will offset the top-left corner of the rendered html.
        /// </summary>
        public PointF Location
        {
            get { return _location; }
            set { _location = value; }
        }

        /// <summary>
        /// The max width and height of the rendered html.<br/>
        /// The max width will effect the html layout wrapping lines, resize images and tables where possible.<br/>
        /// The max height does NOT effect layout, but will not render outside it (clip).<br/>
        /// <see cref="ActualSize"/> can be exceed the max size by layout restrictions (unwrappable line, set image size, etc.).<br/>
        /// Set zero for unlimited (width\height separately).<br/>
        /// </summary>
        public SizeF MaxSize
        {
            get { return _maxSize; }
            set { _maxSize = value; }
        }

        /// <summary>
        /// The actual size of the rendered html (after layout)
        /// </summary>
        public SizeF ActualSize
        {
            get { return new SizeF(this._actualWidth, this._actualHeight); }
        }
        internal void UpdateSizeIfWiderOrHeigher(float newWidth, float newHeight)
        {
            if (newWidth > this._actualWidth)
            {
                this._actualWidth = newWidth;
            }
            if (newHeight > this._actualHeight)
            {
                this._actualHeight = newHeight;
            }
        }


        /// <summary>
        /// the text fore color use for selected text
        /// </summary>
        public Color SelectionForeColor
        {
            get { return _selectionForeColor; }
            set { _selectionForeColor = value; }
        }

        /// <summary>
        /// the back-color to use for selected text
        /// </summary>
        public Color SelectionBackColor
        {
            get { return _selectionBackColor; }
            set { _selectionBackColor = value; }
        }
        public CssBox GetRootCssBox()
        {
            return this._rootBox;
        }
        public void SetRootCssBox(CssBox rootBox)
        {
            if (_rootBox != null)
            {
                _rootBox = null;
                //---------------------------
                this.OnRootDisposed();
            }
            _rootBox = rootBox;
            if (rootBox != null)
            {
                this.OnRootCreated(_rootBox);
            }
        }

        protected virtual void OnRootDisposed()
        {

        }
        protected virtual void OnRootCreated(CssBox root)
        {
        }
        protected virtual void OnAllDisposed()
        {
        }
         


        public void PerformLayout(IGraphics ig)
        {

            if (this._rootBox == null)
            {
                return;
            }
            //----------------------- 
            _actualWidth = _actualHeight = 0;
            // if width is not restricted we set it to large value to get the actual later    
            _rootBox.SetLocation(_location.X, _location.Y);
            _rootBox.SetSize(_maxSize.Width > 0 ? _maxSize.Width : MAX_WIDTH, 0);

            CssBox.ValidateComputeValues(_rootBox);

            LayoutVisitor layoutArgs = new LayoutVisitor(ig, this);
            layoutArgs.PushContaingBlock(_rootBox);

            _rootBox.PerformLayout(layoutArgs);

            if (_maxSize.Width <= 0.1)
            {
                // in case the width is not restricted we need to double layout, first will find the width so second can layout by it (center alignment)

                _rootBox.SetWidth((int)Math.Ceiling(this._actualWidth));
                _actualWidth = _actualHeight = 0;
                _rootBox.PerformLayout(layoutArgs);
            }

            layoutArgs.PopContainingBlock();

        }
        public RectangleF PhysicalViewportBound
        {
            get;
            set;
        }

        /// <summary>
        /// Render the html using the given device.
        /// </summary>
        /// <param name="g"></param>
        public void PerformPaint(IGraphics ig)
        {
            if (_rootBox == null)
            {
                return;
            }



            PaintVisitor args = new PaintVisitor(this, ig);
            float scX = this.ScrollOffset.X;
            float scY = this.ScrollOffset.Y;

            var physicalViewportSize = this.PhysicalViewportBound.Size;

            float ox = ig.CanvasOriginX;
            float oy = ig.CanvasOriginY;

            ig.SetCanvasOrigin(scX, scY);

            args.PushContaingBlock(_rootBox);
            args.SetPhysicalViewportBound(0, 0, physicalViewportSize.Width, physicalViewportSize.Height);


            _rootBox.Paint(ig, args);


            args.PopContainingBlock();

            ig.SetCanvasOrigin(ox, oy);
        }


        
        //------------------------------------------------------------------
        protected abstract void OnRequestImage(ImageBinder binder,
            CssBox requestBox, bool _sync);
        internal static void RaiseRequestImage(HtmlContainer container,
            ImageBinder binder,
            CssBox requestBox,
            bool _sync)
        {
            container.OnRequestImage(binder, requestBox, false);
        }
        //------------------------------------------------------------------

        protected abstract void RequestRefresh(bool layout);
        public abstract IGraphics GetSampleGraphics();



        /// <summary>
        /// Report error in html render process.
        /// </summary>
        /// <param name="type">the type of error to report</param>
        /// <param name="message">the error message</param>
        /// <param name="exception">optional: the exception that occured</param>
        public void ReportError(HtmlRenderErrorType type, string message, Exception exception = null)
        {
            //try
            //{
            //    if (RenderError != null)
            //    {
            //        RenderError(this, new HtmlRenderErrorEventArgs(type, message, exception));
            //    }
            //}
            //catch
            //{
            //}
        }



        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }


        #region Private methods

        /// <summary>
        /// Adjust the offset of the given location by the current scroll offset.
        /// </summary>
        /// <param name="location">the location to adjust</param>
        /// <returns>the adjusted location</returns>
        protected Point OffsetByScroll(Point location)
        {
            location.Offset(-(int)ScrollOffset.X, -(int)ScrollOffset.Y);
            return location;
        }

        /// <summary>
        /// Check if the mouse is currently on the html container.<br/>
        /// Relevant if the html container is not filled in the hosted control (location is not zero and the size is not the full size of the control).
        /// </summary>
        protected bool IsMouseInContainer(Point location)
        {
            return location.X >= _location.X &&
                location.X <= _location.X + _actualWidth &&
                location.Y >= _location.Y + ScrollOffset.Y &&
                location.Y <= _location.Y + ScrollOffset.Y + _actualHeight;
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        private void Dispose(bool all)
        {
            try
            {

                if (all)
                {
                    this.OnAllDisposed();

                    //RenderError = null;
                    //StylesheetLoadingRequest = null;
                    //ImageLoadingRequest = null;
                }


                if (_rootBox != null)
                {

                    _rootBox = null;
                    this.OnRootDisposed();
                }


                //if (_selectionHandler != null)
                //    _selectionHandler.Dispose();
                //_selectionHandler = null;
            }
            catch
            { }
        }

        #endregion
    }
}