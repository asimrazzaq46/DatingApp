using System;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// Http://localhost:5001/api/[controller]   => controller will be change by the name
//                                              of file in this case it's "users"

[Route("api/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{

}
