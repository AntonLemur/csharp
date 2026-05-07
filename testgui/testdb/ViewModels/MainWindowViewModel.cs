using System.Collections.ObjectModel;
using System.Windows.Input;
// using ReactiveUI; // Это стандартная библиотека в Avalonia для команд
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using FirebirdSql.Data.FirebirdClient;
using testdb.Models;
using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace testdb.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // public string Greeting { get; } = "Welcome to Avalonia!";

    private readonly NavigationService _nav;

    public MainWindowViewModel(NavigationService nav)
    {
         _nav = nav;
   }

    private ViewModelBase? _currentPage;
    
    public ViewModelBase? CurrentPage
    {
        get => _currentPage;
        // SetProperty сам проверит изменения и уведомит XAML
        set => SetProperty(ref _currentPage, value); 
    }

    // Команда для кнопки "Сотрудники"
    [RelayCommand]
    private async Task ShowEmployees()
    {
        CurrentPage = await _nav.NavigateTo<EmployeesViewModel>();
    }

    // Команда для кнопки "Спецификации"
    [RelayCommand]
    private async Task ShowSpecifications()
    {
        CurrentPage = await _nav.NavigateTo<SpecificationsViewModel>(); 
    }
}
