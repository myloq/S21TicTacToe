using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToe.Datasource.Model;

[Table("users")]
public class UserModel
{
    [Key] [Column("id")] public Guid PlayerId { get; init; }

    [Required]
    [Column("login")]
    [MaxLength(64)]
    public string Login { get; set; }

    [Required]
    [Column("password")]
    [MaxLength(64)]
    public string Password { get; set; }
}
