using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.DTOS;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using DatingApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]    
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _config;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> config)
        {
            _repo = repo;
            _mapper = mapper;
            _config = config;

            Account acc = new Account(_config.Value.Cloudname, _config.Value.ApiKey, _config.Value.ApiSecret);

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser( int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromrepo = await _repo.GetUser(userId);

            var file = photoForCreationDto.File;
            var uploadResult = new ImageUploadResult(); 

            if(file.Length > 0)
            {
                using(var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);

                }
            }
            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if (!userFromrepo.Photos.Any(u=>u.IsMain))
            {
                photo.IsMain = true;
            }
            userFromrepo.Photos.Add(photo);

            

            if(await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }
            return BadRequest("Could not add photot");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var user = await _repo.GetUser(userId);
            if (!user.Photos.Any(p=>p.Id==id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo.IsMain)
            {
                return BadRequest("It's already main photo");
            }

            var currentMainPhoto = await _repo.MakeMainPhoto(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if (await _repo.SaveAll())
            {
                return NoContent();
            }
            return BadRequest("Not set photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var user = await _repo.GetUser(userId);
            if (!user.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo.IsMain)
            {
                return BadRequest("You can't delete main photo");
            }

            if(photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);
                if (result.Result == "ok")
                {
                    _repo.Delete(photoFromRepo);
                }
            }

            if (photoFromRepo.PublicId == null)
            {
                _repo.Delete(photoFromRepo);
            }

            if(await _repo.SaveAll())
            {
                return Ok();
            }
            return BadRequest("Fail to delete photo");
        }
    }
}
