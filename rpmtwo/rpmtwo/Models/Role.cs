using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace rpmtwo.Models;

[Table("Role")]
[Index("RoleName", Name = "UQ__Role__8A2B616074823713", IsUnique = true)]
public partial class Role
{
    [Key]
    [Column("RoleID")]
    public int RoleId { get; set; }

    [StringLength(50)]
    public string RoleName { get; set; } = null!;

    [InverseProperty("UserRoleNavigation")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
