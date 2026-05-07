using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace rpmtwo.Models;

[Table("User")]
[Index("UserUsername", Name = "UQ__User__04C7FD879B4EC447", IsUnique = true)]
[Index("UserLogin", Name = "UQ__User__7F8E8D5E2DE98FC1", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(50)]
    public string UserUsername { get; set; } = null!;

    [StringLength(50)]
    public string UserName { get; set; } = null!;

    [StringLength(50)]
    public string? UserPatronymic { get; set; }

    [StringLength(50)]
    public string UserLogin { get; set; } = null!;

    [StringLength(100)]
    public string UserPassword { get; set; } = null!;

    public int UserRole { get; set; }

    [ForeignKey("UserRole")]
    [InverseProperty("Users")]
    public virtual Role UserRoleNavigation { get; set; } = null!;
}
