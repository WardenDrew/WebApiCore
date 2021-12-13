namespace WebApiCore.Models
{
    public class ResponseMeta : IResponseMeta
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
		public int NumRemaining
		{
			get
			{
				int remain = NumTotal - NumSkipped - NumReturned;
				if (remain < 0)
				{
					remain = 0;
				}

				return remain;
			}
		}

		/// <summary>
		/// Are there no more items remaining
		/// </summary>
		public bool EndOfResults
		{
			get
			{
				return NumRemaining == 0;
			}
		}
	}
}
