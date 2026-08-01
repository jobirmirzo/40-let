using _40Let.Enum;
using _40Let.Features;
using _40Let.Models;
using Kippo.Attribute;
using Kippo.Contexs;
using Kippo.Extensions;
using Kippo.Handlers;
using Kippo.Keyboard;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace _40Let.Handler;

public class KippoHandler(
    IServiceScopeFactory scopeFactory,
    IOptions<WebAppOptions> webAppOptions,
    IOptions<SuperAdminOptions> superAdminOptions) : BotUpdateHandler
{
    [Command("start")]
    public async Task Start(Context context)
    {
        var chatId = context.ChatId;

        if (superAdminOptions.Value.IsSuperAdmin(chatId))
        {
            await StartSuperAdmin(context, chatId);
            return;
        }

        var reply = ReplyKeyboardBuilder.Create()
            .ContactButton("Share Contact")
            .Resize()
            .Build();

        context.Session?.Data["state"] = "awaiting_contact";
        await context.Reply("Welcome to my bot! Choose an option:", reply);
    }

    /// <summary>
    /// A superadmin is authorized purely by chat id (from config), not by
    /// sharing a phone number, so their BotUser row is bootstrapped right here
    /// instead of waiting on the contact-share flow — they're never asked to
    /// share contact at all. This runs on every /start, so a chat id that was
    /// already registered under some other role (e.g. plain "user", from
    /// before it was added to SuperAdmin:ChatIds) gets promoted too, not just
    /// brand-new chat ids. They get both the normal menu button and an extra
    /// "Manage admins" button that opens a dedicated Mini App page
    /// (?page=admins) for promoting/demoting other users.
    /// </summary>
    private async Task StartSuperAdmin(Context context, long chatId)
    {
        using var scope = scopeFactory.CreateScope();
        var userService = GetService<IBotUserService>(scope);

        var from = context.Update.Message?.From;
        var fullname = from is null ? null : $"{from.FirstName} {from.LastName}".Trim();
        var user = await userService.EnsureSuperAdmin(chatId, fullname);

        var reply = new ReplyKeyboardMarkup(new[]
        {
            new[]
            {
                new KeyboardButton("Open the menu")
                {
                    WebApp = new WebAppInfo { Url = $"{webAppOptions.Value.Url}?clientId={user.Id}" }
                }
            },
            new[]
            {
                new KeyboardButton("Adminlarni boshqarish")
                {
                    WebApp = new WebAppInfo { Url = $"{webAppOptions.Value.Url}?clientId={user.Id}&page=admins" }
                }
            }
        })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };

        await context.Reply("Welcome back, superadmin! Choose an option:", reply);
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
        
        Role role = Role.User;
        if (phoneNumber is "998950645042" or "998909763696")
            role = Role.Admin;
        
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
                WebApp = new WebAppInfo { Url = $"{webAppOptions.Value.Url}?clientId={user.Id}" }
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