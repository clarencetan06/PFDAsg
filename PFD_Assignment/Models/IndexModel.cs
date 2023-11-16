using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PFD_Assignment.Models
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;

        public IndexModel(IConfiguration config)
        {
            _config = config;
        }

        public string ApiKey { get; private set; }

        public void OnGet()
        {
            // Retrieve the API key from user secrets
            ApiKey = _config["OpenAI:apikey"];

            // call Movies service with the API key
        }
    }
}
