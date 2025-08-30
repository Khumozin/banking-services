using System.ComponentModel.DataAnnotations.Schema;

namespace AccountService.Domain.Common;

public class BaseDomainEntity
{
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}