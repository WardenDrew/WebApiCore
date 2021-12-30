using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebApiCore.Models
{
    public class Response : IResponse
    {
		/// <inheritdoc/>
		public bool Success { get; set; }
		/// <inheritdoc/>
		public int? OverridenStatusCode { get; set; }

		/// <inheritdoc/>
		public List<IErrorCode> Errors { get; set; }
		/// <inheritdoc/>
		public bool HasErrors { get { return Errors.Any(); } }
		/// <inheritdoc/>
		public int ErrorCount { get { return Errors.Count; } }

		protected Response()
		{
			Errors = new();
		}

		/// <summary>
		/// Create a new Successful Result
		/// </summary>
		/// <returns></returns>
		public static IResponse FromSuccess()
		{
			var result = new Response();
			result.Success = true;
			return result;
		}

		/// <summary>
		/// Create a new result from an Error
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public static IResponse FromError(IErrorCode? code)
		{
			var result = new Response();
			if (code is not null)
			{
				result.Errors.Add(code);
			}
			return result;
		}

		/// <inheritdoc/>
		public IResponse WithError(IErrorCode? code)
		{
			if (code is not null)
			{
				Errors.Add(code);
			}
			return this;
		}

		/// <inheritdoc/>
		public IResponse WithStatusCode(int statusCode)
		{
			OverridenStatusCode = statusCode;
			return this;
		}

		/// <inheritdoc/>
		public IDataResponse<T> WithData<T>(T? data) => DataResponse<T>.FromResponse(this).WithData(data);

		/// <inheritdoc/>
		public int CalculateStatusCode()
		{
			if (OverridenStatusCode.HasValue)
			{
				return OverridenStatusCode.Value;
			}

			if (HasErrors)
			{
				//tiered
				bool has500 = false;
				bool has501 = false;
				bool has401 = false;
				bool has403 = false;
				bool has400 = false;

				foreach (var code in Errors)
				{
					if (!code.HTTPStatusCode.HasValue) { continue; }

					has500 = has500 || (code.HTTPStatusCode.Value == 500);
					has501 = has501 || (code.HTTPStatusCode.Value == 501);
					has401 = has401 || (code.HTTPStatusCode.Value == 401);
					has403 = has403 || (code.HTTPStatusCode.Value == 403);
					has400 = has400 || (code.HTTPStatusCode.Value == 400);
				}

				if (has500) { return 500; }
				else if (has501) { return 501; }
				else if (has401) { return 401; }
				else if (has403) { return 403; }
				else if (has400) { return 400; }
			}

			if (Success)
			{
				return 200;
			}

			return 400;
		}

		/// <inheritdoc/>
		public string Serialize()
		{
			return Serialize(new JsonSerializerSettings());
		}

		/// <inheritdoc/>
		public string Serialize(JsonSerializerSettings serializerSettings)
		{
			return JsonConvert.SerializeObject(this, GetType(), serializerSettings);
		}

		/// <inheritdoc/>
		public new string ToString() => Serialize(new JsonSerializerSettings());
	}
}
