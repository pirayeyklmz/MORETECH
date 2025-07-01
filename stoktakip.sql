-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Anamakine: 127.0.0.1
-- Üretim Zamanı: 24 Haz 2025, 18:49:57
-- Sunucu sürümü: 10.4.32-MariaDB
-- PHP Sürümü: 8.2.12


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Veritabanı: stoktakip
--
-- CREATE DATABASE IF NOT EXISTS stoktakip;

USE stoktakip;
-- --------------------------------------------------------


--
-- Tablo için tablo yapısı kullanicilar
--
DROP TABLE IF EXISTS kullanicilar;
CREATE TABLE kullanicilar (
  id int(11) DEFAULT NULL,
  kullanici_adi varchar(100) NOT NULL,
  sifre_hash char(64) NOT NULL,
  rol enum('admin','kullanici') DEFAULT 'kullanici',
  notlar text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_turkish_ci;

-- --------------------------------------------------------


--
-- Tablo için tablo yapısı urunler
--
DROP TABLE IF EXISTS urunler;
CREATE TABLE urunler (
  urun_id int(11) NOT NULL,
  urun_barkod varchar(13) NOT NULL,
  urun_ad varchar(100) NOT NULL,
  urun_adet int(11) NOT NULL,
  urun_alisfiyat decimal(10,2) DEFAULT NULL,
  urun_satisfiyat decimal(10,2) DEFAULT NULL,
  tarih datetime NOT NULL,
  urun_detay text NOT NULL,
  urun_kdv tinyint(4) DEFAULT NULL,
  urunFoto blob DEFAULT NULL,
  birim_fiyat decimal(10,2) DEFAULT NULL,
  toplam_fiyat decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_turkish_ci;


--
-- Tablo için tablo yapısı musteriler
--
DROP TABLE IF EXISTS musteriler;
CREATE TABLE musteriler (
  musteri_id int(11) NOT NULL,
  musteri_tip varchar(20) NOT NULL,
  musteri_ad varchar(255) NOT NULL,
  musteri_soyad varchar(255) NOT NULL,
  musteri_tel varchar(255) NOT NULL,
  musteri_mail varchar(255) NOT NULL,
  musteri_sirket varchar(255) NOT NULL,
  musteri_vergi varchar(255) NOT NULL,
  musteri_il varchar(255) NOT NULL,
  musteri_ilce varchar(255) NOT NULL,
  musteri_adres text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_turkish_ci;



-- --------------------------------------------------------


--
-- Tablo için tablo yapısı satislar
--
DROP TABLE IF EXISTS satislar;
CREATE TABLE satislar (
  satis_id int(11) NOT NULL,
  musteri_id int(11) NOT NULL,
  urun_id int(11) NOT NULL,
  satis_adet int(11) NOT NULL,
  birim_fiyat int(255) NOT NULL,
  toplam_fiyat int(255) NULL DEFAULT NULL,
  urun_karzarar decimal(10,2) DEFAULT NULL,
  satis_tarih datetime DEFAULT NULL,
  iade_adet varchar(255) DEFAULT NULL,
  fatura_id int(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_turkish_ci;


-- --------------------------------------------------------


--
-- Tablo için tablo yapısı faturalar
--
DROP TABLE IF EXISTS faturalar;
CREATE TABLE faturalar (
  fatura_id int(255) NOT NULL,
  fatura_no bigint(20) DEFAULT NULL,
  musteri_id int(11) NOT NULL,
  musteri_ad varchar(255) NOT NULL,
  musteri_tip varchar(20) NOT NULL,
  tarih datetime NOT NULL,
  urun_id int(11) NOT NULL,
  urun_adi varchar(255) NOT NULL,
  toplam_tutar int(11) NOT NULL,
  kdv varchar(255) NOT NULL,
  odeme_sekli varchar(50) NOT NULL,
  fatura_tipi varchar(50) NOT NULL,
  miktar int(50) NOT NULL,
  aciklama varchar(50) DEFAULT NULL,
  birim_fiyat decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_turkish_ci;


-- --------------------------------------------------------

--
-- Tablo için tablo yapısı fatura_icerik
--
DROP TABLE IF EXISTS fatura_icerik;
CREATE TABLE fatura_icerik (
  id int(11) NOT NULL,
  fatura_id int(255) NOT NULL,
  fatura_no bigint(20) DEFAULT NULL,
  urun_id int(11) NOT NULL,
  urun_adi varchar(255) NOT NULL,
  urun_toplam int(255) NOT NULL,
  miktar int(11) NOT NULL,
  toplam_tutar decimal(10,2) NOT NULL,
  kdv decimal(10,2) NOT NULL,
  birim_fiyat int(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_turkish_ci;

-- --------------------------------------------------------

--
-- Tablo için tablo yapısı stok_hareketleri
--
DROP TABLE IF EXISTS stok_hareketleri;
CREATE TABLE stok_hareketleri (
  hareket_id int(11) NOT NULL,
  urun_id int(11) DEFAULT NULL,
  hareket_turu enum('GIRIS','CIKIS') DEFAULT NULL,
  adet int(11) DEFAULT NULL,
  hareket_tarihi datetime DEFAULT NULL,
  aciklama varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_turkish_ci;

-- --------------------------------------------------------

--
-- Tablo için tablo yapısı talepler
--
DROP TABLE IF EXISTS talepler;
CREATE TABLE talepler (
  talep_id int(11) NOT NULL,
  ad_soyad varchar(100) DEFAULT NULL,
  kullanici_adi varchar(50) NOT NULL,
  eposta varchar(100) DEFAULT NULL,
  talep_turu varchar(50) DEFAULT NULL,
  puan int(50) DEFAULT NULL,
  aciklama text DEFAULT NULL,
  dosya_yolu varchar(255) DEFAULT NULL,
  tarih datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_turkish_ci;


-- --------------------------------------------------------

--
-- Dökümü yapılmış tablolar için indeksler
--

--
-- Tablo için indeksler faturalar
--
ALTER TABLE faturalar
  ADD PRIMARY KEY (fatura_id),
  ADD UNIQUE KEY fatura_no (fatura_no),
  ADD UNIQUE KEY fatura_no_2 (fatura_no),
  ADD UNIQUE KEY fatura_no_3 (fatura_no);

--
-- Tablo için indeksler fatura_icerik
--
ALTER TABLE fatura_icerik
  ADD PRIMARY KEY (id),
  ADD KEY urun_id (urun_id),
  ADD KEY fatura_icerik_ibfk_1 (fatura_no);

--
-- Tablo için indeksler kullanicilar
--
ALTER TABLE kullanicilar
  ADD PRIMARY KEY (id),
  ADD UNIQUE KEY kullanici_adi (kullanici_adi);

--
-- Tablo için indeksler musteriler
--
ALTER TABLE musteriler
  ADD PRIMARY KEY (musteri_id),
  ADD UNIQUE KEY musteri_vergi (musteri_vergi);

--
-- Tablo için indeksler satislar
--
ALTER TABLE satislar
  ADD PRIMARY KEY (satis_id);

--
-- Tablo için indeksler stok_hareketleri
--
ALTER TABLE stok_hareketleri
  ADD PRIMARY KEY (hareket_id);

--
-- Tablo için indeksler talepler
--
ALTER TABLE talepler
  ADD PRIMARY KEY (talep_id);

--
-- Tablo için indeksler urunler
--
ALTER TABLE urunler
  ADD PRIMARY KEY (urun_id),
  ADD UNIQUE KEY urun_barkod (urun_barkod);

--
-- Dökümü yapılmış tablolar için AUTO_INCREMENT değeri
--

--
-- Tablo için AUTO_INCREMENT değeri faturalar
--
ALTER TABLE faturalar
  MODIFY fatura_id int(255) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- Tablo için AUTO_INCREMENT değeri fatura_icerik
--
ALTER TABLE fatura_icerik
  MODIFY id int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- Tablo için AUTO_INCREMENT değeri kullanicilar
--
ALTER TABLE kullanicilar
  MODIFY id int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- Tablo için AUTO_INCREMENT değeri musteriler
--
ALTER TABLE musteriler
  MODIFY musteri_id int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=103;

--
-- Tablo için AUTO_INCREMENT değeri satislar
--
ALTER TABLE satislar
  MODIFY satis_id int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=27;

--
-- Tablo için AUTO_INCREMENT değeri stok_hareketleri
--
ALTER TABLE stok_hareketleri
  MODIFY hareket_id int(11) NOT NULL AUTO_INCREMENT;

--
-- Tablo için AUTO_INCREMENT değeri talepler
--
ALTER TABLE talepler
  MODIFY talep_id int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- Tablo için AUTO_INCREMENT değeri urunler
--
ALTER TABLE urunler
  MODIFY urun_id int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- Dökümü yapılmış tablolar için kısıtlamalar
--

--
-- Tablo kısıtlamaları fatura_icerik
--
ALTER TABLE fatura_icerik
  ADD CONSTRAINT fatura_icerik_ibfk_1 FOREIGN KEY (fatura_no) REFERENCES faturalar (fatura_no) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT fatura_icerik_ibfk_2 FOREIGN KEY (urun_id) REFERENCES urunler (urun_id) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;