using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using DevExpress.Utils;
using System.Diagnostics;

namespace GraphIN2.Windows
{
    public class SelectedItemsBahavior
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached("SelectedItems",

             typeof(INotifyCollectionChanged), typeof(SelectedItemsBahavior),
            new PropertyMetadata(default(IList), OnSelectedItemsChanged));

        public static void SetSelectedItems(DependencyObject d, INotifyCollectionChanged value)
        {
            d.SetValue(SelectedItemsProperty, value);
        }

        public static IList GetSelectedItems(DependencyObject element)
        {
            return (IList)element.GetValue(SelectedItemsProperty);
        }

        private static void OnSelectedItemsChanged(DependencyObject d,
                          DependencyPropertyChangedEventArgs e)
        {
            IList selectedItems = null;
            void CollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs args)
            {
                if (args.OldItems != null)
                    foreach (var item in args.OldItems)
                        if (selectedItems.Contains(item))
                            selectedItems.Remove(item);

                if (args.NewItems != null)
                    foreach (var item in args.NewItems)
                        if (!selectedItems.Contains(item))
                            selectedItems.Add(item);
            };

            if (d is MultiSelector multiSelector)
            {
                selectedItems = multiSelector.SelectedItems;
                multiSelector.SelectionChanged += OnSelectionChanged;
            }
            if (d is ListBox listBox)
            {
                selectedItems = listBox.SelectedItems;
                listBox.SelectionMode = SelectionMode.Multiple;
                listBox.SelectionChanged += OnSelectionChanged;
            }
            if (selectedItems == null) return;

            if (e.OldValue is INotifyCollectionChanged)
                (e.OldValue as INotifyCollectionChanged).CollectionChanged
                               -= CollectionChangedEventHandler;
            if (e.NewValue is INotifyCollectionChanged)
                (e.NewValue as INotifyCollectionChanged).CollectionChanged
                               += CollectionChangedEventHandler;

            
        }

        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            var s = sender as DependencyObject;
            if (!GetIsBusy(s))
            {
                SetIsBusy(s, true);
                var list = GetSelectedItems((DependencyObject)sender);
                foreach (var item in e.RemovedItems)
                    if (list.Contains(item)) list.Remove(item);
                foreach (var item in e.AddedItems)
                    if (!list.Contains(item)) list.Add(item);
                SetIsBusy(s, false);
                foreach (var item in list)
                {
                    Debug.WriteLine(item);
                }
            }

        }

        private static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.RegisterAttached("IsBusy", typeof(bool),

                  typeof(SelectedItemsBahavior), new PropertyMetadata(default(bool)));

        private static void SetIsBusy(DependencyObject element, bool value)
        {
            element.SetValue(IsBusyProperty, value);
        }

        private static bool GetIsBusy(DependencyObject element)
        {
            return (bool)element.GetValue(IsBusyProperty);
        }
    }
}
