using System.Collections.Concurrent;
using ChatApp.Api.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Hubs;

// Slide 7: Hub = titik komunikasi real-time antara server dan klien
public class ChatHub : Hub
{
    private const int MaxMessageLength = 500;

    // ConnectionId -> nama pengguna. Disimpan di memori (cukup untuk praktikum,
    // untuk skala besar gunakan penyimpanan terpusat seperti Redis).
    private static readonly ConcurrentDictionary<string, string> Users = new();

    // Dipanggil klien setelah koneksi berhasil
    public async Task JoinChat(string username)
    {
        var name = username.Trim();
        if (string.IsNullOrEmpty(name))
        {
            throw new HubException("Nama pengguna tidak boleh kosong.");
        }

        Users[Context.ConnectionId] = name;

        await Clients.All.SendAsync("ReceiveMessage",
            new ChatMessage("Sistem", $"{name} bergabung ke obrolan.", DateTime.UtcNow));
        await BroadcastOnlineUsers();
    }

    // Slide 7: Clients.All.SendAsync(...) untuk broadcast ke semua klien
    public async Task SendMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var text = message.Trim();
        if (text.Length > MaxMessageLength)
        {
            text = text[..MaxMessageLength];
        }

        var sender = Users.TryGetValue(Context.ConnectionId, out var name) ? name : "Anonim";

        await Clients.All.SendAsync("ReceiveMessage",
            new ChatMessage(sender, text, DateTime.UtcNow));
    }

    // Dipanggil otomatis saat klien terputus (tab ditutup, jaringan putus, dsb.)
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Users.TryRemove(Context.ConnectionId, out var name))
        {
            await Clients.All.SendAsync("ReceiveMessage",
                new ChatMessage("Sistem", $"{name} meninggalkan obrolan.", DateTime.UtcNow));
            await BroadcastOnlineUsers();
        }

        await base.OnDisconnectedAsync(exception);
    }

    private Task BroadcastOnlineUsers() =>
        Clients.All.SendAsync("OnlineUsers", Users.Values.Distinct().Order().ToArray());
}
