using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Organizador de Arquivos ===");
        Console.Write("Digite o caminho da pasta a ser organizada: ");
        string pasta = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(pasta) || !Directory.Exists(pasta))
        {
            Console.WriteLine("Pasta não encontrada ou caminho inválida!");
            return;
        }

        Console.Write("Deseja ignorar alguma extensão? (separe por vírgula, ex: jpg,png) ");
        string ignorarInput = Console.ReadLine();
        var ignorarExtensoes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(ignorarInput))
        {
            foreach (var ext in ignorarInput.Split(","))
            {
                var e = ext.Trim().ToLower();
                if (!string.IsNullOrEmpty(e)) ignorarExtensoes.Add(e);
            }
        }

        // Filtros avançados
        long? tamanhoMin = null, tamanhoMax = null;
        DateTime? dataMin = null, dataMax = null;

        Console.Write("Deseja filtrar por tamanho mínimo (MB)? (deixe vazio para ignorar): ");
        if (long.TryParse(Console.ReadLine(), out long tMin))
            tamanhoMin = tMin * 1024 * 1024;

        Console.Write("Deseja filtrar por tamanho máximo (MB)? (deixe vazio para ignorar): ");
        if (long.TryParse(Console.ReadLine(), out long tMax))
            tamanhoMax = tMax * 1024 * 1024;

        Console.Write("Deseja filtrar por data de modificação após (ex: 2023-01-01)? (deixe vazio para ignorar): ");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime dMin))
            dataMin = dMin;

        Console.Write("Deseja filtrar por data de modificação antes (ex: 2023-12-31)? (deixe vazio para ignorar): ");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime dMax))
            dataMax = dMax;

        string logPath = Path.Combine(pasta, "log.txt");
        using (var log = new StreamWriter(logPath, append: true))
        {
            log.WriteLine($"\n=== Execução em {DateTime.Now} ===");
            try
            {
                OrganizarArquivosRecursivo(pasta, ignorarExtensoes, log, pasta, tamanhoMin, tamanhoMax, dataMin, dataMax);
                Console.WriteLine("Organização concluída!");
                log.WriteLine("Organização concluída!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao organizar arquivos: {ex.Message}");
                log.WriteLine($"Erro ao organizar arquivos: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Organiza arquivos em subpastas conforme a extensão, ignorando extensões especificadas. Recursivo. Aplica filtros de tamanho e data.
    /// </summary>
    static void OrganizarArquivosRecursivo(string pasta, HashSet<string> ignorarExtensoes, StreamWriter log, string pastaRaiz, long? tamanhoMin, long? tamanhoMax, DateTime? dataMin, DateTime? dataMax)
    {
        var arquivos = Directory.GetFiles(pasta);
        foreach (var arquivo in arquivos)
        {
            string extensao = Path.GetExtension(arquivo).TrimStart('.').ToLower();
            if (string.IsNullOrEmpty(extensao))
                extensao = "outros";

            if (ignorarExtensoes.Contains(extensao))
            {
                string msg = $"Ignorado: {Path.GetFileName(arquivo)}";
                Console.WriteLine(msg);
                log.WriteLine(msg);
                continue;
            }

            var info = new FileInfo(arquivo);
            if (tamanhoMin.HasValue && info.Length < tamanhoMin.Value)
            {
                string msg = $"Ignorado (menor que {tamanhoMin.Value / 1024 / 1024}MB): {info.Name}";
                Console.WriteLine(msg);
                log.WriteLine(msg);
                continue;
            }
            if (tamanhoMax.HasValue && info.Length > tamanhoMax.Value)
            {
                string msg = $"Ignorado (maior que {tamanhoMax.Value / 1024 / 1024}MB): {info.Name}";
                Console.WriteLine(msg);
                log.WriteLine(msg);
                continue;
            }
            if (dataMin.HasValue && info.LastWriteTime < dataMin.Value)
            {
                string msg = $"Ignorado (modificado antes de {dataMin.Value:yyyy-MM-dd}): {info.Name}";
                Console.WriteLine(msg);
                log.WriteLine(msg);
                continue;
            }
            if (dataMax.HasValue && info.LastWriteTime > dataMax.Value)
            {
                string msg = $"Ignorado (modificado depois de {dataMax.Value:yyyy-MM-dd}): {info.Name}";
                Console.WriteLine(msg);
                log.WriteLine(msg);
                continue;
            }

            // Garante que não vai mover arquivos já organizados na raiz
            string destino = Path.Combine(pastaRaiz, extensao);
            if (Path.GetDirectoryName(arquivo) == destino)
                continue;

            Directory.CreateDirectory(destino);

            string nomeArquivo = Path.GetFileName(arquivo);
            string novoCaminho = Path.Combine(destino, nomeArquivo);

            try
            {
                if (!File.Exists(novoCaminho))
                {
                    File.Move(arquivo, novoCaminho);
                    string msg = $"Movido: {nomeArquivo} -> {extensao}/";
                    Console.WriteLine(msg);
                    log.WriteLine(msg);
                }
                else
                {
                    string msg = $"Arquivo já existe em {extensao}/: {nomeArquivo}";
                    Console.WriteLine(msg);
                    log.WriteLine(msg);
                }
            }
            catch (Exception ex)
            {
                string msg = $"Erro ao mover {nomeArquivo}: {ex.Message}";
                Console.WriteLine(msg);
                log.WriteLine(msg);
            }
        }

        // Recursão para subpastas (exceto as criadas pelo organizador)
        foreach (var subpasta in Directory.GetDirectories(pasta))
        {
            string nomeSubpasta = Path.GetFileName(subpasta);
            if (nomeSubpasta == null) continue;
            // Ignora subpastas de organização (extensões)
            if (Directory.GetParent(subpasta)?.FullName == pastaRaiz && nomeSubpasta != null && nomeSubpasta.Length <= 5)
                continue;
            OrganizarArquivosRecursivo(subpasta, ignorarExtensoes, log, pastaRaiz, tamanhoMin, tamanhoMax, dataMin, dataMax);
        }
    }
}
