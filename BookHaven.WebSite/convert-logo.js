const sharp = require('sharp');
const fs = require('fs');
const path = require('path');

// Ensure images directory exists
const imagesDir = path.join(__dirname, 'images');
if (!fs.existsSync(imagesDir)) {
    fs.mkdirSync(imagesDir, { recursive: true });
}

// Convert SVG to PNG
sharp('images/logo.svg')
    .png()
    .toFile(path.join(imagesDir, 'logo.png'))
    .then(info => console.log('Logo PNG created:', info))
    .catch(err => console.error('Error converting logo:', err));