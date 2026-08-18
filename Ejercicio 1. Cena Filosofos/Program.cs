using System;
using System.Threading;

// se declaran las clases con sus atributos 
public class Fork
{
    public int Id { get; }

    public Fork(int id)
    {
        Id = id;
    }
}

public class Philosopher
{
    private int id;
    private Fork leftFork;
    private Fork rightFork;

    public Philosopher(int id, Fork leftFork, Fork rightFork)
    {
        this.id = id;
        this.leftFork = leftFork;
        this.rightFork = rightFork;
    }

    public void Run()
    {
        for (int i = 0; i < 3; i++)
        {
            Think();
            Eat();
        }

        Console.WriteLine($"El filosofo {id} termino de cenar.");
    }

    private void Think()
    {
        Console.WriteLine($"El filosofo {id} esta pensando");
        Thread.Sleep(1000);
    }

    private void Eat()
    {
        Fork firstFork;
        Fork secondFork;

        //se evita el deadlock porque se eligen los tenedores desde el id mas chiquito
        if (leftFork.Id < rightFork.Id)
        {
            firstFork = leftFork;
            secondFork = rightFork;
        }
        else
        {
            firstFork = rightFork;
            secondFork = leftFork;
        }

        // se bloquea el primer tenedor y luego el segundo para 
        // que cada filosofo pueda comer sin que se genere un deadlock
        lock (firstFork)
        {
            Console.WriteLine(
                $"El filosofo {id} agarro el tenedor {firstFork.Id}."
            );

            lock (secondFork)
            {
                Console.WriteLine(
                    $"El filosofo {id} agarro el tenedor {secondFork.Id}."
                );

                Console.WriteLine(
                    $"El filosofo {id} esta comiendo."
                );

                Thread.Sleep(1000);

                Console.WriteLine(
                    $"El filosofo {id} solto el tenedor {secondFork.Id}."
                );
            }

            Console.WriteLine(
                $"El filosofo {id} solto el tenedor {firstFork.Id}."
            );
        }
    }
}

public class MainApplication
{
    public static void Main(string[] args)
    {
        const int numPhilosophers = 5;

        Fork[] forks = new Fork[numPhilosophers];
        Philosopher[] philosophers = new Philosopher[numPhilosophers];
        Thread[] threads = new Thread[numPhilosophers];

        //crear los 5 tenedores
        for (int i = 0; i < numPhilosophers; i++)
        {
            forks[i] = new Fork(i);
        }

        //crear los 5 filosofos
        for (int i = 0; i < numPhilosophers; i++)
        {
            Fork leftFork = forks[i];
            Fork rightFork = forks[(i + 1) % numPhilosophers];

            philosophers[i] =
                new Philosopher(i, leftFork, rightFork);
        }

        //crear los ciclos de los filosofos y su turno de tenedores
        for (int i = 0; i < numPhilosophers; i++)
        {
            threads[i] = new Thread(philosophers[i].Run);
        }

        Console.WriteLine("Dinner started.\n");

        //iniciar el ciclo de los filosofos y su turno de tenedores  
        for (int i = 0; i < numPhilosophers; i++)
        {
            threads[i].Start();
        }

        //esperar a que se recorran los filosofos y se acabe el ciclo 
        for (int i = 0; i < numPhilosophers; i++)
        {
            threads[i].Join();
        }

        Console.WriteLine("\nCena terminada.");
    }
}