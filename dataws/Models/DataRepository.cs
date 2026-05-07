using System.Collections.Generic;
using System.Threading.Tasks;
using DataApi.Models;
using Microsoft.EntityFrameworkCore;

public interface IDataRepository
{
    Task<List<DataItem>> GetAll();
}

public class DataRepository: IDataRepository
{
    private readonly DataContext _context;

    public DataRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<List<DataItem>> GetAll()
    {
        return await _context.DataItems
            .FromSqlRaw("select Id, last_name Name from WTT_EMPLOYEES_ALL_S")
            .ToListAsync();
    }
}