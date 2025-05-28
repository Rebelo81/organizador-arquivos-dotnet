# Organizador de Arquivos

Organizador de Arquivos é um utilitário em .NET para automatizar a organização de arquivos em uma pasta, separando-os em subpastas conforme a extensão, tamanho, data ou outros filtros. Ideal para manter pastas como Downloads organizadas automaticamente.

## Funcionalidades
- Organização automática por extensão
- Filtros por tamanho e data de modificação
- Ignorar extensões específicas
- Organização recursiva (subpastas)
- Geração de logs
- Testes automatizados com xUnit

## Como funciona?
O programa move todos os arquivos de uma pasta para subpastas nomeadas de acordo com a extensão do arquivo (ex: arquivos `.jpg` vão para a subpasta `jpg`). Você pode informar filtros de tamanho, data e extensões a serem ignoradas.

## Como rodar

1. **Clone o repositório ou copie os arquivos para uma pasta.**
2. Abra o terminal na pasta do projeto.
3. Execute:

```sh
dotnet run
```

4. Siga as instruções no console:
   - Informe o caminho da pasta a ser organizada.
   - (Opcional) Informe extensões a serem ignoradas, separadas por vírgula.
   - (Opcional) Informe filtros de tamanho e data.

## Exemplo de uso
```
=== Organizador de Arquivos ===
Digite o caminho da pasta a ser organizada: C:\Users\SeuUsuario\Downloads
Deseja ignorar alguma extensão? (separe por vírgula, ex: jpg,png) txt,mp3
Deseja filtrar por tamanho mínimo (MB)? (deixe vazio para ignorar): 1
Deseja filtrar por tamanho máximo (MB)? (deixe vazio para ignorar): 10
Deseja filtrar por data de modificação após (ex: 2023-01-01)? (deixe vazio para ignorar): 2023-01-01
Deseja filtrar por data de modificação antes (ex: 2023-12-31)? (deixe vazio para ignorar):
Movido: foto1.jpg -> jpg/
Movido: documento.pdf -> pdf/
Ignorado: notas.txt
Ignorado: musica.mp3
Organização concluída!
```

## Como rodar os testes automatizados

1. Entre na pasta do projeto de testes:
   ```sh
   cd TesteLimpo
   ```
2. Execute:
   ```sh
   dotnet test
   ```

Você verá o resultado dos testes automatizados, garantindo a qualidade do código.

## Como contribuir
1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b minha-feature`)
3. Commit suas alterações (`git commit -am 'Adiciona nova feature'`)
4. Faça um push para a branch (`git push origin minha-feature`)
5. Abra um Pull Request

## Licença
Este projeto está sob a licença MIT. 