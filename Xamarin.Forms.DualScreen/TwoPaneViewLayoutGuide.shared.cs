
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms.DualScreen
{
    public class TwoPaneViewLayoutGuide : INotifyPropertyChanged
    {
        IDualScreenService DualScreenService => 
            DependencyService.Get<IDualScreenService>() ?? NoDualScreenServiceImpl.Instance;

        Rectangle hinge;
        Rectangle leftPane;
        Rectangle rightPane;
        TwoPaneViewMode _mode;
        Layout _layout;

        public TwoPaneViewLayoutGuide()
        {
        }

        public TwoPaneViewLayoutGuide(Layout layout)
        {
            _layout = layout;
            DualScreenService.OnScreenChanged += OnScreenChanged;

            if (_layout != null)
            {
                _layout.SizeChanged += OnLayoutChanged;
            }
        }

        void OnLayoutChanged(object sender, EventArgs e)
        {
            UpdateLayouts();
        }

        void OnScreenChanged(object sender, EventArgs e)
        {
            Hinge = DualScreenService.GetHinge();
            UpdateLayouts();
        }

        internal void UpdateLayouts()
        {
            Rectangle containerArea;

            if (_layout == null)
            {
                containerArea = new Rectangle(Point.Zero, Device.info.ScaledScreenSize);
            }
            else
            {
                containerArea = _layout.Bounds;
            }

            if (containerArea.Width <= 0)
            {
                return;
            }

            Mode = GetTwoPaneViewMode();
            Hinge = DualScreenService.GetHinge();
            
            if (!DualScreenService.IsLandscape)
            {
                if (DualScreenService.IsSpanned)
                {
                    var paneWidth = (containerArea.Width - Hinge.Width) / 2;
                    Pane1 = new Rectangle(0, 0, paneWidth, containerArea.Height);
                    Pane2 = new Rectangle(paneWidth + Hinge.Width, 0, paneWidth, Pane1.Height);
                }
                else
                {
                    Pane1 = new Rectangle(0, 0, containerArea.Width, containerArea.Height);
                    Pane2 = Rectangle.Zero;
                }
            }
            else
            {
                Point displayedScreenAbsCoordinates = Point.Zero;

                if (_layout != null)
                    displayedScreenAbsCoordinates = DualScreenService.GetLocationOnScreen(_layout) ?? Point.Zero;

                if (DualScreenService.IsSpanned)
                {
                    var screenSize = Device.info.ScaledScreenSize;
                    var topStuffHeight = displayedScreenAbsCoordinates.Y;
                    var bottomStuffHeight = screenSize.Height - topStuffHeight - containerArea.Height;
                    var paneWidth = containerArea.Width;
                    var leftPaneHeight = Hinge.Y - topStuffHeight;
                    var rightPaneHeight = screenSize.Height - topStuffHeight - leftPaneHeight - bottomStuffHeight - Hinge.Height;

                    Pane1 = new Rectangle(0, 0, paneWidth, leftPaneHeight);
                    Pane2 = new Rectangle(0, Hinge.Y + Hinge.Height - topStuffHeight, paneWidth, rightPaneHeight);
                }
                else
                {
                    Pane1 = new Rectangle(0, 0, containerArea.Width, containerArea.Height);
                    Pane2 = Rectangle.Zero;
                }
            }
        }

        TwoPaneViewMode GetTwoPaneViewMode()
        {
            if (!DualScreenService.IsSpanned)
                return TwoPaneViewMode.SinglePane;

            if (DualScreenService.IsLandscape)
                return TwoPaneViewMode.Tall;

            return TwoPaneViewMode.Wide;
        }

        public TwoPaneViewMode Mode
        {
            get
            {
                return GetTwoPaneViewMode();
            }
            set
            {
                SetProperty(ref _mode, value);
            }
        }

        public Rectangle Pane1
        {
            get
            {
                return leftPane;
            }
            set
            {
                SetProperty(ref leftPane, value);
            }
        }

        public Rectangle Pane2
        {
            get
            {
                return rightPane;
            }
            set
            {
                SetProperty(ref rightPane, value);
            }
        }

        public Rectangle Hinge
        {
            get
            {
                return hinge;
            }
            set
            {
                SetProperty(ref hinge, value);
            }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}