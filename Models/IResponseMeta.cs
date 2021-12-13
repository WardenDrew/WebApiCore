namespace WebApiCore.Models
{
    public interface IResponseMeta
    {
		/// <summary>
		/// Number of items returned in this response
		/// </summary>
		public int NumReturned { get; set; }
		/// <summary>
		/// Number of items skipped in this response
		/// </summary>
		public int NumSkipped { get; set; }
		/// <summary>
		/// Number of items total on the server
		/// </summary>
		public int NumTotal { get; set; }

		/// <summary>
		/// Number of items remaining on the server that have not been returned
		/// </summary>
		public int NumRemaining { get; }

		/// <summary>
		/// Are there no more items remaining
		/// </summary>
		public bool EndOfResults { get; }
	}
}
