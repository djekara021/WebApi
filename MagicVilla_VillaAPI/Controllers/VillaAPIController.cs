using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
namespace MagicVilla_VillaAPI.Controllers
{
	[ApiController]
	[Route("api/VillaAPI")]
	public class VillaAPIController : ControllerBase
	{
		private readonly IVillaRepository _dbVilla;
		private readonly IMapper _mapper;
		protected APIResponse _response;

		public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {
			_dbVilla = dbVilla;
			_mapper = mapper;
			this._response = new();
		}



        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> GetVIllas()
		{

			try
			{
				IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
				_response.Result = _mapper.Map<List<VillaDTO>>(villaList);
				_response.StatuCode = HttpStatusCode.OK;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.ErrorMessage = new List<string>()
				{
					ex.ToString()
				};
			
			}

			return _response;
		}


		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		//[ProducesResponseType(200, Type=typeof(VillaDTO))]
		//[ProducesResponseType(404)]//solving undocumented in swagger
		[HttpGet("int:id",Name ="GetVilla")]
		public async Task<ActionResult<APIResponse>> GetVillaAsync(int id)
		{
			try
			{
				if (id == 0)
				{
					_response.StatuCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}
				var villa = await _dbVilla.GetAsync(u => u.Id == id);

				if (villa == null)
				{
					_response.StatuCode = HttpStatusCode.NotFound;
					return NotFound(_response);
				}
				_response.Result = _mapper.Map<VillaDTO>(villa);
				_response.StatuCode = HttpStatusCode.OK;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.ErrorMessage = new List<string>()
				{
					ex.ToString()
				};

			}
			return _response;

		}
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpPost]
		public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
		{

			//if (villaDTO == null)
			//{
			//	return BadRequest(villaDTO);
			//} those validations are included in [ApiController] on the begining of the class
			try
			{


				if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
				{
					ModelState.AddModelError("", "Villa already exists");
					return BadRequest(ModelState);
				}


				//if (villaDTO.Id > 0)
				//{
				//	return StatusCode(StatusCodes.Status500InternalServerError);
				//}

				Villa villa = _mapper.Map<Villa>(createDTO);

				await _dbVilla.CreateAsync(villa);
				_response.Result = _mapper.Map<VillaDTO>(villa);
				_response.StatuCode = HttpStatusCode.Created;

				return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
			}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.ErrorMessage = new List<string>()
				{
					ex.ToString()
				};

			}
			return _response;
		}



		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpDelete("int:id",Name ="DeleteVilla")]
		public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
		{
			try
			{


				if (id == 0)
				{
					return BadRequest();
				}
				var villa = await _dbVilla.GetAsync(i => i.Id == id);
				if (villa == null)
				{
					return NotFound();
				}

				await _dbVilla.RemoveAsync(villa);
				_response.Result = _mapper.Map<VillaDTO>(villa);
				_response.StatuCode = HttpStatusCode.NoContent;
				_response.isSuccess = true;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.ErrorMessage = new List<string>()
				{
					ex.ToString()
				};

			}
			return _response;
		}


		[HttpPut("int:id", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody]VillaUpdateDTO updateDTO)
		{
			try
			{
				if (updateDTO == null || id != updateDTO.Id)
				{
					return BadRequest();
				}
				//var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);

				//villa.Name = villaDTO.Name;
				//villa.Sqft = villaDTO.Sqft;
				//villa.Occupacy = villaDTO.Occupacy;

				Villa model = _mapper.Map<Villa>(updateDTO);

				await _dbVilla.UpdateAsync(model);
				_response.StatuCode = HttpStatusCode.NoContent;
				_response.isSuccess = true;

				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.ErrorMessage = new List<string>()
				{
					ex.ToString()
				};

			}
			return _response;
		}


		[HttpPatch("int:id", Name = "UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdatePartialVila(int id,JsonPatchDocument<VillaUpdateDTO> patchDTO)
		{

			try
			{
				if (patchDTO == null || id == 0)
				{
					return BadRequest();

				}

				var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);

				VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);


				if (villa == null)
				{
					return BadRequest();
				}

				patchDTO.ApplyTo(villaDTO, ModelState);

				Villa model = _mapper.Map<Villa>(villaDTO);


				_dbVilla.UpdateAsync(model);

				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}


				return NoContent();
			}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.ErrorMessage = new List<string>()
				{
					ex.ToString()
				};

			}
			return _response;
		}


	}
}
