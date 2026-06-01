# Laleli Kozmetik ve Kisisel Bakim Magazasi

BTS304 Veritabani Yonetim Sistemleri II final odevi icin hazirlanan MySQL + C# Windows Forms projesidir.

## Kurulum

1. MySQL uzerinde `sql/laleli_kozmetik_database.sql` scriptini calistirin.
2. `LaleliKozmetik.DAL/Database.cs` icindeki connection string bilgisini kendi MySQL kullaniciniza gore duzenleyin.
3. Visual Studio ile `LaleliKozmetik.sln` dosyasini acin.
4. `LaleliKozmetik.UI` projesini baslangic projesi yapip calistirin.
5. Kategoriler, Urun Yonetimi, Musteri Kaydi ve Satis Ekrani sekmelerinde ekle, sil, guncelle ve listele islemlerini deneyin.

## Katmanlar

- `LaleliKozmetik.DAL`: Veritabani baglantisi ve stored procedure cagrilari.
- `LaleliKozmetik.BLL`: Alan dogrulama, stok ve satis is kurallari.
- `LaleliKozmetik.UI`: Kategoriler, Urun Yonetimi, Musteri Kaydi ve Satis Ekrani.

## Teslim Dosyalari

- SQL script: `sql/laleli_kozmetik_database.sql`
- ER diyagrami: `diagrams/er_diagram.mmd` ve `diagrams/er_diagram.png`
- Rapor: `docs/Laleli_Kozmetik_Final_Rapor.docx`
