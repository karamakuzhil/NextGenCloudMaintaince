using System;
using System.Threading.Tasks;
using AutoMapper;
using Device.Application.Device.Commands.RegisterDevice;
using Device.Application.Device.Queries.DTO;
using Device.Application.Device.Queries.FindDevice;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Devices.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DeviceController : Controller
    {
        private readonly IMediator _mediator;

        public DeviceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Booking/5
        [Produces("application/json")]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string Id)
        {
            try
            {
                Devices.Domain.AggregatesModel.Device resultSet = await _mediator.Send(new FindDeviceQuery() { DeviceId = Id });

                if (resultSet == null)
                {
                    return NotFound();
                }

                var deviceDTO = Mapper.Map<Device.Application.Device.Queries.DTO.DeviceDTO>(resultSet);

                return Ok(resultSet);

            }
            catch (Exception ex)
            {                
                return StatusCode(StatusCodes.Status500InternalServerError, "Sorry Some problem Occured");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] RegisterDeviceCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var deviceId = await _mediator.Send(command);
                //We can replace this with CreatedAtAction as well
                return StatusCode(StatusCodes.Status201Created, deviceId);

            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Sorry We are unable to register device");
            }
        }
    }
}
