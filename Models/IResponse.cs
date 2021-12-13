namespace WebApiCore.Models
{
    public interface IResponse
    {
		/// <summary>
		/// Whether the call was Successful or not
		/// </summary>
		public bool Success { get; set; }
		/// <summary>
		/// An Explicitly set status code to return
		/// </summary>
		public int? OverridenStatusCode { get; set; }

		/// <summary>
		/// Errors encounted during processing of the call
		/// </summary>
		public List<IErrorCode> Errors { get; set; }
		/// <summary>
		/// Whether there are any errors or not
		/// </summary>
		public bool HasErrors { get; }
		/// <summary>
		/// The number of errors
		/// </summary>
		public int ErrorCount { get; }

		/// <summary>
		/// Add data to the result
		/// </summary>
		/// <typeparam name="T">The type of data being returned</typeparam>
		/// <param name="data">The data being returned</param>
		/// <returns></returns>
		public IDataResponse<T> WithData<T>(T data);
		/// <summary>
		/// Add an error code to the result
		/// </summary>
		/// <param name="error"></param>
		/// <returns></returns>
		public IResponse WithError(IErrorCode? error);
		/// <summary>
		/// Explicitly set the status code for the result, rather than letting it be determined automatically
		/// </summary>
		/// <param name="statusCode"></param>
		/// <returns></returns>
		public IResponse WithStatusCode(int statusCode);

		/// <summary>
		/// Used by the Middleware to determine the status code
		/// </summary>
		/// <returns></returns>
		public int CalculateStatusCode();

		/// <summary>
		/// Used by the Middleware to serialize the result.
		/// </summary>
		/// <returns></returns>
		public string Serialize();

		/// <summary>
		/// Override of ToString, should generally call Serialize()
		/// </summary>
		/// <returns></returns>
		public string ToString();
	}
}
