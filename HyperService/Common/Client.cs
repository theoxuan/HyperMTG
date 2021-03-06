using System.Runtime.Serialization;

namespace HyperService.Common
{
	[DataContract]
	public class Client : Entity
	{
		public Client()
		{
		}

		public Client(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Client name
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Client finished current action
		/// </summary>
		[DataMember]
		public bool IsDone { get; set; }

		/// <summary>
		/// Whether it's a bot
		/// </summary>
		[DataMember]
		public bool IsBot { get; set; }

		/// <summary>
		/// Whether it's the host
		/// </summary>
		[DataMember]
		public bool IsHost { get; set; }
	}
}