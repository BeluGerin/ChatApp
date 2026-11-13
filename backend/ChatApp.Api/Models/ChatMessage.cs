namespace ChatApp.Api.Models;

// Dikirim ke klien sebagai JSON camelCase: { user, text, sentAt }
public record ChatMessage(string User, string Text, DateTime SentAt);
