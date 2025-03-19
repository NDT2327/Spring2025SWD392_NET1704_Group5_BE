using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace CCSystem.Presentation.Helpers
{
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public BearerTokenHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _contextAccessor.HttpContext;
            if (context != null && context.Request.Cookies.TryGetValue("accessToken", out var token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
