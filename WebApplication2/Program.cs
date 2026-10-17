using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    // Estruturas de dados globais
    static Dictionary<string, string> reservasIP = new Dictionary<string, string>();
    static Queue<Dispositivo> filaAutenticacao = new Queue<Dispositivo>();
    static List<Dispositivo> dispositivosAtivos = new List<Dispositivo>();
    static Stack<string> historicoAuditoria = new Stack<string>();

    static void Main(string[] args)
    {
        bool executando = true;

        while (executando)
        {
            Console.WriteLine("\n=== SmartNet Manager ===");
            Console.WriteLine("1 - Reservar IP (DHCP)");
            Console.WriteLine("2 - Adicionar dispositivo à fila");
            Console.WriteLine("3 - Processar fila (Autenticar)");
            Console.WriteLine("4 - Exibir Dashboard");
            Console.WriteLine("5 - Imprimir Relatório");
            Console.WriteLine("6 - Aplicar Firewall");
            Console.WriteLine("7 - Desfazer última ação");
            Console.WriteLine("0 - Sair");
            Console.Write("Escolha: ");

            string opcao = Console.ReadLine() ?? string.Empty;

            switch (opcao)
            {
                case "1": ReservarIP(); break;
                case "2": AdicionarFila(); break;
                case "3": ProcessarFila(); break;
                case "4": ExibirDashboard(); break;
                case "5": ImprimirRelatorio(dispositivosAtivos); break;
                case "6": AplicarFirewall(d => d.Fabricante == "Apple"); break; // exemplo
                case "7": DesfazerUltimaAcao(); break;
                case "0": executando = false; break;
                default: Console.WriteLine("Opção inválida."); break;
            }
        }
    }

    // Missão 1: Reservar IP
    static void ReservarIP()
    {
        Console.Write("MAC Address: ");
        string mac = Console.ReadLine() ?? string.Empty;
        Console.Write("IP: ");
        string ip = Console.ReadLine() ?? string.Empty;

        if (!reservasIP.ContainsKey(mac))
        {
            reservasIP[mac] = ip;
            Console.WriteLine("Reserva feita com sucesso!");
        }
        else
        {
            Console.WriteLine("Este MAC já possui IP reservado.");
        }
    }

    // Missão 2: Fila de Autenticação
    static void AdicionarFila()
    {
        Dispositivo d = new Dispositivo();
        Console.Write("Nome: ");
        d.Nome = Console.ReadLine() ?? string.Empty;
        Console.Write("Fabricante: ");
        d.Fabricante = Console.ReadLine() ?? string.Empty;
        Console.Write("MAC: ");
        d.MacAddress = Console.ReadLine() ?? string.Empty;
        Console.Write("IP: ");
        d.EnderecoIP = Console.ReadLine() ?? string.Empty;
        d.DataConexao = DateTime.Now;

        filaAutenticacao.Enqueue(d);
        Console.WriteLine("Dispositivo adicionado à fila.");
    }

    static void ProcessarFila()
    {
        if (filaAutenticacao.Count > 0)
        {
            Dispositivo d = filaAutenticacao.Dequeue();
            dispositivosAtivos.Add(d);
            historicoAuditoria.Push($"Autenticado: {d.Nome} ({d.MacAddress})");
            Console.WriteLine($"Dispositivo {d.Nome} autenticado!");
        }
        else
        {
            Console.WriteLine("Fila vazia.");
        }
    }

    // Missão 3: Dashboard
    static void ExibirDashboard()
    {
        double consumoTotal = dispositivosAtivos.Sum(d => d.ConsumoBanda);
        Console.WriteLine($"Consumo total: {consumoTotal} Mbps");

        var top3 = dispositivosAtivos.OrderByDescending(d => d.ConsumoBanda).Take(3);
        Console.WriteLine("Top 3 consumidores:");
        foreach (var d in top3)
            Console.WriteLine($"{d.Nome} - {d.ConsumoBanda} Mbps");

        var porFabricante = dispositivosAtivos.GroupBy(d => d.Fabricante);
        foreach (var grupo in porFabricante)
            Console.WriteLine($"{grupo.Key}: {grupo.Count()} dispositivos");

        var anomalia = dispositivosAtivos.Where(d => d.ConsumoBanda > 500);
        foreach (var d in anomalia)
            Console.WriteLine($"⚠️ ALERTA: {d.Nome} consumindo {d.ConsumoBanda} Mbps!");
    }

    // Missão 4: Histórico de Auditoria
    static void DesfazerUltimaAcao()
    {
        if (historicoAuditoria.Count > 0)
        {
            string acao = historicoAuditoria.Pop();
            Console.WriteLine($"Ação desfeita: {acao}");
        }
        else
        {
            Console.WriteLine("Nenhuma ação para desfazer.");
        }
    }

    // Missão 5: Relatório
    static void ImprimirRelatorio(IEnumerable<Dispositivo> colecao)
    {
        foreach (var d in colecao)
        {
            Console.WriteLine($"{d.Nome} - {d.Fabricante} - {d.EnderecoIP}");
        }
    }

    // Missão 6: Firewall Dinâmico
    static void AplicarFirewall(Func<Dispositivo, bool> regraBloqueio)
    {
        var bloqueados = dispositivosAtivos.Where(regraBloqueio).ToList();
        foreach (var d in bloqueados)
        {
            dispositivosAtivos.Remove(d);
            historicoAuditoria.Push($"Firewall bloqueou: {d.Nome} ({d.MacAddress})");
            Console.WriteLine($"Dispositivo {d.Nome} bloqueado!");
        }
    }
}


