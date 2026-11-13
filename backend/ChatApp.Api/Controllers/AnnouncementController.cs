using ChatApp.Api.Hubs;
using ChatApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Controllers;

public record AnnouncementRequest(string Text);

// Slide 9: memicu notifikasi dari Controller memakai IHubContext
[ApiController]
[Route("api/announcement")]
public class AnnouncementController(IHubContext<ChatHub> hubContext) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Send([FromBody] AnnouncementRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest("Teks pengumuman tidak boleh kosong.");
        }

        await hubContext.Clients.All.SendAsync("ReceiveMessage",
            new ChatMessage("Pengumuman", request.Text.Trim(), DateTime.UtcNow));

        return Ok();
    }
}
