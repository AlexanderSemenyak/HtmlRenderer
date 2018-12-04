﻿//BSD, 2014-present, WinterDev 


using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    public class CssBoxListItem : CssBox
    {
        CssBox _listItemBulletBox;
        public CssBoxListItem(Css.BoxSpec spec, IRootGraphics rootgfx)
            : base(spec, rootgfx)
        {
        }
        public CssBox BulletBox
        {
            get
            {
                return _listItemBulletBox;
            }
            set
            {
                _listItemBulletBox = value;
            }
        }
        protected override void PerformContentLayout(LayoutVisitor lay)
        {
            base.PerformContentLayout(lay);
            if (_listItemBulletBox != null)
            {
                //layout list item
                var prevSibling = lay.LatestSiblingBox;
                lay.LatestSiblingBox = null;//reset
                _listItemBulletBox.PerformLayout(lay);
                lay.LatestSiblingBox = prevSibling;
                var fRun = _listItemBulletBox.FirstRun;
                _listItemBulletBox.FirstRun.SetSize(fRun.Width, fRun.Height);
                _listItemBulletBox.FirstRun.SetLocation(_listItemBulletBox.VisualWidth - 5, this.ActualPaddingTop);
            }
        }
    }
}