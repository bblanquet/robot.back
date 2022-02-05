using Bob.raspberry.api.Core;
using Bob.raspberry.api.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;

namespace Bob.raspberry.api.Hubs
{
    [ApiController]
    [Route("[controller]")]
    public class RobotController : ControllerBase
    {
        private const string CAPTURE = "CAPTURE";

        [HttpGet("status")]
        public RobotStatus Get()
        {
            return SocketHandler.IsRobotConnected() ? new RobotStatus { IsOnline = true} : new RobotStatus { IsOnline = false };
        }

        [HttpGet("order")]
        public ActionResult Send(string message)
        {
            _ = SocketHandler.GetRobotSocket().Send(Encoding.UTF8.GetBytes(message));
            return Content("ok");
        }

        [HttpGet("capture")]
        public IActionResult Capture()
        {
            var messageWaiter = new MessageReceiver(SocketHandler.GetRobotSocket());
            _ = SocketHandler.GetRobotSocket().Send(Encoding.UTF8.GetBytes(CAPTURE));
            var base64Frag = Encoding.ASCII.GetString(messageWaiter.Receive(5000));
            var bytes = Convert.FromBase64String(base64Frag);
            return File(bytes, "image/jpg","picture.jpg");
        }
    }
}

