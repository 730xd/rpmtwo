using System;
using System.Collections.Generic;

namespace rpmtwo.Models;

public partial class Рейсы
{
    public int Id { get; set; }

    public string НомерРейса { get; set; } = null!;

    public string ПунктНазначения { get; set; } = null!;

    public DateTime ВремяВылета { get; set; }

    public DateTime ВремяПрибытия { get; set; }

    public int СвободныхМест { get; set; }

    public string ТипСамолета { get; set; } = null!;

    public int Вместимость { get; set; }
}
