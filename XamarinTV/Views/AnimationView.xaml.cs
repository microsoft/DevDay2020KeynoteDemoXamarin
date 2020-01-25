using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinTV.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnimationView : ContentView
    {
        public static readonly BindableProperty ActiveViewProperty
            = BindableProperty.Create("ActiveView", typeof(View), typeof(AnimationView), propertyChanged: OnActiveViewPropertyChanged);


        public static readonly BindableProperty ViewToAnimateInProperty
            = BindableProperty.Create("ViewToAnimateIn", typeof(View), typeof(AnimationView), propertyChanged: OnViewToAnimateInPropertyChanged);

        static async void OnViewToAnimateInPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            AnimationView animationView = bindable as AnimationView;
            View newView = (View)newValue;
            View oldView = (View)oldValue;

            if(oldView == null && newView != null)
            {
                animationView.gridView.Children.Add(newView);
                animationView.ActiveView = newView;
                return;
            }

            if (newView != null)
            {
                animationView.gridView.Children.Insert(0, newView);
            }

            if (oldView != null)
            {
                oldView.InputTransparent = true;
                await oldView.FadeTo(0, 300);
            }

            if(newView != null)
                newView.InputTransparent = false;

            animationView.ActiveView = newView;

            if (oldView != null)
            {
                animationView.gridView.Children.Remove(oldView);
            }

        }

        static void OnActiveViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
        }

        public View ActiveView
        {
            get { return (View)GetValue(ActiveViewProperty); }
            set { SetValue(ActiveViewProperty, value); }
        }

        public View ViewToAnimateIn
        {
            get { return (View)GetValue(ViewToAnimateInProperty); }
            set { SetValue(ViewToAnimateInProperty, value); }
        }

        public AnimationView()
        {
            InitializeComponent();
        }
    }
}