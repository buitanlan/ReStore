﻿using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(
        UserManager<User> userManager,
        TokenService tokenService,
        StoreContext context) : BaseApiController
{

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.FindByNameAsync(loginDto.Username);
        if (user == null || !await userManager.CheckPasswordAsync(user, loginDto.Password))
            return Unauthorized();

        var userBasket = await RetrieveBasket(loginDto.Username);
        var anonBasket = await RetrieveBasket(Request.Cookies["buyerId"]);

        if (anonBasket != null)
        {
            if (userBasket != null) context.Baskets.Remove(userBasket);
            anonBasket.BuyerId = user.UserName;
            Response.Cookies.Delete("buyerId");
            await context.SaveChangesAsync();
        }

        return new UserDto
        {
            Email = user.Email,
            Token = await tokenService.GenerateToken(user),
            Basket = anonBasket != null ? anonBasket.MapBasketToDto() : userBasket?.MapBasketToDto()
        };
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterUser(RegisterDto registerDto)
    {
        var user = new User { UserName = registerDto.Username, Email = registerDto.Email };

        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem();
        }

        await userManager.AddToRoleAsync(user, "Member");

        return StatusCode(201);
    }

    [Authorize]
    [HttpGet("currentUser")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var user = await userManager.FindByNameAsync(User.Identity.Name);

        var userBasket = await RetrieveBasket(User.Identity.Name);

        return new UserDto
        {
            Email = user.Email,
            Token = await tokenService.GenerateToken(user),
            Basket = userBasket?.MapBasketToDto()
        };
    }

    [Authorize]
    [HttpGet("savedAddress")]
    public async Task<ActionResult<UserAddress>> GetSavedAddress()
    {
        return await userManager.Users
            .Where(x => x.UserName == User.Identity.Name)
            .Select(user => user.Address)
            .FirstOrDefaultAsync();
    }

    private async Task<Basket> RetrieveBasket(string buyerId)
    {
        if (string.IsNullOrEmpty(buyerId))
        {
            Response.Cookies.Delete("buyerId");
            return null;
        }

        return await context.Baskets
            .Include(i => i.Items)
            .ThenInclude(p => p.Product)
            .FirstOrDefaultAsync(basket => basket.BuyerId == buyerId);
    }
}
