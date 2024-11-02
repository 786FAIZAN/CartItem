using CartItem.Models;
using CartItem.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartItem.Controllers
{
    public class ItemController : Controller
    {
        private readonly ItemRepository itemRepository;

        public ItemController(ItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        public IActionResult Index()
        {
            var items = itemRepository.GetAllItems();
            return View(items);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Item item)
        {
            if (ModelState.IsValid)
            {
                item.UpdatedDate = DateTime.Now;
                itemRepository.AddItem(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public IActionResult Edit(int id)
        {
            var item = itemRepository.GetItemById(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(Item item)
        {
            if (ModelState.IsValid)
            {
                item.UpdatedDate = DateTime.Now;
                itemRepository.UpdateItem(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public IActionResult Delete(int id)
        {
            var item = itemRepository.GetItemById(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            itemRepository.DeleteItem(id);
            return RedirectToAction("Index");
        }
    }
}
