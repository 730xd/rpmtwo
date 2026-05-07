using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace rpmtwo.Models;

[Table("Рейсы")]
public partial class Рейсы
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(10)]
    public string НомерРейса { get; set; } = null!;

    [StringLength(100)]
    public string ПунктНазначения { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime ВремяВылета { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ВремяПрибытия { get; set; }

    public int СвободныхМест { get; set; }

    [StringLength(50)]
    public string ТипСамолета { get; set; } = null!;

    public int Вместимость { get; set; }
}
