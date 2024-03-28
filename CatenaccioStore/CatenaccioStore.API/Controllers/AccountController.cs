using AutoMapper;
using CatenaccioStore.API.Errors;
using CatenaccioStore.API.Infrastructure.Extensions;
using CatenaccioStore.API.Infrastructure.Helpers;
using CatenaccioStore.API.Infrastructure.Models;
using CatenaccioStore.Core.DTOs;
using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Entities.Identities;
using CatenaccioStore.Core.Repositories.Abstraction;
using CatenaccioStore.Infrastructure.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace CatenaccioStore.API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IUserConfirmationRepository _userConfirmationRepository;
        private readonly IUserConfirmationService _userConfirmationService;
        private readonly TokenGeneratorService _tokenGenerator;
        private readonly IMapper _mapper;
        private readonly GoogleAppSettings _googleAppSettings;

        public AccountController(UserManager<AppUser> userManager, IOptions<GoogleAppSettings> googleAppSettings, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper, IUserConfirmationRepository userConfirmationRepository, IUserConfirmationService userConfirmationService, TokenGeneratorService tokenGenerator)
        {
            _googleAppSettings = googleAppSettings.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _userConfirmationRepository = userConfirmationRepository;
            _userConfirmationService = userConfirmationService;
            _tokenGenerator = tokenGenerator;
        }


        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken != null && jwtToken.Claims != null)
                {
                    var userEmail = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                    var tokenFromDb = await _userConfirmationRepository.GetToken(userEmail, default);

                    if (tokenFromDb != null && tokenFromDb.Token == jwtToken.RawData)
                    {
                        var user = await _userManager.FindByEmailAsync(userEmail);
                        if (user != null)
                        {
                            user.EmailConfirmed = true;
                            var result = await _userManager.UpdateAsync(user);
                            if (result.Succeeded)
                            {
                                var res = await _userConfirmationService.DeleteAsync(default, userEmail);
                                if (res == true)
                                    return Ok(res);
                                else
                                    throw new Exception("Failed");
                            }
                            else
                            {
                                throw new Exception("Confirmation Failed");
                            }
                        }
                        else
                            throw new Exception("Confirmation Failed");
                    }
                    else
                    {
                        throw new Exception("Invalid token");
                    }
                }
                else
                {
                    throw new Exception("Invalid token");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailFromClaimsPrincipal(User);
            return new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsycn([FromQuery]string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await _userManager.FindUserByClaimsPrincipalViaAddress(User);
            return _mapper.Map<Address,AddressDto>(user.Address);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if(user == null)
                return Unauthorized(new ApiResponse(401));
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password,false);
            if(user.EmailConfirmed == false)
                return Unauthorized(new ApiResponse(401, "You have to activate account from email"));
            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401,"Incorrect password or email"));
            return new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (CheckEmailExistsAsycn(registerDto.Email).Result.Value)
                return new BadRequestObjectResult(new ApiValidationErrorResponse {Errors = new[]{"Email address is already in use"}});
            var user = new AppUser { Email = registerDto.Email, DisplayName = registerDto.DisplayName, UserName = registerDto.Email };
            var result = await _userManager.CreateAsync(user,registerDto.Password);
            if(!result.Succeeded)
                return BadRequest(new ApiResponse(400));

            //Send Token
            var tokenExists = await _userConfirmationService.GetToken(user.Email, default);
            if (tokenExists != null)
                throw new Exception("Token Already Exists");
            string token = _tokenGenerator.GenerateToken(user.Email);
            string subject = "Password Confirmation Link";
            var recoveryToken = new UserConfirmationToken
            {
                UserEmail = user.Email,
                Token = token,
                ExpiryDate = DateTime.UtcNow.AddHours(170)
            };
            var check = await _userConfirmationRepository.GetToken(user.Email, default);
            if (check != null)
                throw new Exception("Token already exists");
            var res = await _userConfirmationRepository.CreateAsync(CancellationToken.None, recoveryToken);
            if (res == false)
                throw new Exception("Can't save token to database!");
            await SendNotification(user.DisplayName, user.Email, subject, token);




            return new UserDto { Email = user.Email, DisplayName = user.DisplayName,
                Token = _tokenService.CreateToken(user)
            };
        }
        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto address)
        {
            var user = await _userManager.FindUserByClaimsPrincipalViaAddress(HttpContext.User);
            user.Address = _mapper.Map<AddressDto,Address>(address);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok(_mapper.Map<Address,AddressDto>(user.Address));
            return BadRequest("Problem updating address of user!");
        }



        #region SendNotification

        private async Task SendNotification(string displayName, string email, string subject, string token)
        {
            try
            {
                string googleAppPassword = _googleAppSettings.GoogleAppPassword;
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("catenaccio.geo@gmail.com");
                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                string confirmationLink = $"https://localhost:4200/confirmation/?token={token}";
                string customHtml = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html dir=""ltr"" xmlns=""http://www.w3.org/1999/xhtml"" xmlns:o=""urn:schemas-microsoft-com:office:office"" lang=""en"" style=""font-family:arial, 'helvetica neue', helvetica, sans-serif"">
 <head>
  <meta charset=""UTF-8"">
  <meta content=""width=device-width, initial-scale=1"" name=""viewport"">
  <meta name=""x-apple-disable-message-reformatting"">
  <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
  <meta content=""telephone=no"" name=""format-detection"">
  <title>New Template 5</title><!--[if (mso 16)]>
    <style type=""text/css"">
    a {text-decoration: none;}
    </style>
    <![endif]--><!--[if gte mso 9]><style>sup { font-size: 100% !important; }</style><![endif]--><!--[if gte mso 9]>
<xml>
    <o:OfficeDocumentSettings>
    <o:AllowPNG></o:AllowPNG>
    <o:PixelsPerInch>96</o:PixelsPerInch>
    </o:OfficeDocumentSettings>
</xml>
<![endif]--><!--[if !mso]><!-- -->
  <link href=""https://fonts.googleapis.com/css2?family=Barlow&display=swap"" rel=""stylesheet"">
  <link href=""https://fonts.googleapis.com/css2?family=Barlow+Condensed&display=swap"" rel=""stylesheet""><!--<![endif]-->
  <style type=""text/css"">
.rollover span {
	font-size:0;
}
.rollover:hover .rollover-first {
	max-height:0px!important;
	display:none!important;
}
.rollover:hover .rollover-second {
	max-height:none!important;
	display:block!important;
}
#outlook a {
	padding:0;
}
.es-button {
	mso-style-priority:100!important;
	text-decoration:none!important;
}
a[x-apple-data-detectors] {
	color:inherit!important;
	text-decoration:none!important;
	font-size:inherit!important;
	font-family:inherit!important;
	font-weight:inherit!important;
	line-height:inherit!important;
}
.es-desk-hidden {
	display:none;
	float:left;
	overflow:hidden;
	width:0;
	max-height:0;
	line-height:0;
	mso-hide:all;
}
@media only screen and (max-width:600px) {p, ul li, ol li, a { line-height:150%!important } h1, h2, h3, h1 a, h2 a, h3 a { line-height:120% } h1 { font-size:46px!important; text-align:left } h2 { font-size:28px!important; text-align:left } h3 { font-size:20px!important; text-align:center } .es-header-body h1 a, .es-content-body h1 a, .es-footer-body h1 a { font-size:46px!important; text-align:left } .es-header-body h2 a, .es-content-body h2 a, .es-footer-body h2 a { font-size:28px!important; text-align:left } .es-header-body h3 a, .es-content-body h3 a, .es-footer-body h3 a { font-size:20px!important; text-align:center } .es-menu td a { font-size:12px!important } .es-header-body p, .es-header-body ul li, .es-header-body ol li, .es-header-body a { font-size:14px!important } .es-content-body p, .es-content-body ul li, .es-content-body ol li, .es-content-body a { font-size:14px!important } .es-footer-body p, .es-footer-body ul li, .es-footer-body ol li, .es-footer-body a { font-size:14px!important } .es-infoblock p, .es-infoblock ul li, .es-infoblock ol li, .es-infoblock a { font-size:12px!important } *[class=""gmail-fix""] { display:none!important } .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3 { text-align:center!important } .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3 { text-align:right!important } .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3 { text-align:left!important } .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img { display:inline!important } .es-button-border { display:inline-block!important } a.es-button, button.es-button { font-size:18px!important; display:inline-block!important } .es-adaptive table, .es-left, .es-right { width:100%!important } .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header { width:100%!important; max-width:600px!important } .es-adapt-td { display:block!important; width:100%!important } .adapt-img { width:100%!important; height:auto!important } .es-m-p0 { padding:0!important } .es-m-p0r { padding-right:0!important } .es-m-p0l { padding-left:0!important } .es-m-p0t { padding-top:0!important } .es-m-p0b { padding-bottom:0!important } .es-m-p20b { padding-bottom:20px!important } .es-mobile-hidden, .es-hidden { display:none!important } tr.es-desk-hidden, td.es-desk-hidden, table.es-desk-hidden { width:auto!important; overflow:visible!important; float:none!important; max-height:inherit!important; line-height:inherit!important } tr.es-desk-hidden { display:table-row!important } table.es-desk-hidden { display:table!important } td.es-desk-menu-hidden { display:table-cell!important } .es-menu td { width:1%!important } table.es-table-not-adapt, .esd-block-html table { width:auto!important } table.es-social { display:inline-block!important } table.es-social td { display:inline-block!important } .es-desk-hidden { display:table-row!important; width:auto!important; overflow:visible!important; max-height:inherit!important } .es-m-p5 { padding:5px!important } .es-m-p5t { padding-top:5px!important } .es-m-p5b { padding-bottom:5px!important } .es-m-p5r { padding-right:5px!important } .es-m-p5l { padding-left:5px!important } .es-m-p10 { padding:10px!important } .es-m-p10t { padding-top:10px!important } .es-m-p10b { padding-bottom:10px!important } .es-m-p10r { padding-right:10px!important } .es-m-p10l { padding-left:10px!important } .es-m-p15 { padding:15px!important } .es-m-p15t { padding-top:15px!important } .es-m-p15b { padding-bottom:15px!important } .es-m-p15r { padding-right:15px!important } .es-m-p15l { padding-left:15px!important } .es-m-p20 { padding:20px!important } .es-m-p20t { padding-top:20px!important } .es-m-p20r { padding-right:20px!important } .es-m-p20l { padding-left:20px!important } .es-m-p25 { padding:25px!important } .es-m-p25t { padding-top:25px!important } .es-m-p25b { padding-bottom:25px!important } .es-m-p25r { padding-right:25px!important } .es-m-p25l { padding-left:25px!important } .es-m-p30 { padding:30px!important } .es-m-p30t { padding-top:30px!important } .es-m-p30b { padding-bottom:30px!important } .es-m-p30r { padding-right:30px!important } .es-m-p30l { padding-left:30px!important } .es-m-p35 { padding:35px!important } .es-m-p35t { padding-top:35px!important } .es-m-p35b { padding-bottom:35px!important } .es-m-p35r { padding-right:35px!important } .es-m-p35l { padding-left:35px!important } .es-m-p40 { padding:40px!important } .es-m-p40t { padding-top:40px!important } .es-m-p40b { padding-bottom:40px!important } .es-m-p40r { padding-right:40px!important } .es-m-p40l { padding-left:40px!important } }
@media screen and (max-width:384px) {.mail-message-content { width:414px!important } }
</style>
 </head>
 <body style=""width:100%;font-family:arial, 'helvetica neue', helvetica, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0"">
  <div dir=""ltr"" class=""es-wrapper-color"" lang=""en"" style=""background-color:#102B3F""><!--[if gte mso 9]>
			<v:background xmlns:v=""urn:schemas-microsoft-com:vml"" fill=""t"">
				<v:fill type=""tile"" color=""#102b3f""></v:fill>
			</v:background>
		<![endif]-->
   <table class=""es-wrapper"" width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;background-color:#102B3F"">
     <tr>
      <td valign=""top"" style=""padding:0;Margin:0"">
       <table cellpadding=""0"" cellspacing=""0"" class=""es-header"" align=""center"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top"">
         <tr>
          <td align=""center"" style=""padding:0;Margin:0"">
           <table bgcolor=""#ffffff"" class=""es-header-body"" align=""center"" cellpadding=""0"" cellspacing=""0"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#102B3F;width:600px"">
             <tr>
              <td class=""es-m-p20r es-m-p20l"" align=""left"" style=""padding:0;Margin:0;padding-top:20px"">
               <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                 <tr>
                  <td class=""es-m-p0r"" valign=""top"" align=""center"" style=""padding:0;Margin:0;width:600px"">
                   <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                     <tr>
                      <td align=""center"" class=""es-m-txt-c"" style=""padding:0;Margin:0;font-size:0px""><a target=""_blank"" href=""https://viewstripo.email"" style=""-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:underline;color:#E2CFEA;font-size:14px""><img src=""https://drive.google.com/file/d/1YniLN6gExbPCIJpPN_OWaQegnoosn1uC/view"" alt=""Logo"" style=""display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic"" title=""Logo"" height=""115""></a></td>
                     </tr>
                   </table></td>
                 </tr>
               </table></td>
             </tr>
           </table></td>
         </tr>
       </table>
       <table class=""es-content"" cellspacing=""0"" cellpadding=""0"" align=""center"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%"">
         <tr>
          <td align=""center"" style=""padding:0;Margin:0;background-image:url(https://fecyrch.stripocdn.email/content/guids/CABINET_6306d45fd9ea3b681ebe3a603101f0275312c7c136d6957f7ed43fa4b22490f7/images/frame_375_tsP.png);background-repeat:no-repeat;background-position:center top"" background=""https://fecyrch.stripocdn.email/content/guids/CABINET_6306d45fd9ea3b681ebe3a603101f0275312c7c136d6957f7ed43fa4b22490f7/images/frame_375_tsP.png"">
           <table class=""es-content-body"" cellspacing=""0"" cellpadding=""0"" align=""center"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:600px"" role=""none"">
             <tr>
              <td class=""es-m-p10b es-m-p20r es-m-p20l"" align=""left"" style=""padding:0;Margin:0;padding-top:20px"">
               <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                 <tr>
                  <td class=""es-m-p0r es-m-p20b"" valign=""top"" align=""center"" style=""padding:0;Margin:0;width:600px"">
                   <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                     <tr>
                      <td align=""center"" class=""es-m-txt-c"" style=""padding:0;Margin:0""><h1 style=""Margin:0;line-height:55px;mso-line-height-rule:exactly;font-family:'Barlow Condensed', Arial, sans-serif;font-size:46px;font-style:normal;font-weight:normal;color:#E2CFEA"">Welcome to&nbsp;<strong>Catenaccio</strong> 🎉<strong></strong></h1></td>
                     </tr>
                     <tr>
                      <td align=""center"" class=""es-m-txt-c es-m-p10r es-m-p10l"" style=""Margin:0;padding-bottom:10px;padding-top:15px;padding-left:40px;padding-right:40px""><p style=""Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Barlow, sans-serif;line-height:24px;color:#E2CFEA;font-size:16px"">«Exclusive line of unique sports accessories»</p></td>
                     </tr>
                   </table></td>
                 </tr>
               </table></td>
             </tr>
             <tr>
              <td class=""es-m-p20r es-m-p20l"" align=""left"" style=""padding:0;Margin:0;padding-top:30px;padding-bottom:30px"">
               <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                 <tr>
                  <td align=""center"" valign=""top"" style=""padding:0;Margin:0;width:600px"">
                   <table cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:separate;border-spacing:0px;border-radius:10px"" role=""presentation"">
                     <tr>
                      <td align=""center"" style=""padding:0;Margin:0;padding-top:15px;padding-bottom:15px;font-size:0"">
                       <table border=""0"" width=""100%"" height=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                         <tr>
                          <td style=""padding:0;Margin:0;border-bottom:1px solid #ffffff;background:unset;height:1px;width:100%;margin:0px""></td>
                         </tr>
                       </table></td>
                     </tr>
                     <tr>
                      <td align=""left"" style=""padding:0;Margin:0;padding-top:20px;padding-bottom:20px""><h2 style=""Margin:0;line-height:34px;mso-line-height-rule:exactly;font-family:'Barlow Condensed', Arial, sans-serif;font-size:28px;font-style:normal;font-weight:normal;color:#ffffff"">Hi " + displayName + @" ,</h2><p style=""Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Barlow, sans-serif;line-height:24px;color:#E2CFEA;font-size:16px""><br></p><p style=""Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Barlow, sans-serif;line-height:24px;color:#E2CFEA;font-size:16px""><strong>Congratulations on taking the first step towards unlocking a world of premium sports gear and accessories. Your account has been successfully created. Confirm Your email to explor our exclusive accesories!</strong> 🌟🌟🌟<strong></strong></p><p style=""Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Barlow, sans-serif;line-height:24px;color:#E2CFEA;font-size:16px""><br></p></td>
                     </tr>
                     <tr>
                      <td align=""center"" style=""padding:0;Margin:0;padding-bottom:20px""><!--[if mso]><a href=" + confirmationLink + @" target="" _blank"" hidden>
	<v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" esdevVmlButton href=" + confirmationLink + @" style=""height:41px; v-text-anchor:middle; width:235px"" arcsize=""0%"" stroke=""f""  fillcolor=""#a06cd5"">
		<w:anchorlock></w:anchorlock>
		<center style='color:#ffffff; font-family:Barlow, sans-serif; font-size:15px; font-weight:400; line-height:15px;  mso-text-raise:1px'>Confirm Your email</center>
	</v:roundrect></a>
<![endif]--><!--[if !mso]><!-- --><span class=""es-button-border msohide"" style=""border-style:solid;border-color:#2CB543;background:#A06CD5;border-width:0px;display:inline-block;border-radius:0px;width:auto;mso-hide:all""><a href=" + confirmationLink + @" class=""es-button"" target=""_blank"" style=""mso-style-priority:100 !important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;color:#FFFFFF;font-size:18px;padding:10px 20px 10px 20px;display:inline-block;background:#A06CD5;border-radius:0px;font-family:Barlow, sans-serif;font-weight:normal;font-style:normal;line-height:22px;width:auto;text-align:center;mso-padding-alt:0;mso-border-alt:10px solid #A06CD5"">Confirm Email</a></span><!--<![endif]--></td>
                     </tr>
                   </table></td>
                 </tr>
               </table></td>
             </tr>
           </table></td>
         </tr>
       </table>
       <table cellpadding=""0"" cellspacing=""0"" class=""es-content"" align=""center"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%"">
         <tr>
          <td align=""center"" style=""padding:0;Margin:0"">
           <table bgcolor=""#102b3f"" class=""es-content-body"" align=""center"" cellpadding=""0"" cellspacing=""0"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#102B3F;width:600px"">
             <tr>
              <td class=""es-m-p20r es-m-p20l"" align=""left"" style=""padding:0;Margin:0;padding-top:30px;padding-bottom:30px"">
               <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                 <tr>
                  <td align=""center"" valign=""top"" style=""padding:0;Margin:0;width:600px"">
                   <table cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:separate;border-spacing:0px;border-radius:10px"" role=""presentation"">
                     <tr>
                      <td align=""center"" style=""padding:0;Margin:0;padding-top:15px;padding-bottom:15px;font-size:0"">
                       <table border=""0"" width=""100%"" height=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                         <tr>
                          <td style=""padding:0;Margin:0;border-bottom:1px solid #ffffff;background:unset;height:1px;width:100%;margin:0px""></td>
                         </tr>
                       </table></td>
                     </tr>
                     <tr>
                      <td align=""center"" class=""es-m-txt-c"" style=""padding:0;Margin:0;padding-top:20px;padding-bottom:20px""><h2 style=""Margin:0;line-height:34px;mso-line-height-rule:exactly;font-family:'Barlow Condensed', Arial, sans-serif;font-size:28px;font-style:normal;font-weight:normal;color:#ffffff"">Follow us!</h2></td>
                     </tr>
                     <tr>
                      <td align=""center"" class=""es-m-txt-c"" style=""padding:0;Margin:0;padding-top:10px;padding-bottom:20px;font-size:0"">
                       <table cellpadding=""0"" cellspacing=""0"" class=""es-table-not-adapt es-social"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                         <tr>
                          <td align=""center"" valign=""top"" style=""padding:0;Margin:0;padding-right:10px""><a target=""_blank"" href=""https://www.facebook.com/people/Catenaccio-%E1%83%99%E1%83%90%E1%83%A2%E1%83%94%E1%83%9C%E1%83%90%E1%83%A9%E1%83%9D/61552611089466/"" style=""-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:underline;color:#E2CFEA;font-size:16px""><img title=""Facebook"" src=""https://fecyrch.stripocdn.email/content/assets/img/social-icons/logo-colored/facebook-logo-colored.png"" alt=""Fb"" height=""24"" style=""display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic""></a></td>
                          <td align=""center"" valign=""top"" style=""padding:0;Margin:0;padding-right:10px""><a target=""_blank"" href=""https://www.instagram.com/catenaccio.geo/"" style=""-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:underline;color:#E2CFEA;font-size:16px""><img title=""Instagram"" src=""https://fecyrch.stripocdn.email/content/assets/img/social-icons/logo-colored/instagram-logo-colored.png"" alt=""Inst"" height=""24"" style=""display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic""></a></td>
                          <td align=""center"" valign=""top"" style=""padding:0;Margin:0""><a target=""_blank"" href=""https://www.youtube.com/@CatenaccioGeo"" style=""-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:underline;color:#E2CFEA;font-size:16px""><img title=""Youtube"" src=""https://fecyrch.stripocdn.email/content/assets/img/social-icons/logo-colored/youtube-logo-colored.png"" alt=""Yt"" height=""24"" style=""display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic""></a></td>
                         </tr>
                       </table></td>
                     </tr>
                     <tr>
                      <td align=""center"" style=""padding:0;Margin:0;padding-top:15px;padding-bottom:15px;font-size:0"">
                       <table border=""0"" width=""100%"" height=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                         <tr>
                          <td style=""padding:0;Margin:0;border-bottom:1px solid #ffffff;background:unset;height:1px;width:100%;margin:0px""></td>
                         </tr>
                       </table></td>
                     </tr>
                   </table></td>
                 </tr>
               </table></td>
             </tr>
           </table></td>
         </tr>
       </table>
       <table cellpadding=""0"" cellspacing=""0"" class=""es-footer"" align=""center"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top"">
         <tr>
          <td align=""center"" style=""padding:0;Margin:0"">
           <table bgcolor=""#f7fff7"" class=""es-footer-body"" align=""center"" cellpadding=""0"" cellspacing=""0"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#102B3F;width:600px"">
             <tr>
              <td align=""left"" style=""Margin:0;padding-left:20px;padding-right:20px;padding-top:40px;padding-bottom:40px"">
               <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                 <tr>
                  <td align=""center"" valign=""top"" style=""padding:0;Margin:0;width:560px"">
                   <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                     <tr>
                      <td align=""center"" style=""padding:0;Margin:0;display:none""></td>
                     </tr>
                   </table></td>
                 </tr>
               </table></td>
             </tr>
           </table></td>
         </tr>
       </table>
       <table cellpadding=""0"" cellspacing=""0"" class=""es-content"" align=""center"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%"">
         <tr>
          <td class=""es-info-area"" align=""center"" style=""padding:0;Margin:0"">
           <table class=""es-content-body"" align=""center"" cellpadding=""0"" cellspacing=""0"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:600px"" bgcolor=""#102b3f"" role=""none"">
             <tr>
              <td align=""left"" style=""padding:20px;Margin:0"">
               <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                 <tr>
                  <td align=""left"" style=""padding:0;Margin:0;width:560px"">
                   <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                     <tr>
                      <td align=""center"" style=""padding:0;Margin:0;display:none""></td>
                     </tr>
                   </table></td>
                 </tr>
               </table></td>
             </tr>
           </table></td>
         </tr>
       </table></td>
     </tr>
   </table>
  </div>
 </body>
</html>";

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(customHtml, null, "text/html");
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential("catenaccio.geo@gmail.com", googleAppPassword);
                mailMessage.AlternateViews.Add(htmlView);
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send notification to {email}: {ex.Message}");
            }
        }
        #endregion

    }
}
