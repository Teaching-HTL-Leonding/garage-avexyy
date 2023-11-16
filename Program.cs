using System;
using System.Text;

public class ParkingSpot
{
    public string LicensePlate { get; }
    public DateTime EntryDate { get; }

    public ParkingSpot(string licensePlate, DateTime entryDate)
    {
        LicensePlate = licensePlate;
        EntryDate = entryDate;
    }
}

public class Garage
{
    public ParkingSpot[] ParkingSpots { get; } = new ParkingSpot[50];

    public bool IsOccupied(int parkingSpotNumber)
    {
        return ParkingSpots[parkingSpotNumber - 1] != null;
    }

    public bool TryOccupy(int parkingSpotNumber, string licensePlate, DateTime entryTime)
    {
        if (IsOccupied(parkingSpotNumber))
            return false;

        ParkingSpots[parkingSpotNumber - 1] = new ParkingSpot(licensePlate, entryTime);
        return true;
    }

    public bool TryExit(int parkingSpotNumber, DateTime exitTime, out decimal costs)
    {
        costs = 0;
        if (!IsOccupied(parkingSpotNumber))
            return false;

        var entryTime = ParkingSpots[parkingSpotNumber - 1].EntryDate;
        var duration = exitTime - entryTime;

        if (duration.TotalMinutes <= 15)
        {
            ParkingSpots[parkingSpotNumber - 1] = null;
            return true;
        }

        costs = CalculateCosts(duration);
        ParkingSpots[parkingSpotNumber - 1] = null;
        return true;
    }

    private static decimal CalculateCosts(TimeSpan duration)
    {
        int halfHours = (int)Math.Ceiling(duration.TotalMinutes / 30);
        return halfHours * 3;
    }

    public string GenerateReport()
    {
        var report = new StringBuilder();
        report.AppendLine("| Spot | License Plate |");
        report.AppendLine("| ---- | ------------- |");

        for (int i = 0; i < ParkingSpots.Length; i++)
        {
            var spotInfo = ParkingSpots[i]?.LicensePlate ?? string.Empty;
            report.AppendLine($"| {i + 1,4} | {spotInfo,-13} |");
        }
        
        return report.ToString();
    }
}

public class Program
{
    public static void Main()
    {
        var garage = new Garage();

        Console.WriteLine("What do you want to do?");
        Console.WriteLine("1) Enter a car entry");
        Console.WriteLine("2) Enter a car exit");
        Console.WriteLine("3) Generate report");
        Console.WriteLine("4) Exit");

        while (true)
        {
            Console.Write("\nYour selection: ");
            var selection = Console.ReadLine();

            switch (selection)
            {
                case "1": EnterCarEntry(garage); break;
                case "2": EnterCarExit(garage); break;
                case "3": Console.WriteLine(garage.GenerateReport()); break;
                case "4":
                    Console.WriteLine("Good bye!");
                    return;
                default:
                    Console.WriteLine("Invalid selection. Please try again.");
                    break;
            }
        }
    }

    public static void EnterCarEntry(Garage garage)
    {
        Console.Write("Enter parking spot number: ");
        int spotNumber = int.Parse(Console.ReadLine() ?? string.Empty);

        if (garage.IsOccupied(spotNumber))
        {
            Console.WriteLine("Parking spot is occupied");
            return;
        }

        Console.Write("Enter license plate: ");
        string licensePlate = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter entry date/time (e.g. JJJJ-MM-DDT09:00:00): ");
        DateTime entryTime = DateTime.Parse(Console.ReadLine() ?? string.Empty);

        if (garage.TryOccupy(spotNumber, licensePlate, entryTime))
        {
            Console.WriteLine("Car entry recorded.");
        }
    }

    public static void EnterCarExit(Garage garage)
    {
        Console.Write("Enter parking spot number: ");
        int spotNumber = int.Parse(Console.ReadLine() ?? string.Empty);

        if (!garage.IsOccupied(spotNumber))
        {
            Console.WriteLine("Parking spot is not occupied");
            return;
        }

        Console.Write("Enter exit date/time (e.g. JJJJ-MM-DDT09:00:00): ");
        DateTime exitTime = DateTime.Parse(Console.ReadLine() ?? string.Empty);

        if (garage.TryExit(spotNumber, exitTime, out decimal costs))
        {
            Console.WriteLine($"Costs are {costs}€");
        }
    }
}