using System;
using System.IO;
using System.Collections.Generic;
using Xunit;

namespace OrganizadorArquivos.Tests;

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
}
