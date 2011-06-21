using System;
using System.Windows;
using System.Windows.Controls;

namespace Kinect.Semaphore.Utils
{
    /// <summary>
    /// ListBoxItem Behavior class
    /// </summary>
    public static class ListViewItemBehaviour
    {
        /// <summary>
        /// Determines if the ListViewItem is bought into view when enabled   
        /// </summary>   
        public static readonly DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
            DependencyProperty.RegisterAttached("IsBroughtIntoViewWhenSelected", typeof (bool),
                                                typeof (ListViewItemBehaviour),
                                                new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

        /// <summary>    
        /// Gets the IsBroughtIntoViewWhenSelected value    
        /// </summary>    
        /// <param name="listViewItem"></param>    
        /// <returns></returns>
        public static bool GetIsBroughtIntoViewWhenSelected(ListViewItem listViewItem)
        {
            return (bool) listViewItem.GetValue(IsBroughtIntoViewWhenSelectedProperty);
        }

        /// <summary>    
        /// Sets the IsBroughtIntoViewWhenSelected value    
        /// </summary>    
        /// <param name="listViewItem"></param>    
        /// <param name="value"></param>
        public static void SetIsBroughtIntoViewWhenSelected(ListViewItem listViewItem, bool value)
        {
            listViewItem.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
        }

        /// <summary>
        /// Action to take when item is brought into view
        /// </summary>
        /// <param name="depObj"></param>
        /// <param name="e"></param>
        private static void OnIsBroughtIntoViewWhenSelectedChanged(DependencyObject depObj,
                                                                   DependencyPropertyChangedEventArgs e)
        {
            var item = depObj as ListViewItem;
            if (item == null)
            {
                return;
            }
            if (e.NewValue is bool == false)
            {
                return;
            }
            if ((bool) e.NewValue)
            {
                item.Selected += OnListBoxItemSelected;
            }
            else
            {
                item.Selected -= OnListBoxItemSelected;
            }
        }

        private static void OnListBoxItemSelected(object sender, RoutedEventArgs e)
        {
            // Only react to the Selected event raised by the ListBoxItem         
            // whose IsSelected property was modified.  Ignore all ancestors         
            // who are merely reporting that a descendant's Selected fired.         
            if (!ReferenceEquals(sender, e.OriginalSource))
            {
                return;
            }

            var item = e.OriginalSource as ListViewItem;
            if (item != null)
            {
                item.BringIntoView();
            }
        }
    }
}