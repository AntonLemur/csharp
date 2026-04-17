using System;

namespace DataApi.Models
{
    public class DataItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class Employee
    {
        public long Id { get; set; }
        public string FIRST_NAME { get; set; }
        public string MIDDLE_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public long? FIRM_ID { get; set; }
        public short IS_PHOTO { get; set; }
    }
    
    public class EmployeePhoto
    {
        public byte[] PHOTO { get; set; }
    }
    public class EmployeeLate
    {
        public int Id { get; set; }
        public int Count { get; set; }
    }

    //Код для других проектов - не для теко
    //Курьерская программа

    public class OrderCourier
    {
        public long Id { get; set; }
        public DateTime Order_Date { get; set; } 
        public float Amount { get; set; }    
        public short Status { get; set; }     
        public string Firstname { get; set; }
        public string Lastname { get; set; } 
        public string Address { get; set; }  
        public string Email { get; set; }      
    }
}