﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ServiceModel;
using System.Security.Cryptography;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace Nvs.Library
{

    public class Methods 
    {

       public static async Task<string> SendEmailAsync(string recipientEmail, string recipientName, string subject, string message, string htmlMessage, bool isHtml, string senderEmail = "", string senderName = "")
        {
            string result = "";

            string smtpServer = "server121.firstfind.nl";
            string userId = "nattevoetens02";
            string password = "Th1sIsAL0ngP@ssW0rd4NoR3ply";

            if (String.IsNullOrWhiteSpace(senderEmail))
            {
                senderEmail = "NoReply@nattevoetensensor.nl";
                senderName = "De NatteVoetenSensor";
            }

            var emailMessage = new MimeKit.MimeMessage();

            emailMessage.From.Add(new MimeKit.MailboxAddress(senderName, senderEmail));
            emailMessage.To.Add(new MimeKit.MailboxAddress(recipientName, recipientEmail));
            emailMessage.Subject = subject;

            if (isHtml)
            {
                MimeKit.BodyBuilder bodyBuilder = new MimeKit.BodyBuilder();
                bodyBuilder.HtmlBody = htmlMessage;
                bodyBuilder.TextBody = message;
                emailMessage.Body = bodyBuilder.ToMessageBody();
            }
            else
            {
                emailMessage.Body = new MimeKit.TextPart("plain") { Text = message };
            }


            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.LocalDomain = "api.nattevoetensensor.nl";
                    await client.ConnectAsync(smtpServer, 587, MailKit.Security.SecureSocketOptions.Auto).ConfigureAwait(false);
                    await client.AuthenticateAsync(userId, password).ConfigureAwait(false);
                    await client.SendAsync(emailMessage).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }
                result = "OK";
            }
            catch (Exception ex)
            {
                result = $"NOK: {ex.Message}";
            }

            return result;
        }


    }

}