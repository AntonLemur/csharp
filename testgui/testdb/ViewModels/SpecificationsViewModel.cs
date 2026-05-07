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
using testdb.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using testdb.Abstractions; // Для работы с коллекциями

namespace testdb.ViewModels;

public partial class SpecificationsViewModel : ViewModelBase, IAsyncInitializable
{
     public HierarchicalTreeDataGridSource<SpecItem>? Specifications { get; private set; }

    private readonly SpecificationService _service;

    public SpecificationsViewModel(SpecificationService service)
    {
        _service = service;
    }

    public async Task InitializeAsync() 
    { 

                // 2. Получаем данные из базы (упрощенно)
        var roots = await _service.GetAllAsync(); 
 
        // 3. ВСТАВЛЯЕМ КОД СЮДА: Создаем "источник" для грида
        Specifications = new HierarchicalTreeDataGridSource<SpecItem>(roots)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<SpecItem>(
                    new TextColumn<SpecItem, string>("Наименование", x => x.Name, width: new GridLength(1, GridUnitType.Star)),
                    x => x.Children),
                new TextColumn<SpecItem, string>("Название", x => x.Name, width: new GridLength(100)),
                new TextColumn<SpecItem, string>("Артикул", x => x.Articul, width: new GridLength(100)),
                new TextColumn<SpecItem, decimal>("Кол-во", x => x.Quantity, width: new GridLength(80)),
                new TextColumn<SpecItem, string>("Ед. изм.", x => x.Unit, width: new GridLength(60)),
            }
        };
    }
}
