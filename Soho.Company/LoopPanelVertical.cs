using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Timers;
using System.Windows.Threading;
using System.Threading;

namespace SOHO.Company
{
    class LoopPanelVertical : Panel, ISurfaceScrollInfo
    {
        private static System.Timers.Timer time = new System.Timers.Timer(10);
        private ScrollViewer owner;
        private Size extent = new Size(0, 0);
        private Size viewport = new Size(0, 0);
        private Point viewportOffset = new Point();
        private bool viewportPositionDirty;
        private bool verticalScrollAllowed;
        private int firstItem;
        private double firstItemOffset;
        private double previousOffset = double.NaN;
        private double totalContentHeight;
        private DateTime touchTime = DateTime.Now;
        private double touchPoint = 0;
        // Constructor
        public LoopPanelVertical()
        {
            this.RenderTransform = new TranslateTransform();
            time.Elapsed += new ElapsedEventHandler(time_Elapsed);
            StartTimer();
        }


        public  void PauseTimer()
        {
            time.Stop();
        }
        public  void StartTimer()
        {
            this.UpdateLayout();
            previousOffset = double.NaN;
            time.Start();
        }

        void time_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                           (ThreadStart)delegate()
                           {
                               this.ScrollContent(-1);
                           });
            }
            catch (Exception ex)
            {
                StartTimer();
            }
        }

        #region IScrollInfo Properties
        // Gets or sets a value that determines if the content can be scrolled horizontally.
        public bool CanHorizontallyScroll
        {
            get
            {
                return false;
            }
            set
            {
                if (value)
                {
                    throw new NotSupportedException();
                }
            }
        }
 
        // Gets or sets a value that determines if the content can be scrolled vertically.
        public bool CanVerticallyScroll
        {
            get
            {
                return verticalScrollAllowed;
            }
            set
            {
                verticalScrollAllowed = value;
            }
        }
 
        // Gets a value that describes the height of the area.
        public double ExtentHeight
        {
            get { return extent.Height; }
        }
 
        // Gets a value that describes the width of the area.
        public double ExtentWidth
        {
            get { return extent.Width; }
        }
 
        /// Gets a value that describes the vertical offset of the view box.
        public double VerticalOffset
        {
            get { return viewportOffset.Y; }
        }
 
 
        // Gets a value that describes the horizontal offset of the view box.
        public double HorizontalOffset
        {
            get { return viewportOffset.X; }
        }
 
        // Gets a value that describes the height of the viewport.
        public double ViewportHeight
        {
            get { return viewport.Height; }
        }
 
        // Gets a value that describes the width of the viewport.
        public double ViewportWidth
        {
            get { return viewport.Width; }
        }
 
        // Gets or sets the owner.
        public ScrollViewer ScrollOwner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }
        #endregion
 
        #region IScrollInfo Positioning Methods
        // Scroll content up.
        public void LineUp()
        {
            ScrollContent(-1);
        }
 
        // Scroll content down.
        public void LineDown()
        {
            ScrollContent(1);
        }
 
        public void LineLeft()
        {
            // Not implemented. Cannot scroll horizontally.
        }
 
        public void LineRight()
        {
            // Not implemented. Cannot scroll horizontally.
        }
 
        // Scrolls the content up by ten lines.
        public void MouseWheelUp()
        {
            //ScrollContent(10);
        }
 
        // Scrolls the content down by ten lines. 
        public void MouseWheelDown()
        {
            //ScrollContent(-10);
        }
 
        public void MouseWheelLeft()
        {
            // Not implemented. Cannot scroll horizontally.
        }
 
        public void MouseWheelRight()
        {
            // Not implemented. Cannot scroll horizontally. 
        }
 
        public void PageUp()
        {
            //ScrollContent(-viewport.Height);
        }
 
        public void PageDown()
        {
            //ScrollContent(viewport.Height);
        }
 
        public void PageLeft()
        {
            // Not implemented. Cannot scroll horizontally.
        }
 
        public void PageRight()
        {
            // Not implemented. Cannot scroll horizontally.
        }
        #endregion
 
        #region IScrollInfo Offset Methods
        public void SetVerticalOffset(double newOffset)
        {
            if (CanVerticallyScroll)
            {
                // Make sure the offset is set for the current inputs.
                if (double.IsNaN(previousOffset))
                {
                    previousOffset = newOffset;
                }
 
                // Calculate the movement delta.
                double difference = previousOffset - newOffset;
 
                // Scroll the content.
                ScrollContent(difference);
 
                // Update the offset for next time.
                previousOffset = newOffset;
            }
        }
 
        public void SetHorizontalOffset(double newOffset)
        {
            // Not implemented. Cannot scroll horizontally.
        }
        #endregion
 
        #region IScrollInfo MakeVisible Method
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            int itemIndex = Children.IndexOf((UIElement)visual);
            int index = firstItem;
            double itemOffset = firstItemOffset;
 
            // Get the offset for the item that should be visible.
            while (index != itemIndex)
            {
                itemOffset += Children[index].DesiredSize.Height;
                index++;
                if (index >= Children.Count)
                {
                    index = 0;
                }
            }
 
            // If the item is not fully in view on the top, 
            // adjust the offset to bring it into view.
            if (itemOffset < viewportOffset.Y)
            {
                ScrollContent(viewportOffset.Y - itemOffset);
            }
 
            // If the item is not fully in view on the bottom, 
            // adjust the offset to bring it into view.
            if (itemOffset + rectangle.Height > viewportOffset.Y + ViewportHeight)
            {
                ScrollContent((viewportOffset.Y + ViewportHeight) - 
                    (itemOffset + rectangle.Height));
            }
 
            return rectangle;
        }
        #endregion
 
        #region ISurfaceScrollInfo Members
        public Vector ConvertFromViewportUnits(Point origin, Vector offset)
        {
            return offset;
        }
 
        public Vector ConvertToViewportUnits(Point origin, Vector offset)
        {
            return offset;
        }
 
        public Vector ConvertToViewportUnitsForFlick(Point origin, Vector offset)
        {
            return offset;
        }
        #endregion
 
        #region Overridden Layout Methods
        protected override Size MeasureOverride(Size availableSize)
        {
            // If there's no children, take all the space.
            if (this.InternalChildren.Count == 0)
            {
                return availableSize;
            }
 
            // Measure each of the child items. Keep a running sum of heights.
            // Used later to measure the viewport and the extent.
            this.totalContentHeight = 0;
            foreach (UIElement child in this.InternalChildren)
            {
                child.Measure(availableSize);
                this.totalContentHeight += child.DesiredSize.Height;
            }
 
            // The viewport should be as large as it can be.
            this.viewport = availableSize;
 
            // Remeasuring could invalidate the current viewport position. Mark it as dirty.
            this.viewportPositionDirty = true;
 
            return availableSize;
        }
        
        protected override Size ArrangeOverride(Size finalSize)
        {
            // If there's no children, take all the space.
            if (this.InternalChildren.Count == 0)
            {
                return finalSize;
            }
 
            // Arrange the viewport relative to the extent.
            if (this.viewportPositionDirty)
            {
                PositionViewportAndExtent();
            }
 
            double nextDrawingPosition = firstItemOffset;
            Rect arrangeInMe;
 
            // From first item to end
            for (int i = firstItem; i < InternalChildren.Count; i++)
            {
                UIElement item = InternalChildren[i];
                arrangeInMe = 
                    new Rect(0, nextDrawingPosition, 
                        item.DesiredSize.Width, 
                        item.DesiredSize.Height);
                item.Arrange(arrangeInMe);
                nextDrawingPosition += item.DesiredSize.Height;
            }
 
            // From child zero to firstItem - 1
            for (int i = 0; i < firstItem; i++)
            {
                UIElement item = InternalChildren[i];
                arrangeInMe = 
                    new Rect(0, nextDrawingPosition, 
                        item.DesiredSize.Width, 
                        item.DesiredSize.Height);
                item.Arrange(arrangeInMe);
                nextDrawingPosition += item.DesiredSize.Height;
            }
 
            return finalSize;
        }
        #endregion
 
        #region Viewport and Extent Methods
        private void PositionViewportAndExtent()
        {
            // If the items all fit in the viewport at the same time, 
            // disable scrolling and center the items.
            if (totalContentHeight < viewport.Height)
            {
                extent = new Size(viewport.Width, totalContentHeight);
                SetViewport(0);
                firstItemOffset = (viewport.Height - totalContentHeight) / 2;
                CanVerticallyScroll = false;
            }
 
            // Otherwise, extend the extent past both ends of the viewport 
            // so there will be plenty of space in which to scroll the items.
            else
            {
                // Make sure the extent is plenty large. 
                extent = new Size(viewport.Width, Math.Max(1000000, totalContentHeight * 15));
                SetViewport((extent.Height - viewport.Height) / 2);
                firstItemOffset = (extent.Height - totalContentHeight) / 2;
                CanVerticallyScroll = true;
            }
 
            viewportPositionDirty = false;
 
            // Because the offset was changed, the current scroll info isn't valid anymore.
            if (owner != null)
            {
                owner.InvalidateScrollInfo();
            }
        }
        
        private void SetViewport(double newOffset)
        {
            // Validate the input.
            if (newOffset < 0 || viewport.Height >= extent.Height)
            {
                newOffset = 0;
            }
 
            if (newOffset + viewport.Height >= extent.Height)
            {
                newOffset = extent.Height - viewport.Height;
               // newOffset = extent.Height;
            }
 
            // Value is validated, so use it.
            viewportOffset.Y = newOffset;
 
            // Adjust the transform to display based on the new offset.
            ((TranslateTransform)this.RenderTransform).Y = -newOffset;
 
            // Balance the content around the viewport.
            double firstItemBottom = 
                firstItemOffset + InternalChildren[firstItem].DesiredSize.Height;
            int lastItemIndex = 
                firstItem <= 0 ? InternalChildren.Count - 1 : firstItem - 1;
            double lastItemTop = 
                firstItemOffset + totalContentHeight - InternalChildren[lastItemIndex].DesiredSize.Height;
 
            // Move items as needed.
            if (viewportOffset.Y < firstItemBottom)
            {
                MoveItemsFromBottomToTop();
            }
 
            if (viewportOffset.Y + ViewportHeight > lastItemTop)
            {
               MoveItemsFromTopToBottom();     //billjiang 2014-01-20
            }
        }
        protected override void OnPreviewTouchDown(System.Windows.Input.TouchEventArgs e)
        {
            base.OnPreviewTouchDown(e);

            touchTime = DateTime.Now;
            touchPoint = e.GetTouchPoint(this.owner).Position.Y;
        }


        protected override void OnPreviewTouchMove(System.Windows.Input.TouchEventArgs e)
        {
            base.OnPreviewTouchMove(e);
            TimeSpan ts = DateTime.Now - touchTime;
            double  currentPoint= e.GetTouchPoint(this.owner).Position.Y;
            if (ts.TotalMilliseconds > 100 & (Math.Abs(touchPoint - currentPoint) < 60))
            {
                CanVerticallyScroll = false;
                touchTime = DateTime.Now;
                touchPoint = currentPoint;
                return;
            }
            if (!CanVerticallyScroll)
            {
                CanVerticallyScroll = true;
            }
        }

        // Called by various other methods.
        private void ScrollContent(double adjustment)
        {
            SetViewport(viewportOffset.Y - adjustment);
        }
        #endregion
        
        #region Move Item Methods
        private void MoveItemsFromBottomToTop()
        {
            // Move from the left to the right 
            // until the first item that is not in the viewport.
            int index = firstItem;
            double itemTopOffset = firstItemOffset;
            int itemsInViewport = 0;
 
            // Step through the items from the top, and
            // count how many items are in the viewport.
            while (itemTopOffset < viewportOffset.Y + ViewportHeight)
            {
                itemsInViewport++;
                itemTopOffset += InternalChildren[index].DesiredSize.Height;
                index = index >= InternalChildren.Count - 1 ? 0 : index + 1;
            }
 
            // The items that are not in the viewport should be distributed
            // so there are some on either side of the viewport. 
            // It's likely that the viewport will continue to be moved in the 
            // direction it is currently being moved.
            // Move 2/3 of the items, and leave 1/3 where they are.
            int itemsNotInViewport = InternalChildren.Count - itemsInViewport;
            int itemsToRemainInPlace = itemsNotInViewport / 3;
 
            // Skip past the items that will remain in place.
            for (int i = 0; i < itemsToRemainInPlace; i++)
            {
                itemTopOffset += InternalChildren[index].DesiredSize.Height;
                index = index >= InternalChildren.Count - 1 ? 0 : index + 1;
            }
 
            // Find the amount by which firstItemOffset needs to be adjusted.
            double movedHeight = firstItemOffset + totalContentHeight - itemTopOffset;
 
            // Change the first item index, and adjust the offset to match.
            firstItemOffset -= movedHeight;
            firstItem = index;
 
            // Need to redraw the items.
            InvalidateVisual();
        }
 
        private void MoveItemsFromTopToBottom()
        {
            // Move from the top to bottom 
            // until the first item that is not in the viewport.
            int index = firstItem <= 0 ? InternalChildren.Count - 1 : firstItem - 1;
            double itemBottomOffset = firstItemOffset + totalContentHeight;
            int itemsInViewport = 0;
 
            // Step through the items from the bottom, and
            // count how many items are in the viewport.
            while (itemBottomOffset > viewportOffset.Y)
            {
                itemsInViewport++;
                itemBottomOffset -= InternalChildren[index].DesiredSize.Height;
                index = index <= 0 ? InternalChildren.Count - 1 : index - 1;
            }
 
            // The items that are not in the viewport should be distributed
            // so there are some on either side of the viewport. 
            // It's likely that the viewport will continue to be moved in the 
            // direction it is currently being moved.
            // Move 2/3 of the items, and leave 1/3 where they are.
            int itemsNotInViewport = InternalChildren.Count - itemsInViewport;
            int itemsToRemain = itemsNotInViewport / 3;
 
            // Skip past the items that will remain in place.
            for (int i = 0; i < itemsToRemain; i++)
            {
                itemBottomOffset -= InternalChildren[index].DesiredSize.Height;
                index = index <= 0 ? InternalChildren.Count - 1 : index - 1;
            }
 
            // Find the amount by which firstItemOffset needs to be adjusted.
            double movedHeight = itemBottomOffset - firstItemOffset;
 
            // Change the first item index, and adjust the offset to match.
            firstItemOffset += movedHeight;
            firstItem = index >= InternalChildren.Count - 1 ? 0 : index + 1;
 
            // Need to redraw the items.
            InvalidateVisual();
        }
        #endregion
    } // end Class LoopPanelVertical
}
