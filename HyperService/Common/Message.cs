using System;
using System.Runtime.Serialization;

namespace HyperService.Common
{
	[DataContract]
	public class Message
	{
		/// <summary>
		/// ID of the sender player
		/// </summary>
		[DataMember]
		public Guid Sender { get; set; }

		/// <summary>
		/// Content of the message
		/// </summary>
		[DataMember]
		public string Content { get; set; }

		/// <summary>
		/// Sending time of the message
		/// </summary>
		[DataMember]
		public DateTime Time { get; set; }
	}
}