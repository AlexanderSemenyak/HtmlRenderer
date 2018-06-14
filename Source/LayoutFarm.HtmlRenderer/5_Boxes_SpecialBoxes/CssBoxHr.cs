﻿//BSD, 2014
//ArthurHub, Jose Manuel Menendez Poo
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

//MIT, 2018, WinterDev
using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    /// <summary>
    /// CSS box for hr element.
    /// </summary>
    public sealed class CssBoxHr : CssBox
    {
        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="parent">the parent box of this box</param>
        /// <param name="controller">the html tag data of this box</param>
        public CssBoxHr(Css.BoxSpec spec, IRootGraphics rootgfx)
            : base(spec, rootgfx)
        {
            ChangeDisplayType(this, Css.CssDisplay.Block);
        }

        /// <summary>
        /// Measures the bounds of box and children, recursively.<br/>
        /// Performs layout of the DOM structure creating lines by set bounds restrictions.
        /// </summary>
        /// <param name="g">Device context to use</param>
        protected override void PerformContentLayout(LayoutVisitor lay)
        {
            if (this.CssDisplay == Css.CssDisplay.None)
            {
                return;
            }
            var prevSibling = lay.LatestSiblingBox;
            var myContainingBlock = lay.LatestContainingBlock;
            if (this.NeedComputedValueEvaluation)
            {
                this.ReEvaluateComputedValues(lay.SampleIFonts, myContainingBlock);
            }

            float localLeft = myContainingBlock.GetClientLeft() + this.ActualMarginLeft;
            float localTop = 0;
            if (prevSibling == null)
            {
                if (this.ParentBox != null)
                {
                    localTop = myContainingBlock.GetClientTop();
                }
            }
            else
            {
                localTop = prevSibling.LocalVisualBottom;// +prevSibling.ActualBorderBottomWidth;
            }

            float maringTopCollapse = UpdateMarginTopCollapse(prevSibling);
            if (maringTopCollapse < 0.1)
            {
                maringTopCollapse = this.GetEmHeight() * 1.1f;
            }
            localTop += maringTopCollapse;
            this.SetLocation(localLeft, localTop);
            this.SetHeightToZero();
            //width at 100% (or auto)
            float minwidth = CalculateMinimumWidth(lay.EpisodeId);
            float width = myContainingBlock.VisualWidth
                          - myContainingBlock.ActualPaddingLeft - myContainingBlock.ActualPaddingRight
                          - myContainingBlock.ActualBorderLeftWidth - myContainingBlock.ActualBorderRightWidth
                          - ActualMarginLeft - ActualMarginRight - ActualBorderLeftWidth - ActualBorderRightWidth;
            //Check width if not auto
            if (!this.Width.IsEmptyOrAuto)
            {
                width = CssValueParser.ConvertToPx(Width, width, this);
            }


            if (width < minwidth || width >= CssBoxConstConfig.TABLE_MAX_WIDTH)
            {
                width = minwidth;
            }

            float height = ExpectedHeight;
            if (height < 1)
            {
                height = this.VisualHeight + ActualBorderTopWidth + ActualBorderBottomWidth;
            }
            if (height < 1)
            {
                height = 2;
            }
            if (height <= 2 && ActualBorderTopWidth < 1 && ActualBorderBottomWidth < 1)
            {
                DirectSetBorderWidth(CssSide.Top, 1);
                DirectSetBorderWidth(CssSide.Bottom, 1);
            }

            this.SetVisualSize(width, height);
            this.SetVisualHeight(ActualPaddingTop + ActualPaddingBottom + height);
        }

        /// <summary>
        /// Paints the fragment
        /// </summary>
        /// <param name="g">the device to draw to</param>
        protected override void PaintImp(PaintVisitor p)
        {
#if DEBUG
            p.dbugEnterNewContext(this, PaintVisitor.PaintVisitorContextName.Init);
#endif
            var rect = new RectangleF(0, 0, this.VisualWidth, this.VisualHeight);
            if (rect.Height > 2 && RenderUtils.IsColorVisible(ActualBackgroundColor))
            {
                p.FillRectangle(ActualBackgroundColor, rect.X, rect.Y, rect.Width, rect.Height);
            }

            if (rect.Height > 1)
            {
                p.PaintBorders(this, rect);
            }
            else
            {
                p.PaintBorder(this, CssSide.Top, this.BorderTopColor, rect);
            }
#if DEBUG
            p.dbugExitContext();
#endif
        }
    }
}