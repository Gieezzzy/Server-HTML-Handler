# Email: setup and testing guide

Server HTML Handler sends email for itself (console `mail` commands) and for web systems such as NEXUS-L.
One account is set up once and becomes the **default sender** for everything.

```
NEXUS-L (web) --INSERT--> dbnexus.tbl_email_outbox <--every 20 s-- Server HTML Handler (tray) --> Gmail / Outlook / SMTP
Console "mail send/test" -------------------------------------------^ (sent at once, also recorded in the table)
```

- The account and its password/token are stored only on the server, encrypted for the Windows user
  (`%LOCALAPPDATA%\ServerHTMLHandler\mail-settings.json`). The web system never sees them.
- Failed emails are retried after 5, 10, 15, 20 minutes, then marked `failed` (5 attempts).

All commands below are typed in the app's **Console**. `mail help` lists them.

---

## Step 0: before you start

1. Check this PC can reach the mail servers (PowerShell, outside the app):

   ```powershell
   Test-NetConnection smtp.gmail.com -Port 587
   Test-NetConnection smtp.office365.com -Port 587
   ```

   `TcpTestSucceeded : True` is needed for Gmail/SMTP. If it is `False`, ask IT to allow port 587 outbound.
   (Outlook uses Microsoft Graph over HTTPS port 443 instead.)

2. Choose the sender account (see "Which account" at the end). Never use a person's private mailbox.

---

## Step 1: set up the default sender (pick ONE)

### A. Gmail

1. Sign in to the Gmail account that will send (for example `nexusl.noreply@gmail.com`).
2. Google Account > Security > turn on **2-Step Verification**.
3. Google Account > Security > **App passwords** > create one (name it "NEXUS-L"). Copy the 16 letters.
4. In the console:

   ```
   mail setup gmail nexusl.noreply@gmail.com --name "NEXUS-L" --reply-to dtp_qcasst@dtshoes.com
   ```

5. A password box opens: paste the App Password and press **Save** (spaces are fine).

Expected: `[SUCCESS] Default sender saved: NEXUS-L <nexusl.noreply@gmail.com> via smtp.gmail.com:587 (TLS)`

### B. Outlook / Microsoft 365 (Microsoft sign-in, no password stored)

Needs an app registration once (IT or an Azure admin):

1. https://entra.microsoft.com > **App registrations** > **New registration**, name "NEXUS-L Mailer".
   - Company accounts only: "Accounts in this organizational directory only".
   - Also personal outlook.com accounts: "...any organizational directory and personal Microsoft accounts".
2. **Authentication** > Advanced settings > **Allow public client flows: Yes** > Save.
3. **API permissions** > Add > Microsoft Graph > **Delegated** > `Mail.Send` and `User.Read` > Add.
   If your company requires it, click **Grant admin consent**.
4. Copy the **Application (client) ID** (and the **Directory (tenant) ID** for company accounts).
5. To send as a shared mailbox (e.g. `noreply@dtshoes.com`), IT gives the signing-in account **Send As** on it.

In the console:

```
mail setup outlook noreply@dtshoes.com --client-id <application id> --tenant dtshoes.com --name "NEXUS-L" --reply-to dtp_qcasst@dtshoes.com
```

(`--tenant` can be the tenant id or domain; use `consumers` for personal outlook.com accounts. Default: `common`.)

The console shows: `To sign in, use a web browser to open https://microsoft.com/devicelogin and enter the code XXXXXXXX ...`
Open that page (any browser, even on a phone), enter the code, sign in with the sending account, accept.

Expected: `[SUCCESS] Signed in as you@dtshoes.com. Default sender: NEXUS-L <noreply@dtshoes.com>`

### C. Any other SMTP server (company relay, other providers)

```
mail setup smtp <host> <port> <from@address> [--user <login>] [--no-tls] [--name "NEXUS-L"] [--reply-to x]
```

Examples:

```
mail setup smtp smtp.office365.com 587 noreply@dtshoes.com --user noreply@dtshoes.com --name "NEXUS-L"
mail setup smtp 10.0.0.25 25 noreply@dtshoes.com --no-tls --name "NEXUS-L"
```

With `--user` a password box opens. Port 465 (implicit SSL) is not supported: use 587.

Run a setup command again at any time to replace the account. `mail remove` forgets it.

---

## Step 2: test

| # | Command | What it checks | Expected result |
|---|---------|----------------|-----------------|
| 1 | `mail status` | Account and outbox | Shows `Default sender`, `Sends via`, `Outbox sender : running`, and the queue counts |
| 2 | `mail test you@dtshoes.com` | Real email with the saved account | `[SUCCESS] Email sent to ...`, then the email arrives (check spam/junk too) |
| 3 | `mail send you@dtshoes.com "Hello" "First real email"` | Plain text email | `[SUCCESS] ...` |
| 4 | `mail send a@dtshoes.com,b@dtshoes.com "Report" "<b>Done</b>" --html` | Several recipients + HTML | Both receive it, text is bold |
| 5 | `mail send a@dtshoes.com "Hi" "With copy" --cc b@dtshoes.com` | CC (goes through the outbox) | Row shown as `sent` |
| 6 | `mail queue` | History | Your test emails listed as `sent` (green) |
| 7 | NEXUS-L test row (see Step 3), then `mail run` | Web emails | Row changes from `queued` to `sent` |
| 8 | `mail retry <id>` on a failed row | Retry | Row goes back to `queued`, then `sent` |

What the queue looks like:

```
#      Status   Try Source   Created          To                           Subject
12     sent     1   NEXUS-L  11-07 09:15:02  dtp_qcasst@dtshoes.com       Report approved
11     failed   5   NEXUS-L  11-07 08:40:10  wrong@                       Test
       ! Invalid address: wrong@
```

Old raw test (no login, no TLS; only for an internal relay): `mail test you@dtshoes.com <host> [port]`

---

## Step 3: send from the web system (NEXUS-L)

The table `tbl_email_outbox` is in the `dbnexus` database (the tray app creates it if missing).
In NEXUS-L (CodeIgniter 4) queuing an email is a plain insert:

```php
db_connect()->table('tbl_email_outbox')->insert([
    'source'     => 'NEXUS-L',                        // shown in 'mail queue'
    'to_address' => 'dtp_qcasst@dtshoes.com; qa.head@dtshoes.com',
    'cc_address' => null,
    'subject'    => 'Report LR-2026-0123 approved',
    'body'       => view('emails/report_approved', $data), // HTML
    'is_html'    => 1,
]);
```

Status values: `queued` (waiting), `sending`, `sent`, `failed` (after 5 tries; see `last_error`).
To test without code, insert one row with any MySQL tool, then run `mail run` and `mail queue`.

---

## When something fails

| Message contains | Meaning | Fix |
|---|---|---|
| `No email account is set up` | No setup yet | Step 1 |
| `Username and Password not accepted` / `5.7.8` | Wrong Gmail login | Use an **App Password**, not the normal password; check the address |
| `Must issue a STARTTLS command first` / `5.7.0` | TLS off on a TLS server | Use port 587 without `--no-tls` |
| `SmtpClientAuthentication is disabled` / `5.7.139` | Microsoft blocks password SMTP | Use `mail setup outlook` |
| `could not be reached` | Host/port blocked or wrong | Step 0 test, firewall, host name |
| `Microsoft refused the sign-in request: AADSTS700016/700038` | Wrong client id or tenant | Copy the Application (client) ID again; check `--tenant` |
| `AADSTS7000218` | Public client flows off | App registration > Authentication > Allow public client flows: Yes |
| `Outlook sign-in ... expired` | Token revoked or password changed | Run `mail setup outlook ...` again |
| `ErrorSendAsDenied` | No permission for the From mailbox | IT grants **Send As**, or set the From address to the signed-in account |
| `Invalid address` | Typo in recipient | Fix it, then `mail retry <id>` |

Logs: `mail status` shows the last error of the outbox sender.

---

## Which account to use

Best: a company mailbox such as `NEXUS-L <noreply@dtshoes.com>` (a shared mailbox costs no license),
with `--reply-to` set to a team address so replies are not lost.
Fallback: a dedicated Gmail account with an App Password. Switch later by running setup again; no code change.
