import os
import base64, io
from PIL import Image, ImageDraw, ImageFont, ExifTags

def pilImgToBase64(pilImg):

    imgio = io.BytesIO()
    pilImg.save(imgio, 'PNG')
    imgio.seek(0)
    dataimg = base64.b64encode(imgio.read())

    return dataimg.decode('utf-8')

def pilImread(imgPath):
    pilImg = Image.open(imgPath).convert('RGB')
    return pilImg

def base64ToPilImg(base64ImgString):

    if base64ImgString.startswith('b\''):
        base64ImgString = base64ImgString[2:-1]

    base64Img   =  base64ImgString.encode('utf-8')
    decoded_img = base64.b64decode(base64Img)
    img_buffer  = io.BytesIO(decoded_img)
    pil_img = Image.open(img_buffer).convert('RGB')

    return pil_img

def base64ToImage(base64ImgString, workingDir):
    imgPath = os.path.join(workingDir, r"./uploadedImg.jpg")
    pil_img = base64ToPilImg(base64ImgString)
    pil_img.save(imgPath, "JPEG")

    return imgPath

def imageToBase64(img_path):
    return pilImgToBase64(pilImread(img_path))