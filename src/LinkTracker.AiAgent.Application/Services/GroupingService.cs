using LinkTracker.AiAgent.Application.InterfacesServices;
using LinkTracker.AiAgent.Application.Mappers;
using LinkTracker.AiAgent.Application.Options;
using LinkTracker.AiAgent.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkTracker.AiAgent.Application.Services;

public class GroupingService : IGroupingService
{
    private readonly Dictionary<long, GroupBucket> _buckets = new();
    private readonly object _lock = new();
    private readonly TimeSpan _window;
    private readonly IMessageSender _messageSender;
    private readonly IGrouping _grouping;
    private readonly ILogger<GroupingService> _logger;

    public GroupingService(
        IOptions<AiAgentOptions> options,
        IMessageSender messageSender,
        IGrouping grouping,
        ILogger<GroupingService> logger)
    {
        _window = TimeSpan.FromMilliseconds(options.Value.Grouping.WindowMs);
        _messageSender = messageSender;
        _grouping = grouping;
        _logger = logger;
    }

    public Task AddAsync(ProcessedLinkUpdate update, CancellationToken cancellationToken = default)
    {
        foreach (var chatId in update.ChatIds)
        {
            lock (_lock)
            {
                GroupBucket? bucket;

                if (!_buckets.TryGetValue(chatId, out bucket))
                {
                    bucket = new GroupBucket();

                    _buckets[chatId] = bucket;

                    _ = SendLaterAsync(chatId);
                }

                bucket.Updates.Add(update);
            }
        }

        return Task.CompletedTask;
    }

    private async Task SendLaterAsync(long chatId)
    {
        List<ProcessedLinkUpdate> updates = new List<ProcessedLinkUpdate>();
        try
        {
            await Task.Delay(_window);

            GroupBucket? bucket;

            lock (_lock)
            {
                if (!_buckets.TryGetValue(chatId, out bucket))
                {
                    return;
                }
                updates = bucket.Updates.ToList();
                _buckets.Remove(chatId);
            }

            var grouped = _grouping.Group(updates, chatId);

            var result = KafkaMapper.ToDto(grouped);

            try
            {
                await _messageSender.SendAsync(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending grouping message");
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendLaterAsync for chat {ChatId}", chatId);
            if (updates.Count > 0)
            {
                lock (_lock)
                {
                    GroupBucket? newBucket;
                    if (!_buckets.TryGetValue(chatId, out newBucket))
                    {
                        newBucket = new GroupBucket();
                        _buckets[chatId] = newBucket;
                        _ = SendLaterAsync(chatId);
                    }

                    newBucket.Updates.AddRange(updates);
                }
            }
        }
    }
}