﻿//BSD, 2014-present, WinterDev 

using System.Collections.Generic;
namespace LayoutFarm.HtmlBoxes
{
    public abstract class BoxVisitor
    {
        Stack<CssBox> containgBlockStack = new Stack<CssBox>();
        CssBox latestContainingBlock = null;
        float globalXOffset;
        float globalYOffset;
        internal void PushContaingBlock(CssBox box)
        {
            //enter new containing block
            OnPushContainingBlock(box);
            if (box != latestContainingBlock)
            {
                this.globalXOffset += box.LocalX;
                this.globalYOffset += box.LocalY;
                OnPushDifferentContainingBlock(box);
            }
            this.containgBlockStack.Push(box);
            this.latestContainingBlock = box;
        }

        internal void PopContainingBlock()
        {
            switch (this.containgBlockStack.Count)
            {
                case 0:
                    {
                    }
                    break;
                case 1:
                    {
                        //last on
                        CssBox box = this.containgBlockStack.Pop();
                        OnPopContainingBlock();
                        if (this.latestContainingBlock != box)
                        {
                            this.globalXOffset -= box.LocalX;
                            this.globalYOffset -= box.LocalY;
                            OnPopDifferentContaingBlock(box);
                        }
                        this.latestContainingBlock = null;
                    }
                    break;
                default:
                    {
                        CssBox box = this.containgBlockStack.Pop();
                        OnPopContainingBlock();
                        if (this.latestContainingBlock != box)
                        {
                            this.globalXOffset -= box.LocalX;
                            this.globalYOffset -= box.LocalY;
                            OnPopDifferentContaingBlock(box);
                        }
                        this.latestContainingBlock = containgBlockStack.Peek();
                    }
                    break;
            }
        }
        internal float ContainerBlockGlobalX
        {
            get { return this.globalXOffset; }
        }
        internal float ContainerBlockGlobalY
        {
            get { return this.globalYOffset; }
        }
        //-----------------------------------------
        protected virtual void OnPushContainingBlock(CssBox box)
        {
        }
        protected virtual void OnPopContainingBlock()
        {
        }
        protected virtual void OnPushDifferentContainingBlock(CssBox box)
        {
        }
        protected virtual void OnPopDifferentContaingBlock(CssBox box)
        {
        }
        internal CssBox LatestContainingBlock
        {
            get { return this.latestContainingBlock; }
        }
    }
}