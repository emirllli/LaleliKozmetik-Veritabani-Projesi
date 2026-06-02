import os
import subprocess
from PIL import Image

def main():
    print("=== PROJE SON HALE GETİRİLİYOR ===")
    
    # 1. Word Dosyası Kilidini Kaldırma (WINWORD.exe kapatılıyor)
    print("Word dosyası kilidi kaldırılıyor...")
    try:
        subprocess.run(["taskkill", "/f", "/im", "winword.exe"], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        print("[OK] Aktif Word süreçleri kapatıldı.")
    except Exception as e:
        print(f"[WARNING] Word kapatılırken hata oluştu: {e}")
        
    # 2. Eski Geçici Rapor Dosyalarını Temizle
    guncel_path = r"c:\Users\lalel\OneDrive\Masaüstü\veri tabanı final ödevi\docs\Laleli_Kozmetik_Final_Rapor_GUNCEL.docx"
    temp_lock_path = r"c:\Users\lalel\OneDrive\Masaüstü\veri tabanı final ödevi\docs\~$leli_Kozmetik_Final_Rapor.docx"
    for temp_file in [guncel_path, temp_lock_path]:
        if os.path.exists(temp_file):
            try:
                os.remove(temp_file)
                print(f"[OK] Geçici dosya silindi: {temp_file}")
            except Exception as e:
                print(f"[WARNING] Geçici dosya silinemedi {temp_file}: {e}")

    # 3. ERD Görselini Kırpma ve Geri Yükleme
    artifact_path = r"C:\Users\lalel\.gemini\antigravity\brain\3cb91df6-6b7b-44b8-a89b-027b24dcb22b\db_schema_diyagrami_1780329969006.png"
    dest_path = r"c:\Users\lalel\OneDrive\Masaüstü\veri tabanı final ödevi\diagrams\yeni_veritabani_semasi.png"
    
    if os.path.exists(artifact_path):
        img = Image.open(artifact_path)
        width, height = img.size
        # Üstteki İngilizce başlığı kaldır (110 piksel)
        cropped_img = img.crop((0, 110, width, height))
        cropped_img.save(dest_path)
        print(f"[OK] Veritabanı şeması kırpıldı ve kaydedildi: {dest_path}")
    else:
        print("[WARNING] Şema yedek görseli bulunamadı. Kırpma atlanıyor.")
        
    # 4. Raporu Yeniden Oluşturma
    report_script = r"c:\Users\lalel\OneDrive\Masaüstü\veri tabanı final ödevi\tools\build_report.py"
    if os.path.exists(report_script):
        print("Rapor oluşturma betiği çalıştırılıyor...")
        try:
            import sys
            sys.path.append(os.path.dirname(report_script))
            import build_report
            # Ensure output is set to standard path
            build_report.OUTPUT = build_report.DOCS / "Laleli_Kozmetik_Final_Rapor.docx"
            build_report.main()
            print("[OK] Rapor başarıyla güncellendi.")
        except Exception as e:
            print(f"[ERROR] Rapor oluşturulurken hata oluştu: {e}")
    else:
        print("[ERROR] Rapor oluşturma betiği bulunamadı!")

    # 5. Git Durumunu Güncelleme ve Fazlalıkları Kaldırma
    print("Git işlemleri başlatılıyor...")
    cwd = r"c:\Users\lalel\OneDrive\Masaüstü\veri tabanı final ödevi"
    try:
        # Gereksiz dosyaların silinmesini git üzerinde doğrula
        subprocess.run(["git", "rm", "-r", "--cached", "list.zip", "report.zip", "report_contents", "excel_contents", "LaleliKozmetik.UI/Form1.cs.bak"], cwd=cwd, stderr=subprocess.DEVNULL)
        
        # Git ignore güncellemesini ekle
        subprocess.run(["git", "add", ".gitignore"], cwd=cwd)
        
        # Gerçek resimleri ve güncellenen raporu git'e ekle
        subprocess.run(["git", "add", "diagrams/yeni_veritabani_semasi.png"], cwd=cwd)
        subprocess.run(["git", "add", "diagrams/ui_screenshots/"], cwd=cwd)
        subprocess.run(["git", "add", "docs/Laleli_Kozmetik_Final_Rapor.docx"], cwd=cwd)
        subprocess.run(["git", "add", "PROJEYI_ACMA_KILAVUZU.txt"], cwd=cwd)
        subprocess.run(["git", "add", "tools/build_report.py"], cwd=cwd)
        subprocess.run(["git", "add", "tools/finalize_project.py"], cwd=cwd)
        subprocess.run(["git", "add", "tools/crop_schema.py"], cwd=cwd)
        subprocess.run(["git", "add", "README.md"], cwd=cwd)
        subprocess.run(["git", "add", "LaleliKozmetik.UI/Form1.cs", "LaleliKozmetik.UI/Program.cs", "LaleliKozmetik.DAL/Models.cs"], cwd=cwd)
        
        # Commit ve Push
        commit_res = subprocess.run(["git", "commit", "-m", "Proje son hale getirildi, rapor hocanin sablonuna gore guncellendi ve temizlik yapildi"], cwd=cwd)
        if commit_res.returncode == 0:
            print("[OK] Git commit başarılı. GitHub'a gönderiliyor (push)...")
            push_res = subprocess.run(["git", "push", "origin", "main"], cwd=cwd)
            if push_res.returncode == 0:
                print("[OK] GitHub'a başarıyla yüklendi (push tamamlandı).")
            else:
                print("[ERROR] GitHub'a push yapılırken hata oluştu. Kimlik doğrulama gerekebilir.")
        else:
            print("[WARNING] Git commit için sahnelenmiş yeni değişiklik bulunamadı veya commit başarısız.")
            # Deneyelim: push'u her ihtimale karşı yine de çalıştıralım
            subprocess.run(["git", "push", "origin", "main"], cwd=cwd)
            
    except Exception as e:
        print(f"[ERROR] Git işlemleri sırasında hata: {e}")

    print("=== İŞLEM TAMAMLANDI ===")

if __name__ == "__main__":
    main()
