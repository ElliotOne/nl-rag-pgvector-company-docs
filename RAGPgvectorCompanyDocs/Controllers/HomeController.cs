using Microsoft.AspNetCore.Mvc;
using RagBasics.Models;
using RagBasics.Repository;
using RagBasics.Services;
using RAGPgvectorCompanyDocs.Models;
using System.Diagnostics;

namespace RAGPgvectorCompanyDocs.Controllers
{
    public class HomeController : Controller
    {
        private readonly RagService _ragService;
        private readonly TextRepository _textRepository;

        public HomeController(RagService ragService, TextRepository textRepository)
        {
            _ragService = ragService;
            _textRepository = textRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Ask([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query parameter is required.");
            }

            var response = await _ragService.GetAnswerAsync(query);

            return Ok(new { query, response });
        }

        [HttpPost]
        public async Task<IActionResult> AddContent()
        {
            var request = await HttpContext.Request.ReadFromJsonAsync<AddTextRequest>();

            if (string.IsNullOrWhiteSpace(request?.Title))
            {
                return BadRequest("Content is required.");
            }

            if (string.IsNullOrWhiteSpace(request?.Category))
            {
                return BadRequest("Category is required.");
            }


            if (string.IsNullOrWhiteSpace(request?.Content))
            {
                return BadRequest("Content is required.");
            }

            await _textRepository.StoreTextAsync(request.Title, request.Category, request.Content);

            return Ok("Text added successfully.");
        }
 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
