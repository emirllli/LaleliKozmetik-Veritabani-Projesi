import os
from PIL import Image

artifact_path = r"C:\Users\lalel\.gemini\antigravity\brain\3cb91df6-6b7b-44b8-a89b-027b24dcb22b\db_schema_diyagrami_1780329969006.png"
dest_path = r"c:\Users\lalel\OneDrive\Masaüstü\veri tabanı final ödevi\diagrams\yeni_veritabani_semasi.png"

if not os.path.exists(artifact_path):
    print("Artifact not found!")
    exit(1)

# Open image
img = Image.open(artifact_path)
width, height = img.size
print(f"Original size: {width}x{height}")

# The English header is at the top. Let's crop it.
# The table headers start a bit lower. Let's crop the top 100 pixels out of 1000 height.
# Let's adjust crop box: (left, top, right, bottom)
# Since the header text is at y coordinate ~35-80, cropping top 100-110 pixels should be perfect.
crop_top = 110
cropped_img = img.crop((0, crop_top, width, height))

# Save the cropped image
os.makedirs(os.path.dirname(dest_path), exist_ok=True)
cropped_img.save(dest_path)
print(f"Saved cropped image to {dest_path} with size {cropped_img.size}")
