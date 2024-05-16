    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Pustok_MVC.Data;
    using Pustok_MVC.ViewModels;

    namespace Pustok_MVC.Controllers
    {
        public class ShopController : Controller
        {
            private readonly AppDbContext _context;

            public ShopController(AppDbContext context)
            {
                _context = context;
            }
            public IActionResult Index(int? genreId = null, List<int>? authorIds = null, double? minPrice = null, double? maxPrice = null)
            {
                ShopViewModel vm = new ShopViewModel
                {
                    Authors = _context.Authors.Include(x => x.Books).ToList(),
                    Genres = _context.Genres.Include(x => x.Books).ToList(),
                };

                var query = _context.Books
                                    .Include(x => x.BookImages.Where(bi => bi.PosterStatus != null))
                                    .Include(x => x.Author)
                                    .AsQueryable();

                if (genreId.HasValue)
                {
                    query = query.Where(x => x.GenreId == genreId.Value);
                }
                if (authorIds != null && authorIds.Count > 0)
                {
                    query = query.Where(x => authorIds.Contains(x.AuthorId));
                }
                if (minPrice.HasValue && maxPrice.HasValue)
                {
                    query = query.Where(x => x.SalePrice >= minPrice.Value && x.SalePrice <= maxPrice.Value);
                }

                vm.Books = query.ToList();

                ViewBag.GenreId = genreId;
                ViewBag.AuthorIds = authorIds;
                ViewBag.MinPrice = _context.Books.Where(x => !x.IsDeleted).Min(x => x.SalePrice);
                ViewBag.MaxPrice = _context.Books.Where(x => !x.IsDeleted).Max(x => x.SalePrice);
                ViewBag.SelectedMinPrice = minPrice ?? ViewBag.MinPrice;
                ViewBag.SelectedMaxPrice = maxPrice ?? ViewBag.MaxPrice;

                return View(vm);
            }


        }
    }
