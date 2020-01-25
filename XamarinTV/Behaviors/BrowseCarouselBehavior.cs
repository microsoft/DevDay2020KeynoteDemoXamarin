using System.Linq;
using XamarinTV.Models;
using Xamarin.Forms;

namespace XamarinTV.Behaviors
{
    public class BrowseCarouselBehavior : Behavior<CarouselView>
    {
        protected override void OnAttachedTo(CarouselView bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.Scrolled += OnScrolled;
        }

        protected override void OnDetachingFrom(CarouselView bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.Scrolled -= OnScrolled;
        }

        void OnScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            var carousel = (CarouselView)sender;
            var carouselItems = carousel.ItemsSource.Cast<object>().ToList();
            var firstIndex = e.FirstVisibleItemIndex;
            var currentIndex = e.CenterItemIndex;
            var lastIndex = e.LastVisibleItemIndex;

            if (firstIndex != currentIndex)
            {
                var firstItem = carouselItems[firstIndex] as VideoCarouselItem;
                firstItem.Scale = 0.8;
            }

            var currentItem = carouselItems[currentIndex] as VideoCarouselItem;
            currentItem.Scale = 1;

            var lastItem = carouselItems[lastIndex] as VideoCarouselItem;
            lastItem.Scale = 0.8;
        }
    }
}