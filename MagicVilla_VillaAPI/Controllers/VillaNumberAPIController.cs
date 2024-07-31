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
	[Route("api/VillaNumberAPI")]
	public class VillaNumberAPIController : ControllerBase
	{
		private readonly IVillaNumberRepository _dbVillaNumber;
		private readonly IMapper _mapper;
		protected APIResponse _response;

		public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IMapper mapper)
        {
			_dbVillaNumber = dbVillaNumber;
			_mapper = mapper;
			this._response = new();
		}



        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> GetVIllaNumbers()
		{

			try
			{
				IEnumerable<VillaNumber> villaList = await _dbVillaNumber.GetAllAsync();
				_response.Result = _mapper.Map<List<VillaNumberDTO>>(villaList);
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
		[HttpGet("int:id",Name ="GetVillaNumber")]
		public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
		{
			try
			{
				if (id == 0)
				{
					_response.StatuCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}
				var villa = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);

				if (villa == null)
				{
					_response.StatuCode = HttpStatusCode.NotFound;
					return NotFound(_response);
				}
				_response.Result = _mapper.Map<VillaNumberDTO>(villa);
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
		public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
		{

			try
			{


				if (await _dbVillaNumber.GetAsync(u => u.VillaNo == createDTO.VillaNo) != null)
				{
					ModelState.AddModelError("", "Villa already exists");
					return BadRequest(ModelState);
				}



				VillaNumber villa = _mapper.Map<VillaNumber>(createDTO);

				await _dbVillaNumber.CreateAsync(villa);
				_response.Result = _mapper.Map<VillaDTO>(villa);
				_response.StatuCode = HttpStatusCode.Created;

				return CreatedAtRoute("GetVilla", new { id = villa.VillaNo }, _response);
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
		[HttpDelete("int:id",Name ="DeleteVillaNumber")]
		public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
		{
			try
			{


				if (id == 0)
				{
					return BadRequest();
				}
				var villa = await _dbVillaNumber.GetAsync(i => i.VillaNo == id);
				if (villa == null)
				{
					return NotFound();
				}

				await _dbVillaNumber.RemoveAsync(villa);
				_response.Result = _mapper.Map<VillaNumberDTO>(villa);
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


		[HttpPut("int:id", Name = "UpdateVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody]VillaNumberUpdateDTO updateDTO)
		{
			try
			{
				if (updateDTO == null || id != updateDTO.VillaNo)
				{
					return BadRequest();
				}

				VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);

				await _dbVillaNumber.UpdateAsync(model);
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



	}
}
