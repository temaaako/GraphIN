using GraphIN2.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GraphIN2.Windows
{
    /// <summary>
    /// Interaction logic for GraphSettingsWindow.xaml
    /// </summary>
    public partial class GraphSettingsWindow : Window
    {

        public GraphSettingsWindow()
        {
            InitializeComponent();
            Closing += Window_Closing;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.WriteLine("window closing");
            foreach (var item in ((GraphSettingsVM)DataContext).SelectedItems)
            {
                Debug.WriteLine(item.Name);
            }; // Сохранение выбранных элементов при закрытии окна
        }
    }
}
