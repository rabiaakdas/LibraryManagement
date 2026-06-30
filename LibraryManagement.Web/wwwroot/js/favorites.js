
        $(document).on("click", ".add-to-favorites", function () {
            var bookId = $(this).data("id"); // daima butondan al
            var button = $(this);

            console.log("Tıklanan kitap ID:", bookId); // kontrol için

            $.ajax({
                url: '/Books/AddToFavorites',
                type: 'POST',
                data: { id: bookId },
                success: function (res) {
                    if (res.success) {
                        button.find("i")
                              .removeClass("bi-heart")
                              .addClass("bi-heart-fill");
                        alert(res.message);
                    } else {
                        alert(res.message);
                    }
                },
                error: function () {
                    alert("Bir hata oluştu.");
                }
            });
        });
   