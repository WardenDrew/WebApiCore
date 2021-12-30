using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApiCore.Extensions;
using WebApiCore.Models;

namespace WebApiCore.Services
{
	public interface IControllerService : IServiceScanningServiceInterface
	{
		/// <summary>
		/// Executes a MediatR request and returns the response wrapped in an IActionResult
		/// </summary>
		/// <remarks>
		/// Use with AspNetCore MVC (Controllers). For AspNetCore Minimal API's use <see cref="RequestMinimal{TResult}(IRequest{TResult})">RequestMinimal()</see> instead
		/// </remarks>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="request"></param>
		/// <returns></returns>
		public Task<IActionResult> Request<TResult>(IRequest<TResult> request) where TResult : IResponse;

		/// <summary>
		/// Executes a MediatR request and returns the response wrapped in an IResult
		/// </summary>
		/// <remarks>
		/// Use with AspNetCore Minimal API's. For AspNetCore MVC (Controllers) use <see cref="Request{TResult}(IRequest{TResult})">Request()</see> instead
		/// </remarks>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="request"></param>
		/// <returns></returns>
		public Task<IResult> RequestMinimal<TResult>(IRequest<TResult> request) where TResult : IResponse;
	}
	
	public class ControllerService : IControllerService, IServiceScanningScopedImplementation
	{
		private readonly IMediator mediator;
		private readonly IOptions<WebApiCoreOptions> options;

		public ControllerService(IMediator mediator, IOptions<WebApiCoreOptions> options)
		{
			this.mediator = mediator;
			this.options = options;
		}

		public async Task<IActionResult> Request<TResult>(IRequest<TResult> request) where TResult : IResponse
		{
			IResponse result = await mediator.Send(request);

			ContentResult cResult = new ContentResult();
			cResult.StatusCode = result.CalculateStatusCode();
			cResult.Content = result.Serialize(options.Value.ResponseSerializerSettings);
			cResult.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;

			return cResult;
		}

		public async Task<IResult> RequestMinimal<TResult>(IRequest<TResult> request) where TResult : IResponse
		{
			IResponse response = await mediator.Send(request);

			return Results.Extensions.ContentStatus(
				content: response.Serialize(options.Value.ResponseSerializerSettings),
				contentType: System.Net.Mime.MediaTypeNames.Application.Json,
				contentEncoding: System.Text.Encoding.UTF8,
				statusCode: response.CalculateStatusCode());
		}
	}
}
