﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop.DAL;
using MyShop.Models;
using MyShop.ViewModels;

namespace MyShop.Controllers;

public class ItemController : Controller
{
    private readonly IItemRepository _itemRepository;
    private readonly ItemDbContext _itemDbContext;
    private readonly ILogger<ItemController> _logger;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly UserManager<CustomerUser> _userManager;


    public ItemController(IItemRepository itemRepository, ItemDbContext itemDbContext, ILogger<ItemController> logger, IWebHostEnvironment hostEnvironment, UserManager<CustomerUser> userManager)
    {
        _itemRepository = itemRepository;
        _itemDbContext = itemDbContext;
        _logger = logger;
        _hostEnvironment = hostEnvironment;
        _userManager = userManager;
    }
    public async Task<ActionResult> GetData()
    {
        var items = await _itemRepository.GetAll();
        if (items == null)
        {
            _logger.LogError("[ItemController] Item list not found while executing _itemRepository.GetAll()");
            return NotFound("Item list not found");
        }
        return Json(items);
    }

    public async Task<IActionResult> GetItem(int id)
    {
        var item = await _itemRepository.GetItemById(id);
        if (item == null)
        {
            _logger.LogError("[ItemController] Item not found for the ItemId {ItemId:0000}", id);
            return NotFound("Item not found for the ItemId");
        }
        return Json(item);
    }
    
    public async Task<IActionResult> Table()
    {
        var items = await _itemRepository.GetAll();
        if (items == null)
        {
            _logger.LogError("[ItemController] Item list not found while executing _itemRepository.GetAll()");
            return NotFound("Item list not found");
        }
        var itemListViewModel = new ItemListViewModel(items, "Table");
        return View(itemListViewModel);
    }

    public async Task<IActionResult> Grid()
    {
        var items = await _itemRepository.GetAll();
        if (items == null)
        {
            _logger.LogError("[ItemController] Item list not found while executing _itemRepository.GetAll()");
            return NotFound("Item list not found");
        }
        var itemListViewModel = new ItemListViewModel(items, "Grid");
        return View(itemListViewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _itemRepository.GetItemById(id);
        if (item == null)
        {
            _logger.LogError("[ItemController] Item not found for the ItemId {ItemId:0000}", id);
            return NotFound("Item not found for the ItemId");
        }
        return View(item);
    }
    
    /*
    [HttpGet]
    [Authorize]
    public IActionResult Create()
    {
        return Json(items);
    }
    */
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ItemCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            string userId;
            // CustomerUser user = await _userManager.GetUserAsync(User);
            var user = await _itemDbContext.CustomerUsers.FirstOrDefaultAsync(u => u.Email == Constants.DemoUserEmail);
            userId = user.Id;

            var imageUrl = await UploadImage(model.ImageUpload);
            var imageUrl2 = await UploadImage(model.ImageUpload2);
            var imageUrl3 = await UploadImage(model.ImageUpload3);

            if (imageUrl == null)
            {
                return BadRequest("Image upload failed");

            }
            var item = new Item
            {
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                Address = model.Address,
                Phone = model.Phone,
                Rooms = model.Rooms,
                Beds = model.Beds,
                Guests = model.Guests,
                Baths = model.Baths,
                ImageUrl = imageUrl,
                ImageUrl2 = imageUrl2,
                ImageUrl3 = imageUrl3,
                UserId = userId,
                CustomerUser = user,
            };

            bool returnOk = await _itemRepository.Create(item);
            if (returnOk)
            {
                return CreatedAtAction(nameof(GetItem), new { id = item.ItemId }, item);
            }
            else
            {
                return BadRequest("Failed to create item");
            }
        }
        else
        {
            return BadRequest(ModelState);
        }
    }

    [HttpPut("api/Item/Update/{itemId}")]
    public async Task<IActionResult> Update(int itemId, [FromForm] ItemUpdateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        string userId;
        // CustomerUser user = await _userManager.GetUserAsync(User);
        var user = await _itemDbContext.CustomerUsers.FirstOrDefaultAsync(u => u.Email == Constants.DemoUserEmail);
        userId = user.Id;

        var item = await _itemRepository.GetItemById(itemId);
        if (item == null)
        {
            _logger.LogError("[ItemController] Item not found when updating the ItemId {ItemId:0000}", itemId);
            return BadRequest("Item not found for the ItemId");
        }

        item.Name = model.Name;
        item.Price = model.Price;
        item.Description = model.Description;
        item.Address = model.Address;
        item.Phone = model.Phone;
        item.Rooms = model.Rooms;
        item.Beds = model.Beds;
        item.Guests = model.Guests;
        item.Baths = model.Baths;

        // Check and update images if needed
        if (model.ImageUpload != null && model.ImageUpload.Length > 0)
        {
            item.ImageUrl = await UploadImage(model.ImageUpload);
        }
        if (model.ImageUpload2 != null && model.ImageUpload2.Length > 0)
        {
            item.ImageUrl2 = await UploadImage(model.ImageUpload2);
        }
        if (model.ImageUpload3 != null && model.ImageUpload3.Length > 0)
        {
            item.ImageUrl3 = await UploadImage(model.ImageUpload3);
        }

        bool returnOk = await _itemRepository.Update(item);
        if (!returnOk)
        {
            return BadRequest("Failed to update item");
        }
        return Ok(item); // Return the updated item
    }


    [HttpDelete("api/Item/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        {
            var item = await _itemRepository.GetItemById(id);
            if (item == null)
            {
                _logger.LogError("[ItemController] Item not found for the ItemId {ItemId:0000}", id);
                return NotFound($"Item not found for the ItemId {id}");
            }

            await _itemRepository.Delete(id);
            return Ok($"Item with Id {id} deleted successfully");
        }

        /*[HttpPost]
     /*   public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool returnOk = await _itemRepository.Delete(id);
            if (!returnOk)
            {
                _logger.LogError("[ItemController] Item deletion failed for the ItemId {ItemId:0000}", id);
                return BadRequest("Item deletion failed");
            }
            return RedirectToAction(nameof(Table));
        }*/
    }
    private async Task<string> UploadImage(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0) return null;

        var filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", imageFile.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return "/images/" + imageFile.FileName;
    }

}
