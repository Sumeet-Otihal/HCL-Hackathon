import os
import shutil

source = r"c:\Users\siddh\OneDrive\Desktop\frontend"
base_dest = r"c:\Users\siddh\OneDrive\Desktop\Base_Upload"

p1_files = [
    r"src\app\pages\landing.component.ts",
    r"src\app\pages\hotels.component.ts",
    r"src\app\pages\hotel-detail.component.ts",
    r"src\app\pages\booking.component.ts",
    r"src\app\pages\payment.component.ts",
    r"src\app\components\hotel",
    r"src\app\components\booking"
]

p2_files = [
    r"src\app\pages\login.component.ts",
    r"src\app\pages\register.component.ts",
    r"src\app\pages\profile.component.ts",
    r"src\app\pages\user-dashboard.component.ts",
    r"src\app\pages\hotel-admin-dashboard.component.ts",
    r"src\app\pages\super-admin-dashboard.component.ts",
    r"src\app\components\layout",
    r"src\app\components\ui",
    r"src\app\guards",
    r"src\app\services\auth.service.ts",
    r"src\app\services\auth.interceptor.ts"
]

print("Creating Base_Upload folder...")
if os.path.exists(base_dest):
    shutil.rmtree(base_dest)

# Copy everything from frontend to Base_Upload
shutil.copytree(source, base_dest, ignore=shutil.ignore_patterns('node_modules', '.git', 'dist', '.angular'))

print("Removing Person 1 & 2 files from Base_Upload...")
all_person_files = p1_files + p2_files

for item in all_person_files:
    item_path = os.path.join(base_dest, item)
    if os.path.exists(item_path):
        if os.path.isdir(item_path):
            shutil.rmtree(item_path)
        else:
            os.remove(item_path)

print("Done! Base_Upload is ready on your Desktop.")
