﻿using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatFactController : ControllerBase
    {
        private readonly ICatFactRepository _repository;

        public CatFactController(ICatFactRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _repository.GetAllAsync());
    }
}
