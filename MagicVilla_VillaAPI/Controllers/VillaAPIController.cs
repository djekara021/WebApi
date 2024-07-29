using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
	[ApiController]
	[Route("api/VillaAPI")]
	public class VillaAPIController : ControllerBase
	{
		private readonly IVillaRepository _dbVilla;
		private readonly IMapper _mapper;

		public VillaAPIController(IVillaRepository _dbVilla, IMapper mapper)
        {
			_dbVilla = _dbVilla;
			_mapper = mapper;
		}



        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVIllas()
		{
			IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
			return Ok(_mapper.Map<List<VillaDTO>>(villaList));
		}


		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		//[ProducesResponseType(200, Type=typeof(VillaDTO))]
		//[ProducesResponseType(404)]//solving undocumented in swagger
		[HttpGet("int:id",Name ="GetVilla")]
		public async Task<ActionResult<VillaDTO>> GetVillaAsync(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}
			var villa = await _dbVilla.GetAsync(u => u.Id == id);

			if (villa == null)
			{
				return NotFound();
			}

			return Ok(_mapper.Map<VillaDTO>(villa));
		}
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpPost]
		public async Task<ActionResult<VillaUpdateDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
		{

			//if (villaDTO == null)
			//{
			//	return BadRequest(villaDTO);
			//} those validations are included in [ApiController] on the begining of the class

			if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
			{
				ModelState.AddModelError("", "Villa already exists");
				return BadRequest(ModelState);
			}


			//if (villaDTO.Id > 0)
			//{
			//	return StatusCode(StatusCodes.Status500InternalServerError);
			//}

			Villa model = _mapper.Map<Villa>(createDTO);

			await _dbVilla.CreateAsync(model);


			return CreatedAtRoute("GetVilla", new { id = model.Id }, model);

		}



		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpDelete("int:id",Name ="DeleteVilla")]
		public async Task<IActionResult> DeleteVilla(int id)
		{
			if (id==0)
			{
				return BadRequest();
			}
			var villa =  await _dbVilla.GetAsync(i => i.Id == id);
			if (villa == null)
			{
				return NotFound();
			}

			await _dbVilla.RemoveAsync(villa);

			return NoContent();
		}


		[HttpPut("int:id", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaUpdateDTO updateDTO)
		{
			if(updateDTO == null || id != updateDTO.Id)
			{
				return BadRequest();
			}
			//var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);

			//villa.Name = villaDTO.Name;
			//villa.Sqft = villaDTO.Sqft;
			//villa.Occupacy = villaDTO.Occupacy;

			Villa model = _mapper.Map<Villa>(updateDTO);
			 await _dbVilla.UpdateAsync(model);
			


			return NoContent();
		}


		[HttpPatch("int:id", Name = "UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdatePartialVila(int id,JsonPatchDocument<VillaUpdateDTO> patchDTO)
		{
			if(patchDTO==null || id == 0)
			{
				return BadRequest();

			}

			var villa = await _dbVilla.GetAsync(u => u.Id == id,tracked:false);

			VillaUpdateDTO villaDTO= _mapper.Map<VillaUpdateDTO>(villa);
		

			if (villa == null)
			{
				return BadRequest();
			}

			patchDTO.ApplyTo(villaDTO,ModelState);

			Villa model  = _mapper.Map<Villa>(villaDTO);


			_dbVilla.UpdateAsync(model);

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}


			return NoContent();
		}


	}
}
