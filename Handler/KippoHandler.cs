using _40Let.Models;
using Kippo.Attribute;
using Kippo.Contexs;
using Kippo.Extensions;
using Kippo.Handlers;
using Kippo.Keyboard;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace _40Let.Handler;

public class KippoHandler : BotUpdateHandler
{
    [Command("start")]
    public async Task Start(Context context)
    {
        var reply = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton("Open the menu")
            {
                WebApp = new WebAppInfo { Url = "https://tough-actually-imp.ngrok-free.app?role=admin" }
            }
        })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };

        // var reply = ReplyKeyboardBuilder.Create()
        //     .ContactButton("Share Contact")
        //     .Build();
        //
        // context.Session?.Data["state"] = "awaiting_contact";
        await context.Reply("Welcome to my bot! Choose an option:", reply);
    }

    [Contact]
    public async Task ContactHandler(Context context)
    {
        if(context.Message?.Contact == null)
            await context.Reply("Please share your contact information.");

        var rawPhoneNumber = context.Message?.Contact.PhoneNumber;
        string phoneNumber = "";
        if(!string.IsNullOrWhiteSpace(rawPhoneNumber))
            phoneNumber = rawPhoneNumber.Replace("+", "").Replace(" ", "");

        var userDate = new BotUser
        {
            ChatId = context.Update.Message!.Chat.Id,
            Fullname = context.Message?.Contact.FirstName + " " + context.Message?.Contact.LastName ?? "unknown",
            PhoneNumber = phoneNumber,
            Role = phoneNumber == "998950645042" ? "admin" : "user"
        };
        
        
    }
}