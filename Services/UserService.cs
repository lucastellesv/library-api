using Library_API.Data;
using Library_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Library_API.Models.EmailModels;

namespace Library_API.Services
{
    public class UserService
    {
        public IConfiguration configuration { get; }
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signManager;
        private readonly EmailService _emailService;
        private readonly IViewRenderService _viewRenderService;

        public UserService(IConfiguration Configuration, SignInManager<User> signinManager, UserManager<User> userManager, ApplicationDbContext context, EmailService emailService, IViewRenderService viewRenderService)
        {
            configuration = Configuration;
            _userManager = userManager;
            _signManager = signinManager;
            db = context;
            _emailService = emailService;
            _viewRenderService = viewRenderService;
        }
        public SecurityToken GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration.GetSection("TokenAuthentication")["SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Authentication, user.Id)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = configuration.GetSection("TokenAuthentication")["Issuer"],
                Audience = configuration.GetSection("TokenAuthentication")["Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return token;
        }
        public List<User> ListUsers()
        {
            return db.Users.ToList();
        }

        public async Task<GetTokenModel> GetToken(User usr)
        {
            User findUser = db.Users.Where(x => x.UserName == usr.UserName).FirstOrDefault();

            if (findUser != null)
            {
                var result = await _signManager.PasswordSignInAsync(findUser, usr.Password, false, lockoutOnFailure: true);
                if (result.Succeeded)
                {

                    User user = db.Users.Where(x => x.UserName == findUser.UserName).FirstOrDefault();
                    findUser = user;

                    var token = GenerateToken(findUser);
                    User retUser = null;


                    retUser = db.Users.Where(x => x.Id == findUser.Id)
                    .Select(x => new User()
                    {
                        Id = x.Id,
                        UserName = x.UserName,
                        Email = x.Email
                    })
                    .FirstOrDefault();

                    return new GetTokenModel(
                        new JwtSecurityTokenHandler().WriteToken(token),
                        retUser,
                        token.ValidTo,
                        200
                    );
                }
                else
                {
                    return new GetTokenModel("Usuario ou Senha incorretos", 401);
                }
            }
            else
            {
                return new GetTokenModel("Usuario ou Senha incorretos", 401);
            }
        }

            public async Task<RegisterModel> Register(User usr)
            {
                try
                {
                    User findUser = db.Users.Where(x => x.Email == usr.Email).FirstOrDefault();


                    if (findUser == null)
                    {
                        if (usr.UserName == null)
                        {
                            usr.UserName = usr.UserName;
                            usr.Email = usr.UserName;
                    }
                       
                        var result = await _userManager.CreateAsync(usr, usr.Password);

                        if (result.Succeeded)
                        {
                            return new RegisterModel("Sucesso", 200);
                        }
                        else
                        {
                            return new RegisterModel("Erro ao acessar o banco de dados", 422);
                        }
                    }
                    else
                    {
                        return new RegisterModel("Usuário já existe", 409);
                    }
                }
                catch (Exception e)
                {
                    db.Remove(db.Users.Where(x => x.UserName == usr.UserName).FirstOrDefault());
                    await db.SaveChangesAsync();
                    throw e;
                }
            }
        public async Task<ForgotPasswordModel> ForgotPassword(User user)
        {
            try
            {
                user = await db.Users.Where(x => x.Email == user.Email).FirstOrDefaultAsync();

                if (user != null)
                {
                    Random randomNumber = new Random();
                    var code = randomNumber.Next(1000, 9999);


                    var viewResult = _viewRenderService.RenderToStringAsync("Email/EmailTemplate", new EmailModel(
                    user.UserName,
                    user.Email,
                    "Código de Alteração da Senha LibNow",
                    "Seu código para a alteração de senha é " + code
                    )).Result;

                    _emailService.SendEmailAsync(user.Email, "Código de Alteração da Senha LibNow", viewResult);

                    ResetCode resetCode = new ResetCode() { UserId = user.Id, Code = code };

                    db.Add(resetCode);
                    db.SaveChanges();

                    return new ForgotPasswordModel("E-mail enviado", 200, user.Id);
                }
                else
                {
                    return new ForgotPasswordModel("E-mail não cadastrado no banco de dados", 402, " ");
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ResponseModel> ValidateResetCode(ResetCode code)
        {
            ResetCode foundCode = await db.ResetCodes.AsNoTracking().Where(x => x.Code == code.Code && x.UserId == code.UserId).FirstOrDefaultAsync();
            if (foundCode != null)
            {
                return new ResponseModel("Código validado", 200);
            }
            else
            {
                return new ResponseModel("Código Inválido", 402);
            }
        }

        public async Task<ResponseModel> ResetPasswordByCode(ResetPasswordModel model)
        {
            User user = db.Users.Where(x => x.Id == model.UserId).FirstOrDefault();

            try
            {
                ResetCode resetCode = db.ResetCodes.Where(x => x.UserId == model.UserId && x.Code == model.Code).ToList().LastOrDefault();

                if (resetCode == null)
                {
                    return new ResponseModel("Código Inválido", 422);
                }

                if (user == null)
                {
                    return new ResponseModel("Usuario não existe", 409);
                }

                var newPassword = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);
                user.PasswordHash = newPassword;
                var res = await _userManager.UpdateAsync(user);
                if (res.Succeeded)
                {
                    db.Remove(resetCode);
                    db.SaveChanges();

                    return new ResponseModel("Senha atualizada", 200);
                }
                else
                {
                    return new ResponseModel("Erro ao acessar o banco de dados", 422);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ResponseModel> UpdatePassword(UpdatePasswordModel usr)
        {
            try
            {
                User findUser = db.Users.Where(x => x.Id == usr.Id).FirstOrDefault();


                if (findUser != null)
                {
                    var res = await _signManager.PasswordSignInAsync(findUser, usr.CurrentPassword, false, lockoutOnFailure: true);
                    if (res.Succeeded)
                    {
                        var newPassword = _userManager.PasswordHasher.HashPassword(findUser, usr.NewPassword);
                        findUser.PasswordHash = newPassword;
                        var result = await _userManager.UpdateAsync(findUser);

                        if (result.Succeeded)
                        {
                            return new ResponseModel("", 200);
                        }
                        else
                        {
                            return new ResponseModel("Erro ao acessar o banco de dados", 422);
                        }
                    }
                    else
                    {
                        return new ResponseModel("Senha incorreta", 402);
                    }
                }
                else
                {
                    return new ResponseModel("Houve um problema", 409);
                }
            }
            catch (Exception e)
            {
                db.Remove(db.Users.Where(x => x.Id == usr.Id).FirstOrDefault());
                await db.SaveChangesAsync();
                throw e;
            }
        }

        
        }
    }


