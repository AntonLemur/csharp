using System;
using Avalonia.Controls;
using testdb.ViewModels;

namespace testdb.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Важнейшая строка:
        // DataContext = new MainWindowViewModel();
    }
}