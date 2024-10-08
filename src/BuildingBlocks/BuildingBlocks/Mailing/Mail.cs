﻿using MimeKit;

namespace EventPAM.BuildingBlocks.Mailing;

public class Mail
{
    public string Subject { get; set; } = default!;

    public string TextBody { get; set; } = default!;

    public string HtmlBody { get; set; } = default!;

    public AttachmentCollection? Attachments { get; set; }

    public List<MailboxAddress> ToList { get; set; } = default!;

    public List<MailboxAddress>? CcList { get; set; }

    public List<MailboxAddress>? BccList { get; set; }

    public string? UnscribeLink { get; set; }

    public Mail() { }

    public Mail(
        string subject,
        string textBody,
        string htmlBody,
        AttachmentCollection? attachments,
        List<MailboxAddress> toList,
        List<MailboxAddress>? ccList = null,
        List<MailboxAddress>? bccList = null,
        string? unscribeLink = null
    )
    {
        Subject = subject;
        TextBody = textBody;
        HtmlBody = htmlBody;
        Attachments = attachments;
        ToList = toList;
        CcList = ccList;
        BccList = bccList;
        UnscribeLink = unscribeLink;
    }
}
