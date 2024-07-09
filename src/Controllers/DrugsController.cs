using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaPlusPlus.Data;
using PharmaPlusPlus.Models;
using PharmaPlusPlus.Models.Contracts;

namespace PharmaPlusPlus.Controllers
{

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class DrugsController : ControllerBase
    {
        private readonly PharmaPlusPlusContext _context;

        public DrugsController(PharmaPlusPlusContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDrugs()
        {
            var drugs = await _context.Drugs.ToListAsync();
            return Ok(drugs);
        }

        [HttpGet]
        [Route("{drugId:Guid}")]
        public async Task<ActionResult<Drug>> GetDrugById(Guid drugId)
        {
            var drug = await _context.Drugs.FindAsync(drugId);

            if (drug == null)
            {
                return NotFound();
            }

            return drug;
        }

        [HttpGet("{drugName}")]
        public async Task<ActionResult> GetDrugByName(string drugName)
        {
            if (string.IsNullOrEmpty(drugName))
            {
                return BadRequest();
            }
            var drug = await _context.Drugs.Where(d => d.DrugName.Contains(drugName)).FirstOrDefaultAsync();

            if (drug == null)
            {
                return NotFound();
            }

            return Ok(drug);
        }

        [HttpPut("updateDetails/{drugId:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDrugDetails(Guid drugId, UpdateDrugRequest request)
        {
            var drug = await _context.Drugs.FindAsync(drugId);
            if (drug is null)
            {
                return NotFound();
            }
            drug.DrugName = request.DrugName ?? drug.DrugName;
            drug.DrugDescription = request.DrugDescription ?? drug.DrugDescription;
            drug.DrugPrice = request.DrugPrice ?? drug.DrugPrice;
            drug.DrugQuantityAvailable = request.DrugQuantityAvailable ?? drug.DrugQuantityAvailable;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Drug>> PostDrug(CreateDrugRequest request)
        {
            var drug = new Drug
            {
                Id = Guid.NewGuid(),
                DrugName = request.DrugName,
                DrugDescription = request.DrugDescription,
                DrugPrice = request.DrugPrice,
                DrugQuantityAvailable = request.DrugQuantityAvailable
            };
            _context.Drugs.Add(drug);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDrugById", new { drugId = drug.Id }, new CreateDrugResponse { DrugId = drug.Id });
        }

        [HttpDelete("{drugId:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDrug(Guid drugId)
        {
            var drug = await _context.Drugs.FindAsync(drugId);
            if (drug is null)
            {
                return NotFound();
            }

            _context.Drugs.Remove(drug);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
