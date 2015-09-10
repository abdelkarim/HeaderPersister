using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WpfGhost.Helpers;

namespace WpfGhost
{
    public static class HeaderPersister
    {
        #region "Fields"

        // maps an itemsControl to the currently highlighted group item
        private static readonly Dictionary<GroupItem, WeakReference<HeaderAdorner>> CurrentGroupItem;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Initializes static members of the <see cref="HeaderPersister"/> class.
        /// </summary>
        static HeaderPersister()
        {
            CurrentGroupItem = new Dictionary<GroupItem, WeakReference<HeaderAdorner>>();
        }

        #endregion

        #region "Properties"

        #region HeaderTempate

        /// <summary>
        /// Identifies the <see cref="HeaderTempate"/> Attached Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.RegisterAttached("HeaderTemplate", typeof (DataTemplate), typeof (HeaderPersister),
                new FrameworkPropertyMetadata((DataTemplate) null));

        /// <summary>
        /// </summary>
        public static DataTemplate GetHeaderTemplate(DependencyObject d)
        {
            return (DataTemplate) d.GetValue(HeaderTemplateProperty);
        }

        /// <summary>
        /// </summary>
        public static void SetHeaderTemplate(DependencyObject d, DataTemplate value)
        {
            d.SetValue(HeaderTemplateProperty, value);
        }

        #endregion

        #region EnableHeaderFixing

        /// <summary>
        /// Identifies the <see cref="IsEnabled"/> Attached Dependency Property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof (bool),
            typeof (HeaderPersister),
            new FrameworkPropertyMetadata(false, OnEnableHeaderFixingChanged));

        /// <summary>
        /// Gets the EnableHeaderFixing property. This dependency property 
        /// indicates ....
        /// </summary>
        public static bool GetIsEnabled(DependencyObject d)
        {
            return (bool) d.GetValue(IsEnabledProperty);
        }

        /// <summary>
        /// Sets the EnableHeaderFixing property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetIsEnabled(DependencyObject d, bool value)
        {
            d.SetValue(IsEnabledProperty, value);
        }

        /// <summary>
        /// Handles changes to the EnableHeaderFixing property.
        /// </summary>
        private static void OnEnableHeaderFixingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var targetItemsControl = (Control) d;

            if ((bool) e.NewValue)
            {
                targetItemsControl.Loaded += OnItemsControlLoaded;
                targetItemsControl.Unloaded += OnItemsControlUnloaded;
            }
            else
            {
                targetItemsControl.Loaded -= OnItemsControlLoaded;
                targetItemsControl.Unloaded -= OnItemsControlUnloaded;
            }
        }

        private static void OnItemsControlLoaded(object sender, RoutedEventArgs args)
        {
            var targetItemsControl = (ItemsControl) sender;

            // find the ScrollViewer
            var parentScrollViewer = targetItemsControl.FindChild<ScrollViewer>();
            if (parentScrollViewer == null)
            {
                return;
            }
            parentScrollViewer.ScrollChanged += OnScrollChanged;
        }

        private static void OnItemsControlUnloaded(object sender, RoutedEventArgs args)
        {
            var targetItemsControl = (ItemsControl) sender;

            // find the ScrollViewer
            var parentScrollViewer = targetItemsControl.FindChild<ScrollViewer>();
            if (parentScrollViewer == null)
            {
                return;
            }
            parentScrollViewer.ScrollChanged -= OnScrollChanged;
        }

        #endregion

        #endregion

        #region "Methods"

        private static void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var viewer = (ScrollViewer) sender;

            //find the adorner layer on which we will store items
            AdornerLayer adornerLayer;
            var scrollContentPresenter = viewer.Template.FindName("PART_ScrollContentPresenter", viewer) as ScrollContentPresenter;
            if (scrollContentPresenter != null)
            {
                adornerLayer = scrollContentPresenter.AdornerLayer;
            }
            else
            {
                return; // no need to continue
            }


            var itemsControl = viewer.FindParent<ItemsControl>();
            var headerTemplate = GetHeaderTemplate(itemsControl);

            var generator = itemsControl.ItemContainerGenerator;
            var items = generator.Items.ToList();

            // find the view rect
            var viewRect = new Rect(new Point(0, 0), scrollContentPresenter.RenderSize);

            foreach (var item in items)
            {
                // if no container is defined.
                var container = generator.ContainerFromItem(item) as GroupItem;
                if (container == null)
                {
                    continue;
                }

                // Is the container in view?
                var childTransform = container.TransformToAncestor(scrollContentPresenter);
                var rect = childTransform.TransformBounds(
                    new Rect(new Point(0, 0), container.RenderSize));

                // intersects our child rect with the scroll viewer rect?
                var result = Rect.Intersect(viewRect, rect);

                var displayAdorner = true;
                displayAdorner &= result != Rect.Empty;
                displayAdorner &= rect.Top < 0;

                // the child is partially or completely in view
                if (displayAdorner)
                {
                    double top = Math.Abs(rect.Top);

                    // if we already have an adorner, update position
                    var headerAdorner = GetAdorner(container);
                    if (headerAdorner != null)
                    {
                        headerAdorner.UpdateLocation(top);
                        return;
                    }

                    // else, construct a new one.
                    var adorner = new HeaderAdorner(container)
                    {
                        DataContext = item,
                        HeaderTemplate = headerTemplate,
                        Top = top
                    };
                    adornerLayer.Add(adorner);

                    // save a reference to this element,
                    CurrentGroupItem.Add(container, new WeakReference<HeaderAdorner>(adorner));
                }
                else
                {
                    // locate and remove the adorner
                    var adorner = GetAdorner(container);
                    if (adorner != null)
                    {
                        adornerLayer.Remove(adorner);
                        CurrentGroupItem.Remove(container);
                    }
                }
            }
        }

        private static HeaderAdorner GetAdorner(GroupItem container)
        {
            if (CurrentGroupItem.ContainsKey(container))
            {
                HeaderAdorner adorner;
                if (CurrentGroupItem[container].TryGetTarget(out adorner))
                {
                    return adorner;
                }
            }

            return null;
        }

        #endregion
    }
}