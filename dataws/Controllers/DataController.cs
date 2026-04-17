using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DataApi.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;

namespace DataApi.Controllers
{
    public static class LogHelper 
    {
        public static void WriteLog(string path, string message, 
            [System.Runtime.CompilerServices.CallerMemberName] string method = "",
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "")
        {
            // Извлекаем только имя файла без расширения (это и будет имя класса)
            var className = System.IO.Path.GetFileNameWithoutExtension(filePath);

            var logEntry = $"{Environment.NewLine}{DateTime.Now} [{className}.{method}]: {message}";
            System.IO.File.AppendAllText(path, logEntry);
        }
    }

    [Route("api/[controller]")]
    public class TestController : Controller
    {
        public string Test()
        {
            return "Работает!";
        }
    }

    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfigurationRoot Configuration;
        
        public DataController(DataContext context)
        {
            _context = context;

            /*if (_context.DataItems.Count() == 0)
            {
                _context.DataItems.Add(new DataItem { Name = "Item1" });
                _context.SaveChanges();
            }*/

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        // Этот метод вернет JSON (потому что возвращает IEnumerable)        
        [HttpGet]
        public async Task<IEnumerable<DataItem>> GetAll()
        {
            try
            {
                // var employeeList = _context.DataItems.FromSqlRaw("select Id, last_name Name from WTT_EMPLOYEES_ALL_S").ToList();

                var employeeList = await _context.DataItems.FromSqlRaw("select Id, last_name Name from WTT_EMPLOYEES_ALL_S").ToListAsync();
                return employeeList;
            }
            catch (System.Exception ex)
            {
                System.IO.File.AppendAllText(Configuration["Log"], "\n" + 
                    System.DateTime.Now.ToString() + ex.Message);
                return null;
            }
        }

        // Этот метод вернет HTML-страницу (потому что возвращает View)
        [HttpGet("view")]
        public async Task<IActionResult> Index() 
        {
            var employees = await GetAll();
            return View(employees); 
        }        

        [HttpGet("{id}", Name = "GetData")]
        public IActionResult GetById(long id)
        {
            try
            {
                var item = _context.DataItems.FromSqlRaw("select Id, last_name Name from WTT_EMPLOYEES_ALL_S").ToList().FirstOrDefault(t => t.Id == id);
                if (item == null)
                {
                    return NotFound();
                }
                System.IO.File.AppendAllText(Configuration["Log"], "\n" 
                  + System.DateTime.Now.ToString() + ": "+ item.Name);
                return new ObjectResult(item);
            }
            catch (System.Exception ex)
            {
                System.IO.File.AppendAllText(Configuration["Log"], "\n" + 
                    System.DateTime.Now.ToString() + ex.Message);
                return NotFound();
            }
        }

        [HttpGet("[action]")]
        public IEnumerable<Employee> Employees()
        {
            try
            {
                var employeeList = _context.Employees.FromSqlRaw(@"select ID, FIRST_NAME, MIDDLE_NAME, LAST_NAME, FIRM_ID, IS_PHOTO 
                  from  EMPLOYEES_S(0,0,0,1) where LAST_NAME not like 'я%' order by ID").ToList();
                
                System.IO.File.AppendAllText(Configuration["Log"], "\n" + 
                  System.DateTime.Now.ToString() + ": функцией /api/data/employees выбрано " + employeeList.Count.ToString()+" записей");
                
                return employeeList;
            }
            catch (System.Exception ex)
            {
                System.IO.File.AppendAllText(Configuration["Log"], "\n" + 
                  System.DateTime.Now.ToString() + ": в функции /api/data/employees ошибка " + ex.Message);
                return null;
            }
        }

        [HttpGet("[action]/{id}")]
        public /*IEnumerable<EmployeePhoto>*/IActionResult EmployeePhoto(long id)
        {
            _context.Database.GetDbConnection().Open();
            try
            {
                string sql = @"select * from emp_employee_photo_r(@id)";
                // создаём параметры запроса
                var idParam = new FbParameter("id", FbDbType.Integer);
                // создаём SQL команду для обновления записей
                var sqlCommand = _context.Database.GetDbConnection().CreateCommand();
                sqlCommand.CommandText = sql;
                sqlCommand.Parameters.Add(idParam);
                // подготавливаем команду
                sqlCommand.Prepare();
                idParam.Value = id;
                // выполняем sql запрос
                FbDataReader dr = (FbDataReader)sqlCommand.ExecuteReader();
                List<EmployeePhoto> employeephotos = new List<EmployeePhoto>();
                byte[] test = new byte[0];
                while (dr.Read())
                    employeephotos.Add(new EmployeePhoto()
                                    { PHOTO = dr["PHOTO"].Equals(System.DBNull.Value) ? test : (byte[])dr["PHOTO"] });
                _context.Database.GetDbConnection().Close();
                return File(employeephotos[0].PHOTO, "image/jpeg"); //employeephotos;
            }
            catch (System.Exception ex)
            {
                _context.Database.GetDbConnection().Close();
                System.IO.File.AppendAllText(Configuration["Log"], "\n" + 
                  System.DateTime.Now.ToString() + ": в функции /api/data/employeephoto ошибка " + ex.Message);
                return null;
            }
        }

        [HttpGet("[action]/{Date_from}/{Date_to}/{Time}")]
        public IEnumerable<EmployeeLate> EmployeesLate(string Date_from, string Date_to, int Time)
        {
            _context.Database.GetDbConnection().Open();
            try
            {
               string sql = @"select
                                    e.id,
                                    count(e.id)
                                from employees e
                                join WTT_DEPARTMENT_STAFF ds on ds.employee_id=e.id and ds.month_date>=firstdaymonth(@date_b)
                                left outer join WTT_STAFF_RECTIME_S(ds.id) wt on 1=1
                                where wt.DAY_DATE between @date_b and @date_e 
                                    and
                                    cast(cast(wt.FIRST_IN as date) || ' ' || extract(hour from wt.FIRST_IN) || ':' || extract(minute from wt.FIRST_IN) as timestamp)
                                        > dateadd(minute, @t, cast(wt.DAY_DATE as timestamp))
                                group by
                                e.id
                                order by
                                e.id";
                // создаём параметры запроса
                var datebParam = new FbParameter("date_b", FbDbType.Date);
                var dateeParam = new FbParameter("date_e", FbDbType.Date);
                var tParam = new FbParameter("t", FbDbType.Integer);
                // создаём SQL команду для обновления записей
                var sqlCommand = _context.Database.GetDbConnection().CreateCommand();
                sqlCommand.CommandText = sql;
                sqlCommand.Parameters.Add(datebParam);
                sqlCommand.Parameters.Add(dateeParam);
                sqlCommand.Parameters.Add(tParam);
                // подготавливаем команду
                sqlCommand.Prepare();

                System.IO.File.AppendAllText(Configuration["Log"], "\n" + 
                System.DateTime.Now.ToString() + ": " + Date_from  + ", " + Date_to + ", "+Time.ToString());

                datebParam.Value =  Date_from;
                dateeParam.Value = Date_to;
                tParam.Value = Time;

                // выполняем sql запрос
                FbDataReader dr = (FbDataReader)sqlCommand.ExecuteReader();
                List<EmployeeLate> employeeslate = new List<EmployeeLate>();
                while (dr.Read())
                    employeeslate.Add(new EmployeeLate()
                                    { Id = (int)dr["id"], Count=(int)dr["count"] });
                _context.Database.GetDbConnection().Close();

                System.IO.File.AppendAllText(Configuration["Log"], "\n" + 
                System.DateTime.Now.ToString() + ": функцией /api/data/employeeslate выбрано " + employeeslate.Count.ToString()+" записей");

                return employeeslate;
            }
            catch (System.Exception ex)
            {
                _context.Database.GetDbConnection().Close();
                System.IO.File.AppendAllText(Configuration["Log"], "\n" + 
                  System.DateTime.Now.ToString() + ": в функции /api/data/employeeslate ошибка " + ex.Message);
                return null;
            }
        }
        
        /*[HttpPost]
        public IActionResult Create([FromBody] DataItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            _context.DataItems.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetData", new { id = item.Id }, item);
        }*/

        [HttpPost]
        public ActionResult UpdateInsert([FromBody] List<DataItem> Items)
        {
            //отладка
            //foreach (var Item in Items)
            //System.IO.File.AppendAllText(Configuration["Log"], "\n"+Item.Name);

            try
            {
                _context.Database.GetDbConnection().Open();
                using (var dbTransaction = _context.Database.GetDbConnection().BeginTransaction(System.Data.IsolationLevel.Snapshot))
                {
                    try
                    {
                        _context.Database.UseTransaction(dbTransaction);
                        string sql = @"update employees set last_name=@name where id=@id";
                        // создаём параметры запроса
                        var idParam = new FbParameter("id", FbDbType.Integer);
                        var nameParam = new FbParameter("name", FbDbType.VarChar);
                        // создаём SQL команду для обновления записей
                        var sqlCommand = _context.Database.GetDbConnection().CreateCommand();
                        sqlCommand.CommandText = sql;
                        // указываем команде, какую транзакцию использовать
                        sqlCommand.Transaction = dbTransaction;
                        sqlCommand.Parameters.Add(nameParam);
                        sqlCommand.Parameters.Add(idParam);
                        // подготавливаем команду
                        sqlCommand.Prepare();

                        foreach (var Item in Items)
                        {
                            //i.Name =Item.Name;

                            //отладка
                            //System.IO.File.AppendAllText(Configuration["Log"], "\n"+ Item.Name);

                            // инициализируем параметры запроса
                            idParam.Value = Item.Id;
                            nameParam.Value = Item.Name;
                            // выполняем sql запрос
                            sqlCommand.ExecuteNonQuery();

                            //_context.Database.ExecuteSqlCommand(@"update employees set last_name=@name where id=@id", new FbParameter("id", i.Id), 
                            //new FbParameter("name", i.Name));
                        }

                        dbTransaction.Commit();
                        _context.Database.GetDbConnection().Close();
                    }
                    catch (System.Exception ex)
                    {
                        System.IO.File.AppendAllText(Configuration["Log"], "\n" + ex.Message);
                        dbTransaction.Rollback();
                        _context.Database.GetDbConnection().Close();
                        return new NoContentResult();
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.IO.File.AppendAllText(Configuration["Log"], "\n" + ex.Message);
                _context.Database.GetDbConnection().Close();
                return new NoContentResult();
            }
            return new OkResult();
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] DataItem item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var data_item = _context.DataItems.FirstOrDefault(t => t.Id == id);
            if (data_item == null)
            {
                return NotFound();
            }

            data_item.Name = item.Name;

            _context.DataItems.Update(data_item);
            _context.SaveChanges();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var data_item = _context.DataItems.FirstOrDefault(t => t.Id == id);
            if (data_item == null)
            {
                return NotFound();
            }

            _context.DataItems.Remove(data_item);
            _context.SaveChanges();
            return new NoContentResult();
        }

        //Код для других проектов - не для теко
        //Курьерская программа

        [HttpGet("orderscourier")]
        public async Task<IEnumerable<OrderCourier>> GetAllOrders()
        {
            try
            {
                LogHelper.WriteLog(Configuration["Log"], @"SELECT O.ID, O.ORDER_DATE, 
                    O.AMOUNT, O.STATUS, C.FIRSTNAME, C.LASTNAME, C.ADDRESS, C.EMAIL FROM ORDERS O
                    JOIN CUSTOMERS C ON C.ID=O.CUSTOMER_ID");

                var orderList = await _context.OrdersCourier.FromSqlRaw(@"SELECT O.ID, O.ORDER_DATE, 
                    O.AMOUNT, O.STATUS, C.FIRSTNAME, C.LASTNAME, C.ADDRESS, C.EMAIL FROM ORDERS O
                    JOIN CUSTOMERS C ON C.ID=O.CUSTOMER_ID").ToListAsync();
                return orderList;
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(Configuration["Log"], ex.Message);
                return null;
            }
        }

        [HttpPut("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(long id, [FromBody] short newStatus)
        {
            try
            {
                // 1. Находим заказ в базе
                var order = await _context.OrdersCourier.FindAsync(id);
                
                if (order == null)
                {
                    return NotFound($"Заказ с ID {id} не найден");
                }

                // 2. Обновляем поле
                order.Status = newStatus;

                // 3. Сохраняем изменения
                await _context.SaveChangesAsync();

                return Ok(new { message = "Статус успешно обновлен" });
            }
            catch (Exception ex)
            {
                // Логируем ошибку, как вы делали ранее
                return StatusCode(500, "Ошибка при обновлении базы данных");
            }
        }
    }
}