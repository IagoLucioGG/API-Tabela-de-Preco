using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var permissoesClaim = context.User.FindFirst("Permissoes");

        if (permissoesClaim != null)
        {
            var permissoes = permissoesClaim.Value
                .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim());

            if (permissoes.Any(p => p.Equals(requirement.Permission, System.StringComparison.OrdinalIgnoreCase)))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        // Caso não tenha permissão, retorna resposta personalizada
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext != null && !httpContext.Response.HasStarted)
        {
            httpContext.Response.StatusCode = 403;
            httpContext.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(new
            {
                mensagem = "Você não tem a permissão necessária para acessar este recurso."
            });

            // Escreve a resposta apenas se ainda não começou
            return httpContext.Response.WriteAsync(json);
        }

        context.Fail();
        return Task.CompletedTask;
    }
}
