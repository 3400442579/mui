﻿using System;
using System.Windows;
using ToastNotifications.Core;

namespace ToastNotifications.Position
{
    public class GdiPrimaryScreenPositionProvider : IPositionProvider
    {
        private readonly Corner _corner;
        private readonly double _offsetX;
        private readonly double _offsetY;

        private double ScreenHeight => SystemParameters.PrimaryScreenHeight ;
        private double ScreenWidth => SystemParameters.PrimaryScreenWidth ;

        private double WorkAreaHeight => SystemParameters.WorkArea.Size.Height ;
        private double WorkAreaWidth => SystemParameters.WorkArea.Size.Width ;

        public Window ParentWindow { get; }
        public EjectDirection EjectDirection { get; private set; }

       

        public Point GetPosition(double actualPopupWidth, double actualPopupHeight)
        {
            return _corner switch
            {
                Corner.TopRight => GetPositionForTopRightCorner(actualPopupWidth, actualPopupHeight),
                Corner.TopLeft => GetPositionForTopLeftCorner(actualPopupWidth, actualPopupHeight),
                Corner.BottomRight => GetPositionForBottomRightCorner(actualPopupWidth, actualPopupHeight),
                Corner.BottomLeft => GetPositionForBottomLeftCorner(actualPopupWidth, actualPopupHeight),
                Corner.BottomCenter => GetPositionForBottomCenterCorner(actualPopupWidth, actualPopupHeight),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public double GetHeight()
        {
            return ScreenHeight;
        }

        private void SetEjectDirection(Corner corner)
        {
            switch (corner)
            {
                case Corner.TopRight:
                case Corner.TopLeft:
                    EjectDirection = EjectDirection.ToBottom;
                    break;
                case Corner.BottomRight:
                case Corner.BottomLeft:
                case Corner.BottomCenter:
                    EjectDirection = EjectDirection.ToTop;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(corner), corner, null);
            }
        }

        private Point GetPositionForBottomLeftCorner(double actualPopupWidth, double actualPopupHeight)
        {
            double pointX = _offsetX;
            double pointY = WorkAreaHeight - _offsetY - actualPopupHeight;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = (ScreenWidth - WorkAreaWidth) + _offsetX;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - _offsetY - actualPopupHeight;
                    break;
            }

            return new Point(pointX, pointY);
        }

        private Point GetPositionForBottomCenterCorner(double actualPopupWidth, double actualPopupHeight)
        {
            double pointX = (WorkAreaWidth - _offsetX - actualPopupWidth) / 2;
            double pointY = WorkAreaHeight - _offsetY - actualPopupHeight;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = (ScreenWidth - _offsetX - actualPopupWidth) / 2;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - _offsetY - actualPopupHeight;
                    break;
            }

            return new Point(pointX, pointY);
        }


        private Point GetPositionForBottomRightCorner(double actualPopupWidth, double actualPopupHeight)
        {
            double pointX = WorkAreaWidth - _offsetX - actualPopupWidth;
            double pointY = WorkAreaHeight - _offsetY - actualPopupHeight;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = ScreenWidth - _offsetX - actualPopupWidth;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - _offsetY - actualPopupHeight;
                    break;
            }

            return new Point(pointX, pointY);
        }

        private Point GetPositionForTopLeftCorner(double actualPopupWidth, double actualPopupHeight)
        {
            double pointX = _offsetX;
            double pointY = _offsetY;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = ScreenWidth - WorkAreaWidth + _offsetX;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - WorkAreaHeight + _offsetY;
                    break;
            }

            return new Point(pointX, pointY);
        }

        private Point GetPositionForTopRightCorner(double actualPopupWidth, double actualPopupHeight)
        {
            double pointX = WorkAreaWidth - _offsetX - actualPopupWidth;
            double pointY = _offsetY;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = ScreenWidth - actualPopupWidth - _offsetX;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - WorkAreaHeight + _offsetY;
                    break;
            }

            return new Point(pointX, pointY);
        }


        private WindowsTaskBarLocation GetTaskBarLocation()
        {
            if (SystemParameters.WorkArea.Left > 0)
                return WindowsTaskBarLocation.Left;

            if (SystemParameters.WorkArea.Top > 0)
                return WindowsTaskBarLocation.Top;

            if (SystemParameters.WorkArea.Left == 0 &&
                SystemParameters.WorkArea.Width < SystemParameters.PrimaryScreenWidth)
                return WindowsTaskBarLocation.Right;

            return WindowsTaskBarLocation.Bottom;
        }


        public void Dispose()
        {
            // nothing to do here
        }

#pragma warning disable CS0067
        public event EventHandler UpdatePositionRequested;

        public event EventHandler UpdateEjectDirectionRequested;

        public event EventHandler UpdateHeightRequested;
#pragma warning restore CS0067
    }
}
