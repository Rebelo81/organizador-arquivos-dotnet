using System;
using System.IO;
using System.Collections.Generic;
using Xunit;

namespace TesteLimpo;

public class OrganizadorTests
{
    [Fact]
    public void OrganizaArquivos_MovimentaPorExtensao()
    {
        // Arrange
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        string txtFile = Path.Combine(tempDir, "teste.txt");
        string jpgFile = Path.Combine(tempDir, "foto.jpg");
        File.WriteAllText(txtFile, "arquivo de texto");
        File.WriteAllText(jpgFile, "arquivo de imagem");
        var ignorar = new HashSet<string>();
        string logPath = Path.Combine(tempDir, "log.txt");
        using var log = new StreamWriter(logPath, append: false);

        // Act
        Program.OrganizarArquivosRecursivo(tempDir, ignorar, log, tempDir, null, null, null, null);
        log.Close();

        // Assert
        Assert.False(File.Exists(txtFile));
        Assert.False(File.Exists(jpgFile));
        Assert.True(File.Exists(Path.Combine(tempDir, "txt", "teste.txt")));
        Assert.True(File.Exists(Path.Combine(tempDir, "jpg", "foto.jpg")));

        // Cleanup
        Directory.Delete(tempDir, true);
    }

    [Fact]
    public void OrganizaArquivos_FiltraPorTamanho()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        string pequeno = Path.Combine(tempDir, "pequeno.txt");
        string grande = Path.Combine(tempDir, "grande.txt");
        File.WriteAllBytes(pequeno, new byte[100]); // 100 bytes
        File.WriteAllBytes(grande, new byte[1024 * 1024 * 2]); // 2 MB
        var ignorar = new HashSet<string>();
        string logPath = Path.Combine(tempDir, "log.txt");
        using var log = new StreamWriter(logPath, append: false);

        // Só mover arquivos maiores que 1MB
        Program.OrganizarArquivosRecursivo(tempDir, ignorar, log, tempDir, 1024 * 1024, null, null, null);
        log.Close();

        Assert.True(File.Exists(pequeno)); // Não foi movido
        Assert.False(File.Exists(grande)); // Foi movido
        Assert.True(File.Exists(Path.Combine(tempDir, "txt", "grande.txt")));
        Directory.Delete(tempDir, true);
    }

    [Fact]
    public void OrganizaArquivos_FiltraPorData()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        string antigo = Path.Combine(tempDir, "antigo.txt");
        string novo = Path.Combine(tempDir, "novo.txt");
        File.WriteAllText(antigo, "antigo");
        File.WriteAllText(novo, "novo");
        File.SetLastWriteTime(antigo, DateTime.Now.AddDays(-10));
        File.SetLastWriteTime(novo, DateTime.Now);
        var ignorar = new HashSet<string>();
        string logPath = Path.Combine(tempDir, "log.txt");
        using var log = new StreamWriter(logPath, append: false);

        // Só mover arquivos modificados depois de ontem
        Program.OrganizarArquivosRecursivo(tempDir, ignorar, log, tempDir, null, null, DateTime.Now.AddDays(-1), null);
        log.Close();

        Assert.True(File.Exists(antigo)); // Não foi movido
        Assert.False(File.Exists(novo)); // Foi movido
        Assert.True(File.Exists(Path.Combine(tempDir, "txt", "novo.txt")));
        Directory.Delete(tempDir, true);
    }
}
