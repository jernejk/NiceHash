namespace NiceHash.ElgatoStreamDeck.Models
{
	public class NiceSettingsModel
	{
		public string BaseUrl { get; set; } = "https://api2.nicehash.com";
		public string OrganizationId { get; set; }
		public string ApiKey { get; set; }
		public string ApiSecret { get; set; }

		public double UpdateInterval { get; set; } = 1;

		public string MainCurrency { get; set; }
		public string FreeCurrencyApiKey { get; set; }
	}
}
