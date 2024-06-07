using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ManuTalent.Mom.Semi.FrontEnd.EntityFrameworkCore.Models;

[Table("fab_lot_trans_setting")]
[Index("LotName", Name = "idx_LotName")]
public partial class FabLotTransSetting
{
    [Key]
    [MySqlCharSet("ascii")]
    [MySqlCollation("ascii_general_ci")]
    public Guid SysId { get; set; }

    [StringLength(20)]
    public string LotName { get; set; } = null!;

    public int DefaultQuantity { get; set; }

    [StringLength(20)]
    public string Plan { get; set; } = null!;

    [StringLength(20)]
    public string Product { get; set; } = null!;

    [ForeignKey("LotName")]
    [InverseProperty("FabLotTransSettings")]
    public virtual MomLot LotNameNavigation { get; set; } = null!;
}
