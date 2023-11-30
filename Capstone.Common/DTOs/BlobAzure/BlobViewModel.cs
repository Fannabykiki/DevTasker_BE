namespace Capstone.Common.DTOs.BlobAzure
{
	public class BlobViewModel
	{
        public string Uri { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public Stream Content { get; set; }
    }
}
