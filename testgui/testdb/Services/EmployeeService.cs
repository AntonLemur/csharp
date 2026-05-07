using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using testdb.Data;
using testdb.Models;

namespace testdb.Services;
public class EmployeeService
{
    private readonly AppDbContext _db;

    public EmployeeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        return await _db.Employees
                .FromSqlRaw(@"select Id, FIRST_NAME, MIDDLE_NAME, LAST_NAME, 0 FIRM_ID, 0 IS_PHOTO from WTT_EMPLOYEES_ALL_S")
                .ToListAsync(); 
    }
}