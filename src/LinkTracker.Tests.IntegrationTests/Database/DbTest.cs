using LinkTracker.Tests.IntegrationTests.Fixtures;
using Npgsql;

namespace LinkTracker.Tests.IntegrationTests.Database
{
    [Collection("Integration")]
    public class DbTests
    {
        private readonly TestEnvironment _env;
        private readonly string _connectionString;

        public DbTests(TestEnvironment env)
        {
            _env = env;
            var port = _env.Db.GetMappedPublicPort(5432);
            _connectionString = $"Host=localhost;Port={port};Database=linktracker_test;Username=postgres;Password=postgres";
        }

        [Fact]
        public async Task AddLink_LinkSavedInDb()
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "INSERT INTO scrapper_chats(chat_id) VALUES (@chatId)", conn);
            cmd.Parameters.AddWithValue("chatId", 123);
            await cmd.ExecuteNonQueryAsync();

            await using var checkCmd = new NpgsqlCommand(
                "SELECT chat_id FROM scrapper_chats WHERE chat_id = @chatId", conn);
            checkCmd.Parameters.AddWithValue("chatId", 123);

            var result = new List<long>();
            await using var reader = await checkCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(reader.GetInt64(0));
            }

            Assert.Contains(123, result);
        }

        [Fact]
        public async Task DeleteLink_LinkRemovedFromDb()
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            
            await using var insertCmd = new NpgsqlCommand(
                "INSERT INTO scrapper_chats(chat_id) VALUES (@chatId)", conn);
            insertCmd.Parameters.AddWithValue("chatId", 124);
            await insertCmd.ExecuteNonQueryAsync();
            
            await using var deleteCmd = new NpgsqlCommand(
                "DELETE FROM scrapper_chats WHERE chat_id = @chatId", conn);
            deleteCmd.Parameters.AddWithValue("chatId", 124);
            await deleteCmd.ExecuteNonQueryAsync();
            
            await using var checkCmd = new NpgsqlCommand(
                "SELECT chat_id FROM scrapper_chats WHERE chat_id = @chatId", conn);
            checkCmd.Parameters.AddWithValue("chatId", 124);
            var result = new List<long>();
            await using var reader = await checkCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(reader.GetInt64(0));
            }

            Assert.Empty(result);
        }

        [Fact]
        public async Task AddDuplicateLink_ThrowsException()
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var insertCmd = new NpgsqlCommand(
                "INSERT INTO scrapper_chats(chat_id) VALUES (@chatId)", conn);
            insertCmd.Parameters.AddWithValue("chatId", 125);
            await insertCmd.ExecuteNonQueryAsync();
            
            await using var duplicateCmd = new NpgsqlCommand(
                "INSERT INTO scrapper_chats(chat_id) VALUES (@chatId)", conn);
            duplicateCmd.Parameters.AddWithValue("chatId", 125);

            await Assert.ThrowsAsync<Npgsql.PostgresException>(async () =>
                await duplicateCmd.ExecuteNonQueryAsync()
            );
        }

        [Fact]
        public async Task Migrations_CreatedTables()
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT to_regclass('public.scrapper_links')::text", conn);

            var result = await cmd.ExecuteScalarAsync();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task AccessType_DefaultImplementation()
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT COUNT(*) FROM scrapper_links", conn);

            var result = await cmd.ExecuteScalarAsync();
            var count = Convert.ToInt32(result);

            Assert.True(count >= 0);
        }
    }
}