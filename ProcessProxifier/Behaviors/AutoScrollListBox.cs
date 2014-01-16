using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace ProcessProxifier.Behaviors
{
    public class AutoScrollListBox : DependencyObject
    {
        public static readonly DependencyProperty AutoScrollProperty =
                                                    DependencyProperty.RegisterAttached(
                                                            "AutoScroll",
                                                            typeof(bool),
                                                            typeof(AutoScrollListBox),
                                                            new UIPropertyMetadata(default(bool), OnAutoScrollChanged));


        public static bool GetAutoScroll(DependencyObject dp)
        {
            return (bool)dp.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(DependencyObject dp, bool value)
        {
            dp.SetValue(AutoScrollProperty, value);
        }

        public static void OnAutoScrollChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var val = (bool)e.NewValue;
            var lb = s as ListView;
            if (lb == null)
                throw new InvalidOperationException("This behavior can only be attached to a ListView.");

            var ic = lb.Items;
            var data = ic.SourceCollection as INotifyCollectionChanged;
            if (data == null) return;

            var autoscroller = new NotifyCollectionChangedEventHandler(
                (s1, e1) =>
                {
                    var selectedItem = default(object);
                    switch (e1.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                        case NotifyCollectionChangedAction.Move: selectedItem = e1.NewItems[e1.NewItems.Count - 1]; break;
                        case NotifyCollectionChangedAction.Remove: if (ic.Count < e1.OldStartingIndex) { selectedItem = ic[e1.OldStartingIndex - 1]; } else if (ic.Count > 0) selectedItem = ic[0]; break;
                        case NotifyCollectionChangedAction.Reset: if (ic.Count > 0) selectedItem = ic[0]; break;
                    }

                    if (selectedItem == default(object)) return;
                    ic.MoveCurrentTo(selectedItem);
                    lb.ScrollIntoView(selectedItem);
                });

            if (val) data.CollectionChanged += autoscroller;
            else data.CollectionChanged -= autoscroller;
        }
    }
}