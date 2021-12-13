namespace WebApiCore.Models
{
    public class DataResponse<T> : Response, IDataResponse<T>
    {
        /// <inheritdoc/>
        public T? Data { get; set; }
        /// <inheritdoc/>
        public IResponseMeta? Meta { get; set; }

        private DataResponse() : base() { }
        private DataResponse(IResponse? result) : base()
        {
            Success = result?.Success ?? false;
            OverridenStatusCode = result?.OverridenStatusCode;
            Errors = result?.Errors ?? new();
        }

        /// <summary>
        /// Create a new Result with Response Data from an existing Result
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static IDataResponse<T> FromResponse(IResponse? result) => new DataResponse<T>(result);

        /// <inheritdoc/>
        public IDataResponse<T> WithData(T? data)
        {
            Data = data;
            return this;
        }

        /// <inheritdoc/>
        public IDataResponse<T> WithMeta(IResponseMeta? meta)
        {
            Meta = meta;
            return this;
        }

        /// <inheritdoc/>
        public new IDataResponse<T> WithError(IErrorCode? code)
        {
            if (code is not null)
            {
                Errors.Add(code);
            }
            return this;
        }

        /// <inheritdoc/>
        public new IDataResponse<T> WithStatusCode(int statusCode)
        {
            OverridenStatusCode = statusCode;
            return this;
        }
    }
}
