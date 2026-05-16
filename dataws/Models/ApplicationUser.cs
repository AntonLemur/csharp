using System.Collections.Generic;
using DataApi.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


//User = логин/пароль
// Identity автоматически создаёт таблицы
// Таблица	Назначение
// AspNetUsers	пользователи
// AspNetRoles	роли
// AspNetUserRoles	связь пользователей и ролей
// AspNetUserClaims	claims
// AspNetUserTokens	токены
// AspNetUserLogins	внешние логины
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }

    // KYC
    public string KycStatus { get; set; } // Pending / Approved / Rejected
    public string PassportFilePath { get; set; }
}