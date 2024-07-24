using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	[ApiController]
	[Route("api/VillaAPI")]
	public class VillaAPIController : ControllerBase
	{
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<VillaDTO>> GetVIllas()
		{
			return Ok(VillaStore.villaList);
		}


		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		//[ProducesResponseType(200, Type=typeof(VillaDTO))]
		//[ProducesResponseType(404)]//solving undocumented in swagger
		[HttpGet("int:id",Name ="GetVilla")]
		public ActionResult<VillaDTO> GetVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}
			VillaDTO villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);

			if (villa == null)
			{
				return NotFound();
			}

			return Ok(villa);
		}
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpPost]
		public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
		{

			//if (villaDTO == null)
			//{
			//	return BadRequest(villaDTO);
			//} those validations are included in [ApiController] on the begining of the class

			if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
			{
				ModelState.AddModelError("", "Villa already exists");
				return BadRequest(ModelState);
			}


			if (villaDTO.Id > 0)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;

			VillaStore.villaList.Add(villaDTO);

			return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);

		}



		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpDelete("int:id",Name ="DeleteVilla")]
		public IActionResult DeleteVilla(int id)
		{
			if (id==0)
			{
				return BadRequest();
			}
			var villa = VillaStore.villaList.FirstOrDefault(i => i.Id == id);
			if (villa == null)
			{
				return NotFound();
			}

			VillaStore.villaList.Remove(villa);
			return NoContent();
		}


		[HttpPut("int:id", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
		{
			if(villaDTO == null || id != villaDTO.Id)
			{
				return BadRequest();
			}
			var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);

			villa.Name = villaDTO.Name;
			villa.Sqft = villaDTO.Sqft;
			villa.Occupacy = villaDTO.Occupacy;

			return NoContent();
		}


		[HttpPatch("int:id", Name = "UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult UpdatePartialVila(int id,JsonPatchDocument<VillaDTO> patchDTO)
		{
			if(patchDTO==null || id == 0)
			{
				return BadRequest();

			}

			var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
			if (villa == null)
			{
				return BadRequest();
			}

			patchDTO.ApplyTo(villa,ModelState);
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}


			return NoContent();
		}


	}
}
