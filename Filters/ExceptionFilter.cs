using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace InventarioInteligente.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var (status, title) = context.Exception switch
            {
                ArgumentException => (HttpStatusCode.BadRequest, "Validación"),
                InvalidOperationException => (HttpStatusCode.Conflict, "Conflicto de negocio"),
                KeyNotFoundException => (HttpStatusCode.NotFound, "No encontrado"),
                _ => (HttpStatusCode.InternalServerError, "Error interno")
            };

            var problem = new ProblemDetails
            {
                Status = (int)status,
                Title = title,
                Detail = context.Exception.Message
            };

            context.Result = new ObjectResult(problem) { StatusCode = problem.Status };
            context.ExceptionHandled = true;
        }
    }
}
