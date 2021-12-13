using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiCore.Extensions;
using WebApiCore.Models;

namespace WebApiCore.Services
{
	public interface IControllerService : IServiceScanningServiceInterface
	{
		/// <summary>
		/// Executes a MediatR request and returns the response wrapped in an IResult
		/// </summary>
		/// <remarks>
		/// Use with AspNetCore Minimal API's. For AspNetCore MVC (Controllers) use <see cref="RequestMvc{TResult}(IRequest{TResult})">RequestMvc()</see> instead
		/// </remarks>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="request"></param>
		/// <returns></returns>
		public Task<IResult> Request<TResult>(IRequest<TResult> request) where TResult : IResponse;

		/// <summary>
		/// Executes a MediatR request and returns the response wrapped in an IActionResult
		/// </summary>
		/// <remarks>
		/// Use with AspNetCore MVC (Controllers). For AspNetCore Minimal API's use <see cref="Request{TResult}(IRequest{TResult})">Request()</see> instead
		/// </remarks>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="request"></param>
		/// <returns></returns>
		public Task<IActionResult> RequestMvc<TResult>(IRequest<TResult> request) where TResult : IResponse;
	}
	
	public class ControllerService : IControllerService, IServiceScanningScopedImplementation
	{
		private readonly IMediator mediator;

		public ControllerService(IMediator mediator)
		{
			this.mediator = mediator;
		}

		public async Task<IResult> Request<TResult>(IRequest<TResult> request) where TResult : IResponse
		{
			IResponse response = await mediator.Send(request);

			return Results.Extensions.ContentStatus(
				content: response.Serialize(),
				contentType: System.Net.Mime.MediaTypeNames.Application.Json,
				contentEncoding: System.Text.Encoding.UTF8,
				statusCode: response.CalculateStatusCode());
		}

		public async Task<IActionResult> RequestMvc<TResult>(IRequest<TResult> request) where TResult : IResponse
		{
			IResponse result = await mediator.Send(request);

			ContentResult cResult = new ContentResult();
			cResult.StatusCode = result.CalculateStatusCode();
			cResult.Content = result.Serialize();
			cResult.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;

			return cResult;
		}
	}
}
