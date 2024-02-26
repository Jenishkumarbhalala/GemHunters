using System;

public class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class Player
{
    public string Name { get; set; }
    public Position Position { get; set; }
    public int GemCount { get; set; }

    public Player(string name, Position position)
    {
        Name = name;
        Position = position;
        GemCount = 0;
    }

    public bool Move(char direction)
    {
        switch (direction)
        {
            case 'U':  //move up
                if (Position.Y > 0)
                {
                    Position.Y--;
                    return true;
                }
                break;
            case 'D':  //move down
                if (Position.Y < 5)
                {
                    Position.Y++;
                    return true;
                }
                break;
            case 'L':  //move left
                if (Position.X > 0)
                {
                    Position.X--;
                    return true;
                }
                break;
            case 'R':  //move right
                if (Position.X < 5)
                {
                    Position.X++;
                    return true;
                }
                break;
        }
        return false;
    }
}

public class Cell
{
    public string Occupant { get; set; }

    public Cell(string occupant)
    {
        Occupant = occupant;
    }
}

public class Board
{
    private Cell[,] Grid;

    public Board()
    {
        Grid = new Cell[6, 6];
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        for (int y = 0; y < Grid.GetLength(1); y++)
        {
            for (int x = 0; x < Grid.GetLength(0); x++)
            {
                Grid[x, y] = new Cell("-");
            }
        }

        Grid[0, 0] = new Cell("P1");  //place player on board
        Grid[5, 5] = new Cell("P2");  //place player on board

        Random random = new Random();
        for (int i = 0; i < 5; i++)  //random gems on board
        {
            int gemX, gemY;
            do
            {
                gemX = random.Next(0, 6);
                gemY = random.Next(0, 6);
            } while (Grid[gemX, gemY].Occupant != "-");
            Grid[gemX, gemY] = new Cell("G");
        }

        for (int i = 0; i < 3; i++)  //random obstacles on board
        {
            int obstacleX, obstacleY;
            do
            {
                obstacleX = random.Next(0, 6);
                obstacleY = random.Next(0, 6);
            } while (Grid[obstacleX, obstacleY].Occupant != "-");
            Grid[obstacleX, obstacleY] = new Cell("O");
        }
    }

    //display current status
    public void Display(Player player1, Player player2, int turnsLeftP1, int turnsLeftP2)
    {
        Console.Clear();

        for (int y = 0; y < Grid.GetLength(1); y++)
        {
            for (int x = 0; x < Grid.GetLength(0); x++)
            {
                if (player1.Position.X == x && player1.Position.Y == y)
                    Console.Write("P1 ");
                else if (player2.Position.X == x && player2.Position.Y == y)
                    Console.Write("P2 ");
                else
                    Console.Write(Grid[x, y].Occupant + "  ");
            }
            Console.WriteLine();
        }

        Console.WriteLine($"\nPlayer 1 Gems: {player1.GemCount} | Player 2 Gems: {player2.GemCount}");
        Console.WriteLine($"Turns Left - Player 1: {turnsLeftP1} | Player 2: {turnsLeftP2}");
        Console.WriteLine("U = UP, D = DOWN, L = LEFT, R = RIGHT");
    }

    public bool IsValidMove(Player player, char direction)
    {
        int newX = player.Position.X;
        int newY = player.Position.Y;

        switch (direction)
        {
            case 'U':  //move up
                newY--;
                break;
            case 'D':  //move down
                newY++;
                break;
            case 'L':  //move left
                newX--;
                break;
            case 'R':  //move right
                newX++;
                break;
        }

        if (newX < 0 || newX >= Grid.GetLength(0) || newY < 0 || newY >= Grid.GetLength(1))
            return false;


        if (Grid[newX, newY].Occupant == "O")
            return false;

        return true;
    }

    public void CollectGem(Player player)
    {
        if (Grid[player.Position.X, player.Position.Y].Occupant == "G")
        {
            player.GemCount++;
            Grid[player.Position.X, player.Position.Y] = new Cell("-");
        }
    }
}

public class Game
{
    private Board board;
    private Player player1;
    private Player player2;
    private int totalTurns;

    public Game()
    {
        board = new Board();
        player1 = new Player("Player 1", new Position(0, 0));
        player2 = new Player("Player 2", new Position(5, 5));
        totalTurns = 0;
    }

    public void Start()
    {
        while (!IsGameOver())
        {
            Player currentTurn = (totalTurns % 2 == 0) ? player1 : player2;
            Console.WriteLine($"Current Turn: {currentTurn.Name}");
            board.Display(player1, player2, 15 - totalTurns / 2, 15 - (totalTurns + 1) / 2);

            bool validMove = false;
            do
            {
                Console.Write($"\nEnter move for {currentTurn.Name} (U/D/L/R): ");
                char move = char.ToUpper(Console.ReadKey().KeyChar);
                Console.WriteLine();

                if (move != 'U' && move != 'D' && move != 'L' && move != 'R')
                {
                    Console.WriteLine("\n**Invalid input. Please enter U, D, L, R.");
                    continue;
                }

                if (!board.IsValidMove(currentTurn, move))
                {
                    Console.WriteLine("\n**Invalid move. Please try again.");
                    continue;
                }

                validMove = true;
                currentTurn.Move(move);
                board.CollectGem(currentTurn);

            } while (!validMove);

            totalTurns++;
        }

        AnnounceWinner();
    }



    private bool IsGameOver()
    {
        return totalTurns >= 30 || (player1.GemCount + player2.GemCount == 5);
    }

    private void AnnounceWinner()
    {
        Console.WriteLine("\nGame Over!");
        if (player1.GemCount > player2.GemCount)
        {
            Console.WriteLine($"\n{player1.Name} wins with {player1.GemCount} gems!");
        }
        else if (player2.GemCount > player1.GemCount)
        {
            Console.WriteLine($"\n{player2.Name} wins with {player2.GemCount} gems!");
        }
        else
        {
            Console.WriteLine("\nIt's a tie!");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.Start();
    }
}