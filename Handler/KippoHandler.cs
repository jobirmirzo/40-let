using _40Let.Features;
using _40Let.Models;
using Kippo.Attribute;
using Kippo.Contexs;
using Kippo.Extensions;
using Kippo.Handlers;
using Kippo.Keyboard;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace _40Let.Handler;

public class KippoHandler(IServiceScopeFactory scopeFactory) : BotUpdateHandler
{
    [Command("start")]
    public async Task Start(Context context)
    {
        // var reply = new ReplyKeyboardMarkup(new[]
        // {
        //     new KeyboardButton("Open the menu")
        //     {
        //         WebApp = new WebAppInfo { Url = "https://tough-actually-imp.ngrok-free.app?role=admin" }
        //     }
        // })
        // {
        //     ResizeKeyboard = true,
        //     OneTimeKeyboard = true
        // };

        var reply = ReplyKeyboardBuilder.Create()
            .ContactButton("Share Contact")
            .Resize()
            .Build();
        
        context.Session?.Data["state"] = "awaiting_contact";
        await context.Reply("Welcome to my bot! Choose an option:", reply);
    }

    [Contact]
    public async Task ContactHandler(Context context)
    {
        if (context.Message?.Contact == null)
        {
            await context.Reply("Please share your contact information.");
            return;
        }
        
        var chatId = context.Update.Message!.Chat.Id;
        var contact = context.Message.Contact;
        var rawPhoneNumber = contact.PhoneNumber;
        var phoneNumber = string.IsNullOrWhiteSpace(rawPhoneNumber)
            ? ""
            : rawPhoneNumber.Replace("+", "").Replace(" ", "");
        var role = phoneNumber == "998950645042" ? "admin" : "user";
        
        var view = new BotUserView
        {
            ChatId = chatId,
            Fullname = ($"{contact.FirstName} {contact.LastName}").Trim(),
            PhoneNumber = phoneNumber,
            Role = role
        };

        using var scope = scopeFactory.CreateScope();
        var userService = GetService<IBotUserService>(scope);
        var user = await userService.GetByChatId(chatId) ?? await userService.Create(view);

      
        var reply = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton("Open the menu")
            {
                WebApp = new WebAppInfo { Url = $"https://tough-actually-imp.ngrok-free.app?clientId={user.Id}" }
            }
        })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };
        
        
        await context.Reply("Thanks! You are now registered.", reply);
    }

    private static T GetService<T>(IServiceScope scope) where T : notnull
        => scope.ServiceProvider.GetRequiredService<T>();
}