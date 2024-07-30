using System.Net;

namespace MagicVilla_VillaAPI.Models
{
	public class APIResponse
	{
		public HttpStatusCode StatuCode { get; set; }
		public bool isSuccess { get; set; } = true;
        public List<String> ErrorMessage { get; set; }	
		public object Result { get; set; }
    }
}
