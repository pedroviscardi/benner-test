using System.Xml;
using System.Xml.Serialization;
using Benner.Backend.Domain.Common;
using Benner.Backend.Domain.Repositories;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Infrastructure.Repositories;

public class XmlRepository<T> : IXmlRepository<T> where T : BaseEntity, new()
{
    private readonly string _filePath;
    private readonly object _lockObject = new();
    private readonly XmlSerializer _serializer;

    public XmlRepository(string dataDirectory)
    {
        if (string.IsNullOrWhiteSpace(dataDirectory))
            throw new ArgumentException("Diretório de dados é obrigatório", nameof(dataDirectory));

        var entityName = typeof(T).Name;
        _filePath = Path.Combine(dataDirectory, $"{entityName}s.xml");
        _serializer = new XmlSerializer(typeof(List<T>));

        EnsureDirectoryExists(dataDirectory);
        EnsureFileExists();
    }

    public async Task<Result<T?>> GetByIdAsync(Guid id)
    {
        try
        {
            var entities = await LoadEntitiesAsync();
            var entity = entities.FirstOrDefault(e => e.Id == id);
            return entity == null ? Result<T?>.Failure($"Entidade com ID {id} não encontrada") : Result<T?>.Success(entity);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure($"Erro ao buscar entidade: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<T>?>> GetAllAsync()
    {
        try
        {
            var entities = await LoadEntitiesAsync();
            return Result<IEnumerable<T>?>.Success(entities.Where(e => e.IsActive));
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<T>>.Failure($"Erro ao carregar entidades: {ex.Message}");
        }
    }

    public async Task<Result<T?>> AddAsync(T? entity)
    {
        try
        {
            if (entity == null)
                return Result<T>.Failure("Entidade não pode ser nula");

            var entities = await LoadEntitiesAsync();
            if (entities.Any(e => e.Id == entity.Id))
                return Result<T>.Failure("Entidade com este ID já existe");

            entities.Add(entity);
            await SaveEntitiesAsync(entities);

            return Result<T?>.Success(entity);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure($"Erro ao adicionar entidade: {ex.Message}");
        }
    }

    public async Task<Result<T?>> UpdateAsync(T? entity)
    {
        try
        {
            if (entity == null)
                return Result<T>.Failure("Entidade não pode ser nula");

            var entities = await LoadEntitiesAsync();
            var existingEntity = entities.FirstOrDefault(e => e.Id == entity.Id);

            if (existingEntity == null)
                return Result<T>.Failure("Entidade não encontrada para atualização");

            entities.Remove(existingEntity);
            entity.SetUpdatedAt();
            entities.Add(entity);

            await SaveEntitiesAsync(entities);

            return Result<T?>.Success(entity);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure($"Erro ao atualizar entidade: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var entities = await LoadEntitiesAsync();
            var entity = entities.FirstOrDefault(e => e.Id == id);

            if (entity == null)
                return Result<bool>.Failure("Entidade não encontrada para exclusão");

            entity.Deactivate();
            await SaveEntitiesAsync(entities);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Erro ao excluir entidade: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid id)
    {
        try
        {
            var entities = await LoadEntitiesAsync();
            var exists = entities.Any(e => e.Id == id && e.IsActive);
            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Erro ao verificar existência: {ex.Message}");
        }
    }

    public async Task<Result<int>> CountAsync()
    {
        try
        {
            var entities = await LoadEntitiesAsync();
            var count = entities.Count(e => e.IsActive);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Erro ao contar entidades: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<T>?>> FindAsync(Func<T, bool> predicate)
    {
        try
        {
            var entities = await LoadEntitiesAsync();
            var filteredEntities = entities.Where(e => e.IsActive && predicate(e));
            return Result<IEnumerable<T>?>.Success(filteredEntities);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<T>>.Failure($"Erro ao filtrar entidades: {ex.Message}");
        }
    }

    private async Task<List<T>> LoadEntitiesAsync()
    {
        return await Task.Run(() =>
        {
            lock (_lockObject)
            {
                try
                {
                    if (!File.Exists(_filePath))
                        return [];

                    using var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
                    if (fileStream.Length == 0)
                        return [];

                    var entities = (List<T>?) _serializer.Deserialize(fileStream);
                    return entities ?? [];
                }
                catch (Exception)
                {
                    return [];
                }
            }
        });
    }

    private async Task SaveEntitiesAsync(List<T> entities)
    {
        await Task.Run(() =>
        {
            lock (_lockObject)
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = Environment.NewLine,
                    OmitXmlDeclaration = false
                };

                using var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write);
                using var xmlWriter = XmlWriter.Create(fileStream, settings);
                _serializer.Serialize(xmlWriter, entities);
            }
        });
    }

    private void EnsureDirectoryExists(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private void EnsureFileExists()
    {
        if (File.Exists(_filePath))
            return;

        var emptyList = new List<T>();
        lock (_lockObject)
        {
            using var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write);
            using var xmlWriter = XmlWriter.Create(fileStream);
            _serializer.Serialize(xmlWriter, emptyList);
        }
    }
}