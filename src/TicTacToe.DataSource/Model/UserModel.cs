using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToe.Datasource.Model;

[Table("users")]
public class UserModel
{
    [Key] [Column("id")] public required Guid PlayerId { get; init; }

    [Required]
    [Column("login")]
    [MaxLength(64)]
    public required string Login { get; set; }

    [Required]
    [Column("password")]
    [MaxLength(64)]
    public required string Password { get; set; }
    
    [Column("wins")] public int Wins { get; set; }
    [Column("losses")] public int Losses { get; set; }
    [Column("draws")] public int Draws { get; set; }
}
