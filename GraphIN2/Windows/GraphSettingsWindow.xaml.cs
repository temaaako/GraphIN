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

        public GraphSettingsVM viewModel;
        public GraphSettingsWindow()
        {
            InitializeComponent();
            
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //viewModel.SaveSelectedItems(); // Сохранение выбранных элементов при закрытии окна
        }
    }
}
