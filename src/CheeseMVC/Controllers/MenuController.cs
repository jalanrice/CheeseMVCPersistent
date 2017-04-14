using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;
using CheeseMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {

        private CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            IList<Menu> menus = context.Menus.ToList();

            return View(menus);
        }

        public IActionResult Add()
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();

            return View(addMenuViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addMenuViewModel)
        {
            if (ModelState.IsValid)
            {
                // add the new menu to existing menus
                Menu newMenu = new Menu
                {
                    Name = addMenuViewModel.Name
                };

                context.Menus.Add(newMenu);
                context.SaveChanges();

                ViewBag.Title = addMenuViewModel.Name;

                return Redirect("/Menu/ViewMenu/" + newMenu.ID);
            }

            return View(addMenuViewModel);
        }

        public IActionResult ViewMenu(int id)
        {
            Menu menu = context.Menus.Single(m => m.ID == id);

            List<CheeseMenu> items = context
                .CheeseMenus
                .Include(item => item.Cheese)
                .Where(cm => cm.MenuID == id)
                .ToList();

            ViewMenuViewModel viewMenuViewModel = new ViewMenuViewModel
            {
                Menu = menu,
                Items = items
            };
            
            
            return View(viewMenuViewModel);
        }

        //public IActionResult AddItem(Menu menu, IEnumerable<Cheese> cheeses)
        //{
        //    AddMenuItemViewModel addMenuItemViewModel = new AddMenuItemViewModel(menu, cheeses);

        //    return View(addMenuItemViewModel);

        //}

        public IActionResult AddItem(int id)
        {
            Menu menu = context.Menus.Single(m => m.ID == id);

            IList<Cheese> cheeses = context.Cheeses.ToList();

            AddMenuItemViewModel addMenuItemViewModel = new AddMenuItemViewModel(menu, cheeses);
            

            return View(addMenuItemViewModel);
        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        {
            if(ModelState.IsValid)
            {
                var cheeseID = addMenuItemViewModel.cheeseID;
                var menuID = addMenuItemViewModel.menuID;
                
                IList<CheeseMenu> existingItems = context.CheeseMenus
                    .Where(cm => cm.CheeseID == addMenuItemViewModel.cheeseID)
                    .Where(cm => cm.MenuID == addMenuItemViewModel.menuID).ToList();
                if(existingItems.Count == 0)
                {
                    CheeseMenu newCheeseMenu = new CheeseMenu
                    {
                        Cheese = context.Cheeses.Single(c => c.ID == cheeseID),
                        Menu = context.Menus.Single(m => m.ID == menuID)
                    };

                    context.CheeseMenus.Add(newCheeseMenu);
                    context.SaveChanges();

                    
                }
                
            }

            return Redirect("/Menu/ViewMenu/" + addMenuItemViewModel.menuID);
        }

    }
}
