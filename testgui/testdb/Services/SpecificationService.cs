using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using testdb.Data;
using testdb.Models;

namespace testdb.Services;
public class SpecificationService
{
    private readonly AppDbContext _db;

    public SpecificationService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ObservableCollection<SpecItem>> GetAllAsync()
    {
        var all = await _db.SpecItems.ToListAsync();

        var lookup = all.ToLookup(x => x.ParentId);

        foreach (var item in all)
        {
            item.Children = new ObservableCollection<SpecItem>(lookup[item.Id]);
        }

        var roots = lookup[null];

        return new ObservableCollection<SpecItem>(roots);
    }
}