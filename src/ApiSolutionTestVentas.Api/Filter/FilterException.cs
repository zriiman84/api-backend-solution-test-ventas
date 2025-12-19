using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiSolutionTestVentas.Api.Filter;

public class FilterExceptions : ExceptionFilterAttribute
{
    private readonly ILogger<FilterExceptions> logger;

    public FilterExceptions(ILogger<FilterExceptions> logger)
    {
        this.logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, context.Exception.Message);
        base.OnException(context);
    }
}