using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;

namespace DraughtSurveyWebApp.Middleware
{
    public class SessionTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private const string SidCookie = "sid";
        private static readonly TimeSpan Idle = TimeSpan.FromMinutes(20);
        private static readonly TimeSpan Touch = TimeSpan.FromSeconds(60);

        public SessionTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext httpContext,
            ApplicationDbContext applicationDbContext
            )
        {

            if (httpContext.User.Identity?.IsAuthenticated != true)
            {
                await _next(httpContext);
                return;
            }

            var userId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
            var now = DateTime.UtcNow;


            var sidStr = httpContext.Request.Cookies[SidCookie];
            UserSession? session = null;
            var needSave = false;

            // Пытаемся прочитать сессию по sid из куки
            if (Guid.TryParse(sidStr, out var sid))
            {
                session = await applicationDbContext.UserSessions.FindAsync(sid);
            }

            // Новая сессия, если нет валидной или был idle > 20 минут
            if (session == null || now - session.LastSeenUtc > Idle)
            {
                session = new UserSession
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    StartedUtc = now,
                    LastSeenUtc = now,
                    Ip = GetClientIp(httpContext),
                    UserAgent = httpContext.Request.Headers["User-Agent"].ToString()
                };

                applicationDbContext.UserSessions.Add(session);
                needSave = true;

                httpContext.Response.Cookies.Append(
                    SidCookie,
                    session.Id.ToString(),
                    new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax,
                        Secure = httpContext.Request.IsHttps,
                        Expires = DateTimeOffset.UtcNow.AddDays(30),
                        IsEssential = true
                    });
            }
            else
            {
                // Актуализируем LastSeenUtc, но не чаще раза в 60 сек
                if (now - session.LastSeenUtc >= Touch)
                {
                    session.LastSeenUtc = now;
                    needSave = true;
                }

                if (session.UserId != userId)
                {
                    session.UserId = userId;
                    needSave = true;
                }
            }

            // На всякий случай привяжем UserId, если появилась аутентификация позже
            if (needSave)
            {
                await applicationDbContext.SaveChangesAsync();
            }

            await _next(httpContext);


        }

        private static string GetClientIp(HttpContext httpContext)
        {
            // 1) X-Forwarded-For: может быть "client, proxy1, proxy2"
            var xff = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(xff))
            {
                return xff.Split(',')[0].Trim();
            }

            // 2) X-Real-IP (некоторые прокси)
            var xrip = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(xrip))
            {
                return xrip;
            }

            // 3) fallback — прямое подключение
            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "-";

        }
    }
}
