using System;
using System.Linq;
using Xamarin.Forms;

namespace XamarinTV.Views
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
    public class TwoPaneView : Layout<View>
    {
        FormsWindow _ScreenViewModel;

        public static readonly BindableProperty TallModeConfigurationProperty 
            = BindableProperty.Create("TallModeConfiguration", typeof(TwoPaneViewTallModeConfiguration), typeof(TwoPaneView), defaultValue: TwoPaneViewTallModeConfiguration.SinglePane, propertyChanged: OnJustInvalidateLayout);

        private static void OnJustInvalidateLayout(BindableObject bindable, object oldValue, object newValue)
        {
            var b = (TwoPaneView)bindable;
            if (!b._performingLayout)
                b.InvalidateLayout();
        }

        public static readonly BindableProperty WideModeConfigurationProperty 
            = BindableProperty.Create("WideModeConfiguration", typeof(TwoPaneViewWideModeConfiguration), typeof(TwoPaneView), defaultValue: TwoPaneViewWideModeConfiguration.LeftRight, propertyChanged: OnJustInvalidateLayout);

        public static readonly BindableProperty Pane1Property
            = BindableProperty.Create("Pane1", typeof(View), typeof(TwoPaneView), propertyChanged: OnPanePropertyChanged);

        public static readonly BindableProperty Pane2Property
            = BindableProperty.Create("Pane2", typeof(View), typeof(TwoPaneView), propertyChanged: OnPanePropertyChanged);

        public static readonly BindableProperty PanePriorityProperty
            = BindableProperty.Create("PanePriority", typeof(TwoPaneViewPriority), typeof(TwoPaneView), defaultValue: TwoPaneViewPriority.Pane1, propertyChanged: OnJustInvalidateLayout);
        
        public static readonly BindablePropertyKey ModePropertyKey
            = BindableProperty.CreateReadOnly("Mode", typeof(TwoPaneViewMode), typeof(TwoPaneView), defaultValue: TwoPaneViewMode.SinglePane);
        
        public static readonly BindableProperty ModeProperty = ModePropertyKey.BindableProperty;

        public static readonly BindablePropertyKey IsLandscapePropertyKey
            = BindableProperty.CreateReadOnly("IsLandscape", typeof(bool), typeof(TwoPaneView), defaultValue: false);

        public static readonly BindableProperty IsLandscapeProperty = IsLandscapePropertyKey.BindableProperty;

        static void OnPanePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            TwoPaneView twoPaneView = (TwoPaneView)bindable;

            if (oldValue is View oldView)
            {
                foreach(ContentView child in twoPaneView.Children.ToList())
                {
                    if(child.Content == oldView)
                    {
                        twoPaneView.Children.Remove(child);
                        break;
                    }
                }
            }

            if (newValue is View newView)
            {
                twoPaneView.Children.Add(new ContentView() { BackgroundColor = Color.Transparent, Content = newView });
            }
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
            this.VerticalOptions = LayoutOptions.Fill;
            this.HorizontalOptions = LayoutOptions.Fill;
        }

        FormsWindow CurrentFormsWindow
        {
            get
            {
                if (_ScreenViewModel == null)
                {
                    _ScreenViewModel = new FormsWindow(null, this);
                    _ScreenViewModel.PropertyChanged += OnScreenViewModelChanged;
                }

                return _ScreenViewModel;
            }
        }

        void OnScreenViewModelChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FormsWindow.IsLandscape) || e.PropertyName == nameof(FormsWindow.IsSpanned))
            {
                if (!CurrentFormsWindow.IsSpanned)
                {
                    SetValue(ModePropertyKey, TwoPaneViewMode.SinglePane);
                }
                else if (CurrentFormsWindow.IsLandscape)
                {
                    SetValue(ModePropertyKey, TwoPaneViewMode.Tall);
                }
                else if (CurrentFormsWindow.IsPortrait)
                {
                    SetValue(ModePropertyKey, TwoPaneViewMode.Wide);
                }

                if(!_performingLayout)
                    InvalidateLayout();
            }
        }

        public bool IsDualView
            => Pane1.IsVisible && Pane2.IsVisible;

        public bool IsLandscape { get => (bool)GetValue(IsLandscapeProperty); }

        public bool IsPortrait
            => !IsLandscape;    

        public bool IsSpanned
            => CurrentFormsWindow.IsSpanned;

        bool _performingLayout = false;
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            _performingLayout = true;
            try
            {
                var primary = Pane1?.Parent as View;
                var secondary = Pane2?.Parent as View;

                if (PanePriority == TwoPaneViewPriority.Pane2 &&
                    secondary != null &&
                    !IsSpanned)
                {
                    var temp = primary;
                    primary = secondary;
                    secondary = temp;
                }

                if (primary == null)
                    return;

                var formsWindows = CurrentFormsWindow;
                formsWindows.UpdateLayouts();

                var pane1 = formsWindows.Pane1;
                var pane2 = formsWindows.Pane2;

                Rectangle pane1ViewRect = Rectangle.Zero;
                Rectangle pane2ViewRect = Rectangle.Zero;

                if (!formsWindows.IsSpanned)
                {
                    pane1ViewRect = pane1;
                    pane2ViewRect = pane2;
                }
                else if (formsWindows.IsPortrait)
                {
                    if (WideModeConfiguration == TwoPaneViewWideModeConfiguration.LeftRight)
                    {
                        pane1ViewRect = pane1;
                        pane2ViewRect = pane2;
                    }
                    else if (WideModeConfiguration == TwoPaneViewWideModeConfiguration.RightLeft)
                    {
                        pane1ViewRect = pane2;
                        pane2ViewRect = pane1;
                    }
                    else if (WideModeConfiguration == TwoPaneViewWideModeConfiguration.SinglePane)
                    {
                        pane1ViewRect = formsWindows.ContainerArea;
                    }
                }
                else
                {
                    if (TallModeConfiguration == TwoPaneViewTallModeConfiguration.TopBottom)
                    {
                        pane1ViewRect = pane1;
                        pane2ViewRect = pane2;
                    }
                    else if (TallModeConfiguration == TwoPaneViewTallModeConfiguration.BottomTop)
                    {
                        pane1ViewRect = pane2;
                        pane2ViewRect = pane1;
                    }
                    else if (TallModeConfiguration == TwoPaneViewTallModeConfiguration.SinglePane)
                    {
                        pane1ViewRect = formsWindows.ContainerArea;
                    }
                }

                var newBounds = TransformBoundsBeforeLayout(pane1ViewRect, pane2ViewRect);
                pane1ViewRect = newBounds?.FirstOrDefault() ?? Rectangle.Zero;
                pane2ViewRect = newBounds?.Skip(1)?.FirstOrDefault() ?? Rectangle.Zero;

                primary.IsVisible = pane1ViewRect != Rectangle.Zero;

                if (secondary != null)
                    secondary.IsVisible = pane2ViewRect != Rectangle.Zero;

                if (primary.Bounds != pane1ViewRect)
                {
                    LayoutChildIntoBoundingRegion(primary, pane1ViewRect);
                }

                if (secondary != null && secondary.Bounds != pane2ViewRect)
                {
                    LayoutChildIntoBoundingRegion(secondary, pane2ViewRect);
                }

                SetValue(IsLandscapePropertyKey, formsWindows.IsLandscape);
                OnPropertyChanged(nameof(IsLandscape));
                OnPropertyChanged(nameof(IsPortrait));
                OnPropertyChanged(nameof(IsDualView));
                OnPropertyChanged(nameof(IsSpanned));
            }
            finally
            {
                _performingLayout = false;
            }

        }

        public virtual Rectangle[] TransformBoundsBeforeLayout(Rectangle pane1, Rectangle pane2)
        {
            return new[] { pane1, pane2 };
        }
    }
}
