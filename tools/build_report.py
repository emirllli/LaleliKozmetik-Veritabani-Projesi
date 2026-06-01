from pathlib import Path

from docx import Document
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT, WD_CELL_VERTICAL_ALIGNMENT
from docx.shared import Inches, Pt, RGBColor
from docx.oxml import OxmlElement
from docx.oxml.ns import qn
from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
DOCS = ROOT / "docs"
DIAGRAMS = ROOT / "diagrams"
OUTPUT = DOCS / "Laleli_Kozmetik_Final_Rapor.docx"
ER_IMAGE = DIAGRAMS / "er_diagram.png"
PRODUCT_SCREEN = DIAGRAMS / "urun_yonetimi_ekrani.png"
CUSTOMER_SCREEN = DIAGRAMS / "musteri_kaydi_ekrani.png"
SALE_SCREEN = DIAGRAMS / "satis_ekrani.png"
CATEGORY_SCREEN = DIAGRAMS / "kategori_yonetimi_ekrani.png"


def draw_er_diagram():
    DIAGRAMS.mkdir(exist_ok=True)
    img = Image.new("RGB", (1400, 780), "white")
    draw = ImageDraw.Draw(img)

    try:
        title_font = ImageFont.truetype("arial.ttf", 32)
        box_font = ImageFont.truetype("arial.ttf", 24)
        text_font = ImageFont.truetype("arial.ttf", 20)
    except OSError:
        title_font = box_font = text_font = ImageFont.load_default()

    draw.text((360, 35), "Laleli Kozmetik ER Diyagrami", fill=(31, 78, 121), font=title_font)

    boxes = {
        "KATEGORILER": (80, 160, 380, 390, ["kategori_id PK", "kategori_adi", "aciklama"]),
        "URUNLER": (555, 155, 865, 430, ["urun_id PK", "kategori_id FK", "urun_adi", "marka", "birim_fiyat", "stok_miktari", "barkod"]),
        "MUSTERILER": (80, 505, 380, 705, ["musteri_id PK", "ad", "soyad", "telefon", "eposta", "adres"]),
        "SATISLAR": (1020, 275, 1320, 565, ["satis_id PK", "urun_id FK", "musteri_id FK", "adet", "birim_fiyat", "toplam_tutar", "satis_tarihi"]),
    }

    for name, (x1, y1, x2, y2, attrs) in boxes.items():
        draw.rounded_rectangle((x1, y1, x2, y2), radius=14, fill=(242, 247, 251), outline=(46, 116, 181), width=3)
        draw.rectangle((x1, y1, x2, y1 + 46), fill=(46, 116, 181))
        draw.text((x1 + 18, y1 + 10), name, fill="white", font=box_font)
        y = y1 + 65
        for attr in attrs:
            draw.text((x1 + 22, y), attr, fill=(30, 30, 30), font=text_font)
            y += 30

    def line(start, end, label):
        draw.line((start, end), fill=(35, 35, 35), width=3)
        lx = (start[0] + end[0]) // 2 - 50
        ly = (start[1] + end[1]) // 2 - 28
        draw.rectangle((lx - 8, ly - 4, lx + 148, ly + 28), fill="white")
        draw.text((lx, ly), label, fill=(30, 30, 30), font=text_font)

    line((380, 275), (555, 275), "1 : N")
    line((865, 292), (1020, 392), "1 : N")
    line((380, 610), (1020, 475), "1 : N")

    img.save(ER_IMAGE)


def draw_screen_mockup(path, title, fields, columns):
    img = Image.new("RGB", (1400, 780), (248, 250, 252))
    draw = ImageDraw.Draw(img)

    try:
        title_font = ImageFont.truetype("arial.ttf", 30)
        label_font = ImageFont.truetype("arial.ttf", 22)
        text_font = ImageFont.truetype("arial.ttf", 18)
    except OSError:
        title_font = label_font = text_font = ImageFont.load_default()

    draw.rectangle((0, 0, 1400, 70), fill=(31, 78, 121))
    draw.text((36, 18), title, fill="white", font=title_font)

    y = 105
    x = 50
    for label in fields:
        draw.text((x, y), label, fill=(35, 35, 35), font=label_font)
        draw.rounded_rectangle((x + 120, y - 8, x + 420, y + 36), radius=6, fill="white", outline=(160, 174, 192), width=2)
        x += 455
        if x > 1000:
            x = 50
            y += 70

    button_y = y + 70
    if "Satis" in title:
        captions = ["Satis Yap", "Satis Duzenle", "Satis Sil", "Yenile"]
    else:
        captions = ["Ekle", "Guncelle", "Sil", "Listele"]

    for i, caption in enumerate(captions):
        bx = 50 + i * 155
        draw.rounded_rectangle((bx, button_y, bx + 135, button_y + 46), radius=6, fill=(46, 116, 181), outline=(46, 116, 181))
        draw.text((bx + 22, button_y + 12), caption, fill="white", font=text_font)

    table_y = button_y + 85
    draw.rectangle((50, table_y, 1350, 720), fill="white", outline=(160, 174, 192), width=2)
    draw.rectangle((50, table_y, 1350, table_y + 48), fill=(217, 234, 247), outline=(160, 174, 192), width=2)
    col_width = 1300 // len(columns)
    for i, column in enumerate(columns):
        cx = 60 + i * col_width
        draw.text((cx, table_y + 13), column, fill=(20, 45, 70), font=text_font)
        draw.line((50 + (i + 1) * col_width, table_y, 50 + (i + 1) * col_width, 720), fill=(220, 226, 235), width=1)
    for row in range(1, 6):
        ry = table_y + 48 + row * 48
        draw.line((50, ry, 1350, ry), fill=(230, 235, 242), width=1)

    img.save(path)


def draw_screen_images():
    draw_screen_mockup(
        CATEGORY_SCREEN,
        "Kategori Yonetimi Ekrani",
        ["Kategori Adi", "Aciklama"],
        ["Kategori ID", "Kategori Adi", "Aciklama"],
    )
    draw_screen_mockup(
        PRODUCT_SCREEN,
        "Urun Yonetimi Ekrani",
        ["Kategori", "Urun Adi", "Marka", "Fiyat", "Stok", "Barkod"],
        ["Urun ID", "Kategori", "Urun", "Marka", "Fiyat", "KDV'li", "Stok"],
    )
    draw_screen_mockup(
        CUSTOMER_SCREEN,
        "Musteri Kaydi Ekrani",
        ["Ad", "Soyad", "Telefon", "E-posta", "Adres"],
        ["Musteri ID", "Ad", "Soyad", "Telefon", "E-posta", "Adres"],
    )
    draw_screen_mockup(
        SALE_SCREEN,
        "Satis Ekrani",
        ["Urun", "Musteri", "Adet"],
        ["Satis ID", "Tarih", "Musteri", "Urun", "Adet", "Birim Fiyat", "Toplam"],
    )


def set_cell_shading(cell, fill):
    tc_pr = cell._tc.get_or_add_tcPr()
    shd = OxmlElement("w:shd")
    shd.set(qn("w:fill"), fill)
    tc_pr.append(shd)


def add_heading(doc, text, level=1):
    p = doc.add_heading(text, level=level)
    for run in p.runs:
        run.font.name = "Calibri"
        run.font.color.rgb = RGBColor(46, 116, 181 if level == 1 else 120)
    return p


def add_bullets(doc, items):
    for item in items:
        doc.add_paragraph(item, style="List Bullet")


def add_code(doc, title, code):
    add_heading(doc, title, 2)
    for line in code.strip().splitlines():
        p = doc.add_paragraph()
        p.paragraph_format.space_after = Pt(0)
        run = p.add_run(line)
        run.font.name = "Consolas"
        run.font.size = Pt(8.5)


def add_table(doc, headers, rows):
    table = doc.add_table(rows=1, cols=len(headers))
    table.alignment = WD_TABLE_ALIGNMENT.CENTER
    table.style = "Table Grid"
    hdr = table.rows[0].cells
    for index, header in enumerate(headers):
        hdr[index].text = header
        set_cell_shading(hdr[index], "D9EAF7")
        hdr[index].vertical_alignment = WD_CELL_VERTICAL_ALIGNMENT.CENTER
    for row in rows:
        cells = table.add_row().cells
        for index, value in enumerate(row):
            cells[index].text = value
            cells[index].vertical_alignment = WD_CELL_VERTICAL_ALIGNMENT.CENTER
    return table


def main():
    DOCS.mkdir(exist_ok=True)
    draw_er_diagram()
    draw_screen_images()

    doc = Document()
    section = doc.sections[0]
    section.top_margin = Inches(1)
    section.bottom_margin = Inches(1)
    section.left_margin = Inches(1)
    section.right_margin = Inches(1)

    styles = doc.styles
    styles["Normal"].font.name = "Calibri"
    styles["Normal"].font.size = Pt(11)

    title = doc.add_paragraph()
    title.alignment = WD_ALIGN_PARAGRAPH.CENTER
    run = title.add_run("Laleli Kozmetik ve Kisisel Bakim Magazasi\nVeritabani Yonetim Sistemleri II Final Odevi")
    run.bold = True
    run.font.size = Pt(18)
    run.font.color.rgb = RGBColor(31, 78, 121)

    info = doc.add_paragraph()
    info.alignment = WD_ALIGN_PARAGRAPH.CENTER
    info.add_run("Emirhan Laleli\n23010708033\nBTS304 - 2026").bold = True
    doc.add_page_break()

    add_heading(doc, "ADIM-1: Senaryo ve Problem Tanimi")
    doc.add_paragraph(
        "Bu proje, makyaj, cilt bakimi, parfum ve kisisel bakim urunleri satan Laleli Kozmetik ve "
        "Kisisel Bakim Magazasi icin hazirlanmistir. Magazada urun giris-cikislari, musteri kayitlari "
        "ve satis hareketleri tek bir veritabani uzerinden takip edilir."
    )
    add_bullets(
        doc,
        [
            "Problem: Stok takibi defter veya daginik dosyalarla yapildigi icin guncel stok miktari hizli gorulememektedir.",
            "Problem: Musterilere yapilan satislar raporlanamadigi icin musteri bazli toplam harcama izlenememektedir.",
            "Hedef: Satis yapildiginda urun stok miktarini otomatik azaltan ve satis raporu sunan bir sistem gelistirmek.",
            "Kisit: Kayitli olmayan musteriye veya kayitli olmayan urune satis yapilmaz.",
            "Kisit: Stok miktari yetersizse satis islemi BLL ve trigger tarafinda engellenir.",
        ],
    )

    add_heading(doc, "ADIM-2: Veritabani Tasarimi")
    doc.add_picture(str(ER_IMAGE), width=Inches(6.5))
    doc.paragraphs[-1].alignment = WD_ALIGN_PARAGRAPH.CENTER
    add_table(
        doc,
        ["Varlik", "Nitelikler"],
        [
            ["Kategoriler", "kategori_id PK, kategori_adi, aciklama"],
            ["Urunler", "urun_id PK, kategori_id FK, urun_adi, marka, birim_fiyat, stok_miktari, barkod"],
            ["Musteriler", "musteri_id PK, ad, soyad, telefon, eposta, adres"],
            ["Satislar", "satis_id PK, urun_id FK, musteri_id FK, adet, birim_fiyat, toplam_tutar, satis_tarihi"],
        ],
    )
    doc.add_paragraph()
    add_table(
        doc,
        ["Iliski", "Kardinalite", "Aciklama"],
        [
            ["Kategoriler - Urunler", "1:N", "Bir kategoride birden fazla urun bulunabilir."],
            ["Urunler - Satislar", "1:N", "Bir urun farkli satislarda tekrar satilabilir."],
            ["Musteriler - Satislar", "1:N", "Bir musterinin birden fazla satis kaydi olabilir."],
        ],
    )

    sql_text = (ROOT / "sql" / "laleli_kozmetik_database.sql").read_text(encoding="utf-8")
    add_heading(doc, "ADIM-3: Veritabani Programlama")
    doc.add_paragraph(
        "Veritabani MySQL uzerinde hazirlanmistir. CRUD islemleri stored procedure ile yapilir. "
        "Satis eklenmeden once trigger stok miktarini kontrol eder ve yeterli stok varsa urun stok miktarini dusurur. "
        "KDV hesaplamasi icin fonksiyon kullanilmistir."
    )
    add_code(doc, "Trigger: Satis Yapilinca Stok Dusurme", sql_text[sql_text.index("CREATE TRIGGER"):sql_text.index("CREATE PROCEDURE sp_kategori_ekle")])
    add_code(doc, "Function: KDV'li Fiyat", sql_text[sql_text.index("CREATE FUNCTION"):sql_text.index("CREATE TRIGGER")])
    add_code(doc, "Ornek Stored Procedure: Urun Ekleme", sql_text[sql_text.index("CREATE PROCEDURE sp_urun_ekle"):sql_text.index("CREATE PROCEDURE sp_urun_guncelle")])

    add_heading(doc, "ADIM-4: Uygulama Gelistirme")
    doc.add_paragraph(
        "C# uygulamasi N-katmanli mimari ile hazirlanmistir. Data Access Layer yalnizca MySQL baglantisi ve "
        "stored procedure cagrilarini icerir. Business Logic Layer alan dogrulama ve stok kontrolu yapar. "
        "Presentation Layer Windows Forms ekranlarini icerir."
    )
    add_table(
        doc,
        ["Katman", "Proje/Klasor", "Gorev"],
        [
            ["DAL", "LaleliKozmetik.DAL", "MySqlConnection, MySqlCommand ve SP cagrilari"],
            ["BLL", "LaleliKozmetik.BLL", "Urun, musteri ve satis is kurallari"],
            ["UI", "LaleliKozmetik.UI", "Kategoriler, Urun Yonetimi, Musteri Kaydi ve Satis Ekrani"],
        ],
    )

    add_code(doc, "BLL Stok Kontrolu", (ROOT / "LaleliKozmetik.BLL" / "SaleService.cs").read_text(encoding="utf-8"))
    add_code(doc, "DAL Satis SP Cagrisi", (ROOT / "LaleliKozmetik.DAL" / "SaleRepository.cs").read_text(encoding="utf-8"))

    add_heading(doc, "Ekranlar")
    add_bullets(
        doc,
        [
            "Kategoriler: Kategori ekleme, silme, guncelleme ve listeleme islemleri yapilir.",
            "Urun Yonetimi: Urun ekleme, silme, guncelleme ve listeleme islemleri yapilir.",
            "Musteri Kaydi: Musteri ekleme, silme, guncelleme ve listeleme islemleri yapilir.",
            "Satis Ekrani: Satis ekleme, silme, guncelleme ve listeleme islemleri yapilir; satis sonrasi stok miktari otomatik duser.",
        ],
    )
    for image, caption in [
        (CATEGORY_SCREEN, "Kategori Yonetimi Ekrani"),
        (PRODUCT_SCREEN, "Urun Yonetimi Ekrani"),
        (CUSTOMER_SCREEN, "Musteri Kaydi Ekrani"),
        (SALE_SCREEN, "Satis Ekrani"),
    ]:
        doc.add_paragraph(caption).runs[0].bold = True
        doc.add_picture(str(image), width=Inches(6.5))
        doc.paragraphs[-1].alignment = WD_ALIGN_PARAGRAPH.CENTER

    add_heading(doc, "ADIM-5: Teslimat ve Video Akisi")
    add_bullets(
        doc,
        [
            "SQL scripti: sql/laleli_kozmetik_database.sql dosyasi MySQL uzerinde calistirilir.",
            "C# proje: LaleliKozmetik.sln Visual Studio ile acilir ve calistirilir.",
            "Video 1: MySQL tarafinda trigger ve function gosterilir.",
            "Video 2: Uygulamada kategori, urun, musteri ve satis CRUD islemleri gosterilir.",
            "Video 3: Satis yapildiktan sonra stok miktarinin dustugu, satis silinince stok iadesi oldugu gosterilir.",
            "GitHub: Tum proje, SQL scripti, ER diyagrami ve rapor yuklenir.",
        ],
    )

    doc.save(OUTPUT)
    print(OUTPUT)


if __name__ == "__main__":
    main()
