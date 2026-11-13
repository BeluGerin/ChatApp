1. Jalankan backend dan frontend.
2. Buka `http://localhost:4200` di dua tab atau dua browser (misalnya Chrome dan Edge).
3. Masukkan nama berbeda di masing-masing tab.
4. Kirim pesan dari satu tab dan pastikan muncul di kedua tab secara instan.
5. Tutup salah satu tab dan pastikan daftar "Online" diperbarui dan muncul pesan "meninggalkan obrolan".
6. Jalankan perintah `curl` pada Langkah 4 dan pastikan pengumuman muncul di semua tab.
7. Hentikan backend sebentar (Ctrl+C), lalu jalankan kembali. Status di pojok kanan atas akan berubah menjadi `reconnecting` lalu `connected`. Ini adalah demonstrasi fitur **reconnect otomatis** (slide 13).