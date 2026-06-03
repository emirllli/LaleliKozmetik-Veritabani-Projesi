DROP DATABASE IF EXISTS laleli_kozmetik;
CREATE DATABASE laleli_kozmetik CHARACTER SET utf8mb4 COLLATE utf8mb4_turkish_ci;
USE laleli_kozmetik;

CREATE TABLE kategoriler (
    kategori_id INT AUTO_INCREMENT PRIMARY KEY,
    kategori_adi VARCHAR(80) NOT NULL UNIQUE,
    aciklama VARCHAR(255)
);

CREATE TABLE urunler (
    urun_id INT AUTO_INCREMENT PRIMARY KEY,
    kategori_id INT NOT NULL,
    urun_adi VARCHAR(120) NOT NULL,
    marka VARCHAR(80) NOT NULL,
    birim_fiyat DECIMAL(10,2) NOT NULL,
    stok_miktari INT NOT NULL DEFAULT 0,
    barkod VARCHAR(40) UNIQUE,
    CONSTRAINT fk_urun_kategori FOREIGN KEY (kategori_id)
        REFERENCES kategoriler(kategori_id),
    CONSTRAINT chk_urun_fiyat CHECK (birim_fiyat >= 0),
    CONSTRAINT chk_urun_stok CHECK (stok_miktari >= 0)
);

CREATE TABLE musteriler (
    musteri_id INT AUTO_INCREMENT PRIMARY KEY,
    ad VARCHAR(60) NOT NULL,
    soyad VARCHAR(60) NOT NULL,
    telefon VARCHAR(20),
    eposta VARCHAR(120),
    adres VARCHAR(255)
);

CREATE TABLE satislar (
    satis_id INT AUTO_INCREMENT PRIMARY KEY,
    urun_id INT NOT NULL,
    musteri_id INT NOT NULL,
    adet INT NOT NULL,
    birim_fiyat DECIMAL(10,2) NOT NULL,
    toplam_tutar DECIMAL(10,2) NOT NULL,
    satis_tarihi DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_satis_urun FOREIGN KEY (urun_id) REFERENCES urunler(urun_id),
    CONSTRAINT fk_satis_musteri FOREIGN KEY (musteri_id) REFERENCES musteriler(musteri_id),
    CONSTRAINT chk_satis_adet CHECK (adet > 0)
);

DELIMITER //

-- 1. Fonksiyon: KDV Hesaplama
CREATE FUNCTION fn_kdvli_fiyat(fiyat DECIMAL(10,2), kdv_orani DECIMAL(5,2))
RETURNS DECIMAL(10,2)
DETERMINISTIC
BEGIN
    RETURN ROUND(fiyat + (fiyat * kdv_orani / 100), 2);
END //

-- 2. Fonksiyon: Stok Durumu Kontrolü (YENİ - Hocanın istediği 2. fonksiyon)
CREATE FUNCTION fn_stok_durumu(miktar INT)
RETURNS VARCHAR(20)
DETERMINISTIC
BEGIN
    IF miktar <= 5 THEN
        RETURN 'KRİTİK';
    ELSEIF miktar <= 20 THEN
        RETURN 'DÜŞÜK';
    ELSE
        RETURN 'NORMAL';
    END IF;
END //

-- 1. Trigger: Satış yapılınca stok kontrol et ve stok düşür
CREATE TRIGGER trg_satis_stok_dus
BEFORE INSERT ON satislar
FOR EACH ROW
BEGIN
    DECLARE mevcut_stok INT;

    SELECT stok_miktari INTO mevcut_stok
    FROM urunler
    WHERE urun_id = NEW.urun_id;

    IF mevcut_stok IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Urun bulunamadi.';
    END IF;

    IF mevcut_stok < NEW.adet THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Yetersiz stok.';
    END IF;

    UPDATE urunler
    SET stok_miktari = stok_miktari - NEW.adet
    WHERE urun_id = NEW.urun_id;
END //

-- 2. Trigger: Satış iptal edilince stok iade et (YENİ - Hocanın istediği 2. trigger)
CREATE TRIGGER trg_satis_iptal_stok_iade
AFTER DELETE ON satislar
FOR EACH ROW
BEGIN
    UPDATE urunler
    SET stok_miktari = stok_miktari + OLD.adet
    WHERE urun_id = OLD.urun_id;
END //

DELIMITER //

CREATE PROCEDURE sp_kategori_ekle(IN p_kategori_adi VARCHAR(80), IN p_aciklama VARCHAR(255))
BEGIN
    INSERT INTO kategoriler(kategori_adi, aciklama) VALUES(p_kategori_adi, p_aciklama);
END //

CREATE PROCEDURE sp_kategori_guncelle(IN p_kategori_id INT, IN p_kategori_adi VARCHAR(80), IN p_aciklama VARCHAR(255))
BEGIN
    UPDATE kategoriler
    SET kategori_adi = p_kategori_adi, aciklama = p_aciklama
    WHERE kategori_id = p_kategori_id;
END //

CREATE PROCEDURE sp_kategori_sil(IN p_kategori_id INT)
BEGIN
    DELETE FROM kategoriler WHERE kategori_id = p_kategori_id;
END //

CREATE PROCEDURE sp_kategori_listele()
BEGIN
    SELECT * FROM kategoriler ORDER BY kategori_adi;
END //

CREATE PROCEDURE sp_urun_ekle(
    IN p_kategori_id INT,
    IN p_urun_adi VARCHAR(120),
    IN p_marka VARCHAR(80),
    IN p_birim_fiyat DECIMAL(10,2),
    IN p_stok_miktari INT,
    IN p_barkod VARCHAR(40)
)
BEGIN
    INSERT INTO urunler(kategori_id, urun_adi, marka, birim_fiyat, stok_miktari, barkod)
    VALUES(p_kategori_id, p_urun_adi, p_marka, p_birim_fiyat, p_stok_miktari, p_barkod);
END //

CREATE PROCEDURE sp_urun_guncelle(
    IN p_urun_id INT,
    IN p_kategori_id INT,
    IN p_urun_adi VARCHAR(120),
    IN p_marka VARCHAR(80),
    IN p_birim_fiyat DECIMAL(10,2),
    IN p_stok_miktari INT,
    IN p_barkod VARCHAR(40)
)
BEGIN
    UPDATE urunler
    SET kategori_id = p_kategori_id,
        urun_adi = p_urun_adi,
        marka = p_marka,
        birim_fiyat = p_birim_fiyat,
        stok_miktari = p_stok_miktari,
        barkod = p_barkod
    WHERE urun_id = p_urun_id;
END //

CREATE PROCEDURE sp_urun_sil(IN p_urun_id INT)
BEGIN
    DELETE FROM urunler WHERE urun_id = p_urun_id;
END //

CREATE PROCEDURE sp_urun_listele()
BEGIN
    SELECT u.urun_id, u.kategori_id, k.kategori_adi, u.urun_adi, u.marka,
           u.birim_fiyat, fn_kdvli_fiyat(u.birim_fiyat, 20) AS kdvli_fiyat,
           u.stok_miktari, fn_stok_durumu(u.stok_miktari) AS stok_durumu, u.barkod
    FROM urunler u
    INNER JOIN kategoriler k ON k.kategori_id = u.kategori_id
    ORDER BY u.urun_adi;
END //

CREATE PROCEDURE sp_musteri_ekle(
    IN p_ad VARCHAR(60),
    IN p_soyad VARCHAR(60),
    IN p_telefon VARCHAR(20),
    IN p_eposta VARCHAR(120),
    IN p_adres VARCHAR(255)
)
BEGIN
    INSERT INTO musteriler(ad, soyad, telefon, eposta, adres)
    VALUES(p_ad, p_soyad, p_telefon, p_eposta, p_adres);
END //

CREATE PROCEDURE sp_musteri_guncelle(
    IN p_musteri_id INT,
    IN p_ad VARCHAR(60),
    IN p_soyad VARCHAR(60),
    IN p_telefon VARCHAR(20),
    IN p_eposta VARCHAR(120),
    IN p_adres VARCHAR(255)
)
BEGIN
    UPDATE musteriler
    SET ad = p_ad, soyad = p_soyad, telefon = p_telefon, eposta = p_eposta, adres = p_adres
    WHERE musteri_id = p_musteri_id;
END //

CREATE PROCEDURE sp_musteri_sil(IN p_musteri_id INT)
BEGIN
    DELETE FROM musteriler WHERE musteri_id = p_musteri_id;
END //

CREATE PROCEDURE sp_musteri_listele()
BEGIN
    SELECT * FROM musteriler ORDER BY ad, soyad;
END //

CREATE PROCEDURE sp_satis_ekle(IN p_urun_id INT, IN p_musteri_id INT, IN p_adet INT)
BEGIN
    DECLARE fiyat DECIMAL(10,2);
    SELECT birim_fiyat INTO fiyat FROM urunler WHERE urun_id = p_urun_id;

    INSERT INTO satislar(urun_id, musteri_id, adet, birim_fiyat, toplam_tutar)
    VALUES(p_urun_id, p_musteri_id, p_adet, fiyat, fiyat * p_adet);
END //

CREATE PROCEDURE sp_satis_sil(IN p_satis_id INT)
BEGIN
    DELETE FROM satislar WHERE satis_id = p_satis_id;
END //

CREATE PROCEDURE sp_satis_guncelle(IN p_satis_id INT, IN p_urun_id INT, IN p_musteri_id INT, IN p_adet INT)
BEGIN
    DECLARE eski_urun_id INT;
    DECLARE eski_adet INT;
    DECLARE fiyat DECIMAL(10,2);
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    SELECT urun_id, adet INTO eski_urun_id, eski_adet
    FROM satislar
    WHERE satis_id = p_satis_id;

    UPDATE urunler
    SET stok_miktari = stok_miktari + eski_adet
    WHERE urun_id = eski_urun_id;

    SELECT birim_fiyat INTO fiyat
    FROM urunler
    WHERE urun_id = p_urun_id;

    IF (SELECT stok_miktari FROM urunler WHERE urun_id = p_urun_id) < p_adet THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Yetersiz stok.';
    END IF;

    UPDATE satislar
    SET urun_id = p_urun_id,
        musteri_id = p_musteri_id,
        adet = p_adet,
        birim_fiyat = fiyat,
        toplam_tutar = fiyat * p_adet
    WHERE satis_id = p_satis_id;

    UPDATE urunler
    SET stok_miktari = stok_miktari - p_adet
    WHERE urun_id = p_urun_id;

    COMMIT;
END //

CREATE PROCEDURE sp_satis_listele()
BEGIN
    SELECT s.satis_id, s.satis_tarihi, m.musteri_id,
           CONCAT(m.ad, ' ', m.soyad) AS musteri,
           u.urun_id, u.urun_adi, s.adet, s.birim_fiyat, s.toplam_tutar
    FROM satislar s
    INNER JOIN musteriler m ON m.musteri_id = s.musteri_id
    INNER JOIN urunler u ON u.urun_id = s.urun_id
    ORDER BY s.satis_tarihi DESC;
END //

CREATE PROCEDURE sp_musteri_satis_raporu(IN p_musteri_id INT)
BEGIN
    SELECT m.musteri_id, CONCAT(m.ad, ' ', m.soyad) AS musteri,
           COUNT(s.satis_id) AS satis_sayisi,
           COALESCE(SUM(s.toplam_tutar), 0) AS toplam_harcama
    FROM musteriler m
    LEFT JOIN satislar s ON s.musteri_id = m.musteri_id
    WHERE m.musteri_id = p_musteri_id
    GROUP BY m.musteri_id, m.ad, m.soyad;
END //

DELIMITER ;

INSERT INTO kategoriler(kategori_adi, aciklama) VALUES
('Makyaj', 'Ruj, fondoten, maskara ve renkli kozmetik urunleri'),
('Cilt Bakimi', 'Nemlendirici, serum ve temizleyici urunler'),
('Parfum', 'Kadin ve erkek parfumleri'),
('Kisisel Bakim', 'Sampuan, deodorant ve bakim urunleri');

INSERT INTO urunler(kategori_id, urun_adi, marka, birim_fiyat, stok_miktari, barkod) VALUES
(1, 'Mat Ruj', 'Laleli Beauty', 189.90, 40, '869000000001'),
(2, 'C Vitamini Serum', 'GlowCare', 349.90, 25, '869000000002'),
(3, 'Ciceksi Parfum 50 ml', 'Laleli Fragrance', 599.90, 18, '869000000003'),
(4, 'Besleyici Sampuan', 'CarePlus', 129.90, 60, '869000000004');

INSERT INTO musteriler(ad, soyad, telefon, eposta, adres) VALUES
('Ayse', 'Yilmaz', '05551234567', 'ayse@example.com', 'Bartin Merkez'),
('Zeynep', 'Demir', '05557654321', 'zeynep@example.com', 'Amasra');
