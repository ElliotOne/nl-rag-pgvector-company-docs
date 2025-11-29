using Npgsql;
using RagBasics.EmbeddingGenerator;
using System.Globalization;

namespace RagBasics.Repository;

public class TextRepository(string connectionString, IEmbeddingGenerator embeddingGenerator)
{
    private readonly string _connectionString = connectionString;
    private readonly IEmbeddingGenerator _embeddingGenerator = embeddingGenerator;

    // Store text with title and category
    public async Task StoreTextAsync(string title, string category, string content)
    {
        var embedding = await _embeddingGenerator.GenerateEmbeddingAsync(content);

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        string query = @"
            INSERT INTO text_contexts (title, category, content, embedding) 
            VALUES (@title, @category, @content, @embedding)";

        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("title", title);
        cmd.Parameters.AddWithValue("category", category);
        cmd.Parameters.AddWithValue("content", content);
        cmd.Parameters.AddWithValue("embedding", embedding);

        await cmd.ExecuteNonQueryAsync();
    }

    // Retrieve relevant texts with optional category filter
    public async Task<List<(string Title, string Category, string Content)>> RetrieveRelevantText(string query, string? categoryFilter = null)
    {
        var queryEmbedding = await _embeddingGenerator.GenerateEmbeddingAsync(query);

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        string querySql = @"
            SELECT title, category, content
            FROM text_contexts
            WHERE embedding <-> CAST(@queryEmbedding AS vector) > 0.7
        ";

        if (!string.IsNullOrEmpty(categoryFilter))
            querySql += " AND category = @category ";

        querySql += " ORDER BY embedding <-> CAST(@queryEmbedding AS vector) LIMIT 5";

        using var cmd = new NpgsqlCommand(querySql, conn);

        string embeddingString = $"[{string.Join(",", queryEmbedding.Select(v => v.ToString("G", CultureInfo.InvariantCulture)))}]";
        cmd.Parameters.AddWithValue("queryEmbedding", embeddingString);

        if (!string.IsNullOrEmpty(categoryFilter))
            cmd.Parameters.AddWithValue("category", categoryFilter);

        using var reader = await cmd.ExecuteReaderAsync();

        var results = new List<(string Title, string Category, string Content)>();
        while (await reader.ReadAsync())
        {
            string title = reader.GetString(0);
            string cat = reader.GetString(1);
            string content = reader.GetString(2);
            results.Add((title, cat, content));
        }

        return results.Any()
            ? results
            : new List<(string, string, string)> { ("No Title", "No Category", "No relevant context found.") };
    }
}
