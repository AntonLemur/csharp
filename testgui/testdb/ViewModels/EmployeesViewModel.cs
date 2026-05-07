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
using System.Threading.Tasks;
using testdb.Data;
using testdb.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using testdb.Abstractions; // Для работы с коллекциями

namespace testdb.ViewModels;

public partial class EmployeesViewModel : ViewModelBase, IAsyncInitializable
{
    // Список, который автоматически обновит таблицу на экране
    private readonly EmployeeService _service;

    [ObservableProperty]
    private ObservableCollection<Employee> employees = [];

    public EmployeesViewModel(EmployeeService service)
    {
        _service = service;
    }

    [RelayCommand]
    public async Task InitializeAsync()
    {
        var data = await _service.GetAllAsync();
        Employees = new ObservableCollection<Employee>(data);
    }
}
