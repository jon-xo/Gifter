using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gifter.Repositories;
using Gifter.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gifter.Controllers
{
				[Authorize]
				[Route("api/[controller]")]
				[ApiController]
				public class UserProfileController : ControllerBase
				{

								private readonly IUserProfileRepository _userProfileRepository;

								public UserProfileController(IUserProfileRepository userProfileRepository)
								{
												_userProfileRepository = userProfileRepository;
								}
								
								// GET: api/<ValuesController>
								[HttpGet]
								public IActionResult Get()
								{
												return Ok(_userProfileRepository.GetAll());
								}

								// GET api/<ValuesController>/5
								[HttpGet("{id}")]
								public IActionResult Get(int id)
								{
												UserProfile user = _userProfileRepository.GetById(id);
												if (user == null)
												{
																return NotFound();
												}
												return Ok(user);
								}

								// POST api/<ValuesController>
								[HttpPost]
								public IActionResult Post(UserProfile user)
								{
												_userProfileRepository.Add(user);
												return CreatedAtAction("Get", new { id = user.Id }, user);
								}

								// PUT api/<ValuesController>/5
								[HttpPut("{id}")]
								public IActionResult Put(int id, UserProfile user)
								{
												if (id != user.Id)
												{
																return BadRequest();
												}

												_userProfileRepository.Update(user);
												return NoContent();
								}

								// DELETE api/<ValuesController>/5
								[HttpDelete("{id}")]
								public IActionResult Delete(int id)
								{
												_userProfileRepository.Delete(id);
												return NoContent();
								}

								private UserProfile GetCurrentUserProfile()
								{
												var firebaseUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
												return _userProfileRepository.GetByFirebaseUserId(firebaseUserId);
								}
				}
}
