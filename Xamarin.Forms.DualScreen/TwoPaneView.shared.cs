﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Xamarin.Forms.DualScreen
{
    public enum TwoPaneViewMode
    {
        SinglePane, Wide, Tall
    }

    public enum TwoPaneViewTallModeConfiguration
    {
        SinglePane,
        TopBottom,
        BottomTop,
    }

    public enum TwoPaneViewWideModeConfiguration
    {
        SinglePane,
        LeftRight,
        RightLeft,
    }

    public enum TwoPaneViewPriority
    {
        Pane1,
        Pane2
    }

    [ContentProperty("")]
    public class TwoPaneView : Grid
    {
        enum ViewMode
        {
            Pane1Only,
            Pane2Only,
            LeftRight,
            RightLeft,
            TopBottom,
            BottomTop,
            None
        };

        TwoPaneViewLayoutGuide _twoPaneViewLayoutGuide;
        VisualStateGroup _modeStates;
        ContentView _content1;
        ContentView _content2;
        ViewMode _currentMode;
        bool _hasMeasured = false;
        bool _updatingMode = false;

        public static readonly BindableProperty TallModeConfigurationProperty
            = BindableProperty.Create("TallModeConfiguration", typeof(TwoPaneViewTallModeConfiguration), typeof(TwoPaneView), defaultValue: TwoPaneViewTallModeConfiguration.SinglePane, propertyChanged: OnJustInvalidateLayout);

        static void OnJustInvalidateLayout(BindableObject bindable, object oldValue, object newValue)
        {
            var b = (TwoPaneView)bindable;
            if (!b._performingLayout && !b._updatingMode)
            {
                b.UpdateMode();
            }
        }

        public static readonly BindableProperty WideModeConfigurationProperty
            = BindableProperty.Create("WideModeConfiguration", typeof(TwoPaneViewWideModeConfiguration), typeof(TwoPaneView), defaultValue: TwoPaneViewWideModeConfiguration.LeftRight, propertyChanged: OnJustInvalidateLayout);

        public static readonly BindableProperty Pane1Property
            = BindableProperty.Create("Pane1", typeof(View), typeof(TwoPaneView), propertyChanged: (b, o, n) => OnPanePropertyChanged(b, o, n, 0));

        public static readonly BindableProperty Pane2Property
            = BindableProperty.Create("Pane2", typeof(View), typeof(TwoPaneView), propertyChanged: (b, o, n) => OnPanePropertyChanged(b, o, n, 1));

        public static readonly BindableProperty PanePriorityProperty
            = BindableProperty.Create("PanePriority", typeof(TwoPaneViewPriority), typeof(TwoPaneView), defaultValue: TwoPaneViewPriority.Pane1, propertyChanged: OnJustInvalidateLayout);

        public static readonly BindablePropertyKey ModePropertyKey
            = BindableProperty.CreateReadOnly("Mode", typeof(TwoPaneViewMode), typeof(TwoPaneView), defaultValue: TwoPaneViewMode.SinglePane, propertyChanged: OnModePropertyChanged);

        static void OnModePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TwoPaneView)bindable).ModeChanged?.Invoke(bindable, EventArgs.Empty);
        }

        public static readonly BindableProperty ModeProperty = ModePropertyKey.BindableProperty;

        public static readonly BindablePropertyKey IsLandscapePropertyKey
            = BindableProperty.CreateReadOnly("IsLandscape", typeof(bool), typeof(TwoPaneView), defaultValue: false);

        public static readonly BindableProperty IsLandscapeProperty = IsLandscapePropertyKey.BindableProperty;

        public static readonly BindableProperty MinTallModeHeightProperty
            = BindableProperty.Create("MinTallModeHeight", typeof(double), typeof(TwoPaneView));

        public static readonly BindableProperty MinWideModeWidthProperty
            = BindableProperty.Create("MinWideModeWidth", typeof(double), typeof(TwoPaneView));

        public static readonly BindableProperty Pane1LengthProperty
            = BindableProperty.Create("Pane1Length", typeof(GridLength), typeof(TwoPaneView), defaultValue: GridLength.Star);

        public static readonly BindableProperty Pane2LengthProperty
            = BindableProperty.Create("Pane2Length", typeof(GridLength), typeof(TwoPaneView), defaultValue: GridLength.Star);

        public event EventHandler ModeChanged;

        static void OnPanePropertyChanged(BindableObject bindable, object oldValue, object newValue, int paneIndex)
        {
            TwoPaneView twoPaneView = (TwoPaneView)bindable;
            View newView = (View)newValue;

            if (paneIndex == 0)
                twoPaneView._content1.Content = newView;
            else
                twoPaneView._content2.Content = newView;

            OnJustInvalidateLayout(bindable, null, null);
        }

        public double MinTallModeHeight
        {
            get { return (double)GetValue(MinTallModeHeightProperty); }
            set { SetValue(MinTallModeHeightProperty, value); }
        }
        public double MinWideModeWidth
        {
            get { return (double)GetValue(MinWideModeWidthProperty); }
            set { SetValue(MinWideModeWidthProperty, value); }
        }

        public GridLength Pane1Length
        {
            get { return (GridLength)GetValue(Pane1LengthProperty); }
            set { SetValue(Pane1LengthProperty, value); }
        }
        public GridLength Pane2Length
        {
            get { return (GridLength)GetValue(Pane2LengthProperty); }
            set { SetValue(Pane2LengthProperty, value); }
        }

        public TwoPaneViewMode Mode { get => (TwoPaneViewMode)GetValue(ModeProperty); }

        public TwoPaneViewTallModeConfiguration TallModeConfiguration
        {
            get { return (TwoPaneViewTallModeConfiguration)GetValue(TallModeConfigurationProperty); }
            set { SetValue(TallModeConfigurationProperty, value); }
        }

        public TwoPaneViewWideModeConfiguration WideModeConfiguration
        {
            get { return (TwoPaneViewWideModeConfiguration)GetValue(WideModeConfigurationProperty); }
            set { SetValue(WideModeConfigurationProperty, value); }
        }

        public View Pane1
        {
            get { return (View)GetValue(Pane1Property); }
            set { SetValue(Pane2Property, value); }
        }

        public View Pane2
        {
            get { return (View)GetValue(Pane2Property); }
            set { SetValue(Pane2Property, value); }
        }

        public TwoPaneViewPriority PanePriority
        {
            get { return (TwoPaneViewPriority)GetValue(PanePriorityProperty); }
            set { SetValue(PanePriorityProperty, value); }
        }

        public TwoPaneView() : base()
        {
            _content1 = new ContentView();
            _content2 = new ContentView();

            Children.Add(_content1);
            Children.Add(_content2);

            this.VerticalOptions = LayoutOptions.Fill;
            this.HorizontalOptions = LayoutOptions.Fill;
            ColumnSpacing = 0;
            RowSpacing = 0;

            _modeStates = new VisualStateGroup()
            {
                Name = "ModeStates"
            };

            _modeStates.States.Add(new VisualState() { Name = "ViewMode_OneOnly" });
            _modeStates.States.Add(new VisualState() { Name = "ViewMode_TwoOnly" });
            _modeStates.States.Add(new VisualState() { Name = "ViewMode_LeftRight" });
            _modeStates.States.Add(new VisualState() { Name = "ViewMode_RightLeft" });
            _modeStates.States.Add(new VisualState() { Name = "ViewMode_TopBottom" });
            _modeStates.States.Add(new VisualState() { Name = "ViewMode_BottomTop" });

            VisualStateManager.SetVisualStateGroups(this, new VisualStateGroupList() { _modeStates });

            this.RowDefinitions = new RowDefinitionCollection() { new RowDefinition(), new RowDefinition(), new RowDefinition() };
            this.ColumnDefinitions = new ColumnDefinitionCollection() { new ColumnDefinition(), new ColumnDefinition(), new ColumnDefinition() };
        }

        TwoPaneViewLayoutGuide TwoPaneViewLayoutGuide
        {
            get
            {
                if (_twoPaneViewLayoutGuide == null)
                {
                    _twoPaneViewLayoutGuide = new TwoPaneViewLayoutGuide(this);
                    _twoPaneViewLayoutGuide.PropertyChanged += OnTwoPaneViewLayoutGuide;
                }

                return _twoPaneViewLayoutGuide;
            }
        }

        void OnTwoPaneViewLayoutGuide(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DualScreen.TwoPaneViewLayoutGuide.IsLandscape) || e.PropertyName == nameof(DualScreen.TwoPaneViewLayoutGuide.IsSpanned))
            {
                if (!TwoPaneViewLayoutGuide.IsSpanned)
                {
                    SetValue(ModePropertyKey, TwoPaneViewMode.SinglePane);
                }
                else if (TwoPaneViewLayoutGuide.IsLandscape)
                {
                    SetValue(ModePropertyKey, TwoPaneViewMode.Tall);
                }
                else if (TwoPaneViewLayoutGuide.IsPortrait)
                {
                    SetValue(ModePropertyKey, TwoPaneViewMode.Wide);
                }

                OnJustInvalidateLayout(this, null, null);
            }
        }

        public bool IsDualView
            => Pane1.IsVisible && Pane2.IsVisible;

        public bool IsLandscape { get => (bool)GetValue(IsLandscapeProperty); }

        public bool IsPortrait => !IsLandscape;

        public bool IsSpanned => TwoPaneViewLayoutGuide.IsSpanned;

        bool _performingLayout = false;
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            if (_updatingMode)
                return;

            if (_hasMeasured)
                base.LayoutChildren(x, y, width, height);
            else
                UpdateMode();
        }


        void UpdateMode()
        {
            _updatingMode = true;
            try
            {
                double controlWidth = this.Width;
                double controlHeight = this.Height;

                ViewMode newMode = (PanePriority == TwoPaneViewPriority.Pane1) ? ViewMode.Pane1Only : ViewMode.Pane2Only;

                // Calculate new mode
                // DisplayRegionHelperInfo info = DisplayRegionHelper.GetRegionInfo();
                var screenLocation = DependencyService.Get<ILayoutService>().GetLocationOnScreen(this);
                if (screenLocation == null)
                    return;

                _hasMeasured = true;

                this.TwoPaneViewLayoutGuide.UpdateLayouts();
                if (!IsSet(MinTallModeHeightProperty))
                {
                    // This is the value UWP picks to switch between compact mode
                    SetValue(MinTallModeHeightProperty, 641 / Device.info.ScalingFactor);
                }

                if (!IsSet(MinWideModeWidthProperty))
                {
                    // This is the value UWP picks to switch between compact mode
                    SetValue(MinTallModeHeightProperty, 641 / Device.info.ScalingFactor);
                }

                bool isInMultipleRegions = IsSpanned;
                if (isInMultipleRegions)
                {
                    if (Mode == TwoPaneViewMode.Wide)
                    {
                        // Regions are laid out horizontally
                        if (WideModeConfiguration != TwoPaneViewWideModeConfiguration.SinglePane)
                        {
                            newMode = (WideModeConfiguration == TwoPaneViewWideModeConfiguration.LeftRight) ? ViewMode.LeftRight : ViewMode.RightLeft;
                        }
                    }
                    else if (Mode == TwoPaneViewMode.Tall)
                    {
                        // Regions are laid out vertically
                        if (TallModeConfiguration != TwoPaneViewTallModeConfiguration.SinglePane)
                        {
                            newMode = (TallModeConfiguration == TwoPaneViewTallModeConfiguration.TopBottom) ? ViewMode.TopBottom : ViewMode.BottomTop;
                        }
                    }
                }
                else
                {
                    // One region
                    if (controlWidth > MinWideModeWidth && WideModeConfiguration != TwoPaneViewWideModeConfiguration.SinglePane)
                    {
                        // Split horizontally
                        newMode = (WideModeConfiguration == TwoPaneViewWideModeConfiguration.LeftRight) ? ViewMode.LeftRight : ViewMode.RightLeft;
                    }
                    else if (controlHeight > MinTallModeHeight && TallModeConfiguration != TwoPaneViewTallModeConfiguration.SinglePane)
                    {
                        // Split vertically
                        newMode = (TallModeConfiguration == TwoPaneViewTallModeConfiguration.TopBottom) ? ViewMode.TopBottom : ViewMode.BottomTop;
                    }
                }

                // Update row/column sizes (this may need to happen even if the mode doesn't change)
                UpdateRowsColumns(newMode);

                // Update mode if necessary
                if (newMode != _currentMode)
                {
                    _currentMode = newMode;

                    TwoPaneViewMode newViewMode = TwoPaneViewMode.SinglePane;

                    switch (_currentMode)
                    {
                        case ViewMode.Pane1Only: VisualStateManager.GoToState(this, "ViewMode_OneOnly"); break;
                        case ViewMode.Pane2Only: VisualStateManager.GoToState(this, "ViewMode_TwoOnly"); break;
                        case ViewMode.LeftRight: VisualStateManager.GoToState(this, "ViewMode_LeftRight"); newViewMode = TwoPaneViewMode.Wide; break;
                        case ViewMode.RightLeft: VisualStateManager.GoToState(this, "ViewMode_RightLeft"); newViewMode = TwoPaneViewMode.Wide; break;
                        case ViewMode.TopBottom: VisualStateManager.GoToState(this, "ViewMode_TopBottom"); newViewMode = TwoPaneViewMode.Tall; break;
                        case ViewMode.BottomTop: VisualStateManager.GoToState(this, "ViewMode_BottomTop"); newViewMode = TwoPaneViewMode.Tall; break;
                    }

                    if (newViewMode != Mode)
                    {
                        SetValue(ModePropertyKey, newViewMode);
                    }
                }

                _updatingMode = false;
                InvalidateLayout();
            }
            finally
            {
                _updatingMode = false;
            }
        }

        void UpdateRowsColumns(ViewMode newMode)
        {
            var _columnLeft = ColumnDefinitions[0];
            var _columnMiddle = ColumnDefinitions[1];
            var _columnRight = ColumnDefinitions[2];

            var _rowTop = RowDefinitions[0];
            var _rowMiddle = RowDefinitions[1];
            var _rowBottom = RowDefinitions[2];

            // Reset split lengths
            _columnMiddle.Width = new GridLength(0, GridUnitType.Absolute);
            _rowMiddle.Height = new GridLength(0, GridUnitType.Absolute);

            // Set columns lengths
            if (newMode == ViewMode.LeftRight || newMode == ViewMode.RightLeft)
            {
                _columnLeft.Width = ((newMode == ViewMode.LeftRight) ? Pane1Length : Pane2Length);
                _columnRight.Width = ((newMode == ViewMode.LeftRight) ? Pane2Length : Pane1Length);
            }
            else
            {
                _columnLeft.Width = new GridLength(1, GridUnitType.Star);
                _columnRight.Width = new GridLength(0, GridUnitType.Absolute);
            }

            // Set row lengths
            if (newMode == ViewMode.TopBottom || newMode == ViewMode.BottomTop)
            {
                _rowTop.Height = ((newMode == ViewMode.TopBottom) ? Pane1Length : Pane2Length);
                _rowBottom.Height = ((newMode == ViewMode.TopBottom) ? Pane2Length : Pane1Length);
            }
            else
            {
                _rowTop.Height = new GridLength(1, GridUnitType.Star);
                _rowBottom.Height = new GridLength(0, GridUnitType.Absolute);
            }

            // Handle regions
            if (IsSpanned && newMode != ViewMode.Pane1Only && newMode != ViewMode.Pane2Only)
            {
                Rectangle rc1 = _twoPaneViewLayoutGuide.Pane1;
                Rectangle rc2 = _twoPaneViewLayoutGuide.Pane2;
                Rectangle hinge = _twoPaneViewLayoutGuide.Hinge;

                if (Mode == TwoPaneViewMode.Wide)
                {
                    _columnMiddle.Width = new GridLength(hinge.Width, GridUnitType.Absolute);
                    _columnLeft.Width = new GridLength(rc1.Width, GridUnitType.Absolute);
                    _columnRight.Width = new GridLength(rc2.Width, GridUnitType.Absolute);
                }
                else
                {
                    _rowMiddle.Height = new GridLength(hinge.Height, GridUnitType.Absolute);
                    _rowTop.Height = new GridLength(rc1.Height, GridUnitType.Absolute);
                    _rowBottom.Height = new GridLength(rc2.Height, GridUnitType.Absolute);
                }
            }

            switch (newMode)
            {
                case ViewMode.LeftRight:
                    SetRowColumn(_content1, 0, 0);
                    SetRowColumn(_content2, 0, 2);
                    _content2.IsVisible = true;
                    _content2.IsVisible = true;
                    break;
                case ViewMode.RightLeft:
                    SetRowColumn(_content1, 0, 2);
                    SetRowColumn(_content2, 0, 0);
                    _content2.IsVisible = true;
                    _content2.IsVisible = true;
                    break;
                case ViewMode.TopBottom:
                    SetRowColumn(_content1, 0, 0);
                    SetRowColumn(_content2, 2, 0);
                    _content2.IsVisible = true;
                    _content2.IsVisible = true;
                    break;
                case ViewMode.BottomTop:
                    SetRowColumn(_content1, 2, 0);
                    SetRowColumn(_content2, 0, 0);
                    _content2.IsVisible = true;
                    _content2.IsVisible = true;
                    break;
                case ViewMode.Pane1Only:
                    SetRowColumn(_content1, 0, 0);
                    _content2.IsVisible = false;
                    break;
                case ViewMode.Pane2Only:
                    SetRowColumn(_content1, 0, 0);
                    SetRowColumn(_content2, 0, 2);
                    _content1.IsVisible = false;
                    break;
            }

            void SetRowColumn(BindableObject bo, int row, int column)
            {
                if (bo == null)
                    return;

                Grid.SetColumn(bo, column);
                Grid.SetRow(bo, row);
            }
        }
    }
}