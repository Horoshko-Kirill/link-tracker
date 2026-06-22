using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Enum;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Database.Sql;
using Npgsql;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class SqlChatLinkScanRepository : SqlRepositoryBase, IChatLinkScanReportRepository
{
    public SqlChatLinkScanRepository(NpgsqlDataSource dataSource, SqlSession session) : base(dataSource, session)
    {
    }

    public Task AddAsync(ChatLinkScanReport report, CancellationToken cancellationToken = default)
    {
        string sql = """
                     insert into scrapper_link_scan_report (chat_id, scan_started_at, scan_finished_at, failed_count, message, status, sent_at)
                     values (@chat_id, @scan_started_at, @scan_finished_at, @failed_count, @message, @status, @sent_at)
                     returning id
                     """;

        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("chat_id", report.ChatId);
            cmd.Parameters.AddWithValue("scan_started_at", report.ScanStartedAt);
            cmd.Parameters.AddWithValue("scan_finished_at", report.ScanFinishedAt);
            cmd.Parameters.AddWithValue("failed_count", report.FailedCount);
            cmd.Parameters.AddWithValue("message", report.Message);
            cmd.Parameters.AddWithValue("status", (int)report.Status);
            cmd.Parameters.AddWithValue("sent_at", report.SentAt);
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            report.Id = Convert.ToInt64(result);
        }, cancellationToken);
    }

    public Task<List<ChatLinkScanReport>> GetPendingAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        int limit = Math.Clamp(pageRequest.Size, 1, 1000);

        string sql = $"""
                      select id, chat_id, scan_started_at, scan_finished_at, failed_count, message, status, sent_at
                      from scrapper_link_scan_report
                      where id > @lastId and status = @status
                      order by id
                      limit {limit}
                      """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("status", (int)ReportStatus.Pending);
            cmd.Parameters.AddWithValue("lastId", pageRequest.LastId);

            var result = new List<ChatLinkScanReport>();

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new ChatLinkScanReport
                {
                    Id = reader.GetInt64(reader.GetOrdinal("id")),
                    ChatId = reader.GetInt64(reader.GetOrdinal("chat_id")),
                    ScanStartedAt = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("scan_started_at")),
                    ScanFinishedAt = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("scan_finished_at")),
                    FailedCount = reader.GetFieldValue<int>(reader.GetOrdinal("failed_count")),
                    Message = reader.GetFieldValue<string>(reader.GetOrdinal("message")),
                    Status = (ReportStatus)reader.GetInt32(reader.GetOrdinal("status")),
                    SentAt = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("sent_at")),
                });
            }

            return result;
        }, cancellationToken);
    }

    public Task UpdateAsync(ChatLinkScanReport report, CancellationToken cancellationToken = default)
    {
        string sql = """
                     update scrapper_link_scan_report
                     set chat_id = @chat_id,
                         scan_started_at = @scan_started_at,
                         scan_finished_at = @scan_finished_at,
                         failed_count = @failed_count,
                         message = @message,
                         status = @status,
                         sent_at = @sent_at
                     where id = @id
                     """;

        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("id", report.Id);
            cmd.Parameters.AddWithValue("chat_id", report.ChatId);
            cmd.Parameters.AddWithValue("scan_started_at", report.ScanStartedAt);
            cmd.Parameters.AddWithValue("scan_finished_at", report.ScanFinishedAt);
            cmd.Parameters.AddWithValue("failed_count", report.FailedCount);
            cmd.Parameters.AddWithValue("message", report.Message);
            cmd.Parameters.AddWithValue("status", (int)report.Status);
            cmd.Parameters.AddWithValue("sent_at", report.SentAt);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }, cancellationToken);
    }
}