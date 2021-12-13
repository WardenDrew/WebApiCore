namespace WebApiCore.Models
{
    public interface IDataResponse<T> : IResponse
    {
		/// <summary>
		/// Data to return
		/// </summary>
		public T? Data { get; set; }
		/// <summary>
		/// Metadata to return. Currently is useful for array responses
		/// </summary>
		public IResponseMeta? Meta { get; set; }

		/// <summary>
		/// Set the data in the result
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public IDataResponse<T> WithData(T? data);
		/// <summary>
		/// Set the metadata in the result
		/// </summary>
		/// <param name="meta"></param>
		/// <returns></returns>
		public IDataResponse<T> WithMeta(IResponseMeta? meta);
		/// <summary>
		/// Add an error code to the result
		/// </summary>
		/// <param name="error"></param>
		/// <returns></returns>
		public new IDataResponse<T> WithError(IErrorCode? error);
		/// <summary>
		/// Explicitly set the status code for the result, rather than letting it be determined automatically
		/// </summary>
		/// <param name="statusCode"></param>
		/// <returns></returns>
		public new IDataResponse<T> WithStatusCode(int statusCode);
	}
}
