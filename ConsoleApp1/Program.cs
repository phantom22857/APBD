public interface IHazardNotifier
{
    void NotifyHazard(string containerNumber);
}

public abstract class Container
{
    public string SerialNumber { get; protected set; }
    public double Mass { get; protected set; }
    public double Height { get; protected set; }
    public double TareWeight { get; protected set; }
    public double Depth { get; protected set; }
    protected virtual string GenerateSerialNumber()
    {
        return "KON-" + GetType().Name[0] + "-" + Guid.NewGuid().ToString().Substring(0, 4);
    }
}

public class LiquidContainer : Container, IHazardNotifier
{
    public double MaxPayload { get; protected set; }
    public double Pressure { get; protected set; }
    private bool isHazardous;

    public LiquidContainer(double mass, double height, double tareWeight, double depth, double maxPayload, double pressure)
    {
        Mass = mass;
        Height = height;
        TareWeight = tareWeight;
        Depth = depth;
        MaxPayload = maxPayload;
        Pressure = pressure;
        SerialNumber = GenerateSerialNumber();
    }

    public void LoadCargo(double cargoMass)
    {
        if (cargoMass > MaxPayload)
            throw new OverfillException();
        Console.WriteLine($"Cargo of {cargoMass} kg loaded into liquid container {SerialNumber}.");
    }

    public void NotifyHazard(string containerNumber)
    {
        // Basic notification implementation
        Console.WriteLine($"Hazardous situation detected in container {containerNumber}.");
    }
}

public class GasContainer : Container, IHazardNotifier
{
    public double MaxPayload { get; protected set; }
    public double Pressure { get; protected set; }
    private bool isHazardous;

    public GasContainer(double mass, double height, double tareWeight, double depth, double maxPayload, double pressure)
    {
        Mass = mass;
        Height = height;
        TareWeight = tareWeight;
        Depth = depth;
        MaxPayload = maxPayload;
        Pressure = pressure;
        SerialNumber = GenerateSerialNumber();
    }

    public void LoadCargo(double cargoMass)
    {
        if (cargoMass > MaxPayload)
            throw new OverfillException();
        
        Console.WriteLine($"Cargo of {cargoMass} kg loaded into gas container {SerialNumber}.");
    }

    public void NotifyHazard(string containerNumber)
    {
        Console.WriteLine($"Hazardous situation detected in container {containerNumber}.");
    }
}

public class RefrigeratedContainer : Container
{
    public string ProductType { get; protected set; }
    public double Temperature { get; protected set; }

    public RefrigeratedContainer(double mass, double height, double tareWeight, double depth, string productType, double temperature)
    {
        Mass = mass;
        Height = height;
        TareWeight = tareWeight;
        Depth = depth;
        ProductType = productType;
        Temperature = temperature;
        SerialNumber = GenerateSerialNumber();
    }
}

public class OverfillException : Exception
{
    public OverfillException() : base("Cargo exceeds container capacity.") { }
}

public class ContainerShip
{
    public string Name { get; protected set; }
    public double MaxSpeed { get; protected set; }
    public int MaxContainerNum { get; protected set; }
    public double MaxWeight { get; protected set; }
    public List<Container> Containers { get; protected set; }

    public ContainerShip(string name, double maxSpeed, int maxContainerNum, double maxWeight)
    {
        Name = name;
        MaxSpeed = maxSpeed;
        MaxContainerNum = maxContainerNum;
        MaxWeight = maxWeight;
        Containers = new List<Container>();
    }

    public void LoadContainer(Container container)
    {
        if (Containers.Count >= MaxContainerNum)
        {
            Console.WriteLine("Cannot load more containers. Maximum capacity reached.");
            return;
        }

        double currentWeight = Containers.Sum(c => c.Mass);
        if (currentWeight + container.Mass > MaxWeight)
        {
            Console.WriteLine("Cannot load container. Maximum weight exceeded.");
            return;
        }

        Containers.Add(container);
        Console.WriteLine($"Container {container.SerialNumber} loaded onto {Name}.");
    }

    public void UnloadContainer(string containerSerial)
    {
        Container container = Containers.Find(c => c.SerialNumber == containerSerial);
        if (container != null)
        {
            Containers.Remove(container);
            Console.WriteLine($"Container {containerSerial} unloaded from {Name}.");
        }
        else
        {
            Console.WriteLine($"Container {containerSerial} not found on {Name}.");
        }
    }

    public void ReplaceContainer(string containerSerial, Container newContainer)
    {
        UnloadContainer(containerSerial);
        LoadContainer(newContainer);
    }

    public void PrintShipInfo()
    {
        Console.WriteLine($"Ship Name: {Name}");
        Console.WriteLine($"Max Speed: {MaxSpeed} knots");
        Console.WriteLine($"Max Container Capacity: {MaxContainerNum}");
        Console.WriteLine($"Max Weight Capacity: {MaxWeight} tons");
        Console.WriteLine($"Number of Containers Loaded: {Containers.Count}");
        Console.WriteLine("List of Containers:");
        foreach (var container in Containers)
        {
            Console.WriteLine($"- {container.SerialNumber}");
        }
    }
}

class Program
{
    static List<ContainerShip> containerShips = new List<ContainerShip>();
    static void AddContainerShip()
    {
        Console.WriteLine("Adding a container ship...");
        Console.Write("Enter ship name: ");
        string name = Console.ReadLine();

        Console.Write("Enter max speed (knots): ");
        double maxSpeed = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter max container capacity: ");
        int maxContainerNum = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter max weight capacity (tons): ");
        double maxWeight = Convert.ToDouble(Console.ReadLine());

        ContainerShip newShip = new ContainerShip(name, maxSpeed, maxContainerNum, maxWeight);
        containerShips.Add(newShip);

        Console.WriteLine("Container ship added successfully.");
    }
    static void RemoveContainerShip()
    {
        Console.WriteLine("Removing a container ship...");

        if (containerShips.Count == 0)
        {
            Console.WriteLine("No container ships available to remove.");
            return;
        }

        Console.WriteLine("Available container ships:");
        for (int i = 0; i < containerShips.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {containerShips[i].Name}");
        }

        Console.Write("Enter the number of the ship to remove: ");
        int shipIndex = Convert.ToInt32(Console.ReadLine()) - 1;

        if (shipIndex < 0 || shipIndex >= containerShips.Count)
        {
            Console.WriteLine("Invalid ship number.");
            return;
        }

        ContainerShip removedShip = containerShips[shipIndex];
        containerShips.RemoveAt(shipIndex);
        Console.WriteLine($"Container ship '{removedShip.Name}' removed successfully.");
    }
    static void AddContainer()
{
    Console.WriteLine("Adding a container...");

    if (containerShips.Count == 0)
    {
        Console.WriteLine("No container ships available. Please add a container ship first.");
        return;
    }

    Console.WriteLine("Available container ships:");
    for (int i = 0; i < containerShips.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {containerShips[i].Name}");
    }

    Console.Write("Select the number of the container ship to add the container to: ");
    int shipIndex = Convert.ToInt32(Console.ReadLine()) - 1;

    if (shipIndex < 0 || shipIndex >= containerShips.Count)
    {
        Console.WriteLine("Invalid ship number.");
        return;
    }

    ContainerShip selectedShip = containerShips[shipIndex];

    Console.Write("Enter container type (Liquid/Gas/Refrigerated): ");
    string containerType = Console.ReadLine();

    Container container;
    switch (containerType.ToLower())
    {
        case "liquid":
            container = CreateLiquidContainer();
            break;

        case "gas":
            container = CreateGasContainer();
            break;

        case "refrigerated":
            container = CreateRefrigeratedContainer();
            break;

        default:
            Console.WriteLine("Invalid container type.");
            return;
    }

    selectedShip.LoadContainer(container);
}

static LiquidContainer CreateLiquidContainer()
{
    Console.Write("Enter mass (kg): ");
    double mass = Convert.ToDouble(Console.ReadLine());

    Console.Write("Enter height (cm): ");
    double height = Convert.ToDouble(Console.ReadLine());

    Console.Write("Enter tare weight (kg): ");
    double tareWeight = Convert.ToDouble(Console.ReadLine());

    Console.Write("Enter depth (cm): ");
    double depth = Convert.ToDouble(Console.ReadLine());

    Console.Write("Enter max payload (kg): ");
    double maxPayload = Convert.ToDouble(Console.ReadLine());

    Console.Write("Enter pressure (atm): ");
    double pressure = Convert.ToDouble(Console.ReadLine());

    return new LiquidContainer(mass, height, tareWeight, depth, maxPayload, pressure);
}

static GasContainer CreateGasContainer()
{
    Console.Write("Enter mass (kg): ");
    double mass = Convert.ToDouble(Console.ReadLine());

    Console.Write("Enter height (cm): ");
    double height = Convert.ToDouble(Console.ReadLine());

    Console.Write("Enter tare weight (kg): ");
    double tareWeight = Convert.ToDouble(Console.ReadLine());

    Console.Write("Enter depth (cm): ");
    double depth = Convert.ToDouble(Console.ReadLine());

    Console.Write("Enter max payload (kg): ");
    double maxPayload = Convert.ToDouble(Console.ReadLine());

    Console.Write("Enter pressure (atm): ");
    double pressure = Convert.ToDouble(Console.ReadLine());

    return new GasContainer(mass, height, tareWeight, depth, maxPayload, pressure);
}

static RefrigeratedContainer CreateRefrigeratedContainer()
{
    Console.Write("Enter mass (kg): ");
    double mass = Convert.ToDouble(Console.ReadLine());

    Console.Write("Enter height (cm): ");
    double height = Convert.ToDouble(Console.ReadLine());

    Console.Write("Enter tare weight (kg): ");
    double tareWeight = Convert.ToDouble(Console.ReadLine());

    Console.Write("Enter depth (cm): ");
    double depth = Convert.ToDouble(Console.ReadLine());

    Console.Write("Enter product type: ");
    string productType = Console.ReadLine();

    Console.Write("Enter temperature (°C): ");
    double temperature = Convert.ToDouble(Console.ReadLine());

    return new RefrigeratedContainer(mass, height, tareWeight, depth, productType, temperature);
}

    
    static void Main(string[] args)
    {
        
        ContainerShip ship = new ContainerShip("Ship 1", 10, 100, 40000);

        LiquidContainer liquidContainer = new LiquidContainer(1000, 200, 100, 150, 5000, 2.5);
        GasContainer gasContainer = new GasContainer(1500, 250, 120, 180, 6000, 3.0);
        RefrigeratedContainer refrigeratedContainer = new RefrigeratedContainer(2000, 300, 150, 200, "Bananas", 5.0);

        try
        {
            liquidContainer.LoadCargo(4000);
            gasContainer.LoadCargo(5000);
        }
        catch (OverfillException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        ship.LoadContainer(liquidContainer);
        ship.LoadContainer(gasContainer);
        ship.LoadContainer(refrigeratedContainer);

        ship.PrintShipInfo();

        while (true)
        {   
            
            Console.WriteLine("\nEnter what you want to do (1/2/3):");
            Console.WriteLine("1. Add a container ship");
            Console.WriteLine("2. Remove a container ship");
            Console.WriteLine("3. Add a container");
            Console.WriteLine("4. Exit");

            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    AddContainerShip();
                    break;

                case 2:
                    RemoveContainerShip();
                    break;

                case 3:
                    AddContainer();
                    break;

                case 4:
                    Console.WriteLine("Exiting the program...");
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Invalid choice. Please choose again.");
                    break;
            }
        }
    }
}


