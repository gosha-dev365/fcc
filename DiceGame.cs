using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

var gameSettings = new GameSettings
{
    DieCount = 5,
    Iterations = 10,
    ZeroRoll = 3
};

var diceGameRunner = new DiceGameRunner(gameSettings);

Console.WriteLine("Running new dice game!");

diceGameRunner.Run();

public class DiceGameRunner
{
    private readonly GameSettings _gameSettings;

    public DiceGameRunner(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;
    }

    public void Run()
    {
        var gameResults = new List<int>();
        var stopWatch = new Stopwatch();
        
        stopWatch.Start();
        for (var i = 0; i < _gameSettings.Iterations; i++)
        {
            var diceGame = new DiceGame(_gameSettings.ZeroRoll, _gameSettings.DieCount, _gameSettings.DiceSize);
            
            gameResults.Add(diceGame.Run());
        }
        stopWatch.Stop();
        
        Console.WriteLine($"Number of simulations: {_gameSettings.Iterations}, with {_gameSettings.DieCount} dice");
        
        // Group game results by same number and print  
        gameResults
            .GroupBy(result => result)
            .Select(g => new { Number = g.Key, Count = g.Count() })
            .OrderBy(r => r.Number)
            .ToList()
            .ForEach(item => Console.WriteLine($"Total {item.Number} occured {item.Count} times"));

        Console.WriteLine($"Total simulation took: {stopWatch.ElapsedMilliseconds} ms");
        
    }
}

public class DiceGame
{
    private readonly int _zeroRoll;
    private readonly int _dieSize;
    private readonly int _dieCount;
    // initialize logger when available
    //private readonly ILogger _logger;

    public DiceGame(int zeroRoll, int dieCount, int dieSize)
    {
        _zeroRoll = zeroRoll;
        _dieCount = dieCount;
        _dieSize = dieSize;
    }

    public int Run()
    {
        int diceLeft = _dieCount;

        var total = 0;
        while (diceLeft > 0)
        {
            var result = SingleGameRoll(diceLeft);

            diceLeft = diceLeft - result.Item2;
            total = total + result.Item1;
        }
        
        return total;
    }

    private (int,int) SingleGameRoll(int numberOfDice)
    {
        var die = new Die(_dieSize);
        // TODO: (GK) this is a bug, should be set to _dieSize
        var minRoll = 6;
        var diceRemoved = 0;
        
        for (var i = 0; i < numberOfDice; i++)
        {
            var value = die.Roll();
            
            // _logger.Debug(value);

            if (value == _zeroRoll)
            {
                minRoll = 0;
                diceRemoved++;
            }

            if (minRoll > value)
            {
                minRoll = value;
            } 
        }

        // if no 3 rolled, we need to manually remove 1 die
        if (diceRemoved == 0)
        {
            diceRemoved = 1;
        }
        return (minRoll, diceRemoved);
    }
}
public class Die
{
    private readonly int _numberOfSides;
    private readonly Random _randomGenerator;

    public Die(int numberOfSides)
    {
        _numberOfSides = numberOfSides;
        _randomGenerator = new Random();
    }

    // random upper bound is exclusive, need to add 1
    public int Roll() => _randomGenerator.Next(1, _numberOfSides + 1);
}

public record GameSettings
{
    public int DieCount { get; init; }
    
    // Defaulting to 6 for simplicity
    public int DiceSize { get; init; } = 6;
    public int Iterations { get; init; }
    public int ZeroRoll { get; init; }
}
