using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToe.Datasource.Model;

[Table("games")]
public class GameModel
{
    [Key] [Column("id")] public Guid GameId { get; init; }
    
    [Column("player1")] public Guid Player1Id { get; set; }
    
    [Column("player2")] public Guid? Player2Id { get; set; }

    [Required] [Column("board")] public string BoardJson { get; set; } = "[[0,0,0],[0,0,0],[0,0,0]]";

    [Column("status")] public int Status { get; set; }

    [NotMapped]
    public int[,] Board
    {
        get => Deserialize(BoardJson);
        set => BoardJson = Serialize(value);
    }

    private static string Serialize(int[,] board)
    {
        var result = new int[3][];
        for (int i = 0; i < 3; i++)
        {
            result[i] = new int[3];
            for (int j = 0; j < 3; j++) 
                result[i][j] = board[i, j];
        }
        return System.Text.Json.JsonSerializer.Serialize(result);
    }

    private static int[,] Deserialize(string json)
    {
        var jagged = System.Text.Json.JsonSerializer.Deserialize<int[][]>(json);
        if (jagged is null || jagged.Length == 0) return new int[3, 3];

        var board = new int[3, 3];
        for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
            board[i, j] = jagged[i][j];
        return board;
    }
}