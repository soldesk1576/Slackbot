using StatusNotifier.Data;
using StatusNotifier.Models.Dtos;
using StatusNotifier.Models.Entities;
using StatusNotifier.Models.Enums;
using StatusNotifier.Service;
using System.Text.Json;

namespace StatusNotifier.Scheduler
{
    public class StatusUpdateTask : BackgroundService
    {
        private readonly StatusChecker statusChecker;
        private readonly GeminiStatusNotifier geminiStatusNotifier;
        private readonly IServiceScopeFactory serviceScopeFactory;


        public StatusUpdateTask(StatusChecker statusChecker, GeminiStatusNotifier geminiStatusNotifier, IServiceScopeFactory serviceScopeFactory)
        {
            this.statusChecker = statusChecker;
            this.geminiStatusNotifier = geminiStatusNotifier;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<StatusNotifierDbContext>();
                        // Gemini 교환소의 상태 확인
                        string statusResponse = await statusChecker.GetGeminiStatusAsync();

                        // 여기서 statusResponse를 파싱하고 필요한 정보를 추출.
                        // statusResponse를 GeminiStatus 클래스로 역직렬화
                        var geminiStatus = JsonSerializer.Deserialize<GeminiStatus>(statusResponse);

                        // 교환소의 상태 정보 추출
                        string indicator = geminiStatus.Status.Indicator;
                        string description = geminiStatus.Status.Description;

                        string message = $"The status of the Gemini : {indicator} - {description} / Current Time : {DateTime.Now}";

                        // Slack에 메시지 보내기
                        await geminiStatusNotifier.SendSlackNotificationAsync(message);

                        // 성공적으로 처리된 경우 로그 기록
                        var successLog = new StatusLog
                        {
                            CallType = ECallType.TaskCall,
                            Status = EStatus.Success,
                        };

                        dbContext.StatusLogs.Add(successLog);
                        await dbContext.SaveChangesAsync();

                        await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");

                    // 오류가 발생한 경우 로그 기록
                    var errorLog = new StatusLog
                    {
                        CallType = ECallType.TaskCall,
                        Status = EStatus.Failure,
                        ErrorCause = ex.Message
                    };

                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<StatusNotifierDbContext>();
                        dbContext.StatusLogs.Add(errorLog);
                        await dbContext.SaveChangesAsync();
                    }

                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }
    }
}
