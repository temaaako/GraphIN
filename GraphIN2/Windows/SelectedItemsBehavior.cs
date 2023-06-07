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
using System.Collections.ObjectModel;

namespace GraphIN2.Windows
{
    public static class SelectedItemsBehavior
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItems",
                typeof(ObservableCollection<string>),
                typeof(SelectedItemsBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsChanged));

        public static ObservableCollection<string> GetSelectedItems(DependencyObject obj)
        {
            return (ObservableCollection<string>)obj.GetValue(SelectedItemsProperty);
        }

        public static void SetSelectedItems(DependencyObject obj, ObservableCollection<string> value)
        {
            obj.SetValue(SelectedItemsProperty, value);
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox listBox)
            {
                listBox.SelectionChanged += ListBox_SelectionChanged;
            }
        }

        private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItems is IList selectedItems)
            {
                var selectedItemsCollection = GetSelectedItems(listBox);
                selectedItemsCollection.Clear();
                foreach (var item in selectedItems)
                {
                    selectedItemsCollection.Add((string)item);
                }
            }
        }
    }



}

