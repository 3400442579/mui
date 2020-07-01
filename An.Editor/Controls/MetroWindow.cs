﻿using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;


namespace An.Editor.Controls
{
    public class MetroWindow : Window
    {
        public MetroWindow() { 
        }

        void SetupSide(INameScope NameScope, string name, StandardCursorType cursor, WindowEdge edge)
        {
            var ctl = NameScope.Find<Control>(name);
            if (ctl != null)
            {
                ctl.Cursor = new Cursor(cursor);
                ctl.PointerPressed += (i, e) =>
                {
                    PlatformImpl?.BeginResizeDrag(edge, e);
                };
            }
        }
      
        
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);

            var titleBar = e.NameScope.Find<Grid>("TitleBar");
            if (titleBar != null)
            {
                titleBar.PointerPressed += (i, e) =>
                {
                    PlatformImpl?.BeginMoveDrag(e);
                };
            }

            SetupSide(e.NameScope,"Left", StandardCursorType.LeftSide, WindowEdge.West);
            SetupSide(e.NameScope, "Right", StandardCursorType.RightSide, WindowEdge.East);
            SetupSide(e.NameScope, "Top", StandardCursorType.TopSide, WindowEdge.North);
            SetupSide(e.NameScope, "Bottom", StandardCursorType.BottomSide, WindowEdge.South);
            SetupSide(e.NameScope, "TopLeft", StandardCursorType.TopLeftCorner, WindowEdge.NorthWest);
            SetupSide(e.NameScope, "TopRight", StandardCursorType.TopRightCorner, WindowEdge.NorthEast);
            SetupSide(e.NameScope, "BottomLeft", StandardCursorType.BottomLeftCorner, WindowEdge.SouthWest);
            SetupSide(e.NameScope, "BottomRight", StandardCursorType.BottomRightCorner, WindowEdge.SouthEast);


            //var minimizeButton = e.NameScope.Find<Button>("MinimizeButton");
            //if (minimizeButton != null)
            //{
            //    minimizeButton.Click += delegate { this.WindowState = WindowState.Minimized; };
            //}
            //var maximizeButton = e.NameScope.Find<Button>("MaximizeButton");
            //if (maximizeButton != null)
            //{
            //    maximizeButton.Click += delegate { WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized; };
            //}
            //var closeButton = e.NameScope.Find<Button>("CloseButton");
            //if (closeButton != null)
            //{
            //    closeButton.Click += delegate { Close(); };
            //}
             
        }
    }

   
}
