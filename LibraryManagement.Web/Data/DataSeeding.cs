using LibraryManagement.Web.Data;
using LibraryManagement.Web.Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using BCrypt.Net;

namespace LibraryManagement.Web.Data
{
    public class DataSeeding
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BookContext>();

            context.Database.Migrate();

            // Genre'ları oluştur
            var genre1 = new Genre { Name = "Roman" };
            var genre2 = new Genre { Name = "Şiir" };
            var genre3 = new Genre { Name = "Biyografi" };
            var genre4 = new Genre { Name = "Polisiye" };
            var genre5 = new Genre { Name = "Bilim Kurgu" };

            var genres = new List<Genre> { genre1, genre2, genre3, genre4, genre5 };

            // Book'ları oluştur ve Genre ilişkilerini kur
            var books = new List<Book>
{
    // Roman
    new Book { Title = "Suç ve Ceza", Author = "Fyodor Dostoyevski", PageCount = 671, Price = 89.99m, ImageUrl = "roman1.jpg", Genres = new HashSet<Genre> { genre1 } ,Stock=50},
    new Book { Title = "Sefiller", Author = "Victor Hugo", PageCount = 1232, Price = 129.99m, ImageUrl = "roman2.jpg", Genres = new HashSet<Genre> { genre1 } ,Stock=50},
    new Book { Title = "Kırmızı ve Siyah", Author = "Stendhal", PageCount = 545, Price = 74.99m, ImageUrl = "roman3.jpg", Genres = new HashSet<Genre> { genre1 },Stock=50 },
    new Book { Title = "Madame Bovary", Author = "Gustave Flaubert", PageCount = 378, Price = 59.99m, ImageUrl = "roman4.jpg", Genres = new HashSet<Genre> { genre1 },Stock=50 },
    new Book { Title = "Yabancı", Author = "Albert Camus", PageCount = 185, Price = 49.99m, ImageUrl = "roman5.jpg", Genres = new HashSet<Genre> { genre1, genre2 } ,Stock=50},

    // Şiir
    new Book { Title = "Elem Çiçekleri", Author = "Charles Baudelaire", PageCount = 210, Price = 39.99m, ImageUrl = "siir1.jpg", Genres = new HashSet<Genre> { genre2 },Stock=50 },
    new Book { Title = "Göğe Bakma Durağı", Author = "Turgut Uyar", PageCount = 122, Price = 29.99m, ImageUrl = "siir2.jpg", Genres = new HashSet<Genre> { genre2 } ,Stock=50},
    new Book { Title = "Küllerimden", Author = "Edip Cansever", PageCount = 156, Price = 34.99m, ImageUrl = "siir3.jpg", Genres = new HashSet<Genre> { genre2 } ,Stock=50},
    new Book { Title = "Hayranlık", Author = "Nazım Hikmet", PageCount = 233, Price = 44.99m, ImageUrl = "siir4.jpg", Genres = new HashSet<Genre> { genre2 } ,Stock=50},
    new Book { Title = "Saf ve Düşünceli Romantik", Author = "Ahmet Hamdi Tanpınar", PageCount = 198, Price = 54.99m, ImageUrl = "siir5.jpg", Genres = new HashSet<Genre> { genre2, genre1 },Stock=50 },

    // Biyografi
    new Book { Title = "Benim Adım Tesla", Author = "Nigel Cawthorne", PageCount = 320, Price = 59.99m, ImageUrl = "biyografi1.jpg", Genres = new HashSet<Genre> { genre3 },Stock=50 },
    new Book { Title = "Steve Jobs", Author = "Walter Isaacson", PageCount = 656, Price = 99.99m, ImageUrl = "biyografi2.jpg", Genres = new HashSet<Genre> { genre3 },Stock=50 },
    new Book { Title = "Leonardo da Vinci", Author = "Walter Isaacson", PageCount = 624, Price = 109.99m, ImageUrl = "biyografi3.jpg", Genres = new HashSet<Genre> { genre3, genre1 },Stock=50 },
    new Book { Title = "Atatürk: Bir Milletin Yeniden Doğuşu", Author = "Lord Kinross", PageCount = 636, Price = 84.99m, ImageUrl = "biyografi4.jpg", Genres = new HashSet<Genre> { genre3 } ,Stock=50},
    new Book { Title = "Malcolm X Otobiyografisi", Author = "Malcolm X & Alex Haley", PageCount = 466, Price = 74.99m, ImageUrl = "biyografi5.jpg", Genres = new HashSet<Genre> { genre3 } ,Stock=50},

    // Polisiye
    new Book { Title = "Sherlock Holmes: Kızıl Dosya", Author = "Arthur Conan Doyle", PageCount = 221, Price = 39.99m, ImageUrl = "polisye1.jpg", Genres = new HashSet<Genre> { genre1, genre4 } ,Stock=50},
    new Book { Title = "Doğu Ekspresinde Cinayet", Author = "Agatha Christie", PageCount = 275, Price = 49.99m, ImageUrl = "polisye2.jpg", Genres = new HashSet<Genre> { genre4 },Stock=50 },
    new Book { Title = "On Küçük Zenci", Author = "Agatha Christie", PageCount = 280, Price = 54.99m, ImageUrl = "polisye3.jpg", Genres = new HashSet<Genre> { genre4 },Stock=50 },
    new Book { Title = "Ejderha Dövmeli Kız", Author = "Stieg Larsson", PageCount = 672, Price = 79.99m, ImageUrl = "polisye4.jpg", Genres = new HashSet<Genre> { genre4 },Stock=50 },
    new Book { Title = "Kızıl Nehirler", Author = "Jean-Christophe Grangé", PageCount = 440, Price = 69.99m, ImageUrl = "polisye5.jpg", Genres = new HashSet<Genre> { genre4 } ,Stock=50, },

    // Bilim Kurgu
    new Book { Title = "Dune", Author = "Frank Herbert", PageCount = 412, Price = 59.99m, ImageUrl = "bilim1.jpg", Genres = new HashSet<Genre> { genre1, genre5 } ,Stock=50},
    new Book { Title = "Vakıf", Author = "Isaac Asimov", PageCount = 296, Price = 49.99m, ImageUrl = "bilim2.jpg", Genres = new HashSet<Genre> { genre5 } ,Stock=50},
    new Book { Title = "Neuromancer", Author = "William Gibson", PageCount = 271, Price = 44.99m, ImageUrl = "bilim3.jpg", Genres = new HashSet<Genre> { genre5 },Stock=50 },
    new Book { Title = "Ben, Robot", Author = "Isaac Asimov", PageCount = 253, Price = 39.99m, ImageUrl = "bilim4.jpg", Genres = new HashSet<Genre> { genre5 } ,Stock=50},
    new Book { Title = "1984", Author = "George Orwell", PageCount = 328, Price = 54.99m, ImageUrl = "bilim5.jpg", Genres = new HashSet<Genre> { genre1, genre5 } ,Stock=50}
};

            // Users
            var users = new List<User>
            {
                new User
                {
                    Username = "usera",
                    Password = BCrypt.Net.BCrypt.HashPassword("12345"),
                    Email = "usera@gmail.com"
                },
                new User
                {
                    Username = "userb",
                    Password = BCrypt.Net.BCrypt.HashPassword("12345"),
                    Email = "userb@gmail.com"
                },
                new User
                {
                    Username = "userc",
                    Password = BCrypt.Net.BCrypt.HashPassword("12345"),
                    Email = "userc@gmail.com"
                }
            };

            // Seed işlemi
            if (!context.Genres.Any())
                context.Genres.AddRange(genres);

            if (!context.Books.Any())
                context.Books.AddRange(books);

            if (!context.Users.Any())
                context.Users.AddRange(users);

            context.SaveChanges();
        }
    }
}
