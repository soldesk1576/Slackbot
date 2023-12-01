using Microsoft.AspNetCore.Mvc;
using StatusNotifier.Data;
using StatusNotifier.Models.Dtos;
using StatusNotifier.Models.Entities;
using StatusNotifier.Models.Enums;
using StatusNotifier.Service;
using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
using System.Web;

namespace StatusNotifier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlackWebhookController : ControllerBase
    {
        private const string ExpectedToken = "JibcGSEqWiWKWHVoUnGQ7ojf";

        private readonly StatusChecker statusChecker;
        private readonly GeminiStatusNotifier geminiStatusNotifier;
        private readonly StatusNotifierDbContext dbContext;

        public SlackWebhookController(StatusChecker statusChecker, GeminiStatusNotifier geminiStatusNotifier, StatusNotifierDbContext dbContext)
        {
            this.statusChecker = statusChecker;
            this.geminiStatusNotifier = geminiStatusNotifier;
            this.dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveSlackMessage([FromForm] string token)
        {
            try
            {
                token = Request.Form["token"];

                if (!string.IsNullOrEmpty(token) && token == ExpectedToken)
                {
                    // 토큰이 있다면 여기서 처리
                    string statusResponse = await statusChecker.GetGeminiStatusAsync();
                    var geminiStatus = JsonSerializer.Deserialize<GeminiStatus>(statusResponse);

                    string indicator = geminiStatus.Status.Indicator;
                    string description = geminiStatus.Status.Description;

                    string message = $"The status of the Gemini : {indicator} - {description} / Current Time : {DateTime.Now} - Direct Call!";

                    await geminiStatusNotifier.SendSlackNotificationAsync(message);

                    var successLog = new StatusLog
                    {
                        CallType = ECallType.DirectCall,
                        Status = EStatus.Success,
                    };

                    dbContext.StatusLogs.Add(successLog);
                    await dbContext.SaveChangesAsync();

                    return Ok("Message sent successfully!");
                }
                else
                {
                    // 토큰이 없는 경우 처리
                    return BadRequest("Token not found in the query parameters.");
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                // 예외가 발생한 경우 로그 기록
                var errorLog = new StatusLog
                {
                    CallType = ECallType.DirectCall,
                    Status = EStatus.Failure,
                    ErrorCause = ex.Message // 실패 원인 기록
                };

                dbContext.StatusLogs.Add(errorLog);
                await dbContext.SaveChangesAsync();

                return StatusCode(500, "Internal Server Error");
            }
            
        }
    }
}

