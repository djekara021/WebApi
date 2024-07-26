using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
	[ApiController]
	[Route("api/VillaAPI")]
	public class VillaAPIController : ControllerBase
	{
		private readonly ApplicationDbContext _db;
		public VillaAPIController(ApplicationDbContext db)
        {
			_db = db;
		}



        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<VillaDTO>> GetVIllas()
		{

			return Ok(_db.Villas.ToList());
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
			var villa = _db.Villas.FirstOrDefault(u => u.Id == id);

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

			if (_db.Villas.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
			{
				ModelState.AddModelError("", "Villa already exists");
				return BadRequest(ModelState);
			}


			if (villaDTO.Id > 0)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			Villa model = new Villa()
			{
				Amenity = villaDTO.Amenity,
				Details = villaDTO.Details,
				Id = villaDTO.Id,
				ImageUrl = villaDTO.ImageUrl,
				Name = villaDTO.Name,
				Occupancy = villaDTO.Occupancy,
				Rate = villaDTO.Rate,
				Sqft = villaDTO.Sqft
			};
			_db.Villas.Add(model);
			_db.SaveChanges();


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
			var villa = _db.Villas.FirstOrDefault(i => i.Id == id);
			if (villa == null)
			{
				return NotFound();
			}

			_db.Villas.Remove(villa);
			_db.SaveChanges();
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
			//var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);

			//villa.Name = villaDTO.Name;
			//villa.Sqft = villaDTO.Sqft;
			//villa.Occupacy = villaDTO.Occupacy;

			Villa model = new Villa()
			{
				Amenity = villaDTO.Amenity,
				Details = villaDTO.Details,
				Id = villaDTO.Id,
				ImageUrl = villaDTO.ImageUrl,
				Name = villaDTO.Name,
				Occupancy = villaDTO.Occupancy,
				Rate = villaDTO.Rate,
				Sqft = villaDTO.Sqft
			};
			_db.Villas.Update(model);
			_db.SaveChanges();


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

			var villa = _db.Villas.AsNoTracking().FirstOrDefault(u => u.Id == id);

			VillaDTO villaDTO = new ()
			{
				Amenity = villa.Amenity,
				Details = villa.Details,
				Id = villa.Id,
				ImageUrl = villa.ImageUrl,
				Name = villa.Name,
				Occupancy = villa.Occupancy,
				Rate = villa.Rate, 
				Sqft = villa.Sqft
			};


			if (villa == null)
			{
				return BadRequest();
			}

			patchDTO.ApplyTo(villaDTO,ModelState);

			Villa model = new Villa()
			{
				Amenity = villaDTO.Amenity,
				Details = villaDTO.Details,
				Id = villaDTO.Id,
				ImageUrl = villaDTO.ImageUrl,
				Name = villaDTO.Name,
				Occupancy = villaDTO.Occupancy,
				Rate = villaDTO.Rate,
				Sqft = villaDTO.Sqft
			};

			_db.Update(model);
			_db.SaveChanges();

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}


			return NoContent();
		}


	}
}
