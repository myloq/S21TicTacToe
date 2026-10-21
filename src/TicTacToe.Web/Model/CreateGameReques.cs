namespace TicTacToe.Web.Model;

public class CreateGameRequest
{
    // "human" или "bot"
    public string OpponentType { get; set; } = "bot"; 
}