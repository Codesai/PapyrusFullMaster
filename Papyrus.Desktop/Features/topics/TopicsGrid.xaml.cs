﻿using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Papyrus.Desktop.Features.Topics
{
    /// <summary>
    /// Interaction logic for TopicsGrid.xaml
    /// </summary>
    public partial class TopicsGrid : UserControl
    {
        public TopicsGridVM ViewModel
        {
            get { return (TopicsGridVM)DataContext; }
        }

        public TopicsGrid()
        {
            InitializeComponent();

            DataContext = ViewModelsFactory.TopicsGrid();
            this.Loaded += DocumentsGrid_Loaded;
        }

        private async void DocumentsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.Initialize();
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.RefreshDocuments();
        }

        private void ExportToFolderButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DocumentRow_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
