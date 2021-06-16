using Library_API.Data;
using Library_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Library_API.Services
{
    public class UserService
    {
        public IConfiguration configuration { get; }
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signManager;

        public UserService(IConfiguration Configuration, SignInManager<User> signinManager, UserManager<User> userManager, ApplicationDbContext context)
        {
            configuration = Configuration;
            _userManager = userManager;
            _signManager = signinManager;
            db = context;
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
                        UserName = x.UserName
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
        }
    }

