using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiCore.Extensions
{
	public static class IResultExtensionsExtensions
	{
		public static IResult ContentStatus(this IResultExtensions resultExtensions,
			string content,
			string contentType = System.Net.Mime.MediaTypeNames.Application.Json,
			System.Text.Encoding? contentEncoding = null,
			int statusCode = 200)
		{
			return new ContentStatusResult(content, contentType, contentEncoding, statusCode);
		}
	}

	public class ContentStatusResult : IResult
	{
		private readonly string content;
		private readonly string contentType;
		private readonly Encoding contentEncoding;
		private readonly int statusCode;

		public ContentStatusResult(string content,
			string contentType = System.Net.Mime.MediaTypeNames.Application.Json,
			System.Text.Encoding? contentEncoding = null,
			int statusCode = 200)
		{
			this.content = content;
			this.contentType = contentType;
			this.contentEncoding = contentEncoding ?? Encoding.UTF8;
			this.statusCode = statusCode;
		}

		public Task ExecuteAsync(HttpContext httpContext)
		{
			httpContext.Response.StatusCode = statusCode;
			httpContext.Response.ContentType = contentType;
			httpContext.Response.ContentLength = contentEncoding.GetByteCount(content);
			return httpContext.Response.WriteAsync(content, contentEncoding);
		}
	}
}
