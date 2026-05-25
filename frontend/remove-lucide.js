const fs = require('fs');
const path = require('path');

function walk(dir) {
    let results = [];
    const list = fs.readdirSync(dir);
    list.forEach(file => {
        file = path.join(dir, file);
        const stat = fs.statSync(file);
        if (stat && stat.isDirectory()) {
            results = results.concat(walk(file));
        } else {
            if (file.endsWith('.ts')) results.push(file);
        }
    });
    return results;
}

const files = walk('src/app');
files.forEach(f => {
    let content = fs.readFileSync(f, 'utf8');
    let changed = false;

    // Remove LucideAngularModule from imports array
    if (content.includes('LucideAngularModule')) {
        content = content.replace(/LucideAngularModule,?\s*/g, '');
        changed = true;
    }

    // Remove lucide imports
    if (content.includes("@lucide/angular") || content.includes("lucide-angular")) {
        content = content.replace(/import\s+{[^}]+}\s+from\s+'(@lucide\/angular|lucide-angular)';?\s*/g, '');
        changed = true;
    }

    // Remove lucide-icon tags from templates
    if (content.includes('<lucide-icon')) {
        content = content.replace(/<lucide-icon[^>]*><\/lucide-icon>/g, '');
        content = content.replace(/<lucide-icon[^>]*\/>/g, '');
        changed = true;
    }

    // Remove readonly X = X assignments
    const assignments = [
        'Star', 'MapPin', 'Users', 'Plus', 'Edit', 'Trash2', 'CheckCircle2', 
        'Filter', 'X', 'ChevronLeft', 'ChevronRight', 'HotelIcon', 'Search', 
        'Calendar', 'Hotel', 'ShieldCheck', 'AlertCircle', 'CreditCard', 
        'UserIcon', 'LogOut', 'Shield', 'TrendingUp', 'DollarSign', 'Tag'
    ];
    
    assignments.forEach(icon => {
        const regex = new RegExp(`readonly\\s+${icon}\\s*=\\s*${icon};?\\s*`, 'g');
        if (content.match(regex)) {
            content = content.replace(regex, '');
            changed = true;
        }
        
        // Also handle cases like Hotel as HotelIcon
        if (icon === 'HotelIcon') {
            content = content.replace(/readonly\s+HotelIcon\s*=\s*HotelIcon;?\s*/g, '');
            changed = true;
        }
    });

    if (changed) {
        fs.writeFileSync(f, content);
        console.log(`Cleaned up ${f}`);
    }
});
