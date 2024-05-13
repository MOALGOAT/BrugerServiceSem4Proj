using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BrugerServiceAPI.Models;

namespace BrugerServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrugerController : ControllerBase
    {
        private readonly IBrugerInterface _brugerService;
        private readonly ILogger<BrugerController> _logger; 

        public BrugerController(IBrugerInterface brugerService, ILogger<BrugerController> logger) 
        {
            _brugerService = brugerService;
            _logger = logger; 
        }

        [HttpGet("{bruger_id}")]
        public async Task<ActionResult<Bruger>> GetBruger(Guid brugerID)
        {
            var bruger = await _brugerService.GetBruger(brugerID);
            if (bruger == null)
            {
                return NotFound();
            }
            return bruger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bruger>>> GetBrugerList()
        {
            var brugerList = await _brugerService.GetBrugerList();
            if (brugerList == null) { throw new ApplicationException("listen er null"); };
            return Ok(brugerList);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> AddBruger(Bruger bruger)
        {
            var brugerID = await _brugerService.AddBruger(bruger);
            return CreatedAtAction(nameof(GetBruger), new { b = brugerID }, brugerID);
        }

        [HttpPut("{bruger_id}")]
        public async Task<IActionResult> UpdateBruger(Guid id, Bruger bruger)
        {
            if (id != bruger.brugerID)
            {
                return BadRequest();
            }

            var result = await _brugerService.UpdateBruger(bruger);
            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{bruger_id}")]
        public async Task<IActionResult> DeleteBruger(Guid bruger_id)
        {
            var result = await _brugerService.DeleteBruger(bruger_id);
            if (result == 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
